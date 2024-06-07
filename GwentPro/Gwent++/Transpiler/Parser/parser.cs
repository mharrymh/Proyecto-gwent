
namespace Transpiler;

public class Parser
{
    List<Token> Tokens {get; }
    int Pos {get; set;}
    Token NextToken {get; set;}

    void LookAhead(TokenType tokenType)
    {
        if (Pos+1 >= Tokens.Count) 
        {
            throw new Exception("Lanzar error");
        }
        if (tokenType == Tokens[Pos+1].Definition)
        {
            NextToken = Tokens[Pos+1];
        }
        else 
        {
            throw new Exception("Lanzar error");
        }
    }
    void LookAhead(List<TokenType>? expected = null)
    {
        if (Pos+1 >= Tokens.Count) 
        {
            throw new Exception("Lanzar error");
        }
        if(expected == null) 
        {NextToken = Tokens[Pos+1]; return;}

        if (expected.Contains(Tokens[Pos+1].Definition))
        {
            NextToken = Tokens[Pos+1];
        }
        else 
        {
            throw new Exception("Lanzar error");
        }
    }

    void Consume(TokenType tokenType)
    {
        if (NextToken.Definition == tokenType)
        {
            Pos++;
        }
        else 
        {
            throw new Exception("Lanzar error");
        }
    }

    public Parser(List<Token> tokens)
    {
        this.Tokens = tokens;
        this.Pos = 0;
        this.NextToken = Tokens[Pos];
    }

    public Program Parse()
    {
        return ParseDecBlock();
    }

    DecBlock ParseDecBlock()
    {
        return new DecBlock(ParseEffDecBlock(), ParseCardDecBlock());
    }

    #region EffectNodes
    EffDecBlock ParseEffDecBlock()
    {
        var effect = ParseEffect();
        LookAhead();
        if (NextToken.Definition is TokenType.Effect)
        {
            var right = ParseEffDecBlock();
            return new EffDecBlock(effect, right);
        }
        return new EffDecBlock(effect, null);
    }

    Effect ParseEffect()
    {
        Consume(TokenType.Effect);
        Consume(TokenType.LCurly);

        NameField? name = null;
        ParamField? param = null;
        ActionField? action = null;

        List<TokenType> expected = new List<TokenType>{TokenType.Name, TokenType.Params, TokenType.Action};
        List<TokenType> colons = new List<TokenType>{TokenType.Comma, TokenType.Semicolon};
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
        Consume(TokenType.Semicolon);
        if (name != null && action != null)
        {
            return new Effect(name, param, action);
        }
        throw new Exception("Implementar excepcion"); //name y action no pueden ser nulos
    }

    public NameField ParseName()
    {
        Consume(TokenType.Name);
        Consume(TokenType.Colon);
        LookAhead(TokenType.String);
        return new NameField(NextToken.Value);
    }
    public ParamField ParseParam()
    {
        Consume(TokenType.Params);
        Consume(TokenType.Colon);
        Consume(TokenType.LCurly);

        Dictionary<string, string> id_type = new Dictionary<string, string>();
        string id;
        var expected = new List<TokenType>{TokenType.RCurly, TokenType.Id};
        LookAhead(expected);
        while(NextToken.Definition != TokenType.RCurly)
        {
            id = NextToken.Value;
            Consume(TokenType.Colon);
            LookAhead(TokenType.String);
            //Add pair to the dictionary
            id_type.Add(id, NextToken.Value);
            LookAhead(new List<TokenType>{TokenType.RCurly, TokenType.Comma});
            if (NextToken.Definition == TokenType.Comma) Consume(TokenType.Comma);
            LookAhead(expected);
        }
        Consume(TokenType.RCurly);
        return new ParamField(id_type);
    }

    ActionField ParseAction()
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
        return new ActionField(instruction);
    }
    InstructionBlock ParseInstruction()
    {
        throw new NotImplementedException();
    }
    #endregion

    #region CardNodes
    CardDecBlock ParseCardDecBlock()
    {
        var card = ParseCard();
        LookAhead();
        if(NextToken.Definition is TokenType.Card)
        {
            var right = ParseCardDecBlock();
            return new CardDecBlock(card, right);
        }
        return new CardDecBlock(card, null);
    }
    Card ParseCard()
    {
        Consume(TokenType.Card);
        Consume(TokenType.LCurly);

        NameField? name = null;
        TypeField? type = null;
        FactionField? faction = null;
        PowerField? power = null;
        RangeField? range = null;
        ActivationField? activation = null;

        List<TokenType> expected = new List<TokenType>{TokenType.Name, TokenType.Type, TokenType.Faction, TokenType.Power, TokenType.Range, TokenType.OnActivation};
        List<TokenType> colons = new List<TokenType>{TokenType.Comma, TokenType.Semicolon};
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
            else if (NextToken.Definition is TokenType.Type)
            {
                type = ParseType();
                LookAhead(colons);
                if (NextToken.Definition is TokenType.Comma) Consume(TokenType.Comma);
                else break;
                expected.Remove(TokenType.Type);
                LookAhead(expected);
            }
            else if (NextToken.Definition is TokenType.Faction)
            {
                faction = ParseFaction();
                LookAhead(colons);
                if (NextToken.Definition is TokenType.Comma) Consume(TokenType.Comma);
                else break;
                expected.Remove(TokenType.Faction);
                LookAhead(expected);
            }
            else if (NextToken.Definition is TokenType.Power)
            {
                power = ParsePower();
                LookAhead(colons);
                if (NextToken.Definition is TokenType.Comma) Consume(TokenType.Comma);
                else break;
                expected.Remove(TokenType.Power);
                LookAhead(expected);
            }
            else if (NextToken.Definition is TokenType.Range)
            {
                range = ParseRange();
                LookAhead(colons);
                if (NextToken.Definition is TokenType.Comma) Consume(TokenType.Comma);
                else break;
                expected.Remove(TokenType.Range);
                LookAhead(expected);
            }
            else
            {
                activation = ParseActivation();
                LookAhead(colons);
                if (NextToken.Definition is TokenType.Comma) Consume(TokenType.Comma);
                else break;
                expected.Remove(TokenType.OnActivation);
                LookAhead(expected);
            }
        }
        Consume(TokenType.Semicolon);
        if (name != null && faction != null && type != null && range != null && activation != null)
        {
            return new Card(name, type, faction, power, range, activation);
        }
        throw new Exception("Implementar excepcion"); //name y action no pueden ser nulos
    }

    ActivationField ParseActivation()
    {
        Consume(TokenType.OnActivation);
        Consume(TokenType.Colon);
        Consume(TokenType.LBracket);
        var effBlock = ParseEffBlock();
        Consume(TokenType.RBracket);
        return new ActivationField(effBlock);
    }

    EffectAllocationBlock ParseEffBlock()
    {
        //Estoy esperando un punto y coma o un LCurly
        Consume(TokenType.LCurly);
        var effAllocation = ParseEffAllocation();
        Consume(TokenType.RCurly);
        LookAhead(new List<TokenType>{TokenType.Comma, TokenType.Semicolon});
        if (NextToken.Definition is TokenType.Comma)
        {
            Consume(TokenType.Comma);
            var right = ParseEffBlock();
            return new EffectAllocationBlock(effAllocation, right);
        }
        Consume(TokenType.Semicolon);
        return new EffectAllocationBlock(effAllocation, null);
    }

    EffectAllocation ParseEffAllocation()
    {
        Allocation? allocation = null;
        Selector? selector = null;
        PostActionBlock? postAction = null;

        List<TokenType> expected = new List<TokenType>{TokenType.C_Effect, TokenType.Selector, TokenType.PostAction};
        List<TokenType> colons = new List<TokenType>{TokenType.Comma, TokenType.Semicolon};
        LookAhead(expected);
        while(expected.Contains(NextToken.Definition))
        {
            //Check nextToken
            //Break if it is a semicolon
            if (NextToken.Definition is TokenType.C_Effect)
            {
                allocation = ParseAllocation();
                LookAhead(colons);
                if (NextToken.Definition is TokenType.Comma) Consume(TokenType.Comma);
                else break;
                expected.Remove(TokenType.C_Effect);
                LookAhead(expected);
            }
            else if (NextToken.Definition is TokenType.Type)
            {
                selector = ParseSelector();
                LookAhead(colons);
                if (NextToken.Definition is TokenType.Comma) Consume(TokenType.Comma);
                else break;
                expected.Remove(TokenType.Selector);
                LookAhead(expected);
            }
            else
            {
                postAction = ParsePostAction();
                LookAhead(colons);
                if (NextToken.Definition is TokenType.Comma) Consume(TokenType.Comma);
                else break;
                expected.Remove(TokenType.PostAction);
                LookAhead(expected);
            }
        }
        Consume(TokenType.Semicolon);
        if (allocation != null && selector != null)
        {
            return new EffectAllocation(allocation, selector, postAction);
        }
        throw new Exception("Implementar error");
    }
    TypeField ParseType()
    {
        Consume(TokenType.Type);
        Consume(TokenType.Colon);
        LookAhead(TokenType.String);
        return new TypeField(NextToken.Value);
    }

    FactionField ParseFaction()
    {
        Consume(TokenType.Faction);
        Consume(TokenType.Colon);
        LookAhead(TokenType.String);
        return new FactionField(NextToken.Value);
    }

    PowerField ParsePower()
    {
        Consume(TokenType.Power);
        Consume(TokenType.Colon);

        return new PowerField(ParseNumericExpression());
    }
    RangeField ParseRange()
    {
        Consume(TokenType.Range);
        Consume(TokenType.Colon);
        Consume(TokenType.LBracket);
        List<string> ranges = new List<string>();
        var expected = new List<TokenType>{TokenType.RBracket, TokenType.String};
        LookAhead(expected);
        while(NextToken.Definition != TokenType.RBracket)
        {
            ranges.Add(NextToken.Value);
            LookAhead(new List<TokenType>{TokenType.Comma, TokenType.RBracket});
            if (NextToken.Definition is TokenType.Comma) Consume(TokenType.Comma);
            LookAhead(expected);
        }
        Consume(TokenType.RBracket);
        return new RangeField(ranges);
    }
    PostActionBlock ParsePostAction()
    {
        Consume(TokenType.PostAction);
        Consume(TokenType.Colon);
        Consume(TokenType.LCurly);
        var effAllocation = ParseEffBlock();
        Consume(TokenType.RCurly);

        return new PostActionBlock(effAllocation);
    }

    Selector ParseSelector()
    {
        Consume(TokenType.Selector);
        Consume(TokenType.Colon);
        Consume(TokenType.LCurly);


        Source? source = null;
        SingleField? single = null;
        Predicate? predicate = null;

        List<TokenType> expected = new List<TokenType>{TokenType.Source, TokenType.Single, TokenType.Predicate};
        List<TokenType> colons = new List<TokenType>{TokenType.Comma, TokenType.RCurly};
        LookAhead(expected);
        while(expected.Contains(NextToken.Definition))
        {
            //Check nextToken
            //Break if it is a semicolon
            if (NextToken.Definition is TokenType.Source)
            {
                source = ParseSource();
                LookAhead(colons);
                if (NextToken.Definition is TokenType.Comma) Consume(TokenType.Comma);
                else break;
                expected.Remove(TokenType.Source);
                LookAhead(expected);
            }
            else if (NextToken.Definition is TokenType.Single)
            {
                single = ParseSingle();
                LookAhead(colons);
                if (NextToken.Definition is TokenType.Comma) Consume(TokenType.Comma);
                else break;
                expected.Remove(TokenType.Single);
                LookAhead(expected);
            }
            else
            {
                predicate = ParsePredicate();
                LookAhead(colons);
                if (NextToken.Definition is TokenType.Comma) Consume(TokenType.Comma);
                else break;
                expected.Remove(TokenType.Predicate);
                LookAhead(expected);
            }
        }
        Consume(TokenType.RCurly);
        if (source != null && predicate != null)
        {
            return new Selector(source, single, predicate);
        }
        throw new Exception("Implementar error");

    }

    Predicate ParsePredicate()
    {
        Consume(TokenType.Predicate);
        Consume(TokenType.Colon);
        var predicate = ParsePredicativeExpression();
        return new Predicate(predicate);
    }


    SingleField ParseSingle()
    {
        Consume(TokenType.Single);
        Consume(TokenType.Colon);
        var boolean = ParseBoolExpression();
        return new SingleField(boolean);
    }


    Source ParseSource()
    {
        Consume(TokenType.Source);
        Consume(TokenType.Colon);
        LookAhead(TokenType.String);
        return new Source(NextToken.Value);
    }

    Allocation ParseAllocation()
    {
        LookAhead(new List<TokenType>{TokenType.String, TokenType.LCurly});
        if (NextToken.Definition is TokenType.String)
        {
            return new Allocation(NextToken.Value, null, null);
        }
        NameField? name = null;
        VarAllocation? varAllocation = null; 
        Consume(TokenType.LCurly);

        var expected = new List<TokenType>{TokenType.Id, TokenType.Name};
        List<TokenType> colons = new List<TokenType>{TokenType.Comma, TokenType.Semicolon};
        LookAhead(expected);
        while(expected.Contains(NextToken.Definition))
        {
            if (NextToken.Definition is TokenType.Name)
            {
                name = ParseName();
                LookAhead(colons);
                if (NextToken.Definition is TokenType.Comma) Consume(TokenType.Comma);
                else break;
                expected.Remove(TokenType.Name);
                LookAhead(expected);
            }
            else
            {
                varAllocation = ParseVarAllocation();
                LookAhead(colons);
                if (NextToken.Definition is TokenType.Comma) Consume(TokenType.Comma);
                else break;
                expected.Remove(TokenType.Id);
                LookAhead(expected);
            }
        }
        Consume(TokenType.Semicolon);
        if (name != null)
        {
            return new Allocation(null, name, varAllocation);
        }

        throw new NotImplementedException();
    }

    VarAllocation ParseVarAllocation()
    {
        throw new NotImplementedException();
    }
    BooleanExpression ParseBoolExpression()
    {
        throw new NotImplementedException();
    }
    PredicativeExpression ParsePredicativeExpression()
    {
        throw new NotImplementedException();
    }
    NumericExpression ParseNumericExpression()
    {
        //Saca una lista de la espresion hasta que venga algo que no pertenezca a la expresion
        throw new NotImplementedException();
    }

    #endregion
}
