namespace Transpiler;
internal class MainProgram
{
    static void Main(string[] args)
    {
        Lexer lexer = new Lexer();

        List<Token> tokens = new List<Token>();
        string fileContent = "";
        string filePath = @"C:\Users\mauri\Documents\Proyecto-gwent\GwentPro\Gwent++\Transpiler\Parser\DSL_example.txt";

        if (File.Exists(filePath)) {
            fileContent = File.ReadAllText(filePath);
        }

        // Tokenizar el string de prueba
        tokens = lexer.Tokenize(fileContent);

        // Imprimir cada token
        foreach (Token token in tokens)
        {
            Console.WriteLine($"Value: {token.Value}, Definition: {token.Definition}, Line: {token.Line}, Column: {token.Column}");
        }

        Parser parser = new Parser(tokens);

        DSL_Object program = parser.Parse();  

        Console.WriteLine(program.Validate(new Context()));
    }
}