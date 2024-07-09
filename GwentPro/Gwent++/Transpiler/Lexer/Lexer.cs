using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Transpiler;

//string pattern = @"^([a-zA-Z_][a-zA-Z0-9_]*)(\.[a-zA-Z_][a-zA-Z0-9_]*|\.[a-zA-Z_][a-zA-Z0-9_]*\([a-zA-Z_][a-zA-Z0-9_]*\))*$";


public enum TokenType
{
    // Reserved words
    For, While, Effect, C_Effect, Card, Source, Single, 
    Predicate, PostAction, Type, Name, Faction, Power, Range, OnActivation, 
    Selector, Implication, In, 

    //FIXME: ELIMINAR (QUIZA DEJAR EL FIND)
    // Hand, Deck, Board,
    // TriggerPlayer, Find, Push, SendBottom, Pop, Remove, Shuffle,

    // Operators
    Increment, Decrement, Plus, Minus, Division, Multip, And, 
    Or, Less, More, Equal, LessEq, MoreEq, SpaceConcatenation, 
    Concatenation, Assign, MinusAssign, MoreAssign, NotEquals,

    // Brackets
    LParen, RParen, LBracket, RBracket, LCurly, RCurly,

    // Punctuation
    Semicolon, Colon, Point, Comma,

    // Types
    Boolean, String, Num, Id, Params, Action,

    // Comment and whitespaces
    Null,

    // Value types
    Number, Bool, Text
}

public struct Token {
    public string Value {get; private set;}
    public TokenType Definition {get; private set;}
    public int Line {get; private set;}
    public int Column {get; private set;}
    public Token(string value, TokenType definition, int line, int column)
    {
        this.Value = value;
        this.Definition = definition;
        this.Line = line;
        this.Column = column;
    }
}
public class Lexer {
    private readonly Dictionary<TokenType, string> TokenDefinitions = new Dictionary<TokenType, string>
    {
        // // Access to properties and functions with one or none parameters
        // {TokenType.IdCall, 
        // @"([a-zA-Z_][a-zA-Z0-9_]*)(\.[a-zA-Z_][a-zA-Z0-9_]*)(\(\)|\([a-zA-Z_][a-zA-Z0-9_]*\))?(\.[a-zA-Z_][a-zA-Z0-9_]*(\(\)|\([a-zA-Z_][a-zA-Z0-9_]*\)))*"},


        // Comment and whitespaces
        {TokenType.Null, @"\s+|\/\/.*|(?s)/\*.*?\*/"},

        // Reserved words
        {TokenType.For, @"\bfor\b"}, 
        {TokenType.While, @"\bwhile\b"}, 
        {TokenType.Effect, @"\beffect\b"}, 
        {TokenType.C_Effect, @"\bEffect\b"},
        {TokenType.Card, @"\bcard\b"},
        {TokenType.Predicate, @"\bPredicate\b"},
        {TokenType.PostAction, @"\bPostAction\b"},
        {TokenType.Type, @"\bType\b"}, 
        {TokenType.Name, @"\bName\b"}, 
        {TokenType.Params, @"\bParams\b"}, 
        {TokenType.Action, @"\bAction\b"}, 
        {TokenType.Source, @"\bSource\b" }, 
        {TokenType.Single, @"\bSingle\b" }, 
        {TokenType.In, @"\bin\b" }, 
        {TokenType.Faction, @"\bFaction\b"}, 
        {TokenType.Power, @"\bPower\b"}, 
        {TokenType.Range, @"\bRange\b"}, 
        {TokenType.OnActivation, @"\bOnActivation\b"}, 
        {TokenType.Selector, @"\bSelector\b"},

        // Operators
        {TokenType.MinusAssign, @"\-="}, 
        {TokenType.MoreAssign, @"\+="}, 
        {TokenType.NotEquals, @"!="}, 
        {TokenType.Implication, @"=>"}, 
        {TokenType.Increment, @"\+\+"}, 
        {TokenType.Decrement, @"--"}, 
        {TokenType.Plus, @"\+"}, 
        {TokenType.Minus, @"-"}, 
        {TokenType.Multip, @"\*"}, 
        {TokenType.Division, @"\/"}, 
        {TokenType.And, @"\&\&"}, 
        {TokenType.Or, @"\|\|"},
        {TokenType.Less, "<"}, 
        {TokenType.More, ">"}, 
        {TokenType.Equal, "=="}, 
        {TokenType.LessEq, "<="}, 
        {TokenType.MoreEq, ">="}, 
        {TokenType.SpaceConcatenation, "@@"}, 
        {TokenType.Concatenation, "@"}, 
        {TokenType.Assign, "="},




        // Brackets
        {TokenType.LParen, @"\("}, {TokenType.RParen, @"\)"}, 
        {TokenType.LBracket, @"\["}, {TokenType.RBracket, @"\]"}, 
        {TokenType.LCurly, @"\{"}, {TokenType.RCurly, @"\}"},

        // Punctuation
        {TokenType.Semicolon, ";"}, {TokenType.Colon, @":"}, 
        {TokenType.Point, @"\."}, {TokenType.Comma, ","},
        //Types
        {TokenType.Number, @"\bNumber\b"}, {TokenType.Text, @"\bText\b"}, {TokenType.Bool, @"\bBool\b" },
        // Value Types
        {TokenType.Boolean, @"\b(true|false)\b"}, {TokenType.Num, @"\b\d+(\.\d+)?\b"}, 
        {TokenType.String, "\".*?\""}, 
        {TokenType.Id, @"\b[A-Za-z_][A-Za-z_0-9]*\b"}
    };
    public List<Token> Tokenize(string input)
    {
        List<Token> tokens = new List<Token>();
        string remainingInput = input;
        int actualLine = 1;
        int actualColumn = 1;

        while (!string.IsNullOrEmpty(remainingInput)) {

            // Token actualToken = new Token();
            bool matchFound = false;

            foreach(var tokenDef in TokenDefinitions) {

                Match match = Regex.Match(remainingInput, "^" + tokenDef.Value);

                if (match.Success) {

                    if (match.Value.Contains('\n')) {
                        actualLine += match.Value.Count(c => c == '\n'); //Increment the actual line for each new line
                        actualColumn = 1;
                    }

                    //Ignore whitespaces and comments
                    if (tokenDef.Key != TokenType.Null) 
                    { 
                        tokens.Add(new Token(match.Value, tokenDef.Key, actualLine, actualColumn));
                    }

                    remainingInput = remainingInput.Substring(match.Value.Length);
                    actualColumn += match.Value.Length;
                    matchFound = true;
                    break;
                }
            }

            if (!matchFound) {
                var invalidToken = new InvalidSyntaxError(actualLine, actualColumn, remainingInput[0]);
                throw new Exception(invalidToken.ToString());
            }
        }
        return tokens;
    } 
}
