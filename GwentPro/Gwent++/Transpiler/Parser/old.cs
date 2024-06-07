

class Parser
{
    List<Token> Tokens {get; }
    int Pos {get; set;}
    Token NextToken {get; set;}
    bool ValidNextToken {get; set;}

    void LookAhead(List<TokenType> expected)
    {
        if (expected.Contains(Tokens[Pos+1].Definition))
        {
            NextToken = Tokens[Pos+1];
        }
        //Lanza error
    }

    void Consume(TokenType tokenType)
    {
        if (NextToken.Definition == tokenType)
        {
            Pos++;
            //Cambiar el NextToken??
        }
    }

    void ConsumeAll(List<TokenType> tokenTypes)
    {

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
        List<TokenType> tokensExpected = new List<TokenType>{TokenType.Effect, TokenType.Card};
        LookAhead(tokensExpected);
        var left = ParseDec();
        if (Position < Tokens.Count)
        {
            LookAhead(tokensExpected);
            var right = ParseDeclarations();
            left = new Declarations(left, right);
        }
        return left;
    }

    Expression ParseDec()
    {
        Expression eff = null;
        Expression card = null;
        if (NextToken.Definition is TokenType.Effect)
        {
            eff = ParseEf();
        }
        if (NextToken.Definition is TokenType.Card)
        {
            card = ParseCard();
        }
        if (eff != null && card != null)
        return new Dec(eff, card);
        else if (eff != null)
        return eff;

        throw new Exception("Implementar error");
    }

    Expression ParseEf()
    {
        Consume(TokenType.Effect);

        Consume(TokenType.LCurly);
        var effProps = ParseEffProps();
        Consume(TokenType.RCurly);
        return effProps;
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
        NameField name = null;
        Expression param = null;
        Expression action = null;

        var validTokens = new List<TokenType>{TokenType.Name,TokenType.Params, TokenType.Action};
        LookAhead(validTokens);
        while(validTokens.Contains(NextToken.Definition))
        {
            if (NextToken.Definition is TokenType.Name)
            {
                name = ParseName();
                validTokens.Remove(TokenType.Name);
                LookAhead(validTokens);
            }
            else if (NextToken.Definition is TokenType.Params)
            {
                param = ParseParams();
                validTokens.Remove(TokenType.Params);
                LookAhead(validTokens);
            }
            else
            {
                action = ParseAction();
                validTokens.Remove(TokenType.Action);
                LookAhead(validTokens);
            }
        }

        if (name != null && action != null)
        {
            return new EfProp((NameField)name, param, action);
        }
        throw new Exception("Implementar error");
    }

    NameField ParseName()
    {
        Consume(TokenType.Name);
        Consume(TokenType.Colon);
        LookAhead(new List<TokenType>{TokenType.String});
        return new NameField(NextToken.Value);
    }

    Expression ParseParams()
    {
        Consume(TokenType.Params);
        Consume(TokenType.Colon);
        Consume(TokenType.LCurly);
        var def = ParseDefinitions();
        Consume(TokenType.RCurly);
        return def;
    }
    Expression ParseDefinitions()
    {
        var left = ParseDef();
        LookAhead(new List<TokenType>{TokenType.Comma, TokenType.Semicolon});
        if (NextToken.Definition is TokenType.Comma)
        {
            Consume(TokenType.Comma);
            var right = ParseDefinitions();
            left = new Definitions(left, right);
        }
        Consume(TokenType.Semicolon);
        return left;
    }

    Expression ParseDef()
    {
        LookAhead(new List<TokenType>{TokenType.Id});
        string id = NextToken.Value;
        Consume(TokenType.Id);
        Consume(TokenType.Colon);
        //Seria una lista de todos los valuetypes
        
        LookAhead(new List<TokenType>{TokenType.Number, TokenType.Bool, TokenType.Text});
        string valueType = NextToken.Value;

        return new Definition(id, valueType);
    }

    Expression ParseAction()
    {
        List<TokenType> toConsume = new List<TokenType>{TokenType.Action, TokenType.Colon
        , TokenType.LParen, TokenType.Targets, TokenType.Comma, TokenType.Context
        , TokenType.Context, TokenType.RParen, TokenType.Implication, TokenType.LCurly};

        ConsumeAll(toConsume);
        var instruction = ParseInstruction();
        Consume(TokenType.RCurly);
        return instruction;
    }

    Expression ParseCard()
    {
        Consume(TokenType.Card);
        Consume(TokenType.LCurly);
        var cardProps = ParseCardProps();
        Consume(TokenType.RCurly);
        return cardProps;
    }

    Expression ParseCardProps()
    {
        var left = ParseCardProp();
        LookAhead(new List<TokenType>{TokenType.Comma, TokenType.Semicolon});
        if (NextToken.Definition is TokenType.Comma)
        {
            Consume(TokenType.Comma);
            var right = ParseCardProps();
            left = new CardProperties(left, right);
        }
        Consume(TokenType.Semicolon);
        return left;
    }
    Expression ParseCardProp()
    {
        NameField name = null;
        TypeField type = null;
        FactionField faction = null;
        PowerField power = null;
        Expression range = null;
        Expression activation = null;


        var validTokens = new List<TokenType>{TokenType.Name, TokenType.Type, TokenType.Faction, TokenType.Power
        , TokenType.Range, TokenType.OnActivation};
        LookAhead(validTokens);

        while(validTokens.Contains(NextToken.Definition))
        {
            if (NextToken.Definition is TokenType.Name)
            {
                name = ParseName();
                validTokens.Remove(TokenType.Name);
                LookAhead(validTokens);
            }
            else if (NextToken.Definition is TokenType.Type)
            {
                type = ParseType();
                validTokens.Remove(TokenType.Type);
                LookAhead(validTokens);
            }
            else if(NextToken.Definition is TokenType.Faction)
            {
                faction = ParseFaction();
                validTokens.Remove(TokenType.Faction);
                LookAhead(validTokens);
            }
            else if(NextToken.Definition is TokenType.Power)
            {
                power = ParsePower();
                validTokens.Remove(TokenType.Power);
                LookAhead(validTokens);
            }
            else if(NextToken.Definition is TokenType.Range)
            {
                range = ParseRange();
                validTokens.Remove(TokenType.Range);
                LookAhead(validTokens);
            }
            else
            {
                activation = ParseActivation();
                validTokens.Remove(TokenType.OnActivation);
                LookAhead(validTokens);
            }
        }

        if (name != null && type != null && faction != null)
        {
            return new CardProp(name, type, faction, power, range, activation);
        }
        throw new Exception("Implementar error");
    }
    Expression ParseActivation()
    {
        var to_consume = new List<TokenType>{TokenType.OnActivation, TokenType.Colon, TokenType.LBracket};
        ConsumeAll(to_consume);
        var eff = ParseCardEffects();
        Consume(TokenType.RBracket);
        return eff;
    }
    Expression ParseCardEffects()
    {
        var left = ParseCardEff();
        LookAhead(new List<TokenType>{TokenType.Comma, TokenType.Semicolon});
        if (NextToken.Definition is TokenType.Comma)
        {
            Consume(TokenType.Comma);
            var right = ParseCardEffects();
            left = new CardProperties(left, right);
        }
        Consume(TokenType.Semicolon);
        return left;
    }

    Expression ParseCardEff()
    {
        Consume(TokenType.LCurly);
        var props = ParseCardEffProps();
        Consume(TokenType.RCurly);
        return new CardEffect(props);
    }

    Expression ParseCardEffProps()
    {
        var left = ParseCardEffProp();
        LookAhead(new List<TokenType>{TokenType.Comma, TokenType.Semicolon});
        if (NextToken.Definition is TokenType.Comma)
        {
            Consume(TokenType.Comma);
            var right = ParseCardEffProps();
            left = new CardProperties(left, right);
        }
        Consume(TokenType.Semicolon);
        return left;
    }
    Expression ParseCardEffProp()
    {
        Expression eff = null;
        Expression selector = null;
        Expression post = null;

        var validTokens = new List<TokenType>{TokenType.C_Effect, TokenType.Selector, TokenType.PostAction};
        LookAhead(validTokens);

        while(validTokens.Contains(NextToken.Definition))
        {
            if (NextToken.Definition is TokenType.C_Effect)
            {
                eff = ParseCardEffDec();
                validTokens.Remove(TokenType.C_Effect);
                LookAhead(validTokens);
            }
            else if (NextToken.Definition is TokenType.Selector)
            {
                selector = ParseSelector();
                validTokens.Remove(TokenType.Selector);
                LookAhead(validTokens);
            }
            else
            {
                post = ParsePostActions();
                validTokens.Remove(TokenType.OnActivation);
                LookAhead(validTokens);
            }
        }
        if (eff != null && selector != null)
        {
            return new CardEffectProp(eff, selector, post);
        }
        throw new Exception("Implementar error");
    }
    Expression ParseSelector()
    {
        Consume(TokenType.Selector);
        Consume(TokenType.Colon);
        Consume(TokenType.LCurly);
        var props = ParseSelectorProps();
        Consume(TokenType.RCurly);
        return props;   
    }
    Expression ParseSelectorProps()
    {
        var left = ParseSelectorProp();
        LookAhead(new List<TokenType>{TokenType.Comma, TokenType.Semicolon});
        if (NextToken.Definition is TokenType.Comma)
        {
            Consume(TokenType.Comma);
            var right = ParseSelectorProps();
            left = new SelectorProps(left, right);
        }
        Consume(TokenType.Semicolon);
        return left;
    }
    Expression ParseSelectorProp()
    {
        Source source = null;
        Expression single = null;
        Expression predicate = null;

        var validTokens = new List<TokenType>{TokenType.Source, TokenType.Single, TokenType.Predicate};
        LookAhead(validTokens);
        while (validTokens.Contains(NextToken.Definition));
        {
            if (NextToken.Definition is TokenType.Source)
            {
                source = ParseSource();
                validTokens.Remove(TokenType.Source);
                LookAhead(validTokens);
            }
            else if(NextToken.Definition is TokenType.Single)
            {
                single = ParseSingle();
                validTokens.Remove(TokenType.Single);
                LookAhead(validTokens);
            }
            else
            {
                predicate = ParsePredicate();
                validTokens.Remove(TokenType.Predicate);
                LookAhead(validTokens);
            }
        }
        if (source != null && predicate != null)
        {
            return new SelectorProp(source, single, predicate);
        }
        throw new Exception("Implementar error");
    }
    Source ParseSource()
    {
        Consume(TokenType.Source);
        Consume(TokenType.Colon);
        LookAhead(new List<TokenType>{TokenType.String});
        return new Source(NextToken.Value);
    }
    Expression ParseSingle()
    {
        Consume(TokenType.Single);
        Consume(TokenType.Colon);
        return ParseBoolExp();
    }



    Expression ParsePredicate()
    {
        Consume(TokenType.Predicate);
        Consume(TokenType.Colon);
        return ParsePredicativeExp();
    }

    

    Expression ParseCardEffDec()
    {
        Consume(TokenType.C_Effect);
        Consume(TokenType.Colon);
        return ParseEffBlock();
    }
    Expression ParseEffBlock()
    {
        LookAhead(new List<TokenType>{TokenType.String, TokenType.LCurly});
        if (NextToken.Definition is TokenType.String)
        {
            return new EffBlock(null, NextToken.Value);
        }

        Consume(TokenType.LCurly);
        var props = ParseEffBlockProps();
        Consume(TokenType.RCurly);
        return props;
    }
    Expression ParseEffBlockProps()
    {
        var left = ParseEffBlockProp();
        LookAhead(new List<TokenType>{TokenType.Comma, TokenType.Semicolon});
        if (NextToken.Definition is TokenType.Comma)
        {
            Consume(TokenType.Comma);
            var right = ParseEffBlockProps();
            left = new EffBlockProps(left, right);
        }
        Consume(TokenType.Semicolon);
        return left;
    }
    Expression ParseEffBlockProp()
    {
        var validTokens = new List<TokenType>{TokenType.Name, TokenType.Id};
        LookAhead(validTokens);
        var name = ParseName();
        var asignations = ParseAsignations();

        return new EffBlockProp(name, asignations);
    }

    Expression ParsePostActions()
    {
        Consume(TokenType.PostAction);
        Consume(TokenType.Colon);
        Consume(TokenType.LCurly);

        var left = ParsePostAction();
        LookAhead(new List<TokenType>{TokenType.Comma, TokenType.Semicolon});
        if (NextToken.Definition is TokenType.Comma)
        {
            Consume(TokenType.Comma);
            var right = ParsePostActions();
            left = new PostActionProps(left, right);
        }
        Consume(TokenType.Semicolon);
        return left;
    }
    Expression ParsePostAction()
    {
        return ParseCardEffProps();
    }

    TypeField ParseType()
    {
        Consume(TokenType.Type);
        Consume(TokenType.Colon);
        LookAhead(new List<TokenType>{TokenType.String});
        return new TypeField(NextToken.Value);
    }
    FactionField ParseFaction()
    {
        Consume(TokenType.Faction);
        Consume(TokenType.Colon);
        LookAhead(new List<TokenType>{TokenType.String});
        return new FactionField(NextToken.Value);
    }
    PowerField ParsePower()
    {
        Consume(TokenType.Power);
        Consume(TokenType.Colon);
        var num = ParseNumericExpression();
        return num;
    }
    Expression ParseRange()
    {
        Consume(TokenType.LBracket);
        string[] ranges = new string[3];
        LookAhead(new List<TokenType>{TokenType.String});
        ranges[0] = NextToken.Value;
        
        for (int i = 1; NextToken.Definition != TokenType.String && i < 3; i++)
        {
            ranges[i] = NextToken.Value;
            LookAhead(new List<TokenType>{TokenType.String, TokenType.RBracket});
        }
        return new RangeField(ranges);
    }

    Expression ParseInstruction()
    {
        throw new NotImplementedException();
    }

    Expression ParseBoolExp()
    {
        throw new NotImplementedException();
    }
    Expression ParsePredicativeExp()
    {
        throw new NotImplementedException();
    }
    Asignations ParseAsignations()
    {
        throw new NotImplementedException();
    }
    PowerField ParseNumericExpression()
    {
        throw new NotImplementedException();
    }
}
