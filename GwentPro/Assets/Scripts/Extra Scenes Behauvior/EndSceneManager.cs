using TMPro;
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
            WinnerText.text = $"Congratulations!!! \"{PlayerData.Winner}\" wins the game";
        }
        else
        {
            WinnerText.text = "Uhhh! It's a tie, play again";
        }
    }

    public void Exit()
    {
        soundM.PlayButtonSound();
        Application.Quit();
    }
}
