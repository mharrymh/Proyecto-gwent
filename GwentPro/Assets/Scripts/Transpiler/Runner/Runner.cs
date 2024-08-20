using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class Runner : MonoBehaviour
{
    /// <summary>
    /// This is calles when save button is clicked
    /// </summary>
    public void OnClickSaveButton()
    {

        string relativePath = "Utils/transpiler.txt";
        string filePath = Path.Combine(Application.dataPath, relativePath);

        string fileContent = File.ReadAllText(filePath);

        if (string.IsNullOrEmpty(fileContent))
        {
            //TODO: 
            throw new System.Exception("el archivo esta vacio");
        }

        //Lexer
        Lexer lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize(fileContent);

        //Parser
        Parser parser = new Parser(tokens);
        DSL_Object program = parser.Parse();

        //Semantyc
        program.Validate(new Scope());

        //Evaluate and save the cards
        //Each card saves the effects 
        List<ICard> myCards = ((DecBlock)program).Evaluate();
        
        //Convert your created cards
        CardConverter.SaveCards(myCards);
    }
}
