using System;
using System.Collections.Generic;

public static class BinaryExpressionExecuter 
{
    public static readonly Dictionary<TokenType, Func<BinaryExpression, IExecuteScope, object>> ExecuteByOp = new()
    { 
        {TokenType.Plus, SumExpression},
        {TokenType.Minus, MinusExpression},
        {TokenType.Multip, MultipExpression},
        {TokenType.Division, DivisionExpression},
        {TokenType.Pow, PowExpression},

        {TokenType.Concatenation, ConcatExpression},
        {TokenType.SpaceConcatenation, SpaceConcatExpression},

        {TokenType.And, AndExpression},
        {TokenType.Or, OrExpression},

        {TokenType.Less, LessExp},
        {TokenType.LessEq, LessEqExp},
        {TokenType.MoreEq, MoreEqExp},
        {TokenType.More, MoreExp},
        {TokenType.Equal, EqualExp},

        {TokenType.Assign, AssignExpression},

        {TokenType.MinusAssign, MinusAssign},
        {TokenType.MoreAssign, MoreAssign},
        {TokenType.DivisionAssign, DivisionAssign},
        {TokenType.MultipAssign, MultipAssign},

        
        {TokenType.Point, AccessExpression}
    };

    private static object AccessExpression(BinaryExpression expression, IExecuteScope scope)
    {
        //Get the instance of the left part of the expression (literal expression)
        object leftProperty = expression.Left.Execute(scope);
        return AuxAccessExpression(leftProperty, expression.Right, scope);
    }
    /// <summary>
    /// Objective: Handle the logic of access expressions in a recursive way
    /// Terminal case: 
    ///     1-Get the information of left using Reflexion
    ///     2-Get the property value(always exists because it was checked in the semantyc) and return it
    /// Recursive case:
    ///     1-Update left with the value of the execution of the left part of the binary expression
    ///     2-Call recursively to AuxAccessExpression passing the new value as left, and the (binary)expression.Rightn as expression
    /// </summary>
    /// <param name="left"></param>
    /// <param name="expression"></param>
    /// <param name="scope"></param>
    /// <returns></returns>
    public static object AuxAccessExpression(object left, Expression expression, IExecuteScope scope)
    {
        //Terminal case of the recursive call
        if (expression is LiteralExpression literal)
        {
            //Return left.(the literal expression)           
            var propertyInfo = left.GetType().GetProperty(literal.Value.Value);

            if (propertyInfo != null)
            {
                object value = propertyInfo.GetValue(left);
                return value;
            }
        }
        else // is a binary expression
        {
            BinaryExpression binaryExp = (BinaryExpression)expression;
            //Actualizar left y llamar recursivamente a este mismo metood


            LiteralExpression nextProperty = (LiteralExpression)binaryExp.Left;
            

            var propertyInfo = left.GetType().GetProperty(nextProperty.Value.Value);

            if (propertyInfo != null)
            {
                object value = propertyInfo.GetValue(left);
                return AuxAccessExpression(value, binaryExp.Right, scope);
            }
        }

        //It will never reach this part because it is sure that propertyInfo cant be null cause the semantyc already checked that
        return null;

    }

    private static object AssignExpression(BinaryExpression expression, IExecuteScope scope)
    {
        if (expression.Left is LiteralExpression literal)
        {
            object value = expression.Right.Execute(scope);

            scope.Define(literal.Value.Value, value);
        }
        //Else it is an accesor property
        else 
        {
            //Get the instance of the left part of the expression (binary expression)
            BinaryExpression leftBinary = (BinaryExpression)expression.Left;
            object left = leftBinary.Left.Execute(scope);
            Set(left, leftBinary.Right, expression.Right.Execute(scope), scope);
        }
        return null;
    }
    /// <summary>
    /// Set the property
    /// </summary>
    /// <param name="left">Represent the instance of the left part</param>
    /// <param name="propertyChain">The expression that comes next to the instance (left)</param>
    /// <param name="value">The value that needs to be assigned</param>
    /// <param name="scope">The execute scope</param>
    private static void Set(object left, Expression propertyChain, object value, IExecuteScope scope)
    {
        if (propertyChain is LiteralExpression literal)
        {
            var propertyInfo  = left.GetType().GetProperty(literal.Value.Value);

            if (propertyInfo != null)
            {
                propertyInfo.SetValue(left, value);
            }
            return;
        }
        else
        {
            BinaryExpression binaryExp = (BinaryExpression)propertyChain;
            //Actualizar left y llamar recursivamente a este mismo metood
            LiteralExpression nextProperty = (LiteralExpression)binaryExp.Left;
            

            var propertyInfo = left.GetType().GetProperty(nextProperty.Value.Value);

            if (propertyInfo != null)
            {
                object propertyValue = propertyInfo.GetValue(left);
                Set(propertyValue, binaryExp.Right, value, scope);
            }
        }
    }

    private static object MinusAssign(BinaryExpression expression, IExecuteScope scope)
    {
        if (expression.Left is LiteralExpression literal)
        {
            scope.Define(literal.Value.Value, (int)literal.Execute(scope) - (int)expression.Right.Execute(scope));
        }
        //Else it is an accesor property
        else 
        {
            //Get the value of the left part
            int leftValue = (int)expression.Left.Execute(scope);

            //Get the expression of the left part of the expression (binary expression)
            BinaryExpression leftBinary = (BinaryExpression)expression.Left;
            //Get the instance of the left part of the left part of the expression
            object left = leftBinary.Left.Execute(scope);
            Set(left, leftBinary.Right,leftValue - (int)expression.Right.Execute(scope), scope);
        }
        return null;
    }
    private static object MoreAssign(BinaryExpression expression, IExecuteScope scope)
    {
        if (expression.Left is LiteralExpression literal)
        {
            scope.Define(literal.Value.Value, (int)literal.Execute(scope) + (int)expression.Right.Execute(scope));
        }
        //Else it is an accesor property
        else 
        {
            //Get the value of the left part
            int leftValue = (int)expression.Left.Execute(scope);

            //Get the expression of the left part of the expression (binary expression)
            BinaryExpression leftBinary = (BinaryExpression)expression.Left;
            //Get the instance of the left part of the left part of the expression
            object left = leftBinary.Left.Execute(scope);
            Set(left, leftBinary.Right,leftValue + (int)expression.Right.Execute(scope), scope);
        }
        return null;
    }
    private static object MultipAssign(BinaryExpression expression, IExecuteScope scope)
    {
        if (expression.Left is LiteralExpression literal)
        {
            scope.Define(literal.Value.Value, (int)literal.Execute(scope) * (int)expression.Right.Execute(scope));
        }
        //Else it is an accesor property
        else 
        {
            //Get the value of the left part
            int leftValue = (int)expression.Left.Execute(scope);

            //Get the expression of the left part of the expression (binary expression)
            BinaryExpression leftBinary = (BinaryExpression)expression.Left;
            //Get the instance of the left part of the left part of the expression
            object left = leftBinary.Left.Execute(scope);
            Set(left, leftBinary.Right,leftValue * (int)expression.Right.Execute(scope), scope);
        }
        return null;
    }
    private static object DivisionAssign(BinaryExpression expression, IExecuteScope scope)
    {
        if (expression.Left is LiteralExpression literal)
        {
            int divisor = (int)expression.Right.Execute(scope);
            if (divisor != 0)
                scope.Define(literal.Value.Value, (int)literal.Execute(scope) / divisor);

            else
            {
                ExecutionError DivisionByZero = new Ex_DivisionByZero(expression.Right.GetLine());
                throw DivisionByZero;
            }
        }
        //Else it is an accesor property
        else 
        {
            //Get the value of the left part
            int leftValue = (int)expression.Left.Execute(scope);
            int rightValue = (int)expression.Right.Execute(scope);
            if (rightValue == 0)
            {
                ExecutionError DivisionByZero = new Ex_DivisionByZero(expression.Right.GetLine());
                throw DivisionByZero;
            }

            //Get the expression of the left part of the expression (binary expression)
            BinaryExpression leftBinary = (BinaryExpression)expression.Left;
            //Get the instance of the left part of the left part of the expression
            object left = leftBinary.Left.Execute(scope);
            Set(left, leftBinary.Right, leftValue / rightValue, scope);
        }
        return null;
    }

    private static object EqualExp(BinaryExpression expression, IExecuteScope scope)
    {
        object left = expression.Left.Execute(scope);
        object right = expression.Right.Execute(scope);
        //Dont compare them by refernce
        return left.Equals(right);
    }

    private static object MoreExp(BinaryExpression expression, IExecuteScope scope)
    {
        return (int)expression.Left.Execute(scope) > (int)expression.Right.Execute(scope);
    }

    private static object MoreEqExp(BinaryExpression expression, IExecuteScope scope)
    {
        return (int)expression.Left.Execute(scope) >= (int)expression.Right.Execute(scope);
    }

    private static object LessEqExp(BinaryExpression expression, IExecuteScope scope)
    {
        return (int)expression.Left.Execute(scope) <= (int)expression.Right.Execute(scope);
    }

    private static object LessExp(BinaryExpression expression, IExecuteScope scope)
    {
        return (int)expression.Left.Execute(scope) < (int)expression.Right.Execute(scope);
    }

    private static object OrExpression(BinaryExpression expression, IExecuteScope scope)
    {
        return (bool)expression.Left.Execute(scope) || (bool)expression.Right.Execute(scope);
    }

    private static object AndExpression(BinaryExpression expression, IExecuteScope scope)
    {
        return (bool)expression.Left.Execute(scope) && (bool)expression.Right.Execute(scope);
    }

    private static object SpaceConcatExpression(BinaryExpression expression, IExecuteScope scope)
    {
        string left = (string)expression.Left.Execute(scope);
        string right = (string)expression.Left.Execute(scope);
        return  left + " " + right;
    }

    private static object ConcatExpression(BinaryExpression expression, IExecuteScope scope)
    {
        string left = (string)expression.Left.Execute(scope);
        string right = (string)expression.Left.Execute(scope);
        return string.Concat(left, right);
    }

    private static object PowExpression(BinaryExpression expression, IExecuteScope scope)
    {
        try {
            return (int)Math.Pow((int)expression.Left.Execute(scope), (int)expression.Right.Execute(scope));
        }
        catch(InvalidCastException) {
            ExecutionError ExceededInteger = new ExceededInteger(expression.GetLine());
            throw ExceededInteger;
        }
    }

    private static object DivisionExpression(BinaryExpression expression, IExecuteScope scope)
    {
        int divisor = (int)expression.Right.Execute(scope);
        if (divisor != 0)
            return (int)expression.Left.Execute(scope) / divisor;
        //Throw error of divisionbyZero
        else {
            ExecutionError DivisionByZero = new Ex_DivisionByZero(expression.Right.GetLine());
            throw DivisionByZero;
        }
    }

    private static object MultipExpression(BinaryExpression expression, IExecuteScope scope)
    {
        return (int)expression.Left.Execute(scope) * 
        (int)expression.Right.Execute(scope);
    }

    private static object MinusExpression(BinaryExpression expression, IExecuteScope scope)
    {
        return (int)expression.Left.Execute(scope) - 
        (int)expression.Right.Execute(scope);
    }

    private static object SumExpression(BinaryExpression expression, IExecuteScope scope)
    {
        return (int)expression.Left.Execute(scope) + 
        (int)expression.Right.Execute(scope);
    }
}
