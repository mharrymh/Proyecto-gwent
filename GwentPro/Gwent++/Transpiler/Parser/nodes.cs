

// //Program
// public abstract class Expression
// {}

// public class Declarations : Expression
// {
//     Expression Left { get; }
//     Expression Right { get; } 
//     public Declarations(Expression left, Expression right)
//     {
//         this.Left = left;
//         this.Right = right;
//     }
// }

// public class Dec : Expression
// {
//     Expression EfDec { get; } 
//     Expression CardDec { get; } 

//     public Dec(Expression EfDec, Expression CardDec)
//     {
//         this.EfDec = EfDec;
//         this.CardDec = CardDec;
//     }
// }
// public class EfDec : Expression
// {
//     Expression EfProps { get; } 

//     public EfDec(Expression efProps)
//     {
//         this.EfProps = efProps;
//     }
// }

// public class EfProperties : Expression
// {
//     Expression Efprop { get; } 
//     Expression Efprops { get; } 

//     public EfProperties(Expression efProp, Expression efProps)
//     {
//         this.Efprop = efProp;
//         this.Efprops = efProps;
//     }
// }

// public class EfProp : Expression 
// {
//     NameField NameField { get; } 
//     Expression ParamsField { get; } 
//     Expression ActionField { get; } 

//     public EfProp(NameField nameField, Expression paramsField, Expression actionField)
//     {
//         this.NameField = nameField;
//         this.ParamsField = paramsField;
//         this.ActionField = actionField;
//     }
// }

// public class NameField : Expression
// {
//     string Value {get; } 
//     public NameField(string value)
//     {
//         this.Value = value;
//     }
// }

// public class ParamsField : Expression
// {
//     Expression Def {get ;} 

//     public ParamsField(Expression def)
//     {
//         this.Def = def;
//     }
// }

// public class Definitions : Expression
// {
//     Expression Def {get; } 
//     Expression Defs {get ;}

//     public Definitions(Expression def, Expression defs)
//     {
//         this.Def = def;
//         this.Defs = defs;
//     }
// }

// public class Definition : Expression
// {
//     string Id {get; }
//     string ValueType {get; } 

//     public Definition(string id, string valueType)
//     {
//         this.Id = id;
//         this.ValueType = valueType;
//     }
// }

// public class ActionField : Expression
// {
//     InstructionBlock InstructionBlock{get; }

//     public ActionField(InstructionBlock instructionBlock)
//     {
//         this.InstructionBlock = instructionBlock;
//     }
// }

// public class InstructionBlock
// {

// }

// public class CardDec : Expression
// {
//     CardProperties CardProperties {get; }
//     public CardDec(CardProperties cardProperties)
//     {
//         this.CardProperties = cardProperties;
//     }
// }

// public class CardProperties : Expression
// {
//     Expression CardProp {get; }
//     Expression CardProps {get; }

//     public CardProperties(Expression cardProp, Expression cardProps)
//     {
//         this.CardProp = cardProp;
//         this.CardProps = cardProps;
//     }
// }
// public class CardProp : Expression
// {
//     NameField NameField {get; }
//     TypeField TypeField{get; }
//     FactionField FactionField{get; }
//     PowerField PowerField{get; }
//     Expression RangeField{get; }
//     Expression ActivationField{get; }

//     public CardProp(NameField nameField, TypeField typeField, 
//     FactionField factionField, PowerField powerField, Expression rangeField, 
//     Expression activationField)
//     {
//         this.NameField = nameField;
//         this.TypeField = typeField;
//         this.FactionField = factionField;
//         this.PowerField = powerField;
//         this.RangeField = rangeField;
//         this.ActivationField = activationField;
//     }

// }

// public class ActivationField : Expression
// {
//     CardEffects CardEffects{get; }

//     ActivationField(CardEffects cardEffects)
//     {
//         this.CardEffects = cardEffects;
//     }
// }

// public class CardEffects : Expression
// {
//     CardEffect Left {get; }
//     CardEffects Right {get; }

//     public CardEffects(CardEffect left, CardEffects right)
//     {
//         this.Left = left;
//         this.Right = right;     
//     }
// }

// public class CardEffect : Expression
// {
//     Expression Props {get; }

//     public CardEffect(Expression props)
//     {
//         this.Props = props;
//     }
// }

// public class CardEffectProps : Expression
// {
//     Expression Left {get; }
//     Expression Right {get; }

//     public CardEffectProps(Expression left, Expression right)
//     {
//         this.Left = left;
//         this.Right = right;
//     }
// }

// //marca
// public class CardEffectProp : Expression
// {
//     Expression CardEffDeclaration{get; }
//     Expression SelectorField{get; }
//     Expression PostActionField {get; }

//     public CardEffectProp(Expression cardeff, Expression selectorField, Expression postAction)
//     {
//         this.CardEffDeclaration = cardeff;
//         this.SelectorField = selectorField;
//         this.PostActionField = postAction;
//     }
// }

// public class PostActionProps : Expression
// {
//     Expression Left{get; }
//     Expression Right{get; }

//     public PostActionProps(Expression left, Expression right)
//     {
//         this.Left=left;
//         this.Right=right;
//     }
// }

// public class PostActionProp : Expression
// {
//     CardEffectProps EffProps {get; }
//     public PostActionProp(CardEffectProps effProps)
//     {
//         this.EffProps = effProps;
//     }
// }

// public class SelectorField : Expression
// {
//     SelectorProps SelectorProps {get; }

//     public SelectorField(SelectorProps selectorProps)
//     {
//         this.SelectorProps = selectorProps;
//     }
// }

// public class SelectorProps : Expression
// {
//     Expression Left {get; }
//     Expression Right {get; }

//     public SelectorProps(Expression left, Expression right)
//     {
//         this.Left = left;
//         this.Right= right;
//     }
// }

// public class SelectorProp : Expression
// {
//     Source SourceField {get; }
//     Expression SingleField {get; }
//     Expression Predicate {get; }
//     public SelectorProp(Source source, Expression single, Expression predicate)
//     {
//         this.SourceField = source;
//         this.SingleField = single;
//         this.Predicate = predicate;
//     }
// }

// public class Source : Expression
// {
//     string Value {get; }
//     public Source(string value)
//     {
//         this.Value = value;
//     }
// }

// public class SingleField : Expression
// {
//     BooleanExpression Expression {get; }

//     public SingleField(BooleanExpression Expression)
//     {
//         this.Expression = Expression;
//     }
// }

// public class Predicate : Expression
// {
//     PredicativeExpression Expression {get; }

//     public Predicate(PredicativeExpression Expression)
//     {
//         this.Expression = Expression;
//     }
// }
// public class CardEffDeclaration : Expression
// {
//     EffectBlock EffectBlock{get; }

//     public CardEffDeclaration(EffectBlock effectBlock)
//     {
//         this.EffectBlock = effectBlock;
//     }
// }

// public class EffectBlock : Expression
// {
//     EffBlockProps CardEffProps{get; set;}
//     string AuxString {get; }

//     public EffectBlock(EffBlockProps cardEffProps, string nameEffect)
//     {
//         this.CardEffProps = cardEffProps;
//         this.AuxString = nameEffect;
//     }
// }

// // public class CardEffProps : Expression
// // {
// //     Expression Left {get; }
// //     Expression Right {get;}

// //     public CardEffProps(Expression left, Expression right)
// //     {
// //         this.Left = left;
// //         this.Right = right;
// //     }
// // }
// // public class CardEffProp : Expression
// // {
// //     Expression EffDeclaration {get; }
// //     Expression SelectorField {get; }
// //     Expression PostAction{get; }

// //     public CardEffProp(Expression effDeclaration, Expression selectorField, Expression postAction)
// //     {
// //         this.EffDeclaration = effDeclaration;
// //         this.SelectorField = selectorField;
// //         this.PostAction = postAction;
// //     }
// // }

// // public class EffDeclaration : Expression
// // {
// //     EffBlock EffBlock{get; }

// //     public EffDeclaration(EffBlock effBlock)
// //     {
// //         this.EffBlock = effBlock;
// //     }
// // }

// public class EffBlock : Expression
// {
//     EffBlockProps EffBlockProps{get; }
//     string AuxString {get; }

//     public EffBlock(EffBlockProps effBlockProps, string auxString)
//     {
//         this.EffBlockProps = effBlockProps;
//         this.AuxString = auxString;
//     }
// }

// public class EffBlockProps : Expression
// {
//     Expression Left{get; }
//     Expression Right {get;}

//     public EffBlockProps(Expression left, Expression right)
//     {
//         this.Left = left;
//         this.Right = right;
//     }
// }

// public class EffBlockProp : Expression
// {
//     NameField NameField {get; }
//     Asignations Asignations{get; }

//     public EffBlockProp(NameField name, Asignations asignations)
//     {
//         this.NameField = name;
//         this.Asignations = asignations;
//     }
// }
// public class RangeField : Expression
// {
//     string[] Ranges {get; }
//     public RangeField(string[] ranges)
//     {
//         this.Ranges = ranges;
//     }
// }

// public class PowerField : Expression
// {
//     NumericExpression NumericExpression {get; }
//     public PowerField(NumericExpression numericExpression)
//     {
//         this.NumericExpression = numericExpression;
//     }
// }

// public class FactionField : Expression
// {
//     string FactionName {get; }
//     public FactionField(string factionName)
//     {
//         this.FactionName = factionName;
//     }
// }
// public class TypeField : Expression
// {
//     string TypeValue {get; }
//     public TypeField(string typeValue)
//     {
//         this.TypeValue = typeValue;
//     }
// }

// public class Asignations : Expression
// {

// }

// public class NumericExpression : Expression
// {

// }

// public class BooleanExpression : Expression
// {

// }

// public class PredicativeExpression : Expression
// {

// }