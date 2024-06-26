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

    public bool Player1Chose = false;
    public bool Player2Chose = false;

    public SoundManager soundM;

    public void Start()
    {
         soundM = GameObject.Find("AudioSourceEffects").GetComponent<SoundManager>();
    }
    public void SavePlayer1Name()
    {
        PlayerData.Player1Name = InputField1.text;
    }

    public void SavePlayer2Name()
    {
        PlayerData.Player2Name = InputField2.text;
    }

    public void LightFactionPlayer1()
    {
        PlayerData.FactionPlayer1 = CardFaction.Light;
        Player1Chose = true;
    }

    public void DarkFactionPlayer1()
    {
        PlayerData.FactionPlayer1 = CardFaction.Dark;
        Player1Chose = true;
    }

    public void LightFactionPlayer2()
    {
        PlayerData.FactionPlayer2 = CardFaction.Light;
        Player2Chose = true;
    }

    public void DarkFactionPlayer2()
    {
        PlayerData.FactionPlayer2 = CardFaction.Dark;
        Player2Chose = true;
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
