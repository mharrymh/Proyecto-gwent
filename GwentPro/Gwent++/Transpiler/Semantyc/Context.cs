namespace Transpiler;

public interface IContext {
    //Actions
    bool Define(string name, Dictionary<Token, Token> param);
    //Variables
    bool IsDefined(Token variable);
    bool Define(Token variable, Expression expression);
    IContext CreateChildContext();
}

//TODO: DIFINE PARAMETERS
public class Context : IContext
{
    IContext? parent;
    //Saves each variable with its names and its values
    Dictionary<string, Expression> variables = [];
    public IContext CreateChildContext()
    {
        return new Context {parent = this};
    }
    //Add each variable to the dictionary with its name and its value(expression) 
    public bool Define(Token variable, Expression expression)
    {
        if (!IsDefined(variable)) {
            variables.Add(variable.Value, expression);
            return true;
        }
        else return false;
    }

    //Define a new action with its parameters using the static dictionary 
    //in DefinedActions.cs
    public bool Define(string name, Dictionary<Token, Token> param)
    {
        //Relate the token type of the reserved words to defined variables
        //to the actual type that the variable should have
        Dictionary<TokenType, TokenType> types = new Dictionary<TokenType, TokenType>
        {
            {TokenType.Number, TokenType.Num},
            {TokenType.Text, TokenType.String},
            {TokenType.Bool, TokenType.Boolean}
        };

        foreach (Token token in param.Keys)
        {
            //Cant exist different actions with the same name
            if (DefinedActions.Actions.ContainsKey(name)) return false;
            else {
                DefinedActions.Actions.Add(name, new Parameter(token, types[token.Definition]));
            }
        }
        return true;
    }

    public bool IsDefined(Token variable)
    {
        return variables.ContainsKey(variable.Value) || (parent != null && parent.IsDefined(variable));
    }
}