using System.ComponentModel.DataAnnotations;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Transactions;

/*
Crear el id type null?? 

*/



namespace Transpiler;
//Create an abstract expression class 
public abstract class Expression : Statement {
    ///<summary>
    ///Returns the <see langword="type"/> of the expression: Int, String, Bool, etc...
    ///</summary>
    public abstract IdType GetType(IContext context);
    ///<summary>
    ///Relate the types with a hashset of possible properties or functions represented as string 
    ///</summary>
    //TODO: Agregar otherhand etc...
    public Dictionary<IdType, HashSet<string>> ValidAccess = new Dictionary<IdType, HashSet<string>>{
        {IdType.Context, ["TriggerPlayer", "Board", "Hand", "HandOfPLayer", "FieldOfPlayer", "GraveyardOfPlayer", "DeckOfPlayer"]},
        {IdType.Card, ["Owner", "Power", "Faction", "Name", "Type"]},
        {IdType.Player, ["Enemy"]},
        {IdType.CardCollection, ["Find", "Push", "SendBottom", "Pop", "Remove", "Shuffle", "Add"]}
    };
    ///<summary>
    ///Relate the functions with the types of the arguments 
    ///</summary>
    public Dictionary<string, IdType?> ValidArguments = new Dictionary<string, IdType?>{
        {"Find", IdType.Predicate},
        {"Push", IdType.Card},
        {"SendBottom", IdType.Card},
        {"Pop", null},
        {"Remove", IdType.Card},
        {"Shuffle", null},
        {"Add", IdType.Card},

        {"HandOfPlayer", IdType.Player},
        {"FieldOfPlayer", IdType.Player},
        {"GraveyardOfPlayer", IdType.Player},
        {"DeckOfPlayer", IdType.Player}
    };
    ///<summary>
    ///Relate the functions with its types
    ///</summary>
    public Dictionary<string, IdType> Types = new Dictionary<string, IdType>{
        //Functions
        {"Find", IdType.Card},
        {"Push", IdType.Null},
        {"SendBottom", IdType.Null},
        {"Pop", IdType.Card},
        {"Remove", IdType.Null},
        {"Shuffle", IdType.Null},
        {"Add", IdType.Null},

        {"HandOfPlayer", IdType.CardCollection},
        {"FieldOfPlayer", IdType.CardCollection},
        {"GraveyardOfPlayer", IdType.CardCollection},
        {"DeckOfPlayer", IdType.CardCollection},
        {"Owner", IdType.Player},
        {"TriggerPlayer", IdType.Player},
        {"Board", IdType.CardCollection},

        {"Power", IdType.Number},
        {"Faction", IdType.String},
        {"Type", IdType.String},
        {"Name", IdType.String}
    };
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
        //Saves the type of the operator
        TokenType type = Op.Definition;
        //Save right type
        IdType rightType = Right.GetType(context);
        IdType? leftType = null;
        //Id operators:
        // - Assignment '='
        //TODO: -= += /= *= 

        if (type is TokenType.Assign) {
            //If left expression is an id then add definition to the scope dictionary
            if (Left is LiteralExpression literal && literal.Value.Definition is TokenType.Id) {
                //Define the new variable in the scope
                if (!context.Define(literal.Value.Value, new Variable(Right, rightType))) return false;
            } 
            //TODO: SUPONIENDO QUE PARA SER MODIFICABLE UNA PROPIEDAD SOLO PUEDE SER NUMBER
            //PORQUE NO TIENE SENTIDO QUE UNA ACCION PUEDA MODIFICARTE OTRA COSA, PREGUNTAR
            //Puede ser una carta
            // leftType = Left.GetType(context);
            // if (!(leftType is IdType.Number && rightType == leftType)) return false;
            return true;
        }
        //Save left type
        if (leftType == null)
            leftType = Left.GetType(context);

        #region Bool Expression
        //Save the bool operators in a hash set
        HashSet<TokenType> boolOps = 
        [TokenType.And, TokenType.Or, TokenType.Equal, TokenType.More, TokenType.MoreEq, TokenType.Less, TokenType.LessEq];
        //Bool expressions must have the same type in both left and right expressions
        if (boolOps.Contains(type) && leftType == rightType)
        {
            if (type is TokenType.And || type is TokenType.Or)
            {
                //Check that both expressions are bool 
                return leftType == IdType.Boolean;
            }
            else if (type is TokenType.LessEq || type is TokenType.Less
            || type is TokenType.MoreEq || type is TokenType.More) {
                //Check that both expressions are ints
                return leftType == IdType.Number;
            }
            //Is a equal operator
            else {
                //Already verified that both are the same
                return true;
            }
        }
        #endregion
        #region Int 
        //Saves the numeric and concat operators in a hash set
        HashSet<TokenType> numericOps = 
        [TokenType.Minus, TokenType.Plus, TokenType.Division, TokenType.Multip];

        if (numericOps.Contains(type) && leftType == rightType && leftType == IdType.Number) {
            //Already checked that both left and right are int or string expressions
            return true;
        }
        #endregion
        #region String
        HashSet<TokenType> stringOps = [TokenType.Concatenation, TokenType.SpaceConcatenation];

        if (stringOps.Contains(type) && leftType == rightType && leftType == IdType.String) {
            //Already checked that both left and right are int or string expressions
            return true;
        }
        #endregion
        //Else is an id expression
        #region Id Expression
        //-Access '.'
        if (Op.Definition is TokenType.Point) {
            //Left type is not null here
            ValidateAccess((IdType)leftType, Right, context);
        }

        #endregion
        return true;
    }
    void ValidateAccess(IdType leftType, Expression right, IContext context)
    {
        if (right is BinaryExpression binary)
        {
            if (ValidAccess.ContainsKey(leftType) 
            //The left part of an access binary expression is always an id literal expression
            && ValidAccess[leftType].Contains(((LiteralExpression)binary.Left).Value.Value))
            {
                //Is correct
                return;
            }
        }
        else if (right is LiteralExpression literal) {
            if (ValidAccess.ContainsKey(leftType)
            && ValidAccess[leftType].Contains(literal.Value.Value)) return;
        }
        else if (right is FunctionCall function) {
            //Body of a function call is always a literal expression
            if (function.Validate(context) && ValidAccess[leftType].Contains(((LiteralExpression)function.Body).Value.Value)) return;
        }
        //It is an indexer
        else if (right is Indexer indexer){
            if (indexer.Validate(context))
            {
                if (indexer.Body is LiteralExpression litBody && ValidAccess[leftType].Contains(litBody.Value.Value)) return;
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
    //It is done depending of the operator
    public override IdType GetType(IContext context)
    {
        //If it is an access operatos just return the right type and it becomes recursive
        if (Op.Definition is TokenType.Point) return Right.GetType(context);
        else if (Op.Definition is TokenType.Assign) {
            //If it is an assignment return the type of the right side of the assignment 
            return Right.GetType(context);
        }
        else {
            //TODO:
            if (Left.GetType(context) != Right.GetType(context)) throw new Exception();
            //If they are the same return the right side 
            return Left.GetType(context);
        }
        
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
        if (type is TokenType.Num || type is TokenType.String
        || type is TokenType.Boolean) {
            return true;
        }
        //else return true is the id has been defined
        //TODO: YA EXISTEN IDS DEFINIDOS COMO LOS DE CONTEXT ETC
        //Definir el context cuando se crea el action
        else return context.IsDefined(this.Value.Value);
    }

    public override IdType GetType(IContext? context)
    {
        //If is an id or a reserved word id return its type
        if ((Value.Definition is TokenType.Id || Utils.PropertiesReservedWords.Contains(Value.Definition)) 
        && context != null) {
            if (Types.ContainsKey(Value.Value)) {
                //This can be null
                return Types[Value.Value];
            }
            return context.GetIdType(this.Value.Value);
        }

        //Boolean, ints and strings only 
        Dictionary<TokenType, IdType> pairs = new()
        {
        {TokenType.Num, IdType.Number},
        {TokenType.String, IdType.String},
        {TokenType.Boolean, IdType.Boolean}
        };
        return pairs[Value.Definition];
    }
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

public class FindFunction : Expression {
    Expression Body {get;}
    Predicate Predicate {get;}
    public FindFunction(Expression body, Predicate predicate)
    {
        this.Body = body;
        this.Predicate = predicate;
    }

    public override IdType GetType(IContext context)
    {
        throw new NotImplementedException();
    }

    public override bool Validate(IContext context)
    {
        throw new NotImplementedException();
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
        return Body.GetType(context);
    }

    public override bool Validate(IContext context)
    {
        //Body expression is always a literal expression
        //Check if the body match with the arguments
        string body = ((LiteralExpression)Body).Value.Value;
        if (ValidArguments.ContainsKey(body) && ValidArguments[body] == Argument?.GetType(context))
        {
            return true;
        }
        return false;
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
        return IdType.Card;
    }

    public override bool Validate(IContext context)
    {
        if (Body.GetType(context) is IdType.CardCollection && Index.GetType(context) is IdType.Number)
        {
            return true;
        }
        return false;
    }
}