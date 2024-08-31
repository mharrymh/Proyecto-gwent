//Save all different token types
public enum TokenType
{
    // Reserved words
    For, While, Effect, C_Effect, Card, Source, Single, 
    Predicate, PostAction, Type, Name, Faction, Power, Range, OnActivation, 
    Selector, Implication, In, 

    // Operators
    Increment, Decrement, Plus, Minus, Division, Multip, Pow, And, 
    Or, Less, More, Equal, LessEq, MoreEq, SpaceConcatenation, 
    Concatenation, Assign, MinusAssign, MoreAssign, DivisionAssign,
    MultipAssign, NotEquals,

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

//An enumerable with all the different types of id
public enum IdType {
    Number, 
    String,
    Boolean,
    Context,
    Targets, 
    Card,
    Player,
    CardCollection,
    Predicate,
    Null
}

