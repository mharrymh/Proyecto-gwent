namespace Transpiler;

//TODO: ERROR FALTA TAL PARAMETRO   
interface IParserError {
    Token ErrorToken { get; }
}
public abstract class Error {
    public int Line { get; protected set;}
    public int Column { get; protected set;}
    public Error(int errorLine, int errorColumn) {
        Line = errorLine;
        Column = errorColumn;
    }
    public abstract override string ToString();
}

public class InvalidSyntaxError : Error {
    public char ErrorChar { get; }
    public InvalidSyntaxError(int errorLine, int errorColumn, char errorChar) : base(errorLine, errorColumn) {
        ErrorChar = errorChar;
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
        ErrorToken = token;
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
        ErrorToken = errorToken;
    }
    public override string ToString()
    {
        return $"Unexoected token error. Unexpected token detected: {ErrorToken.Value} in line: {Line}, in column: {Column}";
    }
}


