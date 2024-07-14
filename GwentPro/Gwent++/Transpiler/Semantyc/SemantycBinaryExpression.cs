using System.Security.AccessControl;
using System.Security.Cryptography;

namespace Transpiler;

public static class SemantycBinaryExpression {
    /// <summary>
    /// Helps to validate a binary expression depending of it operator
    /// </summary>
    /// <returns></returns>
    public static Dictionary<TokenType, Action<BinaryExpression, IScope>> ValidateByOp = new()
    {
        {TokenType.Plus, NumericExpression},
        {TokenType.Minus, NumericExpression},
        {TokenType.Multip, NumericExpression},
        {TokenType.Division, NumericExpression},
        {TokenType.Pow, NumericExpression},

        {TokenType.Concatenation, StringExpression},
        {TokenType.SpaceConcatenation, StringExpression},

        {TokenType.Assign, AssignExpression},

        {TokenType.MinusAssign, NumericAssignExpression},
        {TokenType.MoreAssign, NumericAssignExpression},
        {TokenType.DivisionAssign, NumericAssignExpression},
        {TokenType.MultipAssign, NumericAssignExpression},

        {TokenType.And, LogicExpression},
        {TokenType.Or, LogicExpression},

        {TokenType.Less, NumericComparerExpression},
        {TokenType.LessEq, NumericComparerExpression},
        {TokenType.MoreEq, NumericComparerExpression},
        {TokenType.More, NumericComparerExpression},

        {TokenType.Equal, EqualExpression},
        
        {TokenType.Point, AccessExpression}
    };

    /// <summary>
    /// Helps to Validate the access binary expresssion depending of the type of the right expression
    /// </summary>
    /// <returns></returns>
    public static Dictionary<Type, Action<IdType, Expression, IScope>> ValidateAccess= new()
    {
        {typeof(BinaryExpression), ToBinary},
        {typeof(LiteralExpression), ToLiteral},
        {typeof(FunctionCall), TofunctionCall},
        {typeof(Indexer), ToIndexer},
        {typeof(FindFunction), ToFindFunction},
    };

    #region Validate Access
    private static void ToFindFunction(IdType leftType, Expression expression, IScope scope)
    {
        FindFunction right = (FindFunction)expression;
        //Body of a function call is always a literal expression
        right.Validate(scope);
        if (!Utils.ValidAccess[leftType].Contains("Find")) throw new Exception();
        //TODO:
    }
    private static void ToIndexer(IdType leftType, Expression expression, IScope scope)
    {
        //It is an indexer
        Indexer right = (Indexer)expression;

        right.Validate(scope);

        if (right.Body is LiteralExpression litBody) 
        {
            if (!Utils.ValidAccess[leftType].Contains(litBody.Value.Value))
            {
                //TODO: LANZAR ERROR
                throw new Exception();
            }
        }
        //Body can be a function call
        else {
            ValidateAccess[typeof(FunctionCall)].Invoke(leftType, right.Body, scope);
        }
    }

    private static void TofunctionCall(IdType leftType, Expression expression, IScope scope)
    {
        FunctionCall right = (FunctionCall)expression;
        //Body of a function call is always a literal expression
        right.Validate(scope);
        if (Utils.ValidAccess[leftType].Contains(((LiteralExpression)right.Body).Value.Value)) return;
        //TODO:
        else throw new Exception();
    }

    private static void ToLiteral(IdType leftType, Expression expression, IScope scope)
    {
        LiteralExpression right = (LiteralExpression)expression;
        if (!Utils.ValidAccess.ContainsKey(leftType) || !Utils.ValidAccess[leftType].Contains(right.Value.Value))
        {
            //TODO:
            throw new Exception();
        }
    }

    private static void ToBinary(IdType leftType, Expression rightExp, IScope scope)
    {
        //It can only be another binary access expression
        BinaryExpression right = (BinaryExpression)rightExp;
        //The left part of an access binary expression is always an id literal expression
        //that is why we use the cast
        LiteralExpression leftOfRight = (LiteralExpression)right.Left;

        if (!Utils.ValidAccess.TryGetValue(leftType, out HashSet<string>? value) || !value.Contains(leftOfRight.Value.Value))
            throw new Exception();
        //TODO:
    }
    #endregion
    #region Validate By Op
    private static void AccessExpression(BinaryExpression expression, IScope scope)
    {
        Type rightType = expression.Right.GetType();
        if (ValidateAccess.ContainsKey(rightType))
            ValidateAccess[rightType].Invoke(expression.Left.GetType(scope), expression.Right, scope);
        else 
            //TODO:
            throw new Exception();
    }

    private static void StringExpression(BinaryExpression expression, IScope scope)
    {
        CheckSameType(expression.Left, expression.Right, scope);
        expression.Left.CheckType(scope, IdType.String);
    }

    private static void NumericExpression(BinaryExpression expression, IScope scope)
    {
        CheckSameType(expression.Left, expression.Right, scope);
        expression.Left.CheckType(scope, IdType.Number);
   }

    private static void EqualExpression(BinaryExpression expression, IScope scope)
    {
        CheckSameType(expression.Left, expression.Right, scope);
    }

    private static void NumericComparerExpression(BinaryExpression expression, IScope scope)
    {
        CheckSameType(expression.Left, expression.Right, scope);
        expression.Left.CheckType(scope, IdType.Number);
    }

    private static void LogicExpression(BinaryExpression expression, IScope scope)
    {
        CheckSameType(expression.Left, expression.Right, scope);
        expression.Left.CheckType(scope, IdType.Boolean);
    }

    private static void CheckSameType(Expression left, Expression right, IScope scope)
    {
        if (left.GetType(scope) != right.GetType(scope))
        {
            //TODO:
            throw new InvalidOperationException("");
        }
    }

    private static void NumericAssignExpression(BinaryExpression expression, IScope scope)
    {
        if (expression.Left is LiteralExpression literal && literal.Value.Definition is TokenType.Id)
        {
            //This assignment expressions had to be already defined
            //TODO:
            if (scope.IsDefined(literal.Value.Value)) throw new Exception();

            //It has to be a numeric expression
            literal.CheckType(scope, IdType.Number);
            //Define the new expression 
            BinaryExpression value = new(expression.Left, ConvertOp(expression.Op), expression.Right);
            scope.Define(literal.Value.Value, new Variable(value, IdType.Number));
        }
        //TODO:
        else throw new Exception();
    }

    private static Token ConvertOp(Token op)
    {
        string value = op.Value[0].ToString();
        Dictionary<string, TokenType> ValueMatch = new Dictionary<string, TokenType>(){
            {"+", TokenType.Plus},
            {"-", TokenType.Minus},
            {"/", TokenType.Division},
            {"*", TokenType.Multip}
        };
        return new Token(value, ValueMatch[value], op.Line, op.Column);
    }

    private static void AssignExpression(BinaryExpression expression, IScope scope)
    {
        //If left expression is an id then add it to the scopes dictionary
        if (expression.Left is LiteralExpression literal &&
        literal.Value.Definition is TokenType.Id)
        {
            //Define the new variable in the scope
            scope.Define(literal.Value.Value, new Variable(expression.Right, expression.Right.GetType(scope)));
            return;
        }
        //TODO: SUPONIENDO QUE PARA SER MODIFICABLE UNA PROPIEDAD SOLO PUEDE SER NUMBER
        //PORQUE NO TIENE SENTIDO QUE UNA ACCION PUEDA MODIFICARTE OTRA COSA, PREGUNTAR
        //Puede ser una carta
        // leftType = Left.GetType(context);
        // if (!(leftType is IdType.Number && rightType == leftType)) return false;
        // return;



        //TODO: Lanza error de que a la izquierda de una asignacion debe
        //haber un id para ser asignado o un objeto variable valido
        throw new Exception();
    }
    #endregion
}