using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Transpiler;

/*

Agregar /= y *= 








Propiedades de context:
TriggerPlayer
Board

Que reciben como parametro un player: 
HandOfPlayer(player) => Se acepta context.Hand como diminutivo de context.HandOfPLayer(context.TriggerPlayer)
// LO MISMO PARA LAS DEMAS
FieldOfPlayer(player)
GraveyardOfPlayer(player)
DeckOfPlayer(player)

Propiedades de carta: 
Owner

Metodos que tienen cada una de las listas de cartas accesibles desde el contexto
Find(predicate)
Push(card)
SendBottom(card)
Pop()
Remove(card)
Shuffle()
*/

//An enumerable with the different types of id
public enum IdType {
    Number, 
    String,
    Boolean,
    Context,
    Targets, 
    Card,
    Player,
    CardCollection,
    Predicate,
    Null

}
public struct Variable {
    public Expression? Value {get; }
    public IdType IdType {get;}
    public Variable(Expression? value, IdType idType) {
        Value = value;
        IdType = idType;
    }
}
public interface IContext {
    //Actions
    //Receives the effect name and the dictionary with the tokens and the types declared
    bool DefineParams(string name, Dictionary<Token, Token> param);
    //Variables
    //Returns if an id is already defined
    bool IsDefined(string idName);
    //Check if its defined and returns the context where it is defined
    bool IsDefinedInContext(string idName, ref Context contextOf);
    //Define the id with its name, its token type and its expression value
    bool Define(string idName, Variable value);
    //Returns the type of the id
    IdType GetIdType(string id);
    //Creates a subyacent context
    IContext CreateChildContext();
}


//TODO: DIFINE PARAMETERS
public class Context : IContext
{
    IContext? parent;
    //Saves each variable with its names and its values
    Dictionary<string, Variable> variables = [];
    //Create the context child
    public IContext CreateChildContext() => new Context {parent = this};
    //Add each variable to the dictionary with its name and its type and its value(expression) 
    public bool Define(string variable, Variable value)
    {
        //If the variable is not defined add it to the dictionary
        if (!IsDefined(variable)) {
            variables.Add(variable, value);
        }
        //else change its value
        else {
            //change its value, dynamic type
            variables[variable] = value;
        }
        return true;
    }

    //Define a new action with its parameters using the static dictionary 
    //in DefinedActions.cs
    public bool DefineParams(string name, Dictionary<Token, Token> param)
    {
        //Relate the token type of the reserved words to defined variables
        //to the actual type that the variable should have
        Dictionary<TokenType, IdType> types = new Dictionary<TokenType, IdType>
        {
            {TokenType.Number, IdType.Number},
            {TokenType.Text, IdType.String},
            {TokenType.Bool, IdType.Boolean}
        };
        //Cant exist two effects with the same name
        if (DefinedActions.Actions.ContainsKey(name)) return false;
        //Create the new effect with its params
        DefinedActions.Actions.Add(name, new Dictionary<string, IdType>());
        
        //Add parameters
        foreach (Token token in param.Keys)
        {
            //Check that there there are not varaiables with the name repeated
            if (DefinedActions.Actions[name].ContainsKey(token.Value)) {
                return false;
            }
            else {
                //Add the token with the respective type changed using the types dictionary and the params dictionary
                DefinedActions.Actions[name].Add(token.Value, types[param[token].Definition]);
                //Define the variables in the scope
                this.Define(token.Value, new Variable(null, types[param[token].Definition]));
            }
        }
        return true;
    }
    //Returns the id type using the variables dictionary 
    public IdType GetIdType(string idName)
    {
        //If the variable is not defined throw error
        //TODO:
        //Get the context in the hierarchy where it is declared the variable
        Context contextOf = this;
        //Context is passed by reference so it returns modified 
        //TODO:
        if (!IsDefinedInContext(idName, ref contextOf)) throw new Exception();
        //else return the type of the variable
        return contextOf.variables[idName].IdType;
    }
    public bool IsDefinedInContext(string idName, ref Context contextOf) {
        contextOf = this;
        return variables.ContainsKey(idName) || (parent != null && parent.IsDefinedInContext(idName, ref contextOf));
    }
    public bool IsDefined(string variable) {
        return variables.ContainsKey(variable) || (parent != null && parent.IsDefined(variable));
    }
}
