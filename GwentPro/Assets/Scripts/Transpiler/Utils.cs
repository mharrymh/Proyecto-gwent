using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
public static class Utils {
    //TODO: Cambiar imagen
    public static Sprite DefaultCardImage = Resources.Load<Sprite>("0");
    //Hash set that save tokentypes that are reserved words but can be used as properties
    public readonly static HashSet<TokenType> PropertiesReservedWords = new HashSet<TokenType>{TokenType.Faction, TokenType.Name, TokenType.Power, TokenType.Type};
    ///<summary>
    ///Relate the types with a hashset of possible properties or functions represented as string 
    ///</summary>
    public readonly static Dictionary<IdType, HashSet<string>> ValidAccess = new Dictionary<IdType, HashSet<string>>
    {
        {IdType.Context, new HashSet<string>{"TriggerPlayer", "BoardCards", "Hand", "HandOfPlayer", "Field", "FieldOfPlayer", "Graveyard","GraveyardOfPlayer","Deck" ,"DeckOfPlayer", "Enemy"}},
        {IdType.Card, new HashSet<string>{"Power", "Faction", "Name", "Type"}},
        {IdType.CardCollection, new HashSet<string>{"Find", "Push", "SendBottom", "Pop", "Remove", "Shuffle", "Add", "Clear", "RemoveAt", "Count"}}
    };


    ///<summary>
    ///Relate the functions with the types of the arguments 
    /// ///</summary>
    public readonly static Dictionary<string, IdType?> ValidArguments = new Dictionary<string, IdType?>{
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
    ///Relate the functions with the types that it return
    ///</summary>
    public readonly static Dictionary<string, IdType> Types = new Dictionary<string, IdType>{
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
    /// <summary>
    /// Relate the values with the id types that can call to it
    /// </summary>
    /// <value></value>
    public readonly static Dictionary<string, IdType> relateValuesWithTypes = new Dictionary<string, IdType>{
        //Functions
        {"Find", IdType.CardCollection},
        {"Push", IdType.CardCollection},
        {"SendBottom", IdType.CardCollection},
        {"Pop", IdType.CardCollection},
        {"Remove", IdType.CardCollection},
        {"Shuffle", IdType.CardCollection},
        {"Add", IdType.CardCollection},
        {"Clear", IdType.CardCollection},
        {"RemoveAt", IdType.CardCollection},

        {"HandOfPlayer", IdType.Context},
        {"FieldOfPlayer", IdType.Context},
        {"GraveyardOfPlayer", IdType.Context},
        {"DeckOfPlayer", IdType.Context},

        //Properties
        {"Enemy", IdType.Context},
        {"TriggerPlayer", IdType.Context},
        {"BoardCards", IdType.Context},
        {"Deck", IdType.Context},
        {"Hand", IdType.Context},
        {"Field", IdType.Context},
        {"Graveyard", IdType.Context},
        {"Count", IdType.Context},


        {"Power", IdType.Card},
        {"Faction", IdType.Card},
        {"Type", IdType.Card},
        {"Name", IdType.Card}
    };
    /// <summary>
    /// Relate funtion names with the types that can call to it
    /// </summary>
    /// <value></value>
    public readonly static Dictionary<string, IdType> relateFunctionsWithTheTypeThatCallToIt = new Dictionary<string, IdType>
    {
        {"Find", IdType.CardCollection},
        {"Push", IdType.CardCollection},
        {"SendBottom", IdType.CardCollection},
        {"Pop", IdType.CardCollection},
        {"Remove", IdType.CardCollection},
        {"Shuffle", IdType.CardCollection},
        {"Add", IdType.CardCollection},
        {"Clear", IdType.CardCollection},
        {"RemoveAt", IdType.CardCollection},

        {"HandOfPlayer", IdType.Context},
        {"FieldOfPlayer", IdType.Context},
        {"GraveyardOfPlayer", IdType.Context},
        {"DeckOfPlayer", IdType.Context},
    };
}
