using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DisplayCard : MonoBehaviour
{
    public Card card;
    public TMP_Text Description;
    public Image CardImage;
    public TMP_Text Power;


    void Update()
    {
        CardImage.sprite = card.CardImage;
        Description.text = card.Description;

        //Assign power if card is unity card type
        //Else assign 0
        if (card is Card.UnityCard unity_card)
        {
            Power.text = unity_card.Power.ToString();
        }
        else
        {
            Power.text = "";
        }
    }
}
