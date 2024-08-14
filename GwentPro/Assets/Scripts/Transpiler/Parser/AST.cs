#nullable enable
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
using System;
using Unity.VisualScripting;
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
    public List<Effect_Object> Effects {get; set;}
    public List<Card_Object> Cards{get; set;}
    public DecBlock(List<Effect_Object> eff, List<Card_Object> card)
    {
        this.Effects = eff;
        this.Cards = card;
    }
    //Validates all the effects and cards creating a new subyacent scope for each one
    public override void Validate(IScope scope)
    {
        //Effects had to be declared before cards
        foreach(Effect_Object effect in Effects)
        {
            effect.Validate(scope.CreateChildContext());
        }
        foreach(Card_Object card in Cards)
        {
            card.Validate(scope.CreateChildContext());
        }
    }
    /// <summary>
    /// Evaluate the declaration block evaluating each card and saving it in a list
    /// Also save all effects in an static class SavedEffects
    /// </summary>
    /// <returns>Return a list of defined cards</returns>
    public List<ICard> Evaluate()
    {
        //Save all effects in an static class
        foreach (Effect_Object effect_Object in Effects)
        {
            effect_Object.SaveEffect();
        }
        //Instantiate the list we will return
        List<ICard> cards = new List<ICard>();
        foreach (Card_Object card in Cards)
        {
            cards.Add((ICard)card.Evaluate());
        }
        return cards;
    }
}
#region EffectNodes
/// <summary>
/// Represents an effect declaration
/// </summary>
public class Effect_Object : DSL_Object
{
    Expression Name {get;}
    Dictionary<Token, Token>? Param {get; }
    InstructionBlock Action {get; }
    public Effect_Object(Expression name, Dictionary<Token, Token>? param, InstructionBlock action)
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

    public void SaveEffect()
    {
        string effectName = (string)Name.Evaluate();

        List<string> effectParams = new List<string>();
        if (Param != null)
        {
            foreach (Token param in Param.Keys)
            {
                //Add the name of the token
                effectParams.Add(param.Value);
            }
        }

        (string targetsId, string contextId) = Action.GetTargetsAndContextIdName();

        DeclaredEffect effect = new DeclaredEffect(effectName, effectParams, Action, targetsId, contextId);
        //Add unfinished effect to the static class
        DeclaredEffects.AddEffect(effect);
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

    public void Execute(IExecuteScope scope)
    {
        foreach (Statement statement in Statements)
        {
            statement.Execute(scope);
        }
    }

    internal (string targetsId, string contextId) GetTargetsAndContextIdName()
    {
        return (Targets.Value, Context.Value);
    }

}

/// <summary>
/// Represents an instruction in the DSL
/// </summary>
public abstract class Statement : DSL_Object {
    public abstract object Execute(IExecuteScope scope);
}
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
        Collection.CheckType(scope, IdType.CardCollection);

        //Create a new scope
        IScope child = scope.CreateChildContext();

        //Check that types are correct
        Collection.CheckType(child, IdType.CardCollection);

        //Validate instructions inside the for
        Instructions.Validate(child);
    }

    public override object Execute(IExecuteScope scope)
    {
        CardCollection cardCollection = (CardCollection)Collection.Execute(scope);
        foreach (Card card in cardCollection)
        {
            scope.Define(Iterator.Value, card);
            Instructions.Execute(scope);
        }
        return null;
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

    public override object Execute(IExecuteScope scope)
    {
        while((bool)BoolExpression.Execute(scope))
        {
            Instructions.Execute(scope);
        }
        return null;
    }
}
#endregion
#region Card
/// <summary>
/// Represent a card declaration in the DSL
/// </summary>
public class Card_Object : DSL_Object
{
    Expression Name {get; }
    Expression Type {get; }
    Expression Faction {get; }
    //Power can be null for cards that doesn't have power
    Expression? Power {get; }
    //Range can be null for leader cards or another especial cards
    List<Expression>? Range {get; }
    List<EffectAllocation> Activation {get; }

    public Card_Object(Expression name, Expression type, Expression faction, Expression? power,
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
    /// <summary>
    /// It evaluates each property of the card and save it in an MyCardObject
    /// </summary>
    /// <returns>ICard(MyCard) </returns>
    public ICard Evaluate()
    {
        string name = (string)Name.Evaluate();
        string type = (string)Type.Evaluate();
        //Check that the type is valid
        if (!CardConverter.relateType.ContainsKey(type))
        {
            //TODO: THROW NEW EXCEPTION, ESE TYPE NO ESTA PERMITIDO
            throw new Exception();
        }
        string faction_string = (string)Faction.Evaluate();
        //This convert the string of the faction to an actual faction as an enum
        CardFaction faction = ConvertToCardFaction(faction_string);
        //if power is null it is 0
        int power = (Power == null)? 0 : (int)Power.Evaluate();
        //If range is null it is an empty string
        string range = (Range == null)? "" : ChangeFormatOfRange(Range);
        
        List<DeclaredEffect> effects = new List<DeclaredEffect>();
        //Fill the card effects
        foreach(EffectAllocation effectAllocation in Activation)
        {
            effects.Add(effectAllocation.Evaluate());
        }

        return new MyCard(name, type, faction, range, power, effects);
    }
    private string ChangeFormatOfRange(List<Expression> range)
    {
        //Transform expression to strings 
        List<string> rangeString = new List<string>();
        foreach (Expression exp in range)
        {
            rangeString.Add((string)exp.Evaluate());
        }

        string correctFormatRange = "";
        //Save valid ranges 
        List<string> ValidRanges = new List<string> {"Melee", "Ranged", "Siege"};
        for (int i = 0; i < rangeString.Count; i++)
        {
            if (ValidRanges.Contains(rangeString[0])) {
                correctFormatRange += rangeString[0];
                //Remove it so it cant be ranges repeated 
                ValidRanges.Remove(rangeString[0]);
            }
            //TODO: Rango no valido
            else throw new Exception("");
        }
        return correctFormatRange;
    }

    /// <summary>
    /// Check that faction exists and change the string to the actual CardFaction
    /// </summary>
    /// <param name="faction_string"></param>
    /// <returns></returns>
    private CardFaction ConvertToCardFaction(string faction_string)
    {
        Dictionary<string, CardFaction> relate_faction = new Dictionary<string, CardFaction>
        {
            { "Dark", CardFaction.Dark},
            { "Light", CardFaction.Light}
        };

        if (relate_faction.ContainsKey(faction_string))
        {
            return relate_faction[faction_string];
        }
        //TODO: LANZAR EXCEPCION DE QUE ESA FACCION NO EXISTE EN EL JUEGO O NO ES VALIDA
        else throw new Exception();
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
    /// <summary>
    /// It returns an DeclaredEffect with the evaluated effect
    /// </summary>
    /// <param name="PostEffect">It represents if the effect is a postAction decared effect or not</param>
    /// <returns>The completed effect</returns>
    public DeclaredEffect Evaluate(DeclaredEffect? parent = null)
    {
        (string name, Dictionary<string, object> paramsDeclared) = Allocation.Evaluate();

        //Find the effect with that name and fill its parameters
        DeclaredEffect usedEffect = DeclaredEffects.Find(name);
        //Fill the values of the effect
        usedEffect.FillParamsValues(paramsDeclared);

        //Check that selector in not null
        if (parent == null && Selector == null)
        {
            //TODO: Los unicos efectos que pueden ser declarados sin selector son los post efectos
            throw new Exception();
        }

        CardCollection effectTargets = new CardCollection();
        if (Selector != null)
        {
            //Get the cards that match the selector
            usedEffect.Targets = Selector.Evaluate();
        }
        else //It is a post action effect
        {
            if (parent != null)
            usedEffect.Targets = parent.Targets;
        }
        //Fill the post effect
        if (PostAction != null)
        {
            usedEffect.PostEffect = PostAction.Evaluate(usedEffect);
            //Set the post effect parent
            usedEffect.PostEffect.Parent = usedEffect;
        }

        return usedEffect;
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
    /// <summary>
    /// It returns the name as string and the declared params with the name and the expression
    /// </summary>
    /// <param name="name"></param>
    /// <param name="Evaluate("></param>
    /// <returns></returns>
    public (string name, Dictionary<string, object> paramsDeclared) Evaluate()
    {
        string name = (string)Name.Evaluate();
        Dictionary<string, object> paramsDeclared = new Dictionary<string, object>();
        //Fill the paramsDEclaredDictionary 
        if (VarAllocation != null)
        {
            foreach (Token token in VarAllocation.Keys)
            {
                paramsDeclared.Add(token.Value, VarAllocation[token].Evaluate());
            }
        }

        return (name, paramsDeclared);
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

    public EffectSelector Evaluate()
    { 
        bool single = (bool)Single.Evaluate();
        string source = (string)Source.Evaluate();
        return new EffectSelector(source, single, Predicate);
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

    /// <summary>
    /// Define the card and check if the bool expression is true with that card
    /// </summary>
    /// <param name="card"></param>
    /// <param name="scope"></param>
    /// <returns></returns>
    public bool Execute(Card card, IExecuteScope scope)
    {
        scope.Define(Id.Value, card);

        return (bool)BoolExp.Execute(scope);
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

    public DeclaredEffect Evaluate(DeclaredEffect parent)
    {
        return EffectBlock.Evaluate(parent);
    }
}
#endregion