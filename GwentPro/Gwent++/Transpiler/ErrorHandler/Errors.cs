
namespace Transpiler;  
interface IParserError {
    Token ErrorToken { get; }
}
interface INullableError {
    List<string> NullObjects { get; }
}
public abstract class Error {
    public int Line { get; protected set;}
    public int Column { get; protected set;}
    public Error(int errorLine, int errorColumn) {
        this.Line = errorLine;
        this.Column = errorColumn;
    }
    public abstract override string ToString();
}

public class InvalidSyntaxError : Error {
    public char ErrorChar { get; }
    public InvalidSyntaxError(int errorLine, int errorColumn, char errorChar) : base(errorLine, errorColumn) {
        this.ErrorChar = errorChar;
    }
    public override string ToString() {
        return $"Invalid syntax Error. Unexpected character: {ErrorChar} in line: {this.Line} in column: {this.Column}";
    }
}

public class UnexpectedEndOfInput : Error, IParserError
{
    public Token ErrorToken { get; }
    public UnexpectedEndOfInput(int errorLine, int errorColumn, Token token) : base(errorLine, errorColumn) 
    {
        this.ErrorToken = token;
    }
    public override string ToString()
    {
        return $"Unexpected end of input error. Unexpected end detected after token: {ErrorToken.Value} in line: {Line} in column: {Column + ErrorToken.Value}";
    }
}

public class UnexpectedToken : Error, IParserError
{
    public Token ErrorToken {get;}
    public UnexpectedToken(int errorLine, int errorColumn, Token errorToken) : base(errorLine, errorColumn)
    {
        this.ErrorToken = errorToken;
    }
    public override string ToString()
    {
        return $"Unexpected token error. Unexpected token detected: {ErrorToken.Value} in line: {Line}, in column: {Column}";
    }
}

public class ParameterUnknown : Error, IParserError, INullableError
{
    public Token ErrorToken {get; }
    public List<string> NullObjects {get; }
    public ParameterUnknown(int errorLine, int errorColumn, Token errorToken, List<NotNullableObj> notNullableObjects) : base(errorLine, errorColumn)
    {
        this.ErrorToken = errorToken;
        this.NullObjects = new List<string>();
        GetNullObjects(notNullableObjects);
    }

    void GetNullObjects(List<NotNullableObj> notNullableObjects)
    {
        foreach (var obj in notNullableObjects)
        {
            //Add every name of a null parameter
            if (obj.Obj == null) NullObjects.Add(obj.Name);
        }
    }

    public override string ToString()
    {
        return $"Field ignored error. You must declare the parameter {NullObjects[0]} to continue after Token: {ErrorToken.Value} in line: {Line} in column: {Column}. " + PrintNullObjects();
    }
    string PrintNullObjects()
    {
        string aux = "";
        if (NullObjects.Count == 1) return aux;

        aux = "Same with: ";
        for (int i = 1; i < this.NullObjects.Count; i++)
        {
            if (i > 1) aux += ", ";
            aux += $"{NullObjects[i]}";
        }
        return aux;
    }
}

public class NotNullableObj {
    //Save the name of the parameter
    public string Name {get; }
    //Save the parameter to check nullability 
    public DSL_Object? Obj {get; }
    public NotNullableObj(string name, DSL_Object? obj)
    {
        this.Name = name;
        this.Obj = obj;
    }
}


