using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.Linq;

public class Lexer {
    //Create a dictionary that relates types with its regular expressions
    readonly Dictionary<TokenType, string> TokenDefinitions = new Dictionary<TokenType, string>
    {
        // Comment and whitespaces
        {TokenType.Null, @"^\s+|^\/\/[^\n]*|^\/\*[\s\S]*?\*\/"},

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
        {TokenType.MultipAssign, @"\*="},
        {TokenType.DivisionAssign, @"\/="},
        {TokenType.NotEquals, @"!="}, 
        {TokenType.Implication, @"=>"}, 
        {TokenType.Increment, @"\+\+"}, 
        {TokenType.Decrement, @"--"}, 
        {TokenType.Pow, @"\^"},
        {TokenType.Plus, @"\+"}, 
        {TokenType.Minus, @"-"}, 
        {TokenType.Multip, @"\*"}, 
        {TokenType.Division, @"\/"}, 
        {TokenType.And, @"\&\&"}, 
        {TokenType.Or, @"\|\|"},
        {TokenType.LessEq, "<="}, 
        {TokenType.MoreEq, ">="}, 
        {TokenType.Less, "<"}, 
        {TokenType.More, ">"}, 
        {TokenType.Equal, "=="}, 
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
    //Function that returns the input as a list of tokens
    public List<Token> Tokenize(string input)
    {
        //Initialize the needed variables
        List<Token> tokens = new List<Token>();
        string remainingInput = input;
        int actualLine = 1;
        int actualColumn = 1;

        //Keep tokenizing while the remaining input is not empty
        while (!string.IsNullOrEmpty(remainingInput)) {
            bool matchFound = false;

            foreach(var tokenDef in TokenDefinitions) {
                Match match = Regex.Match(remainingInput, "^" + tokenDef.Value);

                if (match.Success) {
                    if (match.Value.Contains('\n')) {
                        int newLines = match.Value.Count(c => c == '\n');
                        int carriageReturnCount = match.Value.Count(c => c == '\r');
                        actualLine += newLines; //Increment the actual line for each new line
                        actualColumn = 1 - newLines - carriageReturnCount; //Ignore characters of new line 
                    }
                    //Ignore whitespaces and comments
                    if (tokenDef.Key != TokenType.Null) 
                    {
                        tokens.Add(new Token(match.Value, tokenDef.Key, actualLine, actualColumn));
                    }
                    //Delete the tokenized string from the remaining input
                    remainingInput = remainingInput.Substring(match.Value.Length);
                    actualColumn += match.Value.Length;
                    matchFound = true;
                    break;
                }
            }
            //A token didn't match with any of the regular expresions
            if (!matchFound) {
                //Throw error
                Error invalidToken = new InvalidSyntaxError(actualLine, actualColumn, remainingInput[0]);
                throw new Exception(invalidToken.ToString());
            }
        }
        //Return the list
        return tokens;
    } 
}
