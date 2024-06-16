namespace Transpiler;
public abstract class DSL_Object
{}

//Root of all classes
public class DecBlock : DSL_Object
{
    List<Effect> Effects {get; set;}
    List<Card> Cards{get; set;}
    public DecBlock(List<Effect> eff, List<Card> card)
    {
        this.Effects = eff;
        this.Cards = card;
    }
}

public class Effect : DSL_Object
{
    Expression Name {get; }
    Dictionary<Token, Token>? Param {get; }
    InstructionBlock Action {get; }
    public Effect(Expression name, Dictionary<Token, Token>? param, InstructionBlock action)
    {
        this.Name = name;
        this.Param = param;
        this.Action = action;
    }
}

public class InstructionBlock : DSL_Object
{
    List<ForLoop> ForLoops {get; }
    List<WhileLoop> WhileLoops {get; }
    List<Expression> IdExpressions {get; }
    public InstructionBlock(List<ForLoop> forLoops, List<WhileLoop> whileLoops, List<Expression> idExpressions)
    {
        this.ForLoops = forLoops;
        this.WhileLoops = whileLoops;
        this.IdExpressions = idExpressions;
    } 
}

public class ForLoop : DSL_Object
{
    InstructionBlock Instructions {get; }
    public ForLoop(InstructionBlock instructions)
    {
        this.Instructions = instructions;
    }
}

public class WhileLoop : DSL_Object
{
    Expression BoolExpression {get; }
    InstructionBlock Instructions {get; }

    public WhileLoop(Expression boolExpression, InstructionBlock instructions)
    {
        this.BoolExpression = boolExpression;
        this.Instructions = instructions;
    }
}
public class Card : DSL_Object
{
    Expression Name {get; }
    Expression Type {get; }
    Expression Faction {get; }
    Expression? Power {get; }
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
}

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
}

public class Allocation : DSL_Object
{
    Expression Name {get; }
    Dictionary<Token, Expression>? VarAllocation {get; }
    public Allocation(Expression name, Dictionary<Token, Expression>? varAllocation)
    {
        this.Name = name;
        this.VarAllocation = varAllocation;
    }
}

public class Selector : DSL_Object 
{
    Expression Source {get; }
    Expression? Single {get; }
    Predicate Predicate {get; }
    public Selector(Expression source, Expression? single, Predicate predicate)
    {
        this.Source = source;
        this.Single = single;
        this.Predicate = predicate;
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
}
public class PostActionBlock : DSL_Object
{
    EffectAllocation EffectBlock {get; }
    public PostActionBlock(EffectAllocation effectBlock)
    {
        this.EffectBlock = effectBlock;
    }
}