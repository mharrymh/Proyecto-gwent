using System;
using System.Collections.Generic;

public abstract class Error : Exception
{
    public override abstract string Message {get;}
}

public abstract class CompilationError : Error
{
    public override abstract string Message {get;}
}

#region Lexic Analysis Errors
public class LexicalError : CompilationError
{
    int Line {get;}
    int Column {get;}
    char Character {get;}
    public override string Message { 
        get
        {
            return $"Lexical Error: Character: {Character} not recognized in line: {Line}, column: {Column}";
        }
    }

    public LexicalError(int line, int column, char character)
    {
        Line = line;
        Column = column;
        Character = character;
    }
}

#endregion

#region Parser Errors

public class UnexpectedEndOfInput : CompilationError
{
    int Line {get; }
    int Column {get; }
    string TokenValue {get;}
    public override string Message {
        get {
            return $"Syntax Error: Unexpected end of input reached after: {TokenValue} in line: {Line} in column: {Column}. \n" +
                   "Please review your code to ensure that no elements are missing. You must declare at least one effect and one card";
        }
    }
    public UnexpectedEndOfInput(Token token)
    {
        Line = token.Line;
        Column = token.Column;
        TokenValue = token.Value;
    }
}


public class UnexpectedToken : CompilationError
{
    int Line {get;}
    int Column {get;}
    string TokenValue {get;}
    string TypeExpected {get;}
    public override string Message {
        get {
            return $"“Syntax error: Unexpected token encountered: \"{TokenValue}\" in line: {Line} in column: {Column}. Type expected: {TypeExpected}";
        }
    }

    public UnexpectedToken(Token token, string type)
    {
        Line = token.Line;
        Column = token.Column;
        TokenValue = token.Value;
        TypeExpected = type;
    }
}

public class NotAvailableFunction : CompilationError
{
    int Line {get;}
    int Column {get;}
    string FunctionName {get;}

    public override string Message {
        get {
            return $"Syntax Error: Not availabe function name: \"{FunctionName}\" in line: {Line} in column: {Column}."
            + " Remember that the language is case sensitive";
        }
    }

    public NotAvailableFunction(Token functionName)
    {
        Line = functionName.Line;
        Column = functionName.Column;
        FunctionName = functionName.Value;
    }
}
#endregion

#region Semantyc Errors
public class NotDeclaredBlockOfEffect : CompilationError
{
    int Line {get;}
    string NullBlock {get;}
    public override string Message {
        get {
            return $"Semantyc Error: You must declare a {NullBlock} block in the effect declaration of the line: {Line}.";
        }
    }
    public NotDeclaredBlockOfEffect(int line, string nullBlock)
    {
        Line = line;
        NullBlock = nullBlock;
    }
}
public class NotDeclaredBlockOfCard : CompilationError
{
    int Line {get;}
    string NullBlock {get;}
    public override string Message {
        get {
            return $"Semantyc Error: You must declare a {NullBlock} block in the card declaration of the line: {Line}.";
        }
    }
    public NotDeclaredBlockOfCard(int line, string nullBlock)
    {
        Line = line;
        NullBlock = nullBlock;
    }
}
public class NotDeclaredBlockOfAllocation : CompilationError
{
    int Line {get;}
    string NullBlock {get;}
    public override string Message {
        get {
            return $"Semantyc Error: You must declare a {NullBlock} block in the effect allocation of the card declaration of the line: {Line}.";
        }
    }
    public NotDeclaredBlockOfAllocation(int line, string nullBlock)
    {
        Line = line;
        NullBlock = nullBlock;
    }
}

public class RangeNotDeclared : CompilationError
{
    int Line {get;}
    string CardType {get;}
    public override string Message {
        get {
            return $"Semantyc Error: You must declare a range block for the declared card of type: \"{CardType}\" in line: {Line}";
        }
    }
    public RangeNotDeclared(int line, string cardType)
    {
        Line = line;
        CardType = cardType;
    }
}
public class PowerNotDeclared : CompilationError
{
    int Line {get;}
    string CardType {get;}
    public override string Message {
        get {
            return $"Semantyc Error: You must declare a power" +
            $" for the declared card of type: \"{CardType}\" in line: {Line}";
        }
    }
    public PowerNotDeclared(int line, string cardType)
    {
        Line = line;
        CardType = cardType;
    }
}

public class TypeNotAvailable : CompilationError
{
    int Line {get;}
    string Type {get;}
    public override string Message {
        get {
            return $"Semantyc Error: The type declared: \"{Type}\" in the card declaration of line: {Line} is not valid. \n" + 
            "Valid types: Gold, Silver, Climate, Cleareance, Decoy, Increment, Leader";
        }
    }
    public TypeNotAvailable(int line, string type)
    {
        Line = line;
        Type = type;
    }
}
public class RangeNotAvailable : CompilationError
{
    int Line {get;}
    string Range {get;}
    public override string Message {
        get {
            return $"Semantyc Error: The range declared: \"{Range}\" in the card declaration of line: {Line} is not valid. \n" + 
            "Valid ranges: Melee, Ranged, Siege";
        }
    }
    public RangeNotAvailable(int line, string range)
    {
        Line = line;
        Range = range;
    }
}
public class FactionNotAvailable : CompilationError
{
    int Line {get;}
    string Faction {get;}
    public override string Message {
        get {
            return $"Semantyc Error: The faction declared: \"{Faction}\" in the card declaration of line: {Line} is not valid. \n" + 
            "Valid factions: Light, Dark";
        }
    }
    public FactionNotAvailable(int line, string faction)
    {
        Line = line;
        Faction = faction;
    }
}

public class NotDeclaredBlockOfEffectAllocation : CompilationError
{
    int Line {get;}
    public override string Message {
        get {
            return $"Semantyc Error: You must declare the name and the parameters of the on actvation block in the card declaration of line: {Line}.";
        }
    }
    public NotDeclaredBlockOfEffectAllocation(int line)
    {
        Line = line;
    }
}

public class NotExpectedTypeOfVariable : CompilationError
{
    int Line {get; }
    string TypeReceived {get;}
    string TypeExpected {get; }
    public override string Message {
        get {
            return $"Semantyc Error: Unexpected variable type received in the expression in line: {Line}. Type received: {TypeReceived}, Type expected: {TypeExpected}";
        }
    }

    public NotExpectedTypeOfVariable(int line, string received, string expected)
    {
        Line = line;
        TypeReceived = received;
        TypeExpected = expected;
    }
}

public class NotDefinedVariable : CompilationError
{
    int Line {get;}
    string VariableName {get;}
    public override string Message {
        get {
            return $"Semantyc Error: The variable: {VariableName} in line: {Line} has not been defined in the scope";
        }
    }
    public NotDefinedVariable(Token variable)
    {
        Line = variable.Line;
        VariableName = variable.Value;
    }
}

public class NotValidAccessToFunctions : CompilationError
{
    int Line {get; }
    string FunctionName {get; }
    string TypeReceived {get; }
    public override string Message {
        get {
            return $"Semantyc Error: An expression of type: \"{TypeReceived}\" tried to called a {FunctionName} function in line: {Line}. "
            + $"Only expressions of type: \"{Utils.relateFunctionsWithTheTypeThatCallToIt[FunctionName]}\" can call to \"{FunctionName}\"";
        }
    }
    public NotValidAccessToFunctions(string received, string functionName, int line)
    {
        Line = line;
        FunctionName = functionName;
        TypeReceived = received;
    }
}

public class NotValidArgument : CompilationError
{
    int Line {get;}
    string TypeReceived {get;}
    string TypeExpected {get;}
    string FunctionName {get;}
    public override string Message {
        get {
            return $"Semantyc Error: The function {FunctionName} in line {Line} has an invalid type of argument. "
            + $"Argument type received: {TypeReceived}. Argument type expected: {TypeExpected}";
        }
    }
    public NotValidArgument(string received, string expected, string function, int line)
    {
        Line = line;
        TypeReceived = received;
        TypeExpected = (expected == "")? "None" : expected;
        FunctionName = function;
    }
}

public class NotValidAccessToProperty : CompilationError
{
    int Line {get;}
    string LeftType {get;}
    string RightName {get;}
    public override string Message {
        get {
            return $"Semantyc Error: An expression of type: \"{LeftType}\" tried to call" +  
            $" a property that it has no access to: \"{RightName}\" in the line: {Line}";
        }
    }
    public NotValidAccessToProperty(string leftType, string propertyName, int line)
    {
        Line = line;
        LeftType = leftType;
        RightName = propertyName; 
    }
}

public class SpecialCardOnlyOneRange : CompilationError
{
    int Line {get;}
    int AmountDeclared {get;}
    string Type {get;}
    public override string Message {
        get {
            return $"Semantyc Error: You must declare only one valid range for a card of type: \"{Type}\" and you declared {AmountDeclared} ranges"
            + $" in the card declaration of the line: {Line}.";
        }
    }

    public SpecialCardOnlyOneRange(int line, int amount, string type)
    {
        Line = line;
        AmountDeclared = amount;
        Type = type;
    }
}
public class InvalidAccess : CompilationError
{
    int Line {get;}
    string LeftType {get;}
    string PropertyName {get;}
    public override string Message {
        get {
            return $"Semantyc Error: You tried to access an invalid property: \"{PropertyName}\" in the expression of type: \"{LeftType}\" in line: {Line}";
        }
    }
    public InvalidAccess(int line, IdType leftType, string name)
    {
        Line = line;
        LeftType = leftType.ToString();
        PropertyName = name;
    }
}
public class SemantycUnexpectedType : CompilationError
{
    string LeftType {get;}
    string RightType {get;}
    int Line {get;}
    public override string Message {
        get {
            return $"Semantyc Error: Unexpected expression type received in line {Line}. Received: {RightType}, Expected: {LeftType}";
        }
    }
    public SemantycUnexpectedType(string left, string right, int line)
    {
        LeftType = left;
        RightType = right;
        Line = line;
    }
}

public class InvalidPropertyAssigned : CompilationError
{
    int Line {get;}
    string Type {get;}
    public override string Message {
        get {
            return $"Semantyc Error: An expression of type \"{Type}\" can not be used in a assign expression. Please review your code in line {Line}";
        }
    }
    public InvalidPropertyAssigned(string received, int line)
    {

    }
}

public class NumericAssignError : CompilationError
{
    int Line {get;}
    string Type {get;}
    public override string Message {
        get {
            return $"Semantyc Error: An expression of type: {Type} was encountered in a numeric assign expression in line: {Line}, only valid type is Number";
        }
    }
    public NumericAssignError(int line, string type)
    {
        Line = line;
        Type = type;
    }
}
public class EffectAllocatedTwice : CompilationError
{
    string Name {get;}
    public override string Message {
        get {
            return $"Semantyc Error: You can not use the same effect: {Name} twice in a card declaration.";
        }
    }
    public EffectAllocatedTwice(string name)
    {
        Name = name;
    }
}

public class MustDeclareParams : CompilationError
{
    int Line {get;}
    string Name {get;}
    List<string> Params {get;}
    public override string Message {
        get {
            string message = $"You must declare the parameters in the allocation of the effect {Name} of the declared card in line {Line}. You need to declare the following parameters: ";
            foreach (string param in Params)
            {
                message += param + ", ";
            }
            return message;
        }
    }
    public MustDeclareParams(string name, List<string> parameters, int line)
    {
        Name = name;
        Params = parameters;
        Line = line;
    }
}

public class EffectNotDefined : CompilationError
{
    int Line {get;}
    string Name {get;}
    public override string Message {
        get {
            return $"The effect with the name: {Name} declared in the card of the line {Line} has not been defined. Please check that you have the correct name";
        }
    }
    public EffectNotDefined(string name, int line)
    {
        Line = line;
        Name = name;
    }

}

public class InvalidParameter : CompilationError
{
    int Line {get;}
    int Column {get;}
    string ParamName {get;}
    public override string Message {
        get {
            return $"Semantyc Error: Invalid parameter: {ParamName} received in line: {Line} in column: {Column}";
        }
    }
    public InvalidParameter(Token token)
    {
        Line = token.Line;
        ParamName = token.Value;
        Column = token.Column;
    }

}
public class TwoEffectsWithSameName : CompilationError
{
    string Name {get;}
    public override string Message {
        get {
            return $"You can not declare two effects with the same name: \"{Name}\"";
        }
    }
    public TwoEffectsWithSameName(string name)
    {
        Name = name;
    }
}
#endregion
#region Evaluate Errors
public class DivideByZeroError : CompilationError
{
    int Line {get;}
    public override string Message {
        get {
            return $"Evaluate Error: Division by Zero detected in line: {Line}";
        }
    }
    public DivideByZeroError(int line)
    {
        Line = line;
    }
}

public class NotPostEffect : CompilationError
{
    string Name {get;}
    public override string Message {
        get {
            return $"Evaluate Error: Invalid source encountered, you declared the source of \"{Name}\" as parent and \"{Name}\" effect is not a post effect";
        }
    }
    public NotPostEffect(string name)
    {
        Name = name;
    }
}
#endregion
#region Runner Error
public class EmptyInput : CompilationError
{
    public override string Message {
        get {
            return $"Your input text is empty, please write some declarations before pressing the save button";
        }
    }
    public EmptyInput() {}
}
#endregion