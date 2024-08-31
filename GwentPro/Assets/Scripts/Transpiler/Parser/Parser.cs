#nullable enable
using System.Collections.Generic;
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
        LookAhead(new List<TokenType> { tokenType });
    }
    // Look ahead to the next token and check if it matches any of the expected token types
    void LookAhead(List<TokenType>? expected = null)
    {
        if (Pos+1 >= Tokens.Count) 
        {
            // If we've reached the end of the token list, throw an exception
            Token last = Tokens[^1];
            CompilationError UnexpectedEndOfInput = new UnexpectedEndOfInput(last);
            throw UnexpectedEndOfInput;
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
            CompilationError UnexpectedToken = new UnexpectedToken(token, expected[0].ToString());
            throw UnexpectedToken;
        }
    }
    void Consume(List<TokenType> tokenTypes)
    {
        foreach (TokenType type in tokenTypes)
        {
            Consume(type);
        }
    }

    // Consume the next token if it matches the expected token type
    void Consume(TokenType tokenType)
    {
        if (Tokens[Pos+1].Definition == tokenType) Pos++;
        else {
            // If the next token doesn't match the expected type, throw an exception
            Token token = Tokens[Pos+1];
            CompilationError UnexpectedToken = new UnexpectedToken(token, tokenType.ToString());
            throw UnexpectedToken;
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
    public DSL_Object Parse()
    {
        var obj = ParseDecBlock();
        return obj;
    }

    // Parse a declaration block
    DecBlock ParseDecBlock()
    {
        //Parse effects and parse cards
        return new DecBlock(ParseEffDecBlock(), ParseCardDecBlock());
    }

    #region EffectNodes
    // Parse a list of effect declarations
    List<Effect_Object> ParseEffDecBlock()
    {
        var effects = new List<Effect_Object>();
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
    Effect_Object ParseEffect()
    {
        LookAhead(TokenType.Effect);
        //Save the line for exceptions handling
        int line = NextToken.Line;
        Consume(new List<TokenType> { TokenType.Effect, TokenType.LCurly });

        Expression? name = null;
        Dictionary<Token, Token>? param = null;
        InstructionBlock? action = null;

        List<TokenType> expected = new List<TokenType>{TokenType.Name, TokenType.Params, TokenType.Action};
        List<TokenType> colons = new List<TokenType>{TokenType.Comma, TokenType.RCurly};
        LookAhead(expected);
        while(expected.Contains(NextToken.Definition))
        {
            TokenType tokenType = NextToken.Definition;
            //Check nextToken
            //Break if it is a semicolon
            if (tokenType is TokenType.Name)
            {
                name = ParseName();
                
            }
            else if (tokenType is TokenType.Params)
            {
                param = ParseParam();
            }
            else
            {
                action = ParseAction();
            }
            LookAhead(colons);
            if (NextToken.Definition is TokenType.Comma) Consume(TokenType.Comma);
            else break;
            expected.Remove(tokenType);
            LookAhead(expected);
        }
        Consume(TokenType.RCurly);
        return new Effect_Object(name, param, action, line); 
    }

    Expression ParseName()
    {
        Consume(new List<TokenType> { TokenType.Name, TokenType.Colon });
        LookAhead(TokenType.String);
        return ParseStringExpression();
    }
    public Dictionary<Token, Token> ParseParam()
    {
        Consume(new List<TokenType> { TokenType.Params, TokenType.Colon, TokenType.LCurly });

        //Create a dictionary to save the id related with its type declared
        Dictionary<Token, Token> id_type = new Dictionary<Token, Token>();
        //Create an id token to use it inside the while statement
        Token id;
        var expected = new List<TokenType>{TokenType.RCurly, TokenType.Id};
        LookAhead(expected);
        while(NextToken.Definition != TokenType.RCurly)
        {
            id = NextToken;
            Consume(new List<TokenType> { NextToken.Definition, TokenType.Colon });
            LookAhead(new List<TokenType> { TokenType.Number, TokenType.Bool, TokenType.Text });
            //Add pair to the dictionary
            id_type.Add(id, NextToken);
            Consume(NextToken.Definition);
            LookAhead(new List<TokenType> { TokenType.RCurly, TokenType.Comma });
            if (NextToken.Definition == TokenType.Comma) 
                Consume(TokenType.Comma);
            LookAhead(expected);
        }
        Consume(TokenType.RCurly);
        return id_type;
    }
    //Targets y context siempre van a aser tokens
    InstructionBlock ParseAction()
    {
        Consume(new List<TokenType> { TokenType.Action, TokenType.Colon, TokenType.LParen });

        LookAhead(TokenType.Id);
        //Saves the targets and the context id
        Token targets = NextToken;
        Consume(new List<TokenType> { TokenType.Id, TokenType.Comma });
        LookAhead(TokenType.Id);
        Token context = NextToken;
        Consume(new List<TokenType> { TokenType.Id, TokenType.RParen, TokenType.Implication });

        LookAhead();
        if (NextToken.Definition is not TokenType.LCurly)
        {
            return ParseOneInstruction(targets, context);
        }
        //else is a left curly 
        Consume(TokenType.LCurly);
        var instruction = ParseInstruction(targets, context);
        Consume(TokenType.RCurly);
        return instruction;
    }

    InstructionBlock ParseOneInstruction(Token targets, Token context)
    {
        List<Statement> statements = new List<Statement>();

        LookAhead();
        if (NextToken.Definition == TokenType.For) {
            statements.Add(ParseForLoop(targets, context));
        }
        else if (NextToken.Definition == TokenType.While) {
            statements.Add(ParseWhileLoop(targets, context));
        }
        else {
            statements.Add(ParseExpression());
            Consume(TokenType.Semicolon);
        }
        return new InstructionBlock(statements, targets, context);
    }
    InstructionBlock ParseInstruction(Token targets, Token context)
    {
        List<Statement> statements = new List<Statement>();

        LookAhead();
        while(NextToken.Definition != TokenType.RCurly)
        {
            if (NextToken.Definition == TokenType.For) {
                statements.Add(ParseForLoop(targets, context));
            }
            else if (NextToken.Definition is TokenType.Semicolon)
            {
                Consume(NextToken.Definition);
            }
            else if (NextToken.Definition == TokenType.While) {
                statements.Add(ParseWhileLoop(targets, context));
            }
            else {
                statements.Add(ParseExpression());
                Consume(TokenType.Semicolon);
            }
            LookAhead();
        }
        return new InstructionBlock(statements, targets, context);
    }
    WhileLoop ParseWhileLoop(Token targets, Token context)
    {
        LookAhead(TokenType.While);
        int line = NextToken.Line;
        Consume(new List<TokenType> { TokenType.While, TokenType.LParen });
        var exp = ParseBoolExpression();
        Consume(TokenType.RParen);
        LookAhead();

        //Check if the while instruction is inside brackets
        if (NextToken.Definition is TokenType.LCurly)
        {
            Consume(TokenType.LCurly);
            var instruction = ParseInstruction(targets, context);
            Consume(TokenType.RCurly);
            return new WhileLoop(exp, instruction, line);
        }
        //Parse the instruction line
        return new WhileLoop(exp, ParseOneInstruction(targets, context), line);
    }

    ForLoop ParseForLoop(Token targets, Token context)
    {
        Consume(TokenType.For);
        LookAhead(TokenType.Id);
        int line = NextToken.Line;
        //Saves the iterator and the collection
        Token iterator = NextToken;
        Consume(new List<TokenType> { TokenType.Id, TokenType.In });
        LookAhead(TokenType.Id);
        Expression collection = ParseExpression();

        LookAhead();
        if (NextToken.Definition is not TokenType.LCurly)
        {
            return new ForLoop(ParseOneInstruction(targets, context), iterator, collection, line);
        }
        Consume(TokenType.LCurly);
        var instruction = ParseInstruction(targets, context);
        Consume(TokenType.RCurly);
        return new ForLoop(instruction, iterator, collection, line);
    }
    
    #endregion
    #region CardNodes
    List<Card_Object> ParseCardDecBlock()
    {
        //Create the list
        List<Card_Object> cards = new List<Card_Object>{ ParseCard() };
        while(Pos + 1 < Tokens.Count)
        {
            LookAhead(TokenType.Card);
            cards.Add(ParseCard());
        }
        return cards;
    }
    Card_Object ParseCard()
    {
        LookAhead(TokenType.Card);
        //Save line for exception handling purposes
        int line = NextToken.Line;
        Consume(new List<TokenType> { TokenType.Card, TokenType.LCurly });
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
            else activation = ParseActivation(line);
            
            LookAhead(colons);
            if (NextToken.Definition is TokenType.Comma) Consume(TokenType.Comma);
            else break;
            expected.Remove(tokenType);
            LookAhead(expected);
        }
        Consume(TokenType.RCurly);
        return new Card_Object(name, type, faction, power, range, activation, line);
    }

    List<EffectAllocation> ParseActivation(int line)
    {
        Consume(new List<TokenType> { TokenType.OnActivation, TokenType.Colon, TokenType.LBracket });
        var effBlock = ParseEffBlock(line);
        Consume(TokenType.RBracket);
        return effBlock;
    }

    List<EffectAllocation> ParseEffBlock(int line)
    {
        Consume(TokenType.LCurly);
        var effects = new List<EffectAllocation>();
        effects.Add(ParseEffAllocation(line));
        Consume(TokenType.RCurly);
        LookAhead(new List<TokenType> { TokenType.Comma, TokenType.RCurly, TokenType.RBracket });
        while (NextToken.Definition is TokenType.Comma)
        {
            Consume(new List<TokenType> { TokenType.Comma, TokenType.LCurly });
            effects.Add(ParseEffAllocation(line));
            Consume(TokenType.RCurly);
        }
        if (NextToken.Definition is TokenType.RCurly)
            Consume(TokenType.RCurly);
        return effects;
    }

    EffectAllocation ParseEffAllocation(int line)
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
            if (tokenType is TokenType.C_Effect) allocation = ParseAllocation(line);
            else if (tokenType is TokenType.Selector) selector = ParseSelector(line);
            else if(tokenType is TokenType.PostAction) postAction = ParsePostAction(line);
            else break;

            LookAhead(colons);
            if (NextToken.Definition is TokenType.Comma) Consume(TokenType.Comma);
            else break;
            expected.Remove(tokenType);
            LookAhead(expected);
        }
        return new EffectAllocation(allocation, selector, postAction, line);
    }
    Expression ParseType()
    {
        Consume(new List<TokenType> { TokenType.Type, TokenType.Colon });
        LookAhead(TokenType.String);
        return ParseStringExpression();
    }

    Expression ParseFaction()
    {
        Consume(new List<TokenType> { TokenType.Faction, TokenType.Colon });
        LookAhead(TokenType.String);
        return ParseStringExpression();
    }

    Expression ParsePower()
    {
        Consume(new List<TokenType> { TokenType.Power, TokenType.Colon });
        return ParseNumericExpression();
    }
    List<Expression> ParseRange()
    {
        Consume(new List<TokenType> { TokenType.Range, TokenType.Colon, TokenType.LBracket });
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
    PostActionBlock ParsePostAction(int line)
    {
        Consume(new List<TokenType> { TokenType.PostAction, TokenType.Colon, TokenType.LCurly });
        return new PostActionBlock(ParseEffAllocation(line));
    }
    Selector ParseSelector(int line)
    {
        Consume(new List<TokenType> { TokenType.Selector, TokenType.Colon, TokenType.LCurly });

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
        return new Selector(source, single, predicate, line);          
    }
    Predicate ParsePredicate()
    {
        Consume(new List<TokenType> {TokenType.Predicate, TokenType.Colon, TokenType.LParen});
        LookAhead(TokenType.Id);
        Token id = NextToken;
        Consume(new List<TokenType> {NextToken.Definition, TokenType.RParen, TokenType.Implication});
        return new Predicate(id, ParseBoolExpression());
    }
    Predicate ParsePredicateExpression()
    {
        Consume(TokenType.LParen);
        LookAhead(TokenType.Id);
        Token id = NextToken;
        Consume(new List<TokenType> {NextToken.Definition, TokenType.RParen, TokenType.Implication});
        return new Predicate(id, ParseBoolExpression());
    }
    Expression ParseSingle()
    {
        Consume(new List<TokenType> { TokenType.Single, TokenType.Colon });
        return ParseBoolExpression();
    }
    Expression ParseSource()
    {
        Consume(new List<TokenType> { TokenType.Source, TokenType.Colon });
        LookAhead(TokenType.String);
        return ParseStringExpression();
    }
    Allocation ParseAllocation(int line)
    {
        Consume(new List<TokenType> { TokenType.C_Effect, TokenType.Colon });
        LookAhead(new List<TokenType>{TokenType.String, TokenType.LCurly});
        if (NextToken.Definition is TokenType.String)
        {
            return new Allocation(ParseStringExpression(), null, line);
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
        return new Allocation(name, varAllocation, line);
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
    
    #endregion
    #region Expressions
    Expression ParseExpression()
    {
        LookAhead();
        if(NextToken.Definition is TokenType.Num || NextToken.Definition is TokenType.Minus) return ParseNumericExpression();
        if (NextToken.Definition is TokenType.String) return ParseStringExpression();
        if (NextToken.Definition is TokenType.Boolean) {
            var boolean = NextToken;
            Consume(TokenType.Boolean);
            return new LiteralExpression(boolean);
        }
        if (NextToken.Definition is TokenType.LParen)
        {
            Consume(TokenType.LParen);
            Expression left = ParseExpression();
            Consume(TokenType.RParen);
            return left;
        }
        else {
            if (NextToken.Definition is TokenType.Increment || NextToken.Definition is TokenType.Decrement)
            {
                return ParseUnaryExpressionAtTheStart();
            }
            var left = ParseIdExpression();
            List<TokenType> BinOperators = new List<TokenType>
            {
                TokenType.Plus, TokenType.Minus, TokenType.Multip, TokenType.Division, TokenType.Concatenation,
                TokenType.SpaceConcatenation, TokenType.Assign, TokenType.MoreAssign, TokenType.MinusAssign, 
                TokenType.MultipAssign, TokenType.DivisionAssign
            };
            LookAhead();
            if (NextToken.Definition is TokenType.Increment || NextToken.Definition is TokenType.Decrement) {
                Token op = NextToken;
                Consume(NextToken.Definition);
                left = new UnaryExpression(left, op, true);
            }
            else if(BinOperators.Contains(NextToken.Definition)) {
                //Save the id operator and continue parsing the right part of the expression
                Token op = NextToken;
                Consume(NextToken.Definition);
                left = new BinaryExpression(left, op, ParseExpression());
            }
            return left;
        }

    }
    /// <summary>
    /// Possible function names
    /// </summary>
    /// <value></value>
    readonly HashSet<string> functionNames = new HashSet<string>
    {
        "Find", "Push", "SendBottom", "Pop", "Remove", "Shuffle","Add",
        "HandOfPlayer", "FieldOfPlayer", "GraveyardOfPlayer", "DeckOfPlayer", "Clear", "RemoveAt"
    };

    Expression ParseIdExpression()
    {
        Token firstId = NextToken;
        Consume(NextToken.Definition);
        Expression left = new LiteralExpression(firstId);
        LookAhead();

        while(NextToken.Definition is TokenType.Point || 
        NextToken.Definition is TokenType.LParen || 
        NextToken.Definition is TokenType.LBracket) {

            if (NextToken.Definition is TokenType.Point)
            {
                Token op = NextToken;
                Consume(TokenType.Point);
                LookAhead();
                while (NextToken.Definition is TokenType.Id || Utils.PropertiesReservedWords.Contains(NextToken.Definition))
                {
                    //Is a function
                    if (functionNames.Contains(NextToken.Value))
                    {
                        Token functionName = NextToken;
                        Consume(NextToken.Definition);
                        Consume(TokenType.LParen);
                        LookAhead();
                        if (functionName.Value == "Find")
                        {
                            left = new FindFunction(left, ParsePredicateExpression());
                        }
                        else if (NextToken.Definition is TokenType.RParen)
                            left = new FunctionCall(left, functionName, null);
                        else
                            left = new FunctionCall(left, functionName, ParseExpression());
                        Consume(TokenType.RParen);
                        LookAhead();
                    }
                    else
                    {
                        //Is a property
                        Token property = NextToken;
                        LiteralExpression literalExpression = new LiteralExpression(property);
                        Consume(NextToken.Definition);
                        left = new BinaryExpression(left, op, literalExpression);
                        LookAhead();

                        if (NextToken.Definition is TokenType.LParen) //it was an unvalid function name
                        {
                            CompilationError NotAvailableFunction = new NotAvailableFunction(property);
                            throw NotAvailableFunction;
                        }
                    }

                    if (NextToken.Definition is TokenType.LBracket)
                    {
                        Consume(NextToken.Definition);
                        left = new Indexer(left, ParseNumericExpression());
                        Consume(TokenType.RBracket);
                        LookAhead();
                    }

                    if (NextToken.Definition is TokenType.Point)
                    {
                        op = NextToken;
                        Consume(NextToken.Definition);
                        LookAhead();
                    }
                    else break;

                }
            }

            else if (NextToken.Definition is TokenType.LParen)
            {
                if (!functionNames.Contains(firstId.Value))
                {
                    CompilationError NotAvailableFunction = new NotAvailableFunction(firstId);
                    throw NotAvailableFunction;
                }

                //else
                Consume(NextToken.Definition);
                LookAhead();

                if (firstId.Value == "Find")
                {
                    left = new FindFunction(left, ParsePredicate());
                }
                else if (NextToken.Definition is TokenType.RParen)
                    left = new FunctionCall(left, firstId, null);
                else
                    left = new FunctionCall(left, firstId, ParseExpression());
                Consume(TokenType.RParen);
                LookAhead();
            }

            if (NextToken.Definition is TokenType.LBracket)
            {
                Consume(NextToken.Definition);
                left = new Indexer(left, ParseNumericExpression());
                Consume(TokenType.RBracket);
                LookAhead();
            }
        }

        return left;
    }


    Expression ParseUnaryExpressionAtTheStart()
    {
        Token oper = NextToken;
        Consume(NextToken.Definition);
        Expression id = ParseIdExpression();
        return new UnaryExpression(id, oper, false);
    }

    Expression ParseBoolExpression()
    {
        Expression left;
        LookAhead();
        if (NextToken.Definition is TokenType.LParen)
        {
            Consume(NextToken.Definition); LookAhead();
            left = ParseBoolExpression();
            Consume(TokenType.RParen);
        }
        else {
            left = ParseExpression();
        }
        var expected = new List<TokenType> { TokenType.And, TokenType.Or, TokenType.Equal, TokenType.Less, TokenType.LessEq, TokenType.More, TokenType.MoreEq };
        LookAhead();
        if (expected.Contains(NextToken.Definition))
        {
            Token op = NextToken;
            Consume(NextToken.Definition);
            LookAhead();
            var right = ParseBoolExpression();
            left = new BinaryExpression(left, op, right);
        }
        return left;
    }
    Expression ParseNumericExpression() => ParseSumExp();

    Expression ParseSumExp()
    {
        Expression left = ParseTerm();
        LookAhead();
        while (NextToken.Definition is TokenType.Plus || NextToken.Definition is TokenType.Minus)
        {
            Token op = NextToken;
            Consume(NextToken.Definition);
            Expression right = ParseTerm();
            left = new BinaryExpression(left, op, right);
            LookAhead();
        }
        return left;
    }
    Expression ParseTerm()
    {
        Expression left = ParsePow();
        LookAhead();
        while (NextToken.Definition is TokenType.Multip || NextToken.Definition is TokenType.Division)
        {
            Token op = NextToken;
            Consume(NextToken.Definition);
            Expression right = ParsePow();
            left = new BinaryExpression(left, op, right);
            LookAhead();
        }
        return left;
    }
    Expression ParsePow() {
        Expression left = ParseFactor();
        LookAhead();
        while (NextToken.Definition is TokenType.Pow)
        {
            Token op = NextToken;
            Consume(NextToken.Definition);
            Expression right = ParseFactor();
            left = new BinaryExpression(left, op, right);
            LookAhead();
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
            return ParseExpression();
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
        return new BinaryExpression(new LiteralExpression(new Token("0", TokenType.Num, op.Line,op.Column)), op, right);
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
            return ParseExpression();
        }
        Consume(TokenType.LCurly);
        var exp = ParseStringExpression();
        Consume(TokenType.RCurly);
        return exp;
    }
    #endregion
}
