using System.Runtime.Serialization;

namespace Transpiler;

public static class DefinedActions
{
    //Saves the declaration of the params foreach effect
    public static Dictionary<Expression, Dictionary<Token, IdType>> Actions = [];

    //Check that all declared params were already defined and add them to the variables in the scope
    public static bool CheckValidParameters(Expression Name, Dictionary<Token, Expression>? allocations, IContext context)
    {
        //If the dictionary is null it means actions must not contain any key with the same effect name
        if (allocations == null) {
            return !Actions.ContainsKey(Name);
        }

        if (!Actions.ContainsKey(Name)) return false;

        foreach (Token idName in allocations.Keys) {
            //Check that all defined variables were declared and with the same type
            if (Actions[Name].ContainsKey(idName) ) {
                IdType expType = allocations[idName].GetType(context);
                if (Actions[Name][idName] == expType) {
                    //Add the defined variable with its name (expression), its values and its type (variable)
                    context.Define(new LiteralExpression(idName), new Variable(allocations[idName], expType));
                    continue;
                }
            }
            else return false;
        }
        return true;
    }
}
