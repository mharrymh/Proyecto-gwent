using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassButtonBehauvior : MonoBehaviour
{
    public GameManager gm;

    //This is called when player clicks pass button
    public void OnClickPassButton()
    {
        gm.currentPlayer.Passed = true;
        gm.ChangeTurn();
    }

}
