using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;

public class EndSceneManager : MonoBehaviour
{
    public TMP_Text WinnerText;
    public SoundManager soundM;


    public void Start()
    {
        soundM = GameObject.Find("AudioSourceEffects").GetComponent<SoundManager>();

        if (PlayerData.Winner != null)
        {
            WinnerText.text = "Felicidades!!! Ganó " + PlayerData.Winner;
        }
        else
        {
            WinnerText.text = "Uhhh! Empataron, jueguen de nuevo";
        }
    }

    public void Exit()
    {
        soundM.PlayButtonSound();
        Debug.Log("Salir...");
        Application.Quit();
    }
}
