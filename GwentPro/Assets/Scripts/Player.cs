using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player
{
    public string ID { get; set; }
    public CardFaction Faction { get; set; }
    public List<Card> PlayerDeck { get; set; }
    public List<Card> Hand { get; set; }
    public int Score { get; set; }
    public Card.LeaderCard Leader { get; set; }
    public List<Card> GraveYard { get; set; }
    public bool Passed { get; set; }
    public int RoundsWon { get; set; }
    public bool LeaderPlayed { get; set; }

    public Player(CardFaction Faction, string ID)
    {
        this.Faction = Faction;
        GetPlayerDeck(Faction);
        this.ID = ID;
        Score = 0;
        RoundsWon = 0;
        Hand = new List<Card>();
    }

    CardDatabase cards = new CardDatabase();
    
    
    public void GetPlayerDeck(CardFaction Faction)
    {
        if (Faction == CardFaction.Light)
        {
            PlayerDeck = cards.GetLightDeck();
        }
        else
        {
            PlayerDeck = cards.GetDarkDeck();
        }
        //Assign Leader property
        for (int i = 0; i < PlayerDeck.Count; i++)
        {
            if (PlayerDeck[i] is Card.LeaderCard leader)
            {
                Leader = leader;
                PlayerDeck.RemoveAt(i);
            }
        }
    }
    public void AssignHand(Card card)
    {
        Hand.Add(card);
    }
    
}
