using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;


public class Runner : MonoBehaviour
{   
    public SoundManager soundM;

    public void Start()
    {
        soundM = GameObject.Find("AudioSourceEffects").GetComponent<SoundManager>();
    }
    /// <summary>
    /// The error text field
    /// </summary>
    public TMP_Text ErrorText;
    /// <summary>
    /// This is calles when save button is clicked
    /// </summary>
    public void OnClickSaveButton()
    {
        soundM.PlayButtonSound();
        string relativePath = "Utils/transpiler.txt";
        string filePath = Path.Combine(Application.dataPath, relativePath);

        string fileContent = File.ReadAllText(filePath);
        try
        {
            if (string.IsNullOrEmpty(fileContent))
            {
                CompilationError EmptyInput = new EmptyInput();
                throw EmptyInput;
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

            //Get a confirmation that the input passed successfully
            Debug.Log(ErrorText == null);
            ErrorText.text = "Input passed successfully, start the game as usual and you will be able to use your cards";

        }
        catch (CompilationError error)
        {
            ErrorText.text = error.Message;
            DefinedActions.ClearActions();
        }
    }
}
