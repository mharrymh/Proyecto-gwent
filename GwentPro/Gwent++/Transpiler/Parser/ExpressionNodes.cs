namespace Transpiler;
public abstract class Expression : DSL_Object {}
public class BinaryExpression : Expression
{
    public Expression Left { get; }
    public Token Op {get; }
    public Expression Right {get; }

    public BinaryExpression(Expression left, Token op, Expression right) {
        this.Left = left;
        this.Op = op;
        this.Right = right;
    }
    //FIXME: DONDE CHEQUEO QUE EL TIPO DE LEFT Y EL TIPO DE RIGHT COINCIDAN?
    public override bool Validate(IContext context)
    {
        return Left.Validate(context) && Right.Validate(context);
    }
}
public class LiteralExpression : Expression
{
    public Token Value {get; }
    public LiteralExpression(Token value) {
        this.Value = value;
    }

    public override bool Validate(IContext context)
    {
        if (Value.Definition is TokenType.Num || Value.Definition is TokenType.Text
        || Value.Definition is TokenType.Boolean) {
            return true;
        }
        else return context.IsDefined(Value);
    }
}

public class UnaryExpression : Expression
{
    Expression ID { get; }
    Token Op {get; }
    bool AtTheEnd { get; }
    public UnaryExpression(Expression id, Token op, bool atTheEnd) {
        this.ID = id;
        this.Op = op;
        this.AtTheEnd = atTheEnd;
    }

    public override bool Validate(IContext context)
    {
        if (ID is BinaryExpression binary) return binary.Validate(context);
        else if(ID is LiteralExpression literal) return literal.Validate(context);
        //TODO: THROW ERROR
        return false;
    }
}