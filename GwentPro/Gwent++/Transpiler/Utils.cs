using Transpiler;

public static class Utils {
    //Hash set that save tokentypes that are reserved words but can be used as properties
    public static HashSet<TokenType> PropertiesReservedWords = [TokenType.Faction, TokenType.Name, TokenType.Power, TokenType.Type];
    ///<summary>
    ///Relate the types with a hashset of possible properties or functions represented as string 
    ///</summary>
    //TODO: Agregar otherhand etc...
    public static Dictionary<IdType, HashSet<string>> ValidAccess = new Dictionary<IdType, HashSet<string>>{
        {IdType.Context, ["TriggerPlayer", "Board", "Hand", "HandOfPLayer", "FieldOfPlayer", "GraveyardOfPlayer", "DeckOfPlayer", "Find"]},
        {IdType.Card, ["Owner", "Power", "Faction", "Name", "Type"]},
        {IdType.Player, ["Enemy"]},
        {IdType.CardCollection, ["Find", "Push", "SendBottom", "Pop", "Remove", "Shuffle", "Add"]}
    };
    ///<summary>
    ///Relate the functions with the types of the arguments 
    ///</summary>
    public static Dictionary<string, IdType?> ValidArguments = new Dictionary<string, IdType?>{
        {"Find", IdType.Predicate},
        {"Push", IdType.Card},
        {"SendBottom", IdType.Card},
        {"Pop", null},
        {"Remove", IdType.Card},
        {"Shuffle", null},
        {"Add", IdType.Card},

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
        {"Find", IdType.Card},
        {"Push", IdType.Null},
        {"SendBottom", IdType.Null},
        {"Pop", IdType.Card},
        {"Remove", IdType.Null},
        {"Shuffle", IdType.Null},
        {"Add", IdType.Null},

        {"HandOfPlayer", IdType.CardCollection},
        {"FieldOfPlayer", IdType.CardCollection},
        {"GraveyardOfPlayer", IdType.CardCollection},
        {"DeckOfPlayer", IdType.CardCollection},
        {"Owner", IdType.Player},
        {"TriggerPlayer", IdType.Player},
        {"Board", IdType.CardCollection},

        {"Power", IdType.Number},
        {"Faction", IdType.String},
        {"Type", IdType.String},
        {"Name", IdType.String}
    };
}