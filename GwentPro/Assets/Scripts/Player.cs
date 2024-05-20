using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player
{
    public string PlayerName { get; set; }
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
    //Track if the player received his hand
    public bool Ready { get; set; }
    //Check if the player has played a card
    public bool HasPlayed { get; set; }
    //Keep track of how many changes has been done on first round
    public int Changes { get; set; }
    public GameObject GraveyardObj { get; set; }

    public Player(CardFaction Faction, string ID, string name)
    {
        this.Faction = Faction;
        GetPlayerDeck(Faction);
        this.ID = ID;
        Score = 0;
        RoundsWon = 0;
        Hand = new List<Card>();
        GraveYard = new List<Card>();
        HasPlayed = false;
        Changes = 0;
        PlayerName = name;
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
    
}
