using System.Runtime.InteropServices;

namespace Transpiler;
public static class EvaluateBinaryExpression {
    public static Dictionary<TokenType, Func<BinaryExpression, object>> EvaluateByOp = new()
    {
        {TokenType.Plus, SumExpression},
        {TokenType.Minus, MinusExpression},
        {TokenType.Multip, MultipExpression},
        {TokenType.Division, DivisionExpression},
        {TokenType.Pow, PowExpression},

        {TokenType.Concatenation, ConcatExpression},
        {TokenType.SpaceConcatenation, SpaceConcatExpression},

        //TODO:
        //It is done in the validate, shoud i do it in the evaluate instead?
        {TokenType.Assign, NotImplemented},

        {TokenType.MinusAssign, NotImplemented},
        {TokenType.MoreAssign, NotImplemented},
        {TokenType.DivisionAssign, NotImplemented},
        {TokenType.MultipAssign, NotImplemented},


        {TokenType.And, AndExpression},
        {TokenType.Or, OrExpression},

        {TokenType.Less, LessExp},
        {TokenType.LessEq, LessEqExp},
        {TokenType.MoreEq, MoreEqExp},
        {TokenType.More, MoreExp},

        {TokenType.Equal, NotImplemented},
        
        {TokenType.Point, NotImplemented}
    };

    private static object NotImplemented(BinaryExpression expression)
    {
        throw new NotImplementedException();
    }

    private static object MoreExp(BinaryExpression expression)
    {
        return int.Parse((string)expression.Left.Evaluate()) > int.Parse((string)expression.Right.Evaluate());
    }

    private static object MoreEqExp(BinaryExpression expression)
    {
        return int.Parse((string)expression.Left.Evaluate()) >= int.Parse((string)expression.Right.Evaluate());
    }

    private static object LessEqExp(BinaryExpression expression)
    {
        return int.Parse((string)expression.Left.Evaluate()) <= int.Parse((string)expression.Right.Evaluate());
    }

    private static object LessExp(BinaryExpression expression)
    {
        return int.Parse((string)expression.Left.Evaluate()) < int.Parse((string)expression.Right.Evaluate());
    }

    private static object OrExpression(BinaryExpression expression)
    {
        return (bool)expression.Left.Evaluate() || (bool)expression.Right.Evaluate();
    }

    private static object AndExpression(BinaryExpression expression)
    {
        return (bool)expression.Left.Evaluate() && (bool)expression.Right.Evaluate();
    }

    private static object SpaceConcatExpression(BinaryExpression expression)
    {
        string left = (string)expression.Left.Evaluate();
        string right = (string)expression.Left.Evaluate();
        return  left + " " + right;
    }

    private static object ConcatExpression(BinaryExpression expression)
    {
        string left = (string)expression.Left.Evaluate();
        string right = (string)expression.Right.Evaluate();
        return string.Concat(left, right);
    }

    private static object SumExpression(BinaryExpression expression)
    {
        return int.Parse((string)expression.Left.Evaluate()) + int.Parse((string)expression.Right.Evaluate());
    }
    private static object MinusExpression(BinaryExpression expression)
    {
        return int.Parse((string)expression.Left.Evaluate()) - int.Parse((string)expression.Right.Evaluate());
    }
    private static object MultipExpression(BinaryExpression expression)
    {
        return int.Parse((string)expression.Left.Evaluate()) * int.Parse((string)expression.Right.Evaluate());
    }
    private static object DivisionExpression(BinaryExpression expression)
    {
        int divisor = int.Parse((string)expression.Right.Evaluate());
        if (divisor != 0)
            return int.Parse((string)expression.Left.Evaluate()) / divisor;
        else {
            Token token = GetToken(expression).Value;
            Error DivisionByZero = new DivideByZeroError(token.Line, token.Column);
            throw new Exception(DivisionByZero.ToString());
        }
    }

    static LiteralExpression GetToken(BinaryExpression exp)
    {
        Expression pointer = exp.Right;
        while (pointer is not LiteralExpression) {
            //Use it to get the token to use its line to throw the error
            //FIXME: expresiones numericas solo pueden ser binarias y literales??
            pointer = ((BinaryExpression)pointer).Left;
        }
        return (LiteralExpression)pointer;
    }
    private static object PowExpression(BinaryExpression expression)
    {
        return Math.Pow(int.Parse((string)expression.Left.Evaluate()),int.Parse((string)expression.Right.Evaluate()));
    }

}