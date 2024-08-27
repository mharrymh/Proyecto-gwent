using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class FactionMenuManager : MonoBehaviour
{
    public TMP_InputField InputField1;
    public TMP_InputField InputField2;

    public Button LightFaction1;
    public Button LightFaction2;
    public Button DarkFaction1;
    public Button DarkFaction2;

    public bool Player1Chose = false;
    public bool Player2Chose = false;

    private ColorBlock originalColor;

    public SoundManager soundM;

    public void Start()
    {
        soundM = GameObject.Find("AudioSourceEffects").GetComponent<SoundManager>();

        // Save original color of the buttons
        originalColor = LightFaction1.colors;
    }
    public void SavePlayer1Name()
    {
        PlayerData.Player1Name = InputField1.text;
    }

    public void SavePlayer2Name()
    {
        PlayerData.Player2Name = InputField2.text;
    }

    void KeepPressedButton(Button pressed, Button unPressed)
    {
        var colors = pressed.colors;
        colors.normalColor = Color.black; // Change color of the button background
        pressed.colors = colors;

        // Disable button interaction
        pressed.interactable = false;

        // Restore state of the other button
        unPressed.colors = originalColor;
        unPressed.interactable = true;
    }

    public void LightFactionPlayer1()
    {
        PlayerData.FactionPlayer1 = CardFaction.Light;
        Player1Chose = true;

        KeepPressedButton(LightFaction1, DarkFaction1);
    }

    public void DarkFactionPlayer1()
    {
        PlayerData.FactionPlayer1 = CardFaction.Dark;
        Player1Chose = true;
        
        KeepPressedButton(DarkFaction1, LightFaction1);
    }

    public void LightFactionPlayer2()
    {
        PlayerData.FactionPlayer2 = CardFaction.Light;
        Player2Chose = true;

        KeepPressedButton(LightFaction2, DarkFaction2);
    }

    public void DarkFactionPlayer2()
    {
        PlayerData.FactionPlayer2 = CardFaction.Dark;
        Player2Chose = true;

        KeepPressedButton(DarkFaction2, LightFaction2);
    }

    public void Back()
    {
        soundM.PlayButtonSound();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
    public void Play()
    {
        SavePlayer1Name();
        SavePlayer2Name();

        if (!Player1Chose || PlayerData.Player1Name == null
            || PlayerData.Player1Name == "" || !Player2Chose || PlayerData.Player2Name == null
            || PlayerData.Player2Name == "") soundM.PlayErrorSound();

        else
        {
            soundM.PlayButtonSound();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }


}
