using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;

public class EndSceneManager : MonoBehaviour
{
    public TMP_Text WinnerText;


    public void Start()
    {
        if (PlayerData.Winner != null)
        {
            WinnerText.text = "Felicidades!!! Gan� " + PlayerData.Winner;
        }
        else
        {
            WinnerText.text = "Uhhh! Empataron, jueguen de nuevo";
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
    public void Exit()
    {
        Debug.Log("Salir...");
        Application.Quit();
    }
}
