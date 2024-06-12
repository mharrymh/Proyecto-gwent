namespace Transpiler;
public abstract class Program
{}

public class DecBlock : Program
{
    EffDecBlock EffBlock {get; }
    CardDecBlock CardBlock {get; }

    public DecBlock(EffDecBlock eff, CardDecBlock card)
    {
        this.EffBlock = eff;
        this.CardBlock = card;
    }
}

public class EffDecBlock : Program
{
    Effect Effect {get; }
    EffDecBlock? EffBlock {get ; }

    public EffDecBlock(Effect eff, EffDecBlock? effBlock)
    {
        this.Effect = eff;
        this.EffBlock = effBlock;
    }
}

public class Effect : Program
{
    NameField Name {get; }
    ParamField? Param {get; }
    ActionField Action {get; }

    public Effect(NameField name, ParamField? param, ActionField action)
    {
        this.Name = name;
        this.Param = param;
        this.Action = action;
    }
}

public class NameField : Program
{
    Expression Name {get; }
    public NameField(Expression name)
    {
        this.Name = name;
    }
}

public class ParamField : Program
{
    Dictionary<Token, Token> VarDeclararions {get; }

    public ParamField(Dictionary<Token, Token> vars)
    {
        this.VarDeclararions = vars;
    }
}

public class ActionField : Program
{
    InstructionBlock Instruction {get; }

    public ActionField(InstructionBlock instruction)
    {
        this.Instruction = instruction;
    }
}
public class InstructionBlock : Program
{
    ForLoop? ForLoop {get; }
    WhileLoop? WhileLoop {get; }
    Expression? IdExpression {get; }
    InstructionBlock? Instruction {get; }

    public InstructionBlock(ForLoop? forLoop, WhileLoop? whileLoop, Expression? idExpression, InstructionBlock? instruction)
    {
        this.ForLoop = forLoop;
        this.WhileLoop = whileLoop;
        this.IdExpression = idExpression;
        this.Instruction = instruction;
    } 
}

public class ForLoop : Program
{
    InstructionBlock? Instructions {get; }
    public ForLoop(InstructionBlock? instructions)
    {
        this.Instructions = instructions;
    }
}

public class WhileLoop : Program
{
    Expression BoolExpression {get; }
    InstructionBlock? Instructions {get; }

    public WhileLoop(Expression boolExpression, InstructionBlock? instructions)
    {
        this.BoolExpression = boolExpression;
        this.Instructions = instructions;
    }
}


public class CardDecBlock : Program
{
    Card Card {get; }
    CardDecBlock? CardDeckBlock {get ; }

    public CardDecBlock(Card card, CardDecBlock? cardDecBlock)
    {
        this.Card = card;
        this.CardDeckBlock = cardDecBlock;
    }
}

public class Card : Program
{
    NameField Name {get; }
    TypeField Type {get; }
    FactionField Faction{get; }
    PowerField? Power{get; }
    RangeField Range{get; }
    ActivationField Activation{get; }

    public Card(NameField name, TypeField type, FactionField faction, PowerField? power,
    RangeField range, ActivationField activation)
    {
        this.Name = name;
        this.Type = type;
        this.Faction = faction;
        this.Power = power;
        this.Range = range;
        this.Activation = activation;
    }
}

public class TypeField : Program
{
    Expression Type {get; }

    public TypeField(Expression type)
    {
        this.Type = type;
    }   
}

public class FactionField : Program
{
    Expression Faction {get; }
    public FactionField(Expression faction)
    {
        this.Faction = faction;
    }
}

public class PowerField : Program
{
    Expression Num {get; }
    public PowerField(Expression num)
    {
        this.Num = num;
    }
}
public class RangeField : Program
{
    List<Expression> Ranges {get; }
    public RangeField(List<Expression> ranges)
    {
        this.Ranges = ranges;
    }
}

public class ActivationField : Program
{
    EffectAllocationBlock EffBlock {get; }
    public ActivationField(EffectAllocationBlock effBlock)
    {
        this.EffBlock = effBlock;
    } 
}

public class EffectAllocationBlock : Program
{
    EffectAllocation Effect {get; }
    EffectAllocationBlock? EffectBlock {get; }

    public EffectAllocationBlock(EffectAllocation effect, EffectAllocationBlock? effBlock)
    {
        this.Effect = effect;
        this.EffectBlock = effBlock;
    }
}

public class EffectAllocation : Program
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

public class Allocation : Program
{
    Expression? Name {get; }
    NameField? NameField {get; }
    VarAllocation? VarAllocation{get; }
    public Allocation(Expression? Name, NameField? NameField, VarAllocation? VarAllocation)
    {
        this.Name = Name;
        this.NameField = NameField;
        this.VarAllocation = VarAllocation;
    }
}

public class VarAllocation
{
    Dictionary<Token, Expression> VarAssignment {get; }

    public VarAllocation(Dictionary<Token, Expression> vars)
    {
        this.VarAssignment = vars;
    }
}

public class Selector : Program 
{
    Source Source {get; }
    SingleField? Single {get; }
    Predicate Predicate {get; }
    public Selector(Source source, SingleField? single, Predicate predicate)
    {
        this.Source = source;
        this.Single = single;
        this.Predicate = predicate;
    }
}

public class Source : Program
{
    Expression SourceValue {get; }

    public Source(Expression source)
    {
        this.SourceValue = source;
    }
}
public class SingleField : Program
{
    Expression BoolExpression{get; }
    public SingleField(Expression boolExpression)
    {
        this.BoolExpression = boolExpression;
    }
}
public class Predicate : Program
{
    Token Id {get; }
    Expression BoolExp {get; }
    public Predicate(Token id, Expression boolExp)
    {
        this.Id = id;
        this.BoolExp = boolExp;
    }
}
public class PostActionBlock : Program
{
    EffectAllocation EffectBlock {get; }
    public PostActionBlock(EffectAllocation effectBlock)
    {
        this.EffectBlock = effectBlock;
    }
}






