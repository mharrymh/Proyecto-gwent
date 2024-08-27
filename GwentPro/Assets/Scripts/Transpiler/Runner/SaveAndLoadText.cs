using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveAndLoadText : MonoBehaviour
{
    public TMPro.TMP_InputField inputField;
    public TMPro.TMP_InputField numberField;
    public SoundManager soundM;

    public void Start()
    {
        soundM = GameObject.Find("AudioSourceEffects").GetComponent<SoundManager>();
    }

    public void GetValue()
    {
        string value = numberField.text;
        if (int.TryParse(value, out int result) && result >= 0 && result < 30)
        {
            Utils.AmountOfCardsOfDeck = result;
        }
    }

    /// <summary>
    /// This is called when the save button is clicked 
    /// </summary>
    public void SaveTextOnFile()
    {
        soundM.PlayButtonSound();
        string text = inputField.text;

        string relativePath = "Utils/transpiler.txt";
        string filePath = Path.Combine(Application.dataPath, relativePath);

        File.WriteAllText(filePath, text);

        Debug.Log("Texto guardado en " + filePath);
    }
    /// <summary>
    /// This is called when transpiler menu is loaded
    /// </summary>
    public void LoadFile()
    {
        string relativePath = "Utils/transpiler.txt";
        string filePath = Path.Combine(Application.dataPath, relativePath);
        
        if (File.Exists(filePath))
        {
            string fileContent = File.ReadAllText(filePath);
            inputField.text = fileContent;
        }
        else
        {
            // the file soes not exists, leave it empty
            inputField.text = string.Empty;
        }
    }
}
