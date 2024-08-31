#nullable enable
using System.Collections.Generic;
using System.Linq;
public static class DefinedActions
{
    public static void ClearActions() {
        Actions.Clear();
    }
    public static Dictionary<string, Dictionary<string, IdType>> Actions = new Dictionary<string, Dictionary<string, IdType>>();

    //Check that all declared params were already defined and add them to the variables in the scope
    public static void CheckValidParameters(string Name, Dictionary<Token, Expression>? allocations, IScope scope, int line)
    {
        //If the dictionary is null it means actions must not contain any key with the same effect name
        if (allocations == null) {
            if (Actions.ContainsKey(Name)) {
                CompilationError MustDeclareParams = new MustDeclareParams(Name, Actions[Name].Keys.ToList(), line);
                throw MustDeclareParams;
            }
        }

        // if (!Actions.ContainsKey(Name)) {
        //     CompilationError effectNotDefined = new EffectNotDefined(Name, line);
        //     throw effectNotDefined;
        // }
 
        if (Actions.ContainsKey(Name))
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
            else {
                CompilationError invalidParameter = new InvalidParameter(idName);
                throw invalidParameter;
            }
        }
    }
}
