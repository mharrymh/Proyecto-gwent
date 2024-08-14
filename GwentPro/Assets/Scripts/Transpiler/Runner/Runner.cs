using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Runner : MonoBehaviour
{
    public void OnClickRunButton()
    {
        string filePath = @"C:\Users\mauri\Documents\Proyecto-gwent\GwentPro\Assets\Utils\transpiler.txt";

        if (!File.Exists(filePath))
        {
            //TODO: 
            throw new System.Exception("El archivo esta vacio");
        }

        string fileContent = File.ReadAllText(filePath);

        //Lexer
        Lexer lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize(fileContent);

        // Imprimir cada token
        //TODO: borrar esto
        foreach (Token token in tokens)
        {
            Debug.Log($"Value: {token.Value}, Definition: {token.Definition}, Line: {token.Line}, Column: {token.Column}");
        }

        //Parser
        Parser parser = new Parser(tokens);
        DSL_Object program = parser.Parse();

        //Semantyc
        program.Validate(new Scope());

        //Evaluate and save the cards
        //Each card saves the effects 
        List<ICard> myCards = ((DecBlock)program).Evaluate();

        CardConverter.SaveCards(myCards);

    }
}
