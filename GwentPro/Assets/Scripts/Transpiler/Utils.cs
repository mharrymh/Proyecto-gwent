using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
public static class Utils {
    //TODO: Cambiar imagen
    public static Sprite DefaultCardImage = Resources.Load<Sprite>("0");
    //Hash set that save tokentypes that are reserved words but can be used as properties
    public static HashSet<TokenType> PropertiesReservedWords = new HashSet<TokenType>{TokenType.Faction, TokenType.Name, TokenType.Power, TokenType.Type};
    ///<summary>
    ///Relate the types with a hashset of possible properties or functions represented as string 
    ///</summary>
    public static Dictionary<IdType, HashSet<string>> ValidAccess = new Dictionary<IdType, HashSet<string>>
    {
        {IdType.Context, new HashSet<string>{"TriggerPlayer", "BoardCards", "Hand", "HandOfPlayer", "FieldOfPlayer", "GraveyardOfPlayer", "DeckOfPlayer", "Find", "Enemy"}},
        {IdType.Card, new HashSet<string>{"Owner", "Power", "Faction", "Name", "Type"}},
        {IdType.CardCollection, new HashSet<string>{"Find", "Push", "SendBottom", "Pop", "Remove", "Shuffle", "Add", "Clear", "RemoveAt"}}
    };
    ///<summary>
    ///Relate the functions with the types of the arguments 
    /// ///</summary>
    public static Dictionary<string, IdType?> ValidArguments = new Dictionary<string, IdType?>{
        {"Find", IdType.Predicate},
        {"Push", IdType.Card},
        {"SendBottom", IdType.Card},
        {"Pop", null},
        {"Remove", IdType.Card},
        {"Shuffle", null},
        {"Add", IdType.Card},
        {"Clear", null},
        {"RemoveAt", IdType.Number},

        {"HandOfPlayer", IdType.Player},
        {"FieldOfPlayer", IdType.Player},
        {"GraveyardOfPlayer", IdType.Player},
        {"DeckOfPlayer", IdType.Player}
    };
    ///<summary>
    ///Relate the functions with the valid types than can call to it
    ///</summary>
    public static Dictionary<string, IdType> Types = new Dictionary<string, IdType>{
        //Functions
        {"Find", IdType.CardCollection},
        {"Push", IdType.Null},
        {"SendBottom", IdType.Null},
        {"Pop", IdType.Card},
        {"Remove", IdType.Null},
        {"Shuffle", IdType.Null},
        {"Add", IdType.Null},
        {"Clear", IdType.Null},
        {"RemoveAt", IdType.Null},

        {"HandOfPlayer", IdType.CardCollection},
        {"FieldOfPlayer", IdType.CardCollection},
        {"GraveyardOfPlayer", IdType.CardCollection},
        {"DeckOfPlayer", IdType.CardCollection},

        //Properties
        {"Owner", IdType.Player},
        {"Enemy", IdType.Player},
        {"TriggerPlayer", IdType.Player},
        {"BoardCards", IdType.CardCollection},
        {"Deck", IdType.CardCollection},
        {"Hand", IdType.CardCollection},
        {"Field", IdType.CardCollection},
        {"Graveyard", IdType.CardCollection},
        {"Count", IdType.Number},


        {"Power", IdType.Number},
        {"Faction", IdType.String},
        {"Type", IdType.String},
        {"Name", IdType.String}
    };

    //Get what type can call to another type
    public static Dictionary<IdType, HashSet<IdType>> RelateTypes = new Dictionary<IdType, HashSet<IdType>>
    {
        {IdType.Context, new HashSet<IdType>{IdType.CardCollection, IdType.Player}},
        {IdType.Card, new HashSet<IdType>{IdType.Number, IdType.Player, IdType.String}},
        {IdType.CardCollection, new HashSet<IdType>{IdType.Null, IdType.CardCollection, IdType.Card}}
    };


    internal static Token GetErrorToken(Expression exp)
    {
        //Create a pointer to the expression
        Expression pointer = exp;
        while (pointer is BinaryExpression binary)
        {
            pointer = binary.Left;
        }
        if (pointer is UnaryExpression unary) return unary.Op;
        else if (pointer is LiteralExpression literal) return literal.Value;
        else if (pointer is Indexer indexer) return GetErrorToken(indexer.Body);
        else if (pointer is FunctionCall functionCall) return functionCall.FunctionName;
        else return GetErrorToken(((FindFunction)exp).Body);
    }
}
