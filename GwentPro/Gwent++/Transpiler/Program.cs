using System.IO;

namespace Transpiler;
internal class Program
{
    static void Main(string[] args)
    {
        Lexer lexer = new Lexer();

        List<Token> tokens = new List<Token>();
        string fileContent = "";
        string filePath = @"C:\Users\mauri\Desktop\Gwent++\Transpiler\Parser\prueba.txt";

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

        ArithmeticParser parser = new ArithmeticParser(tokens);

        var exp = parser.Parse();
        Console.WriteLine(exp.ToString());

        
    }
}