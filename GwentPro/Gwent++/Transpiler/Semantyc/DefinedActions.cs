namespace Transpiler;

public static class DefinedActions
{
    public static Dictionary<string, Parameter> Actions = [];
}

public class Parameter {
    public Token IdName {get;}
    public TokenType Type {get;}
    public Parameter(Token token, TokenType tokenType)
    {
        IdName = token;
        Type = tokenType;
    }
}