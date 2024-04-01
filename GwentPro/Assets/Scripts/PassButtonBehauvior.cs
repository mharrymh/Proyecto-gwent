using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassButtonBehauvior : MonoBehaviour
{
    public GameManager gm;

    //This is called when player clicks pass button
    public void OnClickPassButton()
    {
        //Set the property passed of the player to true
        gm.currentPlayer.Passed = true;
        gm.ChangeTurn();
    }

}
