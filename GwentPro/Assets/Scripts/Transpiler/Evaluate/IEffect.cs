public interface IDeclaredEffect {
    public void Execute(IExecuteScope? scope = null, DeclaredEffect? parent = null);
}