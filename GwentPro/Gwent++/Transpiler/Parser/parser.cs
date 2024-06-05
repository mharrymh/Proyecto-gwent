namespace Transpiler;

class Parser
{
    List<Token> Tokens {get; }
    int Position {get; set;}
    Token NextToken {get; set;}
    bool ValidNextToken {get; set;}

    void LookAhead(List<TokenType> tokenTypes)
    {
        //Chequea el nextToken
    }

    void Consume(TokenType tokenType)
    {
        //Aumenta la posicion
    }

    public Parser(List<Token> tokens)
    {
        this.Tokens = tokens;
        Position = 0;
    }

    public Expression Parse()
    {
        return ParseDeclarations();
    }

    Expression ParseDeclarations()
    {
        var left = ParseDec();
        List<TokenType> tokenTypes = new List<TokenType>{TokenType.Effect, TokenType.Card};
        LookAhead(tokenTypes);
        if (ValidNextToken)
        {
            var right = ParseDeclarations();
            left = new Declarations(left, right); 
        }
        return left;
    }

    Expression ParseDec()
    {
        var left = ParseEfDec();
        List<TokenType> tokenTypes = new List<TokenType>{TokenType.Effect, TokenType.Card};
        LookAhead(tokenTypes);
        //An effect declaration has to exist before a card one???
        if (!ValidNextToken)
        {
            return left;
        }    
        if (NextToken.Definition is TokenType.Effect)
        {
            var right = ParseEfDec();
            left = new Dec(left, right);
        }
        else if (NextToken.Definition is TokenType.Card)
        {
            var right = ParseCardDec();
            left = new Dec(left, right);
        }
        return left;
    }

    Expression ParseEfDec()
    {
        Consume(TokenType.Effect);

        Consume(TokenType.LCurly);
        var left = ParseEffProps();
        Consume(TokenType.RCurly);
        return left;
    }

    Expression ParseEffProps()
    {
        var left = ParseEffProp();
        LookAhead(new List<TokenType>{TokenType.Comma, TokenType.Semicolon});
        if (NextToken.Definition is TokenType.Comma)
        {
            Consume(TokenType.Comma);
            var right = ParseEffProps();
            left = new EfProperties(left, right);
        }
        Consume(TokenType.Semicolon);
        return left;
    }

    Expression ParseEffProp() 
    {
        
        LookAhead(new List<TokenType>{TokenType.Name,TokenType.Params, TokenType.Action});
        if (ValidNextToken)
        {
            if ()
        }
    }




}
