#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System;
using Unity.VisualScripting;
public static class SemantycBinaryExpression {
    #region Get Type
    public static Dictionary<TokenType, Func<BinaryExpression, IScope, IdType>> GetTypeByOp = new()
    {
        {TokenType.Plus, Numeric},
        {TokenType.Minus, Numeric},
        {TokenType.Multip, Numeric},
        {TokenType.Division, Numeric},
        {TokenType.Pow, Numeric},

        {TokenType.Concatenation, StringType},
        {TokenType.SpaceConcatenation, StringType},

        {TokenType.Assign, Assign},

        {TokenType.MinusAssign, NumericAssign},
        {TokenType.MoreAssign, NumericAssign},
        {TokenType.DivisionAssign, NumericAssign},
        {TokenType.MultipAssign, NumericAssign},

        {TokenType.And, Logic},
        {TokenType.Or, Logic},

        {TokenType.Less, NumericComparer},
        {TokenType.LessEq, NumericComparer},
        {TokenType.MoreEq, NumericComparer},
        {TokenType.More, NumericComparer},

        {TokenType.Equal, Equal},
        
        {TokenType.Point, Access}
    };

    private static IdType Access(BinaryExpression expression, IScope scope)
    {
        return expression.Right.GetType(scope);
    }

    private static IdType Logic(BinaryExpression expression, IScope scope)
    {
        expression.Right.CheckType(scope, IdType.Boolean);
        expression.Left.CheckType(scope, IdType.Boolean);
        return IdType.Boolean;
    }

    private static IdType NumericComparer(BinaryExpression expression, IScope scope)
    {
        expression.Right.CheckType(scope, IdType.Number);
        expression.Left.CheckType(scope, IdType.Number);
        return IdType.Boolean;
    }

    private static IdType Equal(BinaryExpression expression, IScope scope)
    {
        //Check that both types are the same
        expression.Left.CheckType(scope, expression.Right.GetType(scope));
        return IdType.Boolean;
    }

    private static IdType NumericAssign(BinaryExpression expression, IScope scope)
    {
        expression.Right.CheckType(scope, IdType.Number);
        return IdType.Number;
    }

    /// <summary>
    /// An assignation operator returns the type of the expression that is assigned
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="scope"></param>
    /// <returns></returns>
    private static IdType Assign(BinaryExpression expression, IScope scope)
    {
        return expression.Right.GetType(scope);
    }

    private static IdType StringType(BinaryExpression expression, IScope scope)
    {
        expression.Right.CheckType(scope, IdType.String);
        expression.Left.CheckType(scope, IdType.String);
        return IdType.String;
    }

    private static IdType Numeric(BinaryExpression expression, IScope scope)
    {
        expression.Right.CheckType(scope, IdType.Number);
        expression.Left.CheckType(scope, IdType.Number);
        return IdType.Number;
    }
    #endregion
    #region Validate
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

    private static void AccessExpression(BinaryExpression expression, IScope scope)
    {
        LiteralExpression right = (LiteralExpression)expression.Right;
        Expression left = expression.Left;
        IdType leftType = left.GetType(scope);

        AuxAccessExpression(right, left, leftType, scope);
    }

    private static void AuxAccessExpression(LiteralExpression right, Expression left, IdType leftType, IScope scope)
    {
        string value = right.Value.Value;
        if (!Utils.relateValuesWithTypes.ContainsKey(value) ||  Utils.relateValuesWithTypes[value] != leftType)
        {
            CompilationError NotValidAccessToProperty = new NotValidAccessToProperty(leftType.ToString(), right.Value.Value, right.GetLine());
            throw NotValidAccessToProperty;
        }
        if (left is BinaryExpression binary)
        {
            //Check that right value can be called from leftType object
            right = (LiteralExpression)binary.Right;
            left = binary.Left;
            leftType = left.GetType(scope);
            AuxAccessExpression(right, left, leftType, scope);
        }
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
            CompilationError SemantycUnexpectedType = new SemantycUnexpectedType(left.GetType(scope).ToString(), right.GetType(scope).ToString(), left.GetLine());
            throw SemantycUnexpectedType;
        }
    }

    private static void NumericAssignExpression(BinaryExpression expression, IScope scope)
    {
        //It has to be a numeric expression
        expression.Left.CheckType(scope, IdType.Number);
        if (expression.Left is LiteralExpression literal && literal.Value.Definition is not TokenType.Id)
        {
            CompilationError NumericAssignError = new NumericAssignError(expression.Op.Line, literal.GetType().ToString());
            throw NumericAssignError;
        }
        expression.Right.CheckType(scope, IdType.Number);    
    }

    private static void AssignExpression(BinaryExpression expression, IScope scope)
    {
        //If left expression is an id then add it to the scopes dictionary
        if (expression.Left is LiteralExpression literal &&
        literal.Value.Definition is TokenType.Id)
        {
            //Define the new variable in the scope
            scope.Define(literal.Value.Value, expression.Right.GetType(scope));
            return;
        }
        IdType leftType = expression.Left.GetType(scope);
        IdType rightType = expression.Right.GetType(scope);
      
        if (leftType is not IdType.Number || leftType != rightType)
        {
            if (leftType != rightType) {
                CompilationError NotSameType = new SemantycUnexpectedType(leftType.ToString(), rightType.ToString(), expression.Op.Line);
                throw NotSameType;
            }

            CompilationError InvalidPropertyAssigned = new InvalidPropertyAssigned(leftType.ToString(), expression.Op.Line);
            throw InvalidPropertyAssigned;
        }
    }
    #endregion
}