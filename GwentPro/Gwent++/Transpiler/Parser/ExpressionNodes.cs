namespace Transpiler;
public abstract class Expression : DSL_Object {}
public class BinaryExpression : Expression
{
    Expression Left { get; }
    Token Op {get; }
    Expression Right {get; }

    public BinaryExpression(Expression left, Token op, Expression right) {
        this.Left = left;
        this.Op = op;
        this.Right = right;
    }
}
public class LiteralExpression : Expression
{
    Token Value {get; }
    public LiteralExpression(Token value) {
        this.Value = value;
    }
}

public class UnaryExpression : Expression
{
    Expression ID { get; }
    Token op {get; }
    bool AtTheEnd { get; }
    public UnaryExpression(Expression id, Token op, bool atTheEnd) {
        this.ID = id;
        this.op = op;
        this.AtTheEnd = atTheEnd;
    }
}