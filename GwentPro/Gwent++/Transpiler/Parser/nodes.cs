namespace Transpiler; 

//Program
public abstract class Expression
{}

public class Declarations : Expression
{
    Expression Left { get; }
    Expression Right { get; } 

    public Declarations(Expression left, Expression right)
    {
        this.Left = left;
        this.Right = right;
    }
}

public class Dec : Expression
{
    Expression EfDec { get; } 
    Expression CardDec { get; } 

    public Dec(Expression EfDec, Expression CardDec)
    {
        this.EfDec = EfDec;
        this.CardDec = CardDec;
    }
}
public class EfDec : Expression
{
    Expression EfProps { get; } 

    public EfDec(Expression efProps)
    {
        this.EfProps = efProps;
    }
}

public class EfProperties : Expression
{
    Expression Efprop { get; } 
    Expression Efprops { get; } 

    public EfProperties(Expression efProp, Expression efProps)
    {
        this.Efprop = efProp;
        this.Efprops = efProps;
    }
}

public class EfProp : Expression 
{
    NameField NameField { get; } 
    ParamsField ParamsField { get; } 
    ActionField ActionField { get; } 

    public EfProp(NameField nameField, ParamsField paramsField, ActionField actionField)
    {
        this.NameField = nameField;
        this.ParamsField = paramsField;
        this.ActionField = actionField;
    }
}

public class NameField : Expression
{
    string Value {get; } 
    public NameField(string value)
    {
        this.Value = value;
    }
}

public class ParamsField : Expression
{
    Definitions Def {get ;} 

    public ParamsField(Definitions def)
    {
        this.Def = def;
    }
}

public class Definitions : Expression
{
    Definition Left {get; } 
    Definitions Right {get ;}

    public Definitions(Definition left, Definitions right)
    {
        this.Left = left;
        this.Right = right;
    }
}

public class Definition : Expression
{
    string Id {get; }
    char Colon {get; } 
    string ValueType {get; } 

    public Definition(string id, char colon, string valueType)
    {
        this.Id = id;
        this.Colon = colon;
        this.ValueType = valueType;
    }
}

public class ActionField : Expression
{
    InstructionBlock InstructionBlock{get; }

    public ActionField(InstructionBlock instructionBlock)
    {
        this.InstructionBlock = instructionBlock;
    }
}

public class InstructionBlock
{

}

public class CardDec : Expression
{
    CardProperties CardProperties {get; }
    public CardDec(CardProperties cardProperties)
    {
        this.CardProperties = cardProperties;
    }
}

public class CardProperties : Expression
{
    NameField NameField {get; }
    TypeField TypeField{get; }
    FactionField FactionField{get; }
    PowerField PowerField{get; }
    RangeField RangeField{get; }
    ActivationField ActivationField{get; }

    public CardProperties(NameField nameField, TypeField typeField, 
    FactionField factionField, PowerField powerField, RangeField rangeField, 
    ActivationField activationField)
    {
        this.NameField = nameField;
        this.TypeField = typeField;
        this.FactionField = factionField;
        this.PowerField = powerField;
        this.RangeField = rangeField;
        this.ActivationField = activationField;
    }

}

public class ActivationField : Expression
{
    CardEffects CardEffects{get; }

    ActivationField(CardEffects cardEffects)
    {
        this.CardEffects = cardEffects;
    }
}

public class CardEffects : Expression
{
    CardEffect Left {get; }
    CardEffects Right {get; }

    public CardEffects(CardEffect left, CardEffects right)
    {
        this.Left = left;
        this.Right = right;     
    }
}

public class CardEffect : Expression
{
    CardEffectProperties Props {get; }

    public CardEffect(CardEffectProperties props)
    {
        this.Props = props;
    }
}

public class CardEffectProperties : Expression
{
    CardEffectProp Left {get; }
    CardEffectProperties Right {get; }

    public CardEffectProperties(CardEffectProp left, CardEffectProperties right)
    {
        this.Left = left;
        this.Right = right;
    }
}

public class CardEffectProp : Expression
{
    CardEffDeclaration CardEffDeclaration{get; }
    SelectorField SelectorField{get; }
    PostActionProps PostActionField {get; }

    public CardEffectProp(CardEffDeclaration cardeff, SelectorField selectorField, PostActionProps postAction)
    {
        this.CardEffDeclaration = cardeff;
        this.SelectorField = selectorField;
        this.PostActionField = postAction;
    }
}

public class PostActionProps : Expression
{
    PostActionProps Left{get; }
    PostActionProp Right{get; }

    public PostActionProps(PostActionProps left, PostActionProp right)
    {
        this.Left=left;
        this.Right=right;
    }
}

public class PostActionProp : Expression
{
    CardEffProps EffProps {get; }
    public PostActionProp(CardEffProps effProps)
    {
        this.EffProps = effProps;
    }
}

public class SelectorField : Expression
{
    SelectorProps SelectorProps {get; }

    public SelectorField(SelectorProps selectorProps)
    {
        this.SelectorProps = selectorProps;
    }
}

public class SelectorProps : Expression
{
    SelectorProp Left {get; }
    SelectorProps Right {get; }

    public SelectorProps(SelectorProp left, SelectorProps right)
    {
        this.Left = left;
        this.Right= right;
    }
}

public class SelectorProp : Expression
{
    Source SourceField {get; }
    SingleField SingleField {get; }
    Predicate Predicate {get; }
    public SelectorProp(Source source, SingleField single, Predicate predicate)
    {
        this.SourceField = source;
        this.SingleField = single;
        this.Predicate = predicate;
    }
}

public class Source : Expression
{
    string Value {get; }
    public Source(string value)
    {
        this.Value = value;
    }
}

public class SingleField : Expression
{
    BooleanExpression Expression {get; }

    public SingleField(BooleanExpression Expression)
    {
        this.Expression = Expression;
    }
}

public class Predicate : Expression
{
    PredicativeExpression Expression {get; }

    public Predicate(PredicativeExpression Expression)
    {
        this.Expression = Expression;
    }
}
public class CardEffDeclaration : Expression
{
    EffectBlock EffectBlock{get; }

    public CardEffDeclaration(EffectBlock effectBlock)
    {
        this.EffectBlock = effectBlock;
    }
}

public class EffectBlock : Expression
{
    CardEffProps CardEffProps{get; set;}
    string AuxString {get; }

    public EffectBlock(CardEffProps cardEffProps, string nameEffect)
    {
        this.CardEffProps = cardEffProps;
        this.AuxString = nameEffect;
    }
}

public class CardEffProps : Expression
{
    CardEffProp Left {get; }
    CardEffProps Right {get;}

    public CardEffProps(CardEffProp left, CardEffProps right)
    {
        this.Left = left;
        this.Right = right;
    }
}
public class CardEffProp : Expression
{
    EffDeclaration EffDeclaration {get; }
    SelectorField SelectorField {get; }
    PostActionProps PostAction{get; }

    public CardEffProp(EffDeclaration effDeclaration, SelectorField selectorField, PostActionProps postAction)
    {
        this.EffDeclaration = effDeclaration;
        this.SelectorField = selectorField;
        this.PostAction = postAction;
    }
}

public class EffDeclaration : Expression
{
    EffBlock EffBlock{get; }

    public EffDeclaration(EffBlock effBlock)
    {
        this.EffBlock = effBlock;
    }
}

public class EffBlock : Expression
{
    EffBlockProps EffBlockProps{get; }
    string AuxString {get; }

    public EffBlock(EffBlockProps effBlockProps, string auxString)
    {
        this.EffBlockProps = effBlockProps;
        this.AuxString = auxString;
    }
}

public class EffBlockProps : Expression
{
    EffBlockProp Left{get; }
    EffBlockProps Right {get;}

    EffBlockProps(EffBlockProp left, EffBlockProps right)
    {
        this.Left = left;
        this.Right = right;
    }
}

public class EffBlockProp : Expression
{
    NameField NameField {get; }
    Asignations Asignations{get; }

    public EffBlockProp(NameField name, Asignations asignations)
    {
        this.NameField = name;
        this.Asignations = asignations;
    }
}
public class RangeField : Expression
{
    string[] Ranges {get; }
    public RangeField(string[] ranges)
    {
        this.Ranges = ranges;
    }
}

public class PowerField : Expression
{
    NumericExpression NumericExpression {get; }
    public PowerField(NumericExpression numericExpression)
    {
        this.NumericExpression = numericExpression;
    }
}

public class FactionField : Expression
{
    string FactionName {get; }
    public FactionField(string factionName)
    {
        this.FactionName = factionName;
    }
}
public class TypeField : Expression
{
    string TypeValue {get; }
    public TypeField(string typeValue)
    {
        this.TypeValue = typeValue;
    }
}

public class Asignations : Expression
{

}

public class NumericExpression : Expression
{

}

public class BooleanExpression : Expression
{

}

public class PredicativeExpression : Expression
{

}