#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Expressions;
using System;
public interface IScope {
    //Actions
    //Receives the effect name and the dictionary with the tokens and the types declared
    bool DefineParams(string name, Dictionary<Token, Token> param);
    //Variables
    //Returns if an id is already defined
    bool IsDefined(string idName);
    //Check if its defined and returns the context where it is defined
    bool IsDefinedInContext(string idName, ref Scope scopeOf);
    //Define the id with its name, its token type and its expression value
    bool Define(string idName, IdType type);
    //Returns the type of the id
    IdType GetIdType(string id);
    //Creates a subyacent context
    IScope CreateChildContext();
}

public class Scope : IScope
{
    IScope? parent;
    //Saves each variable with its names and its values
    Dictionary<string, IdType> variables = new Dictionary<string, IdType>();
    //Create the context child
    public IScope CreateChildContext() => new Scope {parent = this};
    //Add each variable to the dictionary with its name and its type and its value(expression) 
    public bool Define(string variable, IdType type)
    {
        //If the variable is not defined add it to the dictionary
        if (!IsDefined(variable)) {
            variables.Add(variable, type);
        }
        //else change its value
        else {
            //change its value, dynamic type
            variables[variable] = type;
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
                this.Define(token.Value, types[param[token].Definition]);
            }
        }
        return true;
    }
    //Returns the id type using the variables dictionary 
    public IdType GetIdType(string idName)
    {
        //Get the context in the hierarchy where it is declared the variable
        Scope scopeOf = this;
        //Context is passed by reference so it returns modified 
        if (IsDefinedInContext(idName, ref scopeOf))         
            return scopeOf.variables[idName];
        

        //Throw error of use of unasigned variable
        Error unasignedVariable = new UnasignedVariable(idName);
        throw new Exception(unasignedVariable.ToString());
    }
    public bool IsDefinedInContext(string idName, ref Scope scopeOf) {
        scopeOf = this;
        return variables.ContainsKey(idName) || (parent != null && parent.IsDefinedInContext(idName, ref scopeOf));
    }
    public bool IsDefined(string variable) {
        return variables.ContainsKey(variable) || (parent != null && parent.IsDefined(variable));
    }
}

