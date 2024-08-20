using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Struct that represent a token
public struct Token {
    ///<summary>
    ///Return the string value
    ///</summary>
    public string Value {get;}
    ///<summary>
    ///Return the type
    ///</summary>
    public TokenType Definition {get;}
    ///<summary>
    ///Return the line of the value in the input
    ///</summary>
    public int Line {get;}
    ///<summary>
    ///Return the column of the value in the input
    ///</summary>
    public int Column {get;}
    public Token(string value, TokenType definition, int line = 0, int column= 0)
    {
        this.Value = value;
        this.Definition = definition;
        this.Line = line;
        this.Column = column;
    }
}