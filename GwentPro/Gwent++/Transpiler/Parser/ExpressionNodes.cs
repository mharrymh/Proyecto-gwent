using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Transactions;

namespace Transpiler;
//Create an abstract expression class 
public abstract class Expression : DSL_Object {
    ///<summary>
    ///Returns the <see langword="type"/> of the expression: Int, String, Bool, etc...
    ///</summary>
    public abstract IdType GetType(IContext context);
}

//Represent all kind of binary expressions
public class BinaryExpression : Expression
{
    public Expression Left { get; }
    public Token Op {get; }
    public Expression Right {get; }

    public BinaryExpression(Expression left, Token op, Expression right) {
        this.Left = left;
        this.Op = op;
        this.Right = right;
    }

    public override bool Validate(IContext context)
    {
        //if left expression or right expression does not validate is an error
        if(!Left.Validate(context) || !Right.Validate(context)) return false;

        //Saves the type of the operator
        TokenType type = Op.Definition;

        #region Bool Expression
        //Save the bool operators in a hash set
        HashSet<TokenType> boolOps = 
        [TokenType.And, TokenType.Or, TokenType.Equal, TokenType.More, TokenType.MoreEq, TokenType.Less, TokenType.LessEq];
        //Bool expressions must have the same type in both left and right expressions
        if (boolOps.Contains(type) && 
        Left.GetType(context) == Right.GetType(context))
        {
            if (type is TokenType.And || type is TokenType.Or)
            {
                //Check that both expressions are bool 
                return Left.GetType(context) == IdType.Boolean;
            }
            else if (type is TokenType.LessEq || type is TokenType.Less
            || type is TokenType.MoreEq || type is TokenType.More) {
                //Check that both expressions are ints
                return Left.GetType(context) == IdType.Number;
            }
            //Is a equal operator
            else {
                //Already verified that both are the same
                return true;
            }
        }
        #endregion
        #region Int / String 
        //Saves the numeric and concat operators in a hash set
        HashSet<TokenType> numericAndStringOps = 
        [TokenType.Minus, TokenType.Plus, TokenType.Division, TokenType.Multip, TokenType.Concatenation, TokenType.SpaceConcatenation];
        if (numericAndStringOps.Contains(type) && Left.GetType(context) == Right.GetType(context) && 
        (Left.GetType(context) == IdType.Number || Left.GetType(context) == IdType.String)) {
            //Already checked that both left and right are int or string expressions
            return true;
        }
        #endregion
        //Else is an id expression
        #region Id Expression
        //Id operators:
        // - Assignment '='
        //TODO: -= += /= *= 

        if (type is TokenType.Assign) {
            //If left expression is an id then add definition to the scope dictionary
            if (Left is LiteralExpression literal && literal.Value.Definition is TokenType.Id) {
                if (!context.Define(Left, new Variable(Right, Right.GetType(context)))) return false;
            } 
            //TODO: SUPONIENDO QUE PARA SER MODIFICABLE UNA PROPIEDAD SOLO PUEDE SER NUMBER
            //PORQUE NO TIENE SENTIDO QUE UNA ACCION PUEDA MODIFICARTE OTRA COSA, PREGUNTAR
            else if (!(Left.GetType(context) is IdType.Number && Right.GetType(context) == Left.GetType(context))) return false;
        }

        //-Access '.'
        else {
            ValidateAccess(Left.GetType(context), Right, context);
        }


        #endregion
    }

    Dictionary<IdType, string> ValidAccess = [];
    void ValidateAccess(IdType leftType, Expression right, IContext context)
    {
        if (right is BinaryExpression binary)
        {
            if (ValidAccess.ContainsKey(leftType) 
            //Binary.Left is always a literal expression
            && ValidAccess[leftType] == ((LiteralExpression)binary.Left).Value.Value)
            {
                return;
            }
        }
        else if (right is LiteralExpression literal) {
            if (ValidAccess.ContainsKey(leftType)
            && ValidAccess[leftType] == literal.Value.Value) return;
        }
        else if (right is FunctionCall function) {
            if (function.Validate(context) && ValidAccess[leftType] == ((LiteralExpression)function.Body).Value.Value) return;
        }
        //It is an indexer
        else if (right is Indexer indexer){
            if (indexer.Validate(context))
            {
                if (indexer.Body is LiteralExpression litBody && ValidAccess[leftType] == litBody.Value.Value) return;
                //Body is a function call
                else {
                    ValidateAccess(leftType, indexer.Body, context);
                }
            }
        }

        //It doesn't validate
        //TODO:
        throw new Exception();
    }
    public override IdType GetType(IContext context)
    {
        //Can i do just return Right.GetType(context)?
        if (Left.GetType(context) == Right.GetType(context)) {
            return Left.GetType(context);
        }
        //FIXME:
        else throw new NotImplementedException();
    }
}
public class LiteralExpression : Expression
{
    public Token Value {get; }
    public LiteralExpression(Token value) {
        this.Value = value;
    }

    public override bool Validate(IContext context)
    {
        TokenType type = Value.Definition;
        //returns true is it is not an id
        if (type is TokenType.Num || type is TokenType.Text
        || type is TokenType.Boolean) {
            return true;
        }
        //else return true is the id has been defined
        //TODO: YA EXISTEN IDS DEFINIDOS COMO LOS DE CONTEXT ETC
        //Definir el context cuando se crea el action
        else return context.IsDefined(this);
    }

    public override IdType GetType(IContext? context)
    {
        //If is an id return its type
        if (Value.Definition is TokenType.Id && context != null) {
            return context.GetIdType(this);
        }

        //Boolean, ints and strings only 
        Dictionary<TokenType, IdType> pairs = new()
        {
        {TokenType.Num, IdType.Number},
        {TokenType.Text, IdType.String},
        {TokenType.Boolean, IdType.Boolean}
        };
        return pairs[Value.Definition];
    }

    /*
    HACERLO EN EXPRESION BINARIA
    DONDE A LA IZQUIERA SIEMPRE QUEDE UNA EXPRESION LITERAL
    Y EMPEZAR A DESGLOSAR DESDE LA IZQUIERDA
    LAS OPERACIONES SON POR PUNTO 
    Y ES ACCESO A PROPIEDAD O LLAMADA A FUNCION

    LUEGO TE DIGO ERES LA EXPRESION LITERAL CONTEXT
    Y TE ESPERO UNA PROPIEDAD O METODO REFERENTE A CONTEXT Y ASI SUCESIVAMENTE
    */
}

public class UnaryExpression : Expression
{
    Expression ID { get; }
    Token Op {get; }
    bool AtTheEnd { get; }
    public UnaryExpression(Expression id, Token op, bool atTheEnd) {
        this.ID = id;
        this.Op = op;
        this.AtTheEnd = atTheEnd;
    }

    public override bool Validate(IContext context)
    {
        //Id must contain a numeric expression
        if(ID is LiteralExpression literal && ID.GetType(context) == IdType.Number) return literal.Validate(context);
        //TODO: THROW ERROR
        return false;
    }

    public override IdType GetType(IContext context)
    {
        return ID.GetType(context);
    }
}

public class FunctionCall : Expression {
    public Expression Body {get;}
    Expression? Argument {get;}
    public FunctionCall(Expression body, Expression? argument)
    {
        this.Body = body;
        this.Argument = argument;
    }

    public override IdType GetType(IContext context)
    {
        //el tipo del body depende de la funcion
    }

    public override bool Validate(IContext context)
    {
        //Valida si existe en el diccionario que tiene los bodys y si el tipo del argumento se corresponde con el que debe ser
    }
}

public class Indexer : Expression {
    public Expression Body {get;}
    Expression Index {get;}
    public Indexer(Expression body, Expression index)
    {
        this.Body = body;
        this.Index = index;
    }

    public override IdType GetType(IContext context)
    {
        //Devuelve el tipo de la coleccion que sea el body
        //Normalmente es una carta
    }

    public override bool Validate(IContext context)
    {
        //Valida si el body es una coleccion 
        //y el index es una expresion numerica
    }
}