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
    public TextMeshProUGUI CardName;
    public Image FactionImage;
    public void ShowCard()
    {
        //CardPrefab gets the name of the card
        this.name = card.Name;
        CardImage.sprite = card.CardImage;
        Description.text = card.Description;
        CardName.text = card.Name;

        if (card.Faction is CardFaction.Light) FactionImage.sprite = Resources.Load<Sprite>("Light");
        else FactionImage.sprite = Resources.Load<Sprite>("Dark");

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
