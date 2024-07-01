namespace Transpiler;
public class Parser
{
    // List of tokens to be parsed
    List<Token> Tokens { get; }
    // Current position in the token list
    int Pos {get; set;}
    // The next token to be parsed
    Token NextToken {get; set;}

    // Look ahead to the next token and check if it matches the expected token type
    void LookAhead(TokenType tokenType)
    {
        if (Pos+1 >= Tokens.Count) 
        {
            // If we've reached the end of the token list, throw an exception
            Token last = Tokens[^1];
            UnexpectedEndOfInput error = new UnexpectedEndOfInput(last.Line, last.Column, last);
            throw new Exception(error.ToString());
        }
        if (tokenType == Tokens[Pos+1].Definition) {
            NextToken = Tokens[Pos+1];
        }
        else 
        {
            // If the next token doesn't match the expected type, throw an exception
            Token token = Tokens[Pos+1];
            UnexpectedToken error = new UnexpectedToken(token.Line, token.Column, token);
            throw new Exception(error.ToString());
        }
    }
    // Look ahead to the next token and check if it matches any of the expected token types
    void LookAhead(List<TokenType>? expected = null)
    {
        if (Pos+1 >= Tokens.Count) 
        {
            // If we've reached the end of the token list, throw an exception
            Token last = Tokens[^1];
            UnexpectedEndOfInput error = new UnexpectedEndOfInput(last.Line, last.Column, last);
            throw new Exception(error.ToString());
        }

        if(expected == null) 
        { 
            // If no expected token types are provided, just move to the next token
            NextToken = Tokens[Pos+1]; 
            return; 
        }

        if (expected.Contains(Tokens[Pos+1].Definition)) {
            NextToken = Tokens[Pos+1];
        }
        else 
        {
            // If the next token doesn't match any of the expected types, throw an exception
            Token token = Tokens[Pos+1];
            UnexpectedToken error = new UnexpectedToken(token.Line, token.Column, token);
            throw new Exception(error.ToString());
        }
    }

    // Consume the next token if it matches the expected token type
    void Consume(TokenType tokenType)
    {
        if (Tokens[Pos+1].Definition == tokenType) Pos++;
        else {
            // If the next token doesn't match the expected type, throw an exception
            Token token = Tokens[Pos+1];
            UnexpectedToken error = new UnexpectedToken(token.Line, token.Column, token);
            throw new Exception(error.ToString());
        }
    }

    // Constructor to initialize the parser with a list of tokens
    public Parser(List<Token> tokens)
    {
        this.Tokens = tokens;
        this.Pos = -1;
        this.NextToken = Tokens[0];
    }

    // Parse the entire input and return a DSL object
    public DecBlock Parse()
    {
        var aux = ParseDecBlock();
        return aux;
    }

    // Parse a declaration block
    DecBlock ParseDecBlock()
    {
        return new DecBlock(ParseEffDecBlock(), ParseCardDecBlock());
    }

    #region EffectNodes

    // Parse a list of effect declarations
    List<Effect> ParseEffDecBlock()
    {
        var effects = new List<Effect>();
        effects.Add(ParseEffect());
        LookAhead();
        while (NextToken.Definition is TokenType.Effect)
        {
            effects.Add(ParseEffect());
            LookAhead();
        }
        return effects;
    }

    // Parse a single effect declaration
    Effect ParseEffect()
    {
        Consume(TokenType.Effect);
        Consume(TokenType.LCurly);

        Expression? name = null;
        Dictionary<Token, Token>? param = null;
        InstructionBlock? action = null;

        List<TokenType> expected = new List<TokenType>{TokenType.Name, TokenType.Params, TokenType.Action};
        List<TokenType> colons = new List<TokenType>{TokenType.Comma, TokenType.RCurly};
        LookAhead(expected);
        while(expected.Contains(NextToken.Definition))
        {
            //Check nextToken
            //Break if it is a semicolon
            if (NextToken.Definition is TokenType.Name)
            {
                name = ParseName();
                LookAhead(colons);
                if (NextToken.Definition is TokenType.Comma) Consume(TokenType.Comma);
                else break;
                expected.Remove(TokenType.Name);
                LookAhead(expected);
            }
            else if (NextToken.Definition is TokenType.Params)
            {
                param = ParseParam();
                LookAhead(colons);
                if (NextToken.Definition is TokenType.Comma) Consume(TokenType.Comma);
                else break;
                expected.Remove(TokenType.Params);
                LookAhead(expected);
            }
            else
            {
                action = ParseAction();
                LookAhead(colons);
                if (NextToken.Definition is TokenType.Comma) Consume(TokenType.Comma);
                else break;
                expected.Remove(TokenType.Action);
                LookAhead(expected);
            }
        }
        Consume(TokenType.RCurly);
        if (name != null && action != null)
        {
            return new Effect(name, param, action);
        }
        //FIXME:
        throw new Exception("Implementar excepcion"); //name y action no pueden ser nulos
    }

    Expression ParseName()
    {
        Consume(TokenType.Name);
        Consume(TokenType.Colon);
        LookAhead(TokenType.String);
        return ParseStringExpression();
    }
    public Dictionary<Token, Token> ParseParam()
    {
        Consume(TokenType.Params);
        Consume(TokenType.Colon);
        Consume(TokenType.LCurly);

        Dictionary<Token, Token> id_type = new Dictionary<Token, Token>();
        Token id;
        var expected = new List<TokenType>{TokenType.RCurly, TokenType.Id};
        LookAhead(expected);
        while(NextToken.Definition != TokenType.RCurly)
        {
            id = NextToken;
            Consume(NextToken.Definition);
            Consume(TokenType.Colon);
            LookAhead(new List<TokenType>{TokenType.Number, TokenType.Bool, TokenType.Text});
            //Add pair to the dictionary
            id_type.Add(id, NextToken);
            Consume(NextToken.Definition);
            LookAhead(new List<TokenType>{TokenType.RCurly, TokenType.Comma});
            if (NextToken.Definition == TokenType.Comma) Consume(TokenType.Comma);
            LookAhead(expected);
        }
        Consume(TokenType.RCurly);
        return id_type;
    }

    InstructionBlock ParseAction()
    {
        Consume(TokenType.Action);
        Consume(TokenType.Colon);
        Consume(TokenType.LParen);
        Consume(TokenType.Targets);
        Consume(TokenType.Comma);
        Consume(TokenType.Context);
        Consume(TokenType.RParen);
        Consume(TokenType.Implication);
        Consume(TokenType.LCurly);

        var instruction = ParseInstruction();
        Consume(TokenType.RCurly);
        return instruction;
    }
    InstructionBlock ParseInstruction()
    {
        List<ForLoop> forLoops = new List<ForLoop>();
        List<WhileLoop> whileLoops = new List<WhileLoop>();
        List<Expression> allocations = new List<Expression>();

        var expected = new List<TokenType>{TokenType.For, TokenType.While, TokenType.Id, TokenType.RCurly, TokenType.Context, TokenType.Targets};
        LookAhead();
        while(NextToken.Definition != TokenType.RCurly)
        {
            if (NextToken.Definition == TokenType.Semicolon) {
                Consume(TokenType.Semicolon);
            }
            else if (NextToken.Definition == TokenType.For) {
                forLoops.Add(ParseForLoop());
            }
            else if (NextToken.Definition == TokenType.While) {
                whileLoops.Add(ParseWhileLoop());
            }
            else {
                allocations.Add(ParseIdExpression());
            }
            LookAhead();
        }
        return new InstructionBlock(forLoops, whileLoops, allocations);
    }
    WhileLoop ParseWhileLoop()
    {
        Consume(TokenType.While);
        Consume(TokenType.LParen);
        var exp = ParseBoolExpression();
        Consume(TokenType.RParen);
        LookAhead();

        //Check if the while instruction is inside brackets
        if (NextToken.Definition is TokenType.LCurly)
        {
            Consume(TokenType.LCurly);
            var instruction = ParseInstruction();
            Consume(TokenType.RCurly);
            return new WhileLoop(exp, ParseInstruction());
        }
        //Parse the instruction line
        return new WhileLoop(exp, ParseInstruction());
    }

    ForLoop ParseForLoop()
    {
        Consume(TokenType.For);
        Consume(TokenType.Target);
        Consume(TokenType.In);
        Consume(TokenType.Targets);
        Consume(TokenType.LCurly);
        var instruction = ParseInstruction();
        Consume(TokenType.RCurly);
        return new ForLoop(instruction);
    }
    
    #endregion

    #region CardNodes
    List<Card> ParseCardDecBlock()
    {
        var cards = new List<Card>();
        cards.Add(ParseCard());
        while(Pos + 1 < Tokens.Count)
        {
            LookAhead(TokenType.Card);
            cards.Add(ParseCard());
        }
        return cards;
    }
    Card ParseCard()
    {
        Consume(TokenType.Card);
        Consume(TokenType.LCurly);

        Expression? name = null;
        Expression? type = null;
        Expression? faction = null;
        Expression? power = null;
        List<Expression>? range = null;
        List<EffectAllocation>? activation= null;

        List<TokenType> expected = new List<TokenType>{TokenType.Name, TokenType.Type, TokenType.Faction, TokenType.Power, TokenType.Range, TokenType.OnActivation};
        List<TokenType> colons = new List<TokenType>{TokenType.Comma, TokenType.RCurly};
        LookAhead(expected);
        while(expected.Contains(NextToken.Definition))
        {
            TokenType tokenType = NextToken.Definition;
            //Check nextToken
            //Break if it is a semicolon
            if (tokenType is TokenType.Name) name = ParseName();
            else if (tokenType is TokenType.Type) type = ParseType();
            else if (tokenType is TokenType.Faction) faction = ParseFaction();
            else if (tokenType is TokenType.Power) power = ParsePower();
            else if (tokenType is TokenType.Range) range = ParseRange();
            else activation = ParseActivation();
            
            LookAhead(colons);
            if (NextToken.Definition is TokenType.Comma) Consume(TokenType.Comma);
            else break;
            expected.Remove(tokenType);
            LookAhead(expected);
        }
        Consume(TokenType.RCurly);

        if (name != null && faction != null && type != null && activation != null)
        {
            return new Card(name, type, faction, power, range, activation);
        }
        List<NotNullableObj> notNullableObjs= new List<NotNullableObj>{
            new NotNullableObj("Name", name),
            new NotNullableObj("Type", type),
            new NotNullableObj("Faction", faction)
        };
        if (activation == null) notNullableObjs.Add(new NotNullableObj("OnActivation", null));
        Error error = new ParameterUnknown(NextToken.Column, NextToken.Line, NextToken, notNullableObjs);
        throw new Exception(error.ToString());
    }

    List<EffectAllocation> ParseActivation()
    {
        Consume(TokenType.OnActivation);
        Consume(TokenType.Colon);
        Consume(TokenType.LBracket);
        LookAhead();
        var effBlock = ParseEffBlock();
        Consume(TokenType.RBracket);
        return effBlock;
    }

    List<EffectAllocation> ParseEffBlock()
    {
        Consume(TokenType.LCurly);
        var effects = new List<EffectAllocation>();
        effects.Add(ParseEffAllocation());
        Consume(TokenType.RCurly);
        LookAhead(new List<TokenType>{TokenType.Comma, TokenType.RCurly});
        while (NextToken.Definition is TokenType.Comma)
        {
            Consume(TokenType.Comma);
            Consume(TokenType.LCurly);
            effects.Add(ParseEffAllocation());
            Consume(TokenType.RCurly);
        }
        Consume(TokenType.RCurly);
        return effects;
    }

    EffectAllocation ParseEffAllocation()
    {
        Allocation? allocation = null;
        Selector? selector = null;
        PostActionBlock? postAction = null;

        List<TokenType> expected = new List<TokenType>{TokenType.C_Effect, TokenType.Selector, TokenType.PostAction, TokenType.RCurly};
        List<TokenType> colons = new List<TokenType>{TokenType.Comma, TokenType.RCurly};
        LookAhead(expected);
        while(expected.Contains(NextToken.Definition))
        {
            TokenType tokenType = NextToken.Definition;
            //Check nextToken
            //Break if it is a RCurly
            if (tokenType is TokenType.C_Effect) allocation = ParseAllocation();
            else if (tokenType is TokenType.Selector) selector = ParseSelector();
            else if(tokenType is TokenType.PostAction) postAction = ParsePostAction();
            else break;

            LookAhead(colons);
            if (NextToken.Definition is TokenType.Comma) Consume(TokenType.Comma);
            else break;
            expected.Remove(tokenType);
            LookAhead(expected);
        }
        if (allocation != null)
        {
            return new EffectAllocation(allocation, selector, postAction);
        }
        //Name hasnt been declared
        List<NotNullableObj> notNullableObjs= new List<NotNullableObj>{
            new NotNullableObj("Name", allocation),
        };
        Error error = new ParameterUnknown(NextToken.Column, NextToken.Line, NextToken, notNullableObjs);
        throw new Exception(error.ToString());
    }
    Expression ParseType()
    {
        Consume(TokenType.Type);
        Consume(TokenType.Colon);
        LookAhead(TokenType.String);
        return ParseStringExpression();
    }

    Expression ParseFaction()
    {
        Consume(TokenType.Faction);
        Consume(TokenType.Colon);
        LookAhead(TokenType.String);
        return ParseStringExpression();
    }

    Expression ParsePower()
    {
        Consume(TokenType.Power);
        Consume(TokenType.Colon);

        return ParseNumericExpression();
    }
    List<Expression> ParseRange()
    {
        Consume(TokenType.Range);
        Consume(TokenType.Colon);
        Consume(TokenType.LBracket);
        List<Expression> ranges = new List<Expression>();
        var expected = new List<TokenType>{TokenType.RBracket, TokenType.String};
        LookAhead(expected);
        while(NextToken.Definition != TokenType.RBracket)
        {
            ranges.Add(ParseStringExpression());
            LookAhead(new List<TokenType>{TokenType.Comma, TokenType.RBracket});
            if (NextToken.Definition is TokenType.Comma) Consume(TokenType.Comma);
            LookAhead(expected);
        }
        Consume(TokenType.RBracket);
        return ranges;
    }
    PostActionBlock ParsePostAction()
    {
        Consume(TokenType.PostAction);
        Consume(TokenType.Colon);
        Consume(TokenType.LCurly);
        return new PostActionBlock(ParseEffAllocation());
    }
    Selector ParseSelector()
    {
        Consume(TokenType.Selector);
        Consume(TokenType.Colon);
        Consume(TokenType.LCurly);

        Expression? source = null;
        Expression? single = null;
        Predicate? predicate = null;

        List<TokenType> expected = new List<TokenType>{TokenType.Source, TokenType.Single, TokenType.Predicate};
        List<TokenType> colons = new List<TokenType>{TokenType.Comma, TokenType.RCurly};
        LookAhead(expected);
        while(expected.Contains(NextToken.Definition))
        {
            TokenType tokenType = NextToken.Definition;
            //Check nextToken
            //Break if it is a semicolon
            if (tokenType is TokenType.Source) source = ParseSource();
            else if (tokenType is TokenType.Single) single = ParseSingle();
            else predicate = ParsePredicate();

            LookAhead(colons);
            if (NextToken.Definition is TokenType.Comma) Consume(TokenType.Comma);
            else break;
            expected.Remove(tokenType);
            LookAhead(expected);
        }
        Consume(TokenType.RCurly);
        if (source != null && predicate != null)
        {
            return new Selector(source, single, predicate);
        }
        List<NotNullableObj> notNullableObjs= new List<NotNullableObj>{
            new NotNullableObj("Source", source),
            new NotNullableObj("Predicate", predicate),
        };
        Error error = new ParameterUnknown(NextToken.Column, NextToken.Line, NextToken, notNullableObjs);
        throw new Exception(error.ToString());
    }
    Predicate ParsePredicate()
    {
        Consume(TokenType.Predicate);
        Consume(TokenType.Colon);
        Consume(TokenType.LParen);
        LookAhead(TokenType.Id);
        Token id = NextToken;
        Consume(NextToken.Definition);
        Consume(TokenType.RParen);
        Consume(TokenType.Implication);
        return new Predicate(id, ParseBoolExpression());
    }
    Expression ParseSingle()
    {
        Consume(TokenType.Single);
        Consume(TokenType.Colon);
        return ParseBoolExpression();
    }
    Expression ParseSource()
    {
        Consume(TokenType.Source);
        Consume(TokenType.Colon);
        LookAhead(TokenType.String);
        return ParseStringExpression();
    }
    Allocation ParseAllocation()
    {
        Consume(TokenType.C_Effect);
        Consume(TokenType.Colon);
        LookAhead(new List<TokenType>{TokenType.String, TokenType.LCurly});
        if (NextToken.Definition is TokenType.String)
        {
            return new Allocation(ParseStringExpression(), null);
        }
        Expression? name = null;
        Dictionary<Token, Expression>? varAllocation = null; 
        Consume(TokenType.LCurly);

        var expected = new List<TokenType>{TokenType.Id, TokenType.Name};
        List<TokenType> colons = new List<TokenType>{TokenType.Comma, TokenType.RCurly};
        LookAhead(expected);
        while(expected.Contains(NextToken.Definition))
        {
            TokenType tokenType = NextToken.Definition;
            
            if (NextToken.Definition is TokenType.Name) name = ParseName();
            else varAllocation = ParseVarAllocation();

            LookAhead(colons);
            if (NextToken.Definition is TokenType.Comma) Consume(TokenType.Comma);
            else break;
            expected.Remove(tokenType);
            LookAhead(expected);
        }
        Consume(TokenType.RCurly);
        if (name != null)
        {
            return new Allocation(name, varAllocation);
        }
        //Throw an error
        //Name cant be null
        List<NotNullableObj> notNullableObjs= new List<NotNullableObj>{
            new NotNullableObj("Name", name),
        };
        Error error = new ParameterUnknown(NextToken.Column, NextToken.Line, NextToken, notNullableObjs);
        throw new Exception(error.ToString());
    }
    Dictionary<Token, Expression> ParseVarAllocation()
    {
        Dictionary<Token, Expression> id_allocation = new Dictionary<Token, Expression>();
        Token id;
        var expected = new List<TokenType>{TokenType.RCurly, TokenType.Id};
        LookAhead(expected);
        while(NextToken.Definition != TokenType.RCurly)
        {
            id = NextToken;
            Consume(NextToken.Definition);
            Consume(TokenType.Colon);
            LookAhead();
            Expression right = ParseExpression();
            //Add pair to the dictionary
            id_allocation.Add(id, right);
            LookAhead(new List<TokenType>{TokenType.RCurly, TokenType.Comma});
            if (NextToken.Definition == TokenType.Comma) Consume(TokenType.Comma);
            else break;
            LookAhead(expected);
        }
        return id_allocation;
    }
    
    Expression ParseExpression()
    {
        LookAhead();
        if(NextToken.Definition is TokenType.Num) return ParseNumericExpression();
        if (NextToken.Definition is TokenType.String) return ParseStringExpression();
        if (NextToken.Definition is TokenType.Boolean) {
            var boolean = NextToken;
            Consume(TokenType.Boolean);
            return new LiteralExpression(boolean);
        }
        if (NextToken.Definition is TokenType.LParen)
        {
            Consume(TokenType.LParen);
            var left = ParseExpression();
            Consume(TokenType.RParen);
            return left;
        }
        else {
            var left = ParseIdExpression();
            LookAhead();
            if (NextToken.Definition is TokenType.Increment || NextToken.Definition is TokenType.Decrement) {
                Token op = NextToken;
                Consume(NextToken.Definition);
                left = new UnaryExpression(left, op, true);
            }
            else if (NextToken.Definition is TokenType.Plus || NextToken.Definition is TokenType.Minus
            || NextToken.Definition is TokenType.Multip || NextToken.Definition is TokenType.Division)
            {
                Token op = NextToken;
                Consume(NextToken.Definition);
                left = new BinaryExpression(left, op, ParseNumericExpression());
            }
            else if (NextToken.Definition is TokenType.And || NextToken.Definition is TokenType.Or || NextToken.Definition is TokenType.Equal
            || NextToken.Definition is TokenType.Less || NextToken.Definition is TokenType.LessEq
            || NextToken.Definition is TokenType.More || NextToken.Definition is TokenType.MoreEq)
            {
                Token op = NextToken;
                Consume(NextToken.Definition);
                left = new BinaryExpression(left, op, ParseBoolExpression());
            }
            else if(NextToken.Definition is TokenType.Concatenation || NextToken.Definition is TokenType.SpaceConcatenation)
            {
                Token op = NextToken;
                Consume(NextToken.Definition);
                left = new BinaryExpression(left, op, ParseStringExpression());
            }
            return left;
        }

    }
    Expression ParseIdExpression()
    {
        var left = ParseIdLiteral();
        LookAhead();
        if (NextToken.Definition is TokenType.Point || NextToken.Definition is TokenType.Assign
        || NextToken.Definition is TokenType.MoreAssign || NextToken.Definition is TokenType.MinusAssign)
        {
            Token op = NextToken;
            Consume(NextToken.Definition);
            LookAhead();
            var right = ParseExpression();
            left = new BinaryExpression(left, op, right);
        }
        else if (NextToken.Definition is TokenType.LParen)
        {
            Token op = NextToken;
            Consume(TokenType.LParen);
            LookAhead();
            if (NextToken.Definition is TokenType.RParen) {
                Consume(TokenType.RParen); 
            }
            else {
                var right = ParseIdExpression();
                Consume(TokenType.RParen);
                left = new BinaryExpression(left, op, right);
            }
        }
        return left;
    }
    Expression ParseIdLiteral()
    {
        if (NextToken.Definition is TokenType.Increment || NextToken.Definition is TokenType.Decrement)
        {
            Token op = NextToken;
            Consume(NextToken.Definition);
            Expression id = ParseIdExpression();
            return new UnaryExpression(id, op, false);
        }
        var value = NextToken;
        Consume(NextToken.Definition);
        return new LiteralExpression(value);
    }
    Expression ParseBoolExpression()
    {
        var left = ParseExpression();
        var expected = new List<TokenType>{TokenType.And, TokenType.Or, TokenType.Equal, TokenType.Less, TokenType.LessEq
        , TokenType.More, TokenType.MoreEq};
        LookAhead();
        if (expected.Contains(NextToken.Definition))
        {
            Token op = NextToken;
            Consume(NextToken.Definition);
            var right = ParseBoolExpression();
            left = new BinaryExpression(left, op, right);
        }
        return left;
    }
    Expression ParseNumericExpression()
    {
        return ParseSumExp();
    }
    Expression ParseSumExp()
    {
        Expression left = ParseTerm();
        LookAhead();
        if (NextToken.Definition is TokenType.Plus || NextToken.Definition is TokenType.Minus)
        {
            Token op = NextToken;
            Consume(NextToken.Definition);
            Expression right = ParseSumExp();
            left = new BinaryExpression(left, op, right);
        }
        return left;
    }
    Expression ParseTerm()
    {
        Expression left = ParseFactor();
        LookAhead();
        if (NextToken.Definition is TokenType.Multip || NextToken.Definition is TokenType.Division)
        {
            Token op = NextToken;
            Consume(NextToken.Definition);
            Expression right = ParseTerm();
            left = new BinaryExpression(left, op, right);
        }
        return left;
    }
    Expression ParseFactor()
    {
        //Can be an id
        //Can be negative number
        //Can be an open parenthesis
        //Can be a number
        LookAhead(new List<TokenType>{TokenType.Num, TokenType.Minus, TokenType.LParen, TokenType.Id});
        if (NextToken.Definition is TokenType.Num)
        {
            var literal = NextToken;
            Consume(NextToken.Definition);
            return new LiteralExpression(literal);
        }
        if (NextToken.Definition is TokenType.Id)
        {
            return ParseIdExpression();
        }
        if (NextToken.Definition is TokenType.LParen)
        {
            Consume(TokenType.LParen);
            var exp = ParseSumExp();
            Consume(TokenType.RParen);
            return exp;
        }
        Token op = NextToken;
        Consume(TokenType.Minus);
        var right = ParseSumExp();
        return new BinaryExpression(new LiteralExpression(new Token("0", TokenType.Num, 0,0)), op, right);
    }
    Expression ParseStringExpression()
    {
        Expression left = ParseWord();
        LookAhead();
        if (NextToken.Definition is TokenType.Concatenation || NextToken.Definition is TokenType.SpaceConcatenation)
        {
            Token op = NextToken;
            Consume(NextToken.Definition);
            Expression right = ParseStringExpression();
            left = new BinaryExpression(left, op, right);
        }
        return left;
    }
    Expression ParseWord()
    {
        //Can be an id
        //Can be an open parenthesis
        //Can be an string
        LookAhead(new List<TokenType>{TokenType.String, TokenType.LCurly, TokenType.Id});
        if (NextToken.Definition is TokenType.String)
        {
            var literal = NextToken;
            Consume(NextToken.Definition);
            return new LiteralExpression(literal);
        }
        if (NextToken.Definition is TokenType.Id)
        {
            return ParseIdExpression();
        }
        Consume(TokenType.LCurly);
        var exp = ParseStringExpression();
        Consume(TokenType.RCurly);
        return exp;
    }
    #endregion
}
