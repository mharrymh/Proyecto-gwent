namespace Transpiler;
public abstract class Expression {}
public class BinaryExpression : Expression
{
    Expression Left { get; }
    string Op {get; }
    Expression Right {get; }

    public BinaryExpression(Expression left, string op, Expression right) {
        this.Left = left;
        this.Op = op;
        this.Right = right;
    }
}
public class LiteralExpression : Expression
{
    string Value {get; }
    public LiteralExpression(string value) {
        this.Value = value;
    }
}