using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DisplayCard : MonoBehaviour
{
    /// <summary>
    /// Represent the object Card
    /// </summary>
    public Card card;
    public TMP_Text DescriptionText;
    public Image CardImage;
    public TMP_Text PowerText;
    public TextMeshProUGUI NameText;
    public Image FactionImage;
    
    /// <summary>
    /// Handle the logic of representing a visual card
    /// </summary>
    public void ShowCard()
    {
        //CardPrefab gets the name of the card
        //(this) calls the prefab
        this.name = card.Name;
        CardImage.sprite = card.CardImage;
        NameText.text = card.Name;
        DescriptionText.text = card.Description;

        if (card.CardFaction is CardFaction.Light) FactionImage.sprite = Resources.Load<Sprite>("Light");
        else FactionImage.sprite = Resources.Load<Sprite>("Dark");

        //Assign power if card is unity card type
        //Else assign a blank string to represent that it has no power
        if (card is Card.UnityCard unity)
        {
            PowerText.text = unity.Power.ToString();
        }
        else
            PowerText.text = "";
    }
}
