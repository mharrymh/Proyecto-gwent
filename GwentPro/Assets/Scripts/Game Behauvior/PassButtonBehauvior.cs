using UnityEngine;

public class PassButtonBehauvior : MonoBehaviour
{
    public GameManager gm;
    public SoundManager soundM;

    public void Start()
    {
        soundM = GameObject.Find("AudioSourceEffects").GetComponent<SoundManager>();
    }

    //This is called when player clicks pass button
    public void OnClickPassButton()
    {
        soundM.PlayButtonSound();
        //Set the property passed of the player to true
        gm.currentPlayer.Passed = true;
        gm.ChangeTurn();
    }

}
