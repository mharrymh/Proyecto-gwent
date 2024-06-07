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
    string Name {get; }

    public NameField(string name)
    {
        this.Name = name;
    }
}

public class ParamField : Program
{
    Dictionary<string, string> VarDeclararions {get; }

    public ParamField(Dictionary<string, string> vars)
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

public class InstructionBlock
{
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
    string Type {get; }

    public TypeField(string type)
    {
        this.Type = type;
    }   
}

public class FactionField : Program
{
    string Faction {get; }
    public FactionField(string faction)
    {
        this.Faction = faction;
    }
}

public class PowerField : Program
{
    NumericExpression Num {get; }
    public PowerField(NumericExpression num)
    {
        this.Num = num;
    }
}

public abstract class NumericExpression : Program
{

}
public class BinaryExpression : NumericExpression
{
    NumericExpression Left { get; }
    char Op {get; }
    NumericExpression Right {get; }

    public BinaryExpression(NumericExpression left, char op, NumericExpression right) {
        this.Left = left;
        this.Op = op;
        this.Right = right;
    }
}

public class LiteralExpression : NumericExpression
{
    int Value {get; }

    public LiteralExpression(int value) {
        this.Value = value;
    }
}

public class RangeField : Program
{
    List<string> Ranges {get; }
    public RangeField(List<string> ranges)
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
    Selector Selector{get; }
    PostActionBlock? PostAction {get; }

    public EffectAllocation(Allocation allocation, Selector selector, PostActionBlock? postAction)
    {
        this.Allocation = allocation;
        this.Selector = selector;
        this.PostAction = postAction;
    }
}

public class Allocation : Program
{
    string? Name {get; }
    NameField? NameField {get; }
    VarAllocation? VarAllocation{get; }
    public Allocation(string? Name, NameField? NameField, VarAllocation? VarAllocation)
    {
        this.Name = Name;
        this.NameField = NameField;
        this.VarAllocation = VarAllocation;
    }
}

public class VarAllocation
{
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
    string SourceValue {get; }

    public Source(string source)
    {
        this.SourceValue = source;
    }
}
public class SingleField : Program
{
    BooleanExpression BoolExpression{get; }
    public SingleField(BooleanExpression boolExpression)
    {
        this.BoolExpression = boolExpression;
    }
}

public class BooleanExpression
{
}

public class Predicate : Program
{
    PredicativeExpression PredicateExp {get;}
    public Predicate(PredicativeExpression predicate)
    {
        this.PredicateExp = predicate;
    }
}


public class PostActionBlock : Program
{
    EffectAllocationBlock EffectBlock {get; }
    public PostActionBlock(EffectAllocationBlock effectBlock)
    {
        this.EffectBlock = effectBlock;
    }
}
public class PredicativeExpression
{
}






