namespace Transpiler;  
//Abstract class that represents an error in the DSL
public abstract class Error {
    /// <summary>
    /// Returns the line of the error
    /// </summary>
    /// <value></value>
    public int Line { get; protected set;}
    /// <summary>
    /// Returns the column of the error
    /// </summary>
    /// <value></value>
    public int Column { get; protected set;}
    public Error(int errorLine, int errorColumn) {
        this.Line = errorLine;
        this.Column = errorColumn;
    }
    ///<summary>
    ///Throw the error message to the user
    ///</summary>
    public abstract override string ToString();
}

#region LexerError
public class InvalidSyntaxError : Error {
    public char ErrorChar { get; }
    public InvalidSyntaxError(int errorLine, int errorColumn, char errorChar) : base(errorLine, errorColumn) {
        this.ErrorChar = errorChar;
    }
    public override string ToString() {
        return $"Invalid syntax Error. Unexpected character: {ErrorChar} in line: {this.Line} in column: {this.Column}";
    }
}
#endregion
#region ParserError

//Represents an unexpected end of the tokens when parsing
public class UnexpectedEndOfInput : Error
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

//Represents when another token was expected
//TODO: Decir que token se esperaba
public class UnexpectedToken : Error
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
#endregion
#region SemantycError
//Represent a semantyc error
//Checked in the parsing 
//It is thrown when a must-declare parameter wasn't declared
public class ParameterUnknown : Error
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
    /// <summary>
    /// Function used to print all null parameters that can't be null
    /// </summary>
    /// <returns></returns>
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
    //Save the parameter value to check nullability 
    public DSL_Object? Obj {get; }
    public NotNullableObj(string name, DSL_Object? obj)
    {
        this.Name = name;
        this.Obj = obj;
    }
}
#endregion