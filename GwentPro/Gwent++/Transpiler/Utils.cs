using Transpiler;

public static class Utils {
    //Hash set that save tokentypes that are reserved words but can be used as properties
    public static HashSet<TokenType> PropertiesReservedWords = [TokenType.Faction, TokenType.Name, TokenType.Power, TokenType.Type];
}