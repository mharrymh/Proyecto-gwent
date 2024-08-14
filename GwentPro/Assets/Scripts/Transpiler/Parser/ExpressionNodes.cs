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
    //Not all expressions can be evaluated
    public virtual object? Evaluate() {
        return null;
    }
    //The execute of expressions
    public abstract override object Execute(IExecuteScope scope);
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

    public override object Evaluate()
    {
        return EvaluateBinaryExpression.EvaluateByOp[Op.Definition].Invoke(this);
    }

    public override object Execute(IExecuteScope scope)
    {
       return BinaryExpressionExecuter.ExecuteByOp[Op.Definition](this, scope);
    }
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
        || type is TokenType.Boolean || scope.IsDefined(Value.Value)
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
    //It is secure that it will not be an id, ids are called in the instructions
    public override object Evaluate()
    {
        if (this.Value.Definition is TokenType.String)
        {
            //Remove the character " at the start and at the end
            return this.Value.Value[1..^1];
        }
        if (Value.Definition is TokenType.Num)
        {
            return int.Parse(Value.Value);
        }
        return (this.Value.Value == "true")? true : false;
    }

    public override object Execute(IExecuteScope scope)
    {
        TokenType definition = this.Value.Definition;
        //It is an string
        if (definition is TokenType.String)
        {
            //Remove the character " at the start and at the end
            return this.Value.Value[1..^1];
        }   
        //It is an integer
        if (definition is TokenType.Num)
        {
            return int.Parse(this.Value.Value);
        }
        //It is a variable
        if (definition is TokenType.Id)
        {
            //Get the id value (it can be null)
            return scope.GetValue(Value.Value);
        }
        //it is a boolean
        if (Value.Value == "true") return true;
        else return false;
    }
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

    public override object Execute(IExecuteScope scope)
    {
        //Save the variable value
        int varValue = (int)ID.Execute(scope);
        //Redefine its value without changing the value of varValue
        scope.Define(ID.Value.Value, varValue + 1);
        if (!AtTheEnd)
        {
            //Change the varValue
            varValue++;
        }
        return varValue;
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

    public override object Execute(IExecuteScope scope)
    {
        CardCollection body = (CardCollection)Body.Execute(scope);
        return body.Find(Predicate, scope);
    }
}

public class FunctionCall : Expression {
    /// <summary>
    /// The left part before the function is called 
    /// </summary>
    /// <value></value>
    public Expression LeftExpression {get;}
    /// <summary>
    /// The function Name
    /// </summary>
    /// <value></value>
    public Token FunctionName {get;}
    /// <summary>
    /// The arguments of the functions
    /// </summary>
    /// <value>If null it means it has no arguments</value>
    Expression? Argument {get;}
    public FunctionCall(Expression left, Token name, Expression? argument)
    {
        LeftExpression = left;
        FunctionName = name;
        Argument = argument;
    }

    public override IdType GetType(IScope scope)
    {
        //Return the type that the function returns
        return Utils.Types[FunctionName.Value];
    }

    public override void Validate(IScope scope)
    {
        IdType leftType = LeftExpression.GetType(scope);
        if (leftType != IdType.Context || leftType != IdType.CardCollection)
        {
            //TODO: LOS UNICOS QUE PUEDEN ACCEDER A FUNCIONES SON CONTEXT Y CARDCOLLECTION
            throw new Exception();
        }
        //Check that the left expression match the function name
        if (!Utils.ValidAccess[leftType].Contains(FunctionName.Value))
        {
            //TODO: esta funcion no esta disponible para context o cardCollection
            throw new Exception();
        }
        //Check if the functionName match with the arguments
        string body = FunctionName.Value;
        if (Utils.ValidArguments.TryGetValue(body, out IdType? value) && value == Argument?.GetType(scope))
        {
            return;
        }
        //TODO:
        throw new Exception();
    }

    public override object Execute(IExecuteScope scope)
    {
        object body = LeftExpression.Execute(scope);
        if (body is CardCollection collection)
        {
            return Executer.CollectionFunctions[FunctionName.Value](Argument, collection, scope);
        }
        //It is a context function
        else 
        {
            //Argument is always a player here
            return Executer.ContextFunctions[FunctionName.Value](new Context(), Argument, scope);
        }
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

    public override object Execute(IExecuteScope scope)
    {
        CardCollection body = (CardCollection)Body.Execute(scope);
        int index = (int)Index.Execute(scope);
        return body[index];
    }
}