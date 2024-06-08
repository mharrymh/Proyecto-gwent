namespace Transpiler;
internal class MainProgram
{
    static void Main(string[] args)
    {
        Lexer lexer = new Lexer();

        List<Token> tokens = new List<Token>();
        string fileContent = "";
        string filePath = @"C:\Users\mauri\Documents\Proyecto-gwent\GwentPro\Gwent++\test.txt";

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

        var exp = parser.Parse();       

        string ToDot(this Program node)
        {
            if (node == null) return "";

            var sb = new StringBuilder();
            sb.Append(node.GetType().Name + " [label=\"" + node.GetType() + "\"]; // Etiqueta del nodo

            foreach (var child in Program.Children)
            {
                sb.Append(child.GetType() + " -> " + ToDot(child) + "\n");
            }
            return sb.ToString();
        }
    }
}