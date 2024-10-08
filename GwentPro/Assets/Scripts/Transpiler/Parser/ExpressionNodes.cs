#nullable enable
using System.Collections.Generic;
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
            CompilationError NotExpectedTypeOfVariable = new NotExpectedTypeOfVariable(GetLine(), type.ToString(), expected.ToString());
            throw NotExpectedTypeOfVariable;
        }

    }
    /// <summary>
    /// Returns the line where the expression is for error handling purposes
    /// </summary>
    /// <returns></returns>
    public abstract int GetLine();
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
        //If the op is assign expression we should not validate the left expression
        if (Op.Definition is not TokenType.Assign)
            Left.Validate(scope);
        //else
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

    public override int GetLine()
    {
        return Op.Line;
    }
}
public class LiteralExpression : Expression
{
    public Token Value {get; }
    public LiteralExpression(Token value) {
        this.Value = value;
    }
    public override void Validate(IScope scope)
    {
        TokenType type = Value.Definition;
        //returns true is it is not an id
        if (type is TokenType.Num || type is TokenType.String
        || type is TokenType.Boolean || scope.IsDefined(Value.Value)
        || Utils.Types.ContainsKey(Value.Value)) {
            return;
        }
        
        //Throw error 
        CompilationError NotDefinedVariable = new NotDefinedVariable(Value);
        throw NotDefinedVariable;
    }

    public override int GetLine()
    {
        return Value.Line;
    }

    public override IdType GetType(IScope? scope)
    {
        //If is an id or a reserved word id return its type
        if ((Value.Definition is TokenType.Id || Utils.PropertiesReservedWords.Contains(Value.Definition)
        || Utils.Types.ContainsKey(Value.Value)) 
        && scope != null) {
            if (Utils.Types.TryGetValue(Value.Value, out IdType value)) {
                //This can be null
                return value;
            }
            if (Utils.Types.ContainsKey(Value.Value)) return Utils.Types[Value.Value];
            return scope.GetIdType(Value.Value, Value);
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
        return Value.Value == "true";
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
            int num = int.Parse(this.Value.Value);
            return num;
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
    public Expression ID { get; }
    public Token Op {get; }
    bool AtTheEnd { get; }
    public UnaryExpression(Expression id, Token op, bool atTheEnd) {
        this.ID = id;
        this.Op = op;
        this.AtTheEnd = atTheEnd;
    }

    public override int GetLine()
    {
        return Op.Line;
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
        BinaryExpression binaryExpression; 
        //Define with the new variable value, transforming the unary expression in a binary expression
        if (Op.Definition == TokenType.Increment) {
            binaryExpression = 
            new BinaryExpression(ID, new Token("+=", TokenType.MoreAssign), new LiteralExpression(new Token("1", TokenType.Num)));
        }
        else {
            binaryExpression = 
            new BinaryExpression(ID, new Token("-=", TokenType.MinusAssign), new LiteralExpression(new Token("1", TokenType.Num)));
        }

        binaryExpression.Execute(scope);
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

    public override int GetLine()
    {
        return Body.GetLine();
    }

    public override IdType GetType(IScope scope)
    {
        return IdType.CardCollection;
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
    public override int GetLine()
    {
        return FunctionName.Line;
    }
    public override IdType GetType(IScope scope)
    {
        //Return the type that the function returns
        return Utils.Types[FunctionName.Value];
    }

    public override void Validate(IScope scope)
    {
        IdType leftType = LeftExpression.GetType(scope);
        //Check that the left expression match the function name
        if (!Utils.ValidAccess.ContainsKey(leftType) || !Utils.ValidAccess[leftType].Contains(FunctionName.Value))
        {
            CompilationError NotValidAccessToFunctions = new NotValidAccessToFunctions(leftType.ToString(), FunctionName.Value, GetLine());
            throw NotValidAccessToFunctions;
        }
        //Check if the functionName match with the arguments
        string body = FunctionName.Value;
        if (!Utils.ValidArguments.TryGetValue(body, out IdType? value) || value != (Argument?.GetType(scope)))
        {
            string Type = Argument.GetType(scope).ToString();
            string TypeExpected = Utils.ValidArguments[body].ToString();

            CompilationError NotValidArgument = new NotValidArgument(Type, TypeExpected, body, GetLine());
            throw NotValidArgument;
        }
    }

    public override object Execute(IExecuteScope scope)
    {
        object body = LeftExpression.Execute(scope);
        if (body is CardCollection collection)
        {
            return Executer.CollectionFunctions[FunctionName.Value](Argument, collection, scope, GetLine());
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

    public override int GetLine()
    {
        return Body.GetLine();
    }

    public override void Validate(IScope scope)
    {
        Body.CheckType(scope, IdType.CardCollection);
        Index.CheckType(scope, IdType.Number);
    }

    public override object Execute(IExecuteScope scope)
    {
        CardCollection body = (CardCollection)Body.Execute(scope);
        int index = (int)Index.Execute(scope);

        if (index >= body.Count)
        {
            ExecutionError IndexOutOfRange = new Ex_IndexOutOfRange(GetLine());
            throw IndexOutOfRange;
        }
        return body[index];
    }
}