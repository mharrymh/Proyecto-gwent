using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Transpiler;
public enum TokenType
{
    For,
    While,
    Effect,
    C_Effect,
    Card,
    If,
    Elif,
    Else,
    Source,
    Single,
    Predicate,
    PostAction,
    Type,
    Name,
    Faction,
    Power,
    Range,
    OnActivation,
    Selector,
    Pow,
    Implication,
    Increment,
    Plus,
    Minus,
    Division, 
    Multip,
    And,
    Or,
    Less,
    More,
    Equal,
    LessEq,
    MoreEq,
    SpaceConcatenation,
    Concatenation,
    Assign,
    LParen,
    RParen,
    LBracket,
    RBracket,
    LCurly,
    RCurly,
    Semicolon,
    Colon, 
    Point, 
    Comma,
    Boolean,
    String,
    Num,
    Id,
    Params,
    Action,
    EffectCard,
    Amount,
    In,
    Hand,
    Deck,
    Board,
    Target,
    Targets,
    Context,
    TriggerPlayer,
    Find,
    Push,
    SendBottom,
    Pop,
    Remove,
    Shuffle,
    NotEquals,
    Null, 

    //Value types
    Number, 
    Bool, 
    Text
}
public class Token {
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
        //Comment and whitespaces
        {TokenType.Null, @"\s+|\/\/.*|(?s)/\*.*?\*/"},
        //Reserved words 
        {TokenType.For, @"\bfor\b"},
        {TokenType.While, @"\bwhile\b"},
        {TokenType.Effect, @"\beffect\b"},
        {TokenType.C_Effect, @"\bEffect\b"},
        {TokenType.Card, @"\bcard\b"},
        {TokenType.If, @"\bif\b"},
        {TokenType.Elif, @"\belif\b"},
        {TokenType.Else, @"\belse\b"},
        {TokenType.Predicate, @"\bPredicate\b"},
        {TokenType.PostAction, @"\bPostAction\b"},
        {TokenType.Type, @"\bType\b"},
        {TokenType.Name, @"\bName\b"},
        {TokenType.Params, @"\bParams\b"},
        {TokenType.Action, @"\bAction\b"},
        {TokenType.Source, @"\bSource\b" },
        {TokenType.Single, @"\bSingle\b" },
        {TokenType.Amount, @"\bAmount\b" },
        {TokenType.In, @"\bin\b" },
        {TokenType.Hand, @"\bhand\b" },
        {TokenType.Deck, @"\bdeck\b" },
        {TokenType.Board, @"\bboard\b" },
        {TokenType.Targets, @"\btargets\b" },
        {TokenType.Target, @"\btarget\b" },
        {TokenType.Context, @"\bcontext\b" },
        {TokenType.TriggerPlayer, @"\bTriggerPlayer\b" },
        {TokenType.Find, @"\bFind\b" },
        {TokenType.Push, @"\bPush\b" },
        {TokenType.SendBottom, @"\bSendBottom\b" },
        {TokenType.Pop, @"\bPop\b" },
        {TokenType.Remove, @"\bRemove\b" },
        {TokenType.Shuffle, @"\bShuffle\b" },
        {TokenType.Faction, @"\bFaction\b"},
        {TokenType.Power, @"\bPower\b"},
        {TokenType.Range, @"\bRange\b"},
        {TokenType.OnActivation, @"\bOnActivation\b"},
        {TokenType.Selector, @"\bSelector\b"},
        //Value Types
        {TokenType.Bool, @"\bBool\b"},
        {TokenType.Text, @"\bText\b"},
        {TokenType.Number, @"\bNumber\b"},

        //Operators

        {TokenType.NotEquals, @"!="},
        {TokenType.Implication, @"=>"},
        {TokenType.Increment, @"\+\+"},
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
        {TokenType.LParen, @"\("},
        {TokenType.RParen, @"\)"},
        {TokenType.LBracket, @"\["},
        {TokenType.RBracket, @"\]"},
        {TokenType.LCurly, @"\{"},
        {TokenType.RCurly, @"\}"},
        {TokenType.Semicolon, ";"},
        {TokenType.Colon, @":"},
        {TokenType.Point, @"\."},
        {TokenType.Comma, ","},
        //Types
        {TokenType.Boolean, @"\b(true|false)\b"},
        {TokenType.Num, @"\b\d+(\.\d+)?\b"},
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
