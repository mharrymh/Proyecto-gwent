
using System.Runtime.CompilerServices;
using System.Security.Principal;

namespace Transpiler;
//Abstract root of all classes
public abstract class DSL_Object
{
    public abstract bool Validate(IContext context);
}

public class DecBlock : DSL_Object
{
    public List<Effect> Effects {get; set;}
    public List<Card> Cards{get; set;}
    public DecBlock(List<Effect> eff, List<Card> card)
    {
        this.Effects = eff;
        this.Cards = card;
    }

    //Validates all the effects and cards creating a new subyacent context for each one
    public override bool Validate(IContext context)
    {
        foreach(Effect effect in Effects)
        {
            if(!effect.Validate(context.CreateChildContext())) return false;
        }
        foreach(Card card in Cards)
        {
            if (!card.Validate(context.CreateChildContext())) return false;
        }
        return true;
    }
}

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

    public override bool Validate(IContext context)
    {
        if (!Name.Validate(context)) return false;

        //TODO: NAME DEBE SER UN STRING YA 
        return Action.Validate(context) && (Param == null || context.Define(Name.Convert(), Param));
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

    public override bool Validate(IContext context)
    {
        foreach (ForLoop forLoop in ForLoops)
        {
            if (!forLoop.Validate(context)) return false;
        }
        foreach (WhileLoop whileLoop in WhileLoops)
        {
            if (!whileLoop.Validate(context)) return false;
        }
        foreach (Expression exp in IdExpressions)
        {
            if(!exp.Validate(context)) return false;
        }
        return true;
    }
}

public class ForLoop : DSL_Object
{
    InstructionBlock Instructions {get; }
    public ForLoop(InstructionBlock instructions)
    {
        this.Instructions = instructions;
    }

    public override bool Validate(IContext context)
    {
        return Instructions.Validate(context.CreateChildContext());
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
    public override bool Validate(IContext context)
    {
        return Instructions.Validate(context.CreateChildContext());
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

    public override bool Validate(IContext context)
    {
        if (Range != null)
            foreach(Expression exp in Range)
            {
                    if (!exp.Validate(context)) return false;
            }
        foreach(EffectAllocation effect in Activation)
        {
            if (!effect.Validate(context)) return false;
        }
        return Name.Validate(context) && Type.Validate(context) && Faction.Validate(context)
        && (Power == null || Power.Validate(context));
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

    public override bool Validate(IContext context)
    {
        return Allocation.Validate(context) && 
        (Selector == null || Selector.Validate(context)) && 
        (PostAction == null || PostAction.Validate(context));
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

    public override bool Validate(IContext context)
    {
        return Name.Validate(context) && (VarAllocation == null || ValidateVarAllocation(VarAllocation, context));
    }

    //TODO:
    private bool ValidateVarAllocation(Dictionary<Token, Expression> varAllocation, IContext context)
    {
        throw new NotImplementedException();
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

    public override bool Validate(IContext context)
    {
        return Source.Validate(context) 
        && (Single == null || Single.Validate(context)) 
        && Predicate.Validate(context);
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

    public override bool Validate(IContext context)
    {
        //TODO: CONTEXT IS DEFINED??
        return context.IsDefined(Id) && BoolExp.Validate(context);
    }
}
public class PostActionBlock : DSL_Object
{
    EffectAllocation EffectBlock {get; }
    public PostActionBlock(EffectAllocation effectBlock)
    {
        this.EffectBlock = effectBlock;
    }

    public override bool Validate(IContext context)
    {
        return EffectBlock.Validate(context);
    }
}
