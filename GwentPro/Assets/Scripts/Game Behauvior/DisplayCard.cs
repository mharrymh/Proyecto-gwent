using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;
using System.Linq;

public class DisplayCard : MonoBehaviour
{
    /// <summary>
    /// Represent the object Card
    /// </summary>
    public Card card;
    public TMP_Text DescriptionText;
    public Image CardImage;
    public TMP_Text PowerText;
    public TMP_Text RangeText;
    public TMP_Text TypeText;
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
        TypeText.text = GetTypeText(card);

        if (card.CardFaction is CardFaction.Light) FactionImage.sprite = Resources.Load<Sprite>("Light");
        else FactionImage.sprite = Resources.Load<Sprite>("Dark");

        //Assign power if card is unity card type
        //Else assign a blank string to represent that it has no power
        if (card is Card.UnityCard unity)
        {
            RangeText.text = unity.Range;
            PowerText.text = unity.Power.ToString();
        }
        else
            PowerText.text = "";

        if (card is Card.ClimateCard climate)
        {
            RangeText.text = climate.Range;
        }
        else if (card is Card.IncrementCard increment)
        {
            RangeText.text = increment.Range;
        }
    }

    private string GetTypeText(Card card)
    {
        Dictionary<Type, string> relateTypeText = new Dictionary<Type, string>{
            { typeof(Card.SilverCard), "Silver card"},
            { typeof(Card.GoldCard), "Gold card"},
            { typeof(Card.ClimateCard), "Climate card"},
            { typeof(Card.IncrementCard), "Increment card"},
            { typeof(Card.DecoyCard), "Decoy card"},
            { typeof(Card.CleareanceCard), "Cleareance card"},
            { typeof(Card.LeaderCard), "Leader card"}
        };

        return relateTypeText[card.GetType()];
    }
}
