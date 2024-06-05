// namespace Transpiler; 
// public abstract class Expression { 
//     public override abstract string ToString();
// }

// public class BinaryArithmeticExpression : Expression
// {
//     Expression Left { get; }
//     char Op {get; }
//     Expression Right {get; }

//     public BinaryExpression(Expression left, char op, Expression right) {
//         this.Left = left;
//         this.Op = op;
//         this.Right = right;
//     }

//     public override string ToString() {

//         return $"({Left.ToString()} {Op} {Right.ToString()})";
//     }
// }
//     public class LiteralExpression : Expression {
//         int Value {get; }

//         public LiteralExpression(int value) {
//             this.Value = value;
//         }

//     public override string ToString()
//     {
//         return Value.ToString();
//     }
// }
