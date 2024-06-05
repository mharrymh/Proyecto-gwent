namespace Transpiler;
interface IParserError {
    Token ErrorToken { get; }
}
public abstract class Error {
    public int ErrorLine { get; protected set;}
    public int ErrorColumn { get; protected set;}
    public Error(int errorLine, int errorColumn) {
        ErrorLine = errorLine;
        ErrorColumn = errorColumn;
    }
    public abstract override string ToString();
}

public class InvalidSyntaxError : Error {
    public char ErrorChar { get; private set;}
    public InvalidSyntaxError(int errorLine, int errorColumn, char errorChar) : base(errorLine, errorColumn) {
        ErrorChar = errorChar;
    }
    public override string ToString() {
        return $"Invalid character: {ErrorChar} in line: {this.ErrorLine} in column: {this.ErrorColumn}";
    }
}

public class NotNumericalToken : Error, IParserError {
    public Token ErrorToken {get; set;}

    public NotNumericalToken(int errorLine, int errorColumn, Token errorToken) : base(errorLine, errorColumn) {
        ErrorToken = errorToken;
    }

    public override string ToString()
    {
        return $"Numerical expression expected. {ErrorToken.Value}: {ErrorToken.Definition} received at line {ErrorLine}, at column {ErrorColumn}";
    }
}

public class NotClosedParen : Error, IParserError {

    public Token ErrorToken {get; set; }

    public NotClosedParen(int errorLine,int errorColumn, Token errorToken) : base(errorLine, errorColumn) {
        ErrorToken = errorToken;
    }
    public override string ToString()
    {
        return $"Not closed parenthesis. ')' expected after {ErrorToken.Value}: {ErrorToken.Definition} in line {ErrorLine}, at column {ErrorColumn}";
    }
}

public class InvalidTokenInParen : Error, IParserError {

    public Token ErrorToken {get; set; }

    public InvalidTokenInParen(int errorLine,int errorColumn, Token errorToken) : base(errorLine, errorColumn) {
        ErrorToken = errorToken;
    }
    public override string ToString()
    {
        return $"')' expected. Invalid token {ErrorToken.Value}: {ErrorToken.Definition} received in line {ErrorLine}, at column {ErrorColumn}";
    }
}

public class UnexpectedEnd : Error, IParserError {
    public Token ErrorToken {get; set; }

    public UnexpectedEnd(int errorLine,int errorColumn, Token errorToken) : base(errorLine, errorColumn) {
        ErrorToken = errorToken;
    }
    public override string ToString()
    {
        return $"Numerical expression expected after {ErrorToken.Value}: {ErrorToken.Definition} in line {ErrorLine}, at column {ErrorColumn}";
    }
}
