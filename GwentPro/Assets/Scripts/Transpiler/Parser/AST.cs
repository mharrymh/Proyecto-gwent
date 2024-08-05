using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* SUMMARY

ERRORES EN EL EVALUATE
MUCHO CATCH 

*/

using System.Data;
using System.Runtime.CompilerServices;
using System.Security.Principal;
/// <summary>
/// Represent an object in the DSL
/// </summary>
public abstract class DSL_Object
{
    /// <summary>
    /// Analyze the object semantyc
    /// </summary>
    /// <param name="scope"></param>
    public abstract void Validate(IScope scope);
}
/// <summary>
/// Represent a block of declarations
/// </summary>
public class DecBlock : DSL_Object
{
    public List<Effect> Effects {get; set;}
    public List<Card> Cards{get; set;}
    public DecBlock(List<Effect> eff, List<Card> card)
    {
        this.Effects = eff;
        this.Cards = card;
    }
    //Validates all the effects and cards creating a new subyacent scope for each one
    public override void Validate(IScope scope)
    {
        //Effects had to be declared before cards
        foreach(Effect effect in Effects)
        {
            effect.Validate(scope.CreateChildContext());
        }
        foreach(Card card in Cards)
        {
            card.Validate(scope.CreateChildContext());
        }
    }
}
#region EffectNodes
/// <summary>
/// Represents an effect declaration
/// </summary>
public class Effect : DSL_Object
{
    Expression Name {get;}
    Dictionary<Token, Token>? Param {get; }
    InstructionBlock Action {get; }
    public Effect(Expression name, Dictionary<Token, Token>? param, InstructionBlock action)
    {
        this.Name = name;
        this.Param = param;
        this.Action = action;
    }

    public override void Validate(IScope scope)
    {
        Name.ValidateAndCheck(scope, IdType.String);
        
        if (Param != null) scope.DefineParams((string)Name.Evaluate(), Param);
        Action.Validate(scope.CreateChildContext());
    }
}
/// <summary>
/// Represent an action declaration
/// </summary>
public class InstructionBlock : DSL_Object
{
    //Target and context are defined always in the OnActivation
    Token Targets {get;}
    Token Context {get;}
    //Statements: ForLoops, WhileLoops, Expressions
    List<Statement> Statements {get;}
    public InstructionBlock(List<Statement> statements, Token targets, Token context)
    {
        this.Statements = statements;
        this.Targets = targets;
        this.Context = context;
    }

    public override void Validate(IScope scope)
    {
        //Check that targets and context are correct, just id types
        if (!scope.IsDefined(Targets.Value))
            //The value is defined after in the selector
            scope.Define(Targets.Value, IdType.CardCollection);
        if (!scope.IsDefined(Context.Value))
            //It has no value, context define all context in the game
            scope.Define(Context.Value, IdType.Context);
        
        foreach (Statement statement in Statements)
        {
            statement.Validate(scope);
        }
    }
}

/// <summary>
/// Represents an instruction in the DSL
/// </summary>
public abstract class Statement : DSL_Object {}
/// <summary>
/// Represent a for loop statement
/// </summary>
public class ForLoop : Statement
{
    /// <summary>
    /// The iterator of a for loop can be any name but it always represents a card
    /// </summary>
    /// <value></value>
    Token Iterator {get; }
    Expression Collection {get;}
    InstructionBlock Instructions {get; }
    public ForLoop(InstructionBlock instructions, Token iterator, Expression collection)
    {
        this.Instructions = instructions;
        this.Iterator = iterator;
        this.Collection = collection;
    }
    public override void Validate(IScope scope)
    {
        //Define the iterator 
        scope.Define((string)Iterator.Value, IdType.Card);
        //Collection had to be already defined
        if (!scope.IsDefined((string)Collection.Evaluate())) 
        {
            //TODO:
            //Iterator is the token before where the collection is defined
            Error notDefinedCollection = new CollectionNotDefined(Iterator.Line, Iterator.Column);
            throw new Exception(notDefinedCollection.ToString());
        }

        //Create a new scope
        IScope child = scope.CreateChildContext();

        //Check that types are correct
        Collection.CheckType(child, IdType.CardCollection);

        //Validate instructions inside the for
        Instructions.Validate(child);
    }
}
/// <summary>
/// Represent a while loop statement
/// </summary>
public class WhileLoop : Statement
{
    Expression BoolExpression {get; }
    InstructionBlock Instructions {get; }

    public WhileLoop(Expression boolExpression, InstructionBlock instructions)
    {
        this.BoolExpression = boolExpression;
        this.Instructions = instructions;
    }
    public override void Validate(IScope scope)
    {
        //Create a new child scope
        IScope child = scope.CreateChildContext();
        //Validate the bool expression and the instruction inside
        BoolExpression.ValidateAndCheck(child, IdType.Boolean);
        Instructions.Validate(child);
    }
}
#endregion
#region Card
/// <summary>
/// Represent a card declaration in the DSL
/// </summary>
public class Card : DSL_Object
{
    Expression Name {get; }
    Expression Type {get; }
    Expression Faction {get; }
    //Power can be null for cards that doesn't have power
    Expression? Power {get; }
    //Range can be null for leader cards or another especial cards
    List<Expression>? Range {get; }
    List<EffectAllocation> Activation {get; }

    public Card(Expression name, Expression type, Expression faction, Expression? power,
    List<Expression>? range, List<EffectAllocation> activation)
    {
        this.Name = name;
        this.Type = type;
        this.Faction = faction;
        this.Power = power;
        this.Range = range;
        this.Activation = activation;
    }

    public override void Validate(IScope scope)
    {
        if (Range != null) {
            foreach(Expression exp in Range)
            {
                exp.Validate(scope);
                //Check that all expressions in range are string expressions
                exp.CheckType(scope, IdType.String);
            }
        }
        
        foreach(EffectAllocation effect in Activation)
        {
            effect.Validate(scope.CreateChildContext());
        }
        Name.ValidateAndCheck(scope, IdType.String);
        Type.ValidateAndCheck(scope, IdType.String);
        Faction.ValidateAndCheck(scope, IdType.String);
        //It calls the validate function only if power is not null
        Power?.ValidateAndCheck(scope, IdType.Number);
    }
}
/// <summary>
/// Represent an effect assignment in the DSL
/// </summary>
public class EffectAllocation : DSL_Object
{
    Allocation Allocation {get; }
    Selector? Selector{get; }
    PostActionBlock? PostAction {get; }

    public EffectAllocation(Allocation allocation, Selector? selector, PostActionBlock? postAction)
    {
        this.Allocation = allocation;
        this.Selector = selector;
        this.PostAction = postAction;
    }

    public override void Validate(IScope scope)
    {
        Allocation.Validate(scope);
        Selector?.Validate(scope);
        PostAction?.Validate(scope);
    }
}
/// <summary>
/// Represent the declaration of the card effect with its name and params assignments
/// </summary>
public class Allocation : DSL_Object
{
    Expression Name {get; }
    Dictionary<Token, Expression>? VarAllocation {get; }
    public Allocation(Expression name, Dictionary<Token, Expression>? varAllocation)
    {
        this.Name = name;
        this.VarAllocation = varAllocation;
    }

    public override void Validate(IScope scope)
    {
        Name.ValidateAndCheck(scope, IdType.String);
        DefinedActions.CheckValidParameters((string)Name.Evaluate(), VarAllocation, scope);
    }
}
/// <summary>
/// Assign the card collection that will be affected by the effect activation
/// </summary>
public class Selector : DSL_Object 
{
    /// <summary>
    /// Card collection from where the cards are get 
    /// </summary>
    /// <value></value>
    Expression Source {get; }
    /// <summary>
    /// Represent if we only affect a single card in the collection or all of them
    /// </summary>
    /// <value>By default is false</value>
    Expression? Single {get; }
    /// <summary>
    /// Predicate used to restrict source even more
    /// </summary>
    /// <value></value>
    Predicate Predicate {get; }
    public Selector(Expression source, Expression? single, Predicate predicate)
    {
        this.Source = source;
        this.Single = single;
        this.Predicate = predicate;
    }
    public override void Validate(IScope scope)
    {
        Source.ValidateAndCheck(scope, IdType.String);
        Single?.ValidateAndCheck(scope, IdType.Boolean);
        Predicate.Validate(scope);
    }
}

public class Predicate : DSL_Object
{
    Token Id {get; }
    Expression BoolExp {get; }
    public Predicate(Token id, Expression boolExp)
    {
        this.Id = id;
        this.BoolExp = boolExp;
    }

    public override void Validate(IScope scope)
    {
        scope.Define(Id.Value, IdType.Card);
        BoolExp.ValidateAndCheck(scope, IdType.Boolean);
    }
}   
public class PostActionBlock : DSL_Object
{
    EffectAllocation EffectBlock {get; }
    public PostActionBlock(EffectAllocation effectBlock)
    {
        this.EffectBlock = effectBlock;
    }

    public override void Validate(IScope scope)
    {
        EffectBlock.Validate(scope);
    }
}
#endregion