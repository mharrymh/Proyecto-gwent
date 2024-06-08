
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
            throw new Exception("Lanza");
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
        if (Tokens[Pos+1].Definition == tokenType)
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
        this.Pos = -1;
        this.NextToken = Tokens[0];
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
        throw new Exception("Implementar excepcion"); //name y action no pueden ser nulos
    }

    public NameField ParseName()
    {
        Consume(TokenType.Name);
        Consume(TokenType.Colon);
        LookAhead(TokenType.String);
        return new NameField(ParseStringExpression());
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
        InstructionBlock? instruction = null;
        ForLoop? forLoop= null;
        WhileLoop? whileLoop= null;
        Assignment? assignment= null;

        var expected = new List<TokenType>{TokenType.For, TokenType.While, TokenType.Id, TokenType.RCurly};
        LookAhead(expected);
        while(NextToken.Definition != TokenType.RCurly)
        {
            if (NextToken.Definition == TokenType.For)
            {
                if(forLoop != null) 
                {
                    instruction = ParseInstruction();
                }
                else {
                    forLoop = ParseForLoop();
                    LookAhead(expected);
                }
            }
            else if (NextToken.Definition == TokenType.While)
            {
                if(whileLoop != null) 
                {
                    instruction = ParseInstruction();
                }
                else {
                    whileLoop = ParseWhileLoop();
                    LookAhead(expected);
                }
            }
            else
            {
                if (assignment != null)
                {
                    instruction = ParseInstruction();
                }
                else {
                    assignment = ParseAssignment();
                    LookAhead(expected);
                }
            }
        }
        return new InstructionBlock(forLoop, whileLoop, assignment, instruction);
    }
    WhileLoop ParseWhileLoop()
    {
        Consume(TokenType.While);
        Consume(TokenType.LParen);
        var exp = ParseBoolExpression();
        Consume(TokenType.RParen);
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
    Assignment ParseAssignment()
    {
        throw new NotImplementedException();
    }
    #endregion

    #region CardNodes
    CardDecBlock ParseCardDecBlock()
    {
        var card = ParseCard();
        if (Pos + 1 >= Tokens.Count)
        {
            return new CardDecBlock(card, null);
        }
        else
        {
            LookAhead(TokenType.Card);
            var right = ParseCardDecBlock();
            return new CardDecBlock(card, right);
        }
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
        Consume(TokenType.RCurly);
        if (name != null && faction != null && type != null && range != null)
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
        return new TypeField(ParseStringExpression());
    }

    FactionField ParseFaction()
    {
        Consume(TokenType.Faction);
        Consume(TokenType.Colon);
        LookAhead(TokenType.String);
        return new FactionField(ParseStringExpression());
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
        List<Expression> ranges = new List<Expression>();
        var expected = new List<TokenType>{TokenType.RBracket, TokenType.String};
        LookAhead(expected);
        while(NextToken.Definition != TokenType.RBracket)
        {
            ranges.Add(ParseStringExpression());
            //FIXME: NO ES NECESARIO?
            // Consume(TokenType.String);
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
        Consume(TokenType.LCurly);
        LookAhead(TokenType.Id);
        string id = NextToken.Value;
        Consume(NextToken.Definition);
        Consume(TokenType.RCurly);
        Consume(TokenType.Implication);
        return new Predicate(id, ParseBoolExpression());
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
        return new Source(ParseStringExpression());
    }
    Allocation ParseAllocation()
    {
        //FIXME:
        //Allocation lo que tiene son RCurly y no semicolon
        LookAhead(new List<TokenType>{TokenType.String, TokenType.LCurly});
        if (NextToken.Definition is TokenType.String)
        {
            return new Allocation(ParseExpression(), null, null);
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
        Dictionary<string, string> id_allocation = new Dictionary<string, string>();
        string id;
        var expected = new List<TokenType>{TokenType.RCurly, TokenType.Id};
        LookAhead(expected);
        while(NextToken.Definition != TokenType.RCurly)
        {
            id = NextToken.Value;
            Consume(TokenType.Colon);
            LookAhead(TokenType.String);
            //Add pair to the dictionary
            id_allocation.Add(id, NextToken.Value);
            LookAhead(new List<TokenType>{TokenType.RCurly, TokenType.Comma});
            if (NextToken.Definition == TokenType.Comma) Consume(TokenType.Comma);
            else break;
            LookAhead(expected);
        }
        // Consume(TokenType.RCurly);
        //Lo consume el metodo padre
        return new VarAllocation(id_allocation);
    }
    
    Expression ParseExpression()
    {
        LookAhead();
        if (NextToken.Definition is TokenType.Id)
        {
            Expression left = new LiteralExpression(NextToken.Value);
            Consume(TokenType.Id);
            LookAhead();
            if (NextToken.Definition is TokenType.Plus || NextToken.Definition is TokenType.Minus
            || NextToken.Definition is TokenType.Multip || NextToken.Definition is TokenType.Division)
            {
                string op = NextToken.Value;
                Consume(NextToken.Definition);
                left = new BinaryExpression(left, op, ParseNumericExpression());
            }
            else if (NextToken.Definition is TokenType.And || NextToken.Definition is TokenType.Or || NextToken.Definition is TokenType.Equal
            || NextToken.Definition is TokenType.Less || NextToken.Definition is TokenType.LessEq
            || NextToken.Definition is TokenType.More || NextToken.Definition is TokenType.MoreEq)
            {
                string op = NextToken.Value;
                Consume(NextToken.Definition);
                left = new BinaryExpression(left, op, ParseBoolExpression());
            }
            else if(NextToken.Definition is TokenType.Concatenation || NextToken.Definition is TokenType.SpaceConcatenation)
            {
                string op = NextToken.Value;
                Consume(NextToken.Definition);
                left = new BinaryExpression(left, op, ParseStringExpression());
            }
            return left;
        }
        if(NextToken.Definition is TokenType.Number) return ParseNumericExpression();
        if (NextToken.Definition is TokenType.String) return ParseStringExpression();
        if (NextToken.Definition is TokenType.Boolean) return new LiteralExpression(NextToken.Value);
        if (NextToken.Definition is TokenType.LParen)
        {
            Consume(TokenType.LParen);
            var left = ParseExpression();
            Consume(TokenType.RParen);
            return  left;
        }

        //Launch error
        Consume(TokenType.Id);
        throw new Exception("Not implemented");
    }
    Expression ParseBoolExpression()
    {
        var left = ParseExpression();
        var expected = new List<TokenType>{TokenType.And, TokenType.Or, TokenType.Equal, TokenType.Less, TokenType.LessEq
        , TokenType.More, TokenType.MoreEq};
        LookAhead();
        if (expected.Contains(NextToken.Definition))
        {
            string op = NextToken.Value;
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
            string op = NextToken.Value;
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
            string op = NextToken.Value;
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
        if (NextToken.Definition is TokenType.Id || NextToken.Definition is TokenType.Num)
        {
            var literal = NextToken.Value;
            Consume(NextToken.Definition);
            return new LiteralExpression(literal);
        }
        if (NextToken.Definition is TokenType.LParen)
        {
            Consume(TokenType.LParen);
            var exp = ParseSumExp();
            Consume(TokenType.RParen);
            return exp;
        }
        string op = NextToken.Value;
        Consume(TokenType.Minus);
        var right = ParseSumExp();
        return new BinaryExpression(new LiteralExpression("0"), op, right);
    }
    Expression ParseStringExpression()
    {
        Expression left = ParseWord();
        LookAhead();
        if (NextToken.Definition is TokenType.Concatenation || NextToken.Definition is TokenType.SpaceConcatenation)
        {
            string op = NextToken.Value;
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
        if (NextToken.Definition is TokenType.Id || NextToken.Definition is TokenType.String)
        {
            var literal = NextToken.Value;
            Consume(NextToken.Definition);
            return new LiteralExpression(literal);
        }
        Consume(TokenType.LCurly);
        var exp = ParseStringExpression();
        Consume(TokenType.RCurly);
        return exp;
    }
    Expression ParsePredicativeExpression()
    {
        throw new NotImplementedException();
    }
    #endregion
}
