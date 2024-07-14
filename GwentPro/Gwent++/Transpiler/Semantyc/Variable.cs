namespace Transpiler;
public struct Variable {
    public Expression? Value {get; }
    public IdType IdType {get;}
    public Variable(Expression? value, IdType idType) {
        Value = value;
        IdType = idType;
    }
}