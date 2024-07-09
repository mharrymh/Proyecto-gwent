/* SUMMARY


Evaluar las expresiones corticas

La propiedad Find voy a tener que hacerla en el parser poruque como parametro tiene un PREDICADO

Implementar en el lexer y el parser la potenciacion

Implementar el indexado en lista en el parser

//ERRORES EN EL EVALUATE
//MUCHO CATCH 


FORMAS DE VALIDAR ACCESO A PROPIEDADES Y FUNCIONES
//VOY CONSUMIENDO DE IZQUIERDA A DERECHA HASTA TENER ALGO O QUE ME DE ERROR
//SI LLEGO A UNA CARTA ME QUEDO CON ELLA Y EMPIEZO A COGER DE LA PARTE DERECHA LO QUE ES VALIDO DE UNA CARTA

*/

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
        return (Param == null || context.DefineParams(Name, Param)) && Action.Validate(context);
    }
}

public class InstructionBlock : DSL_Object
{
    //Target and context are defined always in the OnActivation
    Expression Targets {get;}
    Expression Context {get;}
    List<ForLoop> ForLoops {get; }
    List<WhileLoop> WhileLoops {get; }
    List<Expression> IdExpressions {get; }
    public InstructionBlock(List<ForLoop> forLoops, List<WhileLoop> whileLoops, List<Expression> idExpressions, Expression targets, Expression context)
    {
        this.ForLoops = forLoops;
        this.WhileLoops = whileLoops;
        this.IdExpressions = idExpressions;
        this.Targets = targets;
        this.Context = context;
    }

    public override bool Validate(IContext context)
    {
        //Check that targets and context are correct, just id types
        if (Targets is LiteralExpression targets && targets.Value.Definition is TokenType.Id
        && Context is LiteralExpression contextExp && contextExp.Value.Definition is TokenType.Id)
        {
            //The value is defined after in the selector
            context.Define(targets, new Variable(null, IdType.Targets));
            //It has no value, context define all context in the game
            context.Define(contextExp, new Variable(null, IdType.Context));
        }
        
        foreach (ForLoop forLoop in ForLoops)
        {
            if (!forLoop.Validate(context.CreateChildContext())) return false;
        }
        foreach (WhileLoop whileLoop in WhileLoops)
        {
            if (!whileLoop.Validate(context.CreateChildContext())) return false;
        }
        foreach (Expression exp in IdExpressions)
        {
            if(!exp.Validate(context.CreateChildContext())) return false;
        }
        return true;
    }
}

public class ForLoop : DSL_Object
{
    Expression Iterator {get; }
    Expression Collection {get;}
    InstructionBlock Instructions {get; }
    public ForLoop(InstructionBlock instructions, Expression iterator, Expression collection)
    {
        this.Instructions = instructions;
        this.Iterator = iterator;
        this.Collection = collection;
    }
    public override bool Validate(IContext context)
    {
        //TODO: ISITERATOR Y ISCOLLECTION PREGUNTAN SI YA ES UN ITERADOR O UNA COLECCION DE LOS YA CONOCIDOS
        //puede ser targets o otra coleccion
        //se hace con get type


        //FIXME:
        // if (Iterator.GetType != Type.iterator || Collection.GetType != Type.collection) return false;

        //Create a new scope
        IContext child = context.CreateChildContext();
        //Define the iterator and the collection 
        //TODO:UNCOMMENT
        // child.Define(Iterator, new Variable(null, IdType.Iterator));
        // child.Define(Collection, new Variable(null, IdType.Collection));


        //Validate instructions inside the for
        return Instructions.Validate(child);
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
        //Validate the bool expression and the instruction inside
        IContext child = context.CreateChildContext();
        return BoolExpression.Validate(child) && Instructions.Validate(child);
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
                //Check that all expressions in range are string expressions
                if (!exp.Validate(context) || !(exp.GetType(context) == IdType.String)) return false;
            }
        foreach(EffectAllocation effect in Activation)
        {
            if (!effect.Validate(context.CreateChildContext())) return false;
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
        return Name.Validate(context) && DefinedActions.CheckValidParameters(Name, VarAllocation, context);
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
        return Source.Validate(context) && Source.GetType(context) == IdType.String
        && (Single == null || (Single.Validate(context) && Single.GetType(context) == IdType.Boolean))
        && Predicate.Validate(context);
    }
}

public class Predicate : DSL_Object
{
    //TODO: RECIBE UNA EXPRESION?
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
        return context.IsDefined(new LiteralExpression(Id)) && BoolExp.Validate(context) && BoolExp.GetType(context) == IdType.Boolean;
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
