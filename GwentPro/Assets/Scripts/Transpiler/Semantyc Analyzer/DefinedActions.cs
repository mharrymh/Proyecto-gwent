#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System;
public static class DefinedActions
{
    //Saves the declaration of the params foreach effect
    //TODO: hacerlo string pero evaluando la expresion 
    public static Dictionary<string, Dictionary<string, IdType>> Actions = new Dictionary<string, Dictionary<string, IdType>>();

    //Check that all declared params were already defined and add them to the variables in the scope
    public static void CheckValidParameters(string Name, Dictionary<Token, Expression>? allocations, IScope scope)
    {
        //If the dictionary is null it means actions must not contain any key with the same effect name
        if (allocations == null) {
            //TODO: Error de que se no se declara un efecto que tiene que ser declarable
            if (Actions.ContainsKey(Name)) throw new Exception();
            return;
        }

        //TODO: Error de que un efecto con ese nombre no esta definido
        if (!Actions.ContainsKey(Name)) throw new Exception();
 

        foreach (Token idName in allocations.Keys) {
            //Check that all defined variables were declared and with the same type
            if (Actions[Name].ContainsKey(idName.Value)) {
                //Get the expected param type
                IdType expType = allocations[idName].GetType(scope);
                //If everything is correct, define the new variable and continue checking
                if (Actions[Name][idName.Value] == expType) {
                    //Add the defined variable with its name (expression), its values and its type (variable)
                    scope.Define(idName.Value, expType);
                    continue;
                }
            }
            //TODO:
            else throw new Exception();
        }
    }
}
