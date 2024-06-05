namespace Transpiler;
class ArithmeticParser {

    List<Token> Tokens {get; }
    int Pos {get; set;}

    public ArithmeticParser(List<Token> tokens)
    {
        this.Tokens = tokens;
        Pos = 0;
    }
    public Expression Parse() {

        Expression result = ParseExpression();
        if (Pos < Tokens.Count) 
        {
            var token = Tokens[Pos];
            var error = new NotNumericalToken(token.Line, token.Column, token);
            throw new Exception(error.ToString());
        }
        return result; 
    }
    Expression ParseExpression() {
        
        Expression left = ParseTerm();
        if (Pos < Tokens.Count && (Tokens[Pos].Definition is TokenType.Plus || Tokens[Pos].Definition is TokenType.Minus)) 
        {
            char op = Tokens[Pos++].Value[0];
            Expression right = ParseExpression();
            left = new BinaryExpression(left, op, right);
        }
        return left;
    }
    Expression ParseTerm()
    {
        Expression left = ParseFactor();
        if (Pos < Tokens.Count && (Tokens[Pos].Definition is TokenType.Multip || Tokens[Pos].Definition is TokenType.Division))
        {
            char op = Tokens[Pos++].Value[0];
            var right = ParseTerm();
            left = new BinaryExpression(left, op, right);
        }
        return left;
    }
    Expression ParseFactor()
    {   
        if (Pos < Tokens.Count)
        {
            if (Tokens[Pos].Definition == TokenType.Number) 
            {
                int value = int.Parse(Tokens[Pos++].Value);
                return new LiteralExpression(value);
            }

            //if is an ID

            if (Tokens[Pos].Definition == TokenType.LParen)
            {
                Pos++;
                var expr = ParseExpression();
                if (Pos >= Tokens.Count)
                {
                    var token = Tokens[^1];
                    var error = new NotClosedParen(token.Line, token.Column, token);
                    throw new Exception(error.ToString());
                }
                else if (Tokens[Pos++].Definition != TokenType.RParen)
                {
                    var token = Tokens[Pos-1];
                    var error = new InvalidTokenInParen(token.Line, token.Column, token);
                    throw new Exception(error.ToString());
                }
                return expr;
            }

            else if(Tokens[Pos].Definition == TokenType.Minus)
            {
                Pos++;
                if (Tokens[Pos].Definition == TokenType.Number)
                {
                    int value = int.Parse(Tokens[Pos++].Value);
                    return new LiteralExpression(-value); //return the negative value
                }
            }
            else {
            var token = Tokens[Pos];
            var error = new NotNumericalToken(token.Line, token.Column, token);
            throw new Exception(error.ToString());
            }
        }
        var lastToken = Tokens[^1];
        var end_error = new UnexpectedEnd(lastToken.Line, lastToken.Column, lastToken); 
        throw new Exception(end_error.ToString());
    }
}

