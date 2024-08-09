#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Transactions;
using System;
/// <summary>
/// Represent an expression in the DSL
/// </summary>
public abstract class Expression : Statement {
    ///<summary>
    ///Returns the <see langword="type"/> of the expression: Int, String, Bool, etc...
    ///</summary>
    public abstract IdType GetType(IScope scope);
    /// <summary>
    /// Throw an error if the expected type is different from the actual expression type
    /// </summary>
    /// <param name="scope"></param>
    /// <param name="expected"></param>
    public void CheckType(IScope scope, IdType expected)
    {
        IdType type = this.GetType(scope);
        if (type != expected) {
            //Throw error
            Error differentTypeOfVariable = new DifferentType(type, expected);
            throw new Exception(differentTypeOfVariable.ToString());
        }

    }
    /// <summary>
    /// Validate and check the expression with only one function
    /// </summary>
    /// <param name="scope"></param>
    /// <param name="expected"></param>
    public void ValidateAndCheck(IScope scope, IdType expected)
    {
        this.Validate(scope);
        this.CheckType(scope, expected);
    }
    
};

/// <summary>
/// Represent a Binary Expression
/// </summary>
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

    public override void Validate(IScope scope)
    {
        Left.Validate(scope);
        Right.Validate(scope);
        //Validate depending of the operator definition
        SemantycBinaryExpression.ValidateByOp[Op.Definition].Invoke(this, scope);
    }

    //It is done depending of the operator
    public override IdType GetType(IScope scope)
    {
        //Get the type using the dictionary in SemantycBinaryExpression class
        return SemantycBinaryExpression.GetTypeByOp[Op.Definition].Invoke(this, scope);
    }

    //TODO: Chequear evaluate
    // public override object Evaluate()
    // {
    //     return EvaluateBinaryExpression.EvaluateByOp[Op.Definition].Invoke(this);
    // }
}
public class LiteralExpression : Expression
{
    public Token Value {get; }
    public LiteralExpression(Token value) {
        this.Value = value;
    }
    /// <summary>
    /// A hash set with the reserved words types that are also properties of a card
    /// </summary>
    /// <param name="scope"></param>
    readonly HashSet<TokenType> ReservedWordsProperties = new()
    {
        TokenType.Power, TokenType.Faction, TokenType.Name, TokenType.Type
    };
    public override void Validate(IScope scope)
    {
        TokenType type = Value.Definition;
        //returns true is it is not an id
        if (type is TokenType.Num || type is TokenType.String
        || type is TokenType.Boolean || scope.IsDefined(this.Value.Value)
        || ReservedWordsProperties.Contains(type)) {
            return;
        }
        //TODO: Variable no definida
        throw new Exception();
    }

    public override IdType GetType(IScope? scope)
    {
        //If is an id or a reserved word id return its type
        if ((Value.Definition is TokenType.Id || Utils.PropertiesReservedWords.Contains(Value.Definition)) 
        && scope != null) {
            if (Utils.Types.TryGetValue(Value.Value, out IdType value)) {
                //This can be null
                return value;
            }
            return scope.GetIdType(this.Value.Value);
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

    //FIXME: Chequear evaluate
}

public class UnaryExpression : Expression
{
    public LiteralExpression ID { get; }
    Token Op {get; }
    bool AtTheEnd { get; }
    public UnaryExpression(LiteralExpression id, Token op, bool atTheEnd) {
        this.ID = id;
        this.Op = op;
        this.AtTheEnd = atTheEnd;
    }

    public override void Validate(IScope scope)
    {
        //Id must contain a numeric expression
        ID.CheckType(scope, IdType.Number);
        ID.Validate(scope);
    }

    public override IdType GetType(IScope scope)
    {
        return ID.GetType(scope);
    }

    public override object Evaluate()
    {
        throw new NotImplementedException();
    }
}

public class FindFunction : Expression {
    //The body is always a find function
    public Expression Body {get;}
    Predicate Predicate {get;}
    public FindFunction(Expression body, Predicate predicate)
    {
        this.Body = body;
        this.Predicate = predicate;
    }

    public override IdType GetType(IScope scope)
    {
        return IdType.Card;
    }

    public override void Validate(IScope scope)
    {
        Predicate.Validate(scope);
    }

    public override object Evaluate()
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

    public override IdType GetType(IScope scope)
    {
        return Body.GetType(scope);
    }

    public override void Validate(IScope scope)
    {
        //Body expression is always a literal expression
        //Check if the body match with the arguments
        string body = ((LiteralExpression)Body).Value.Value;
        if (Utils.ValidArguments.TryGetValue(body, out IdType? value) && value == Argument?.GetType(scope))
        {
            return;
        }
        //TODO:
        throw new Exception();
    }

    public override object Evaluate()
    {
        throw new NotImplementedException();
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

    public override IdType GetType(IScope scope)
    {
        return IdType.Card;
    }

    public override void Validate(IScope scope)
    {
        IdType bodyType = Body.GetType(scope);
        IdType indexType = Index.GetType(scope);
        if (bodyType is IdType.CardCollection && indexType is IdType.Number)
        {
            return;
        }
        //Throw exception
        //Get the token of the error
        Token errorToken; 
        bool indexMistake = false;
        if (bodyType is not IdType.CardCollection)  errorToken = Utils.GetErrorToken(Body);
        else { 
            errorToken = Utils.GetErrorToken(Index); 
            indexMistake = true; 
        }

        Error indexerError = new IndexerError(errorToken.Line, errorToken.Column, indexMistake);
        throw new Exception(indexerError.ToString());
    }

    //TODO:
    public override object Evaluate()
    {
        throw new NotImplementedException();
    }
}