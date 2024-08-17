using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Player
{
    readonly Board board = Board.Instance;
    
    public string PlayerName { get; set; }
    public string ID { get; set; }
    public CardFaction Faction { get; set; }
    public CardCollection PlayerDeck { get; set; }
    public CardCollection Hand { get; set; }
    public int Score { get; set; }
    public Card.LeaderCard Leader { get; set; }
    public CardCollection GraveYard { get; set; }
    public CardCollection Field {
        get {
            //Get all the cards in the player board
            CardCollection field = new();
            foreach (CardCollection cards in board.sections[this.ID].Values)
            {
                foreach (Card card in cards)
                {
                    field.Add(card);
                }
            }
            return field;
        }
    }
    public bool Passed { get; set; }
    public int RoundsWon { get; set; }
    public bool LeaderPlayed { get; set; }
    //Track if the player received his hand
    public bool Ready { get; set; }
    //Check if the player has played a card
    public bool HasPlayed { get; set; }
    //Keep track of how many changes has been done on first round
    public int Changes { get; set; }
    public GameObject GraveyardObj { get;}

    public Player(CardFaction Faction, string ID, string name, GameObject graveyard)
    {
        this.Faction = Faction;
        GetPlayerDeck(Faction);
        this.ID = ID;
        Score = 0;
        RoundsWon = 0;
        Hand = new();
        GraveYard = new();
        HasPlayed = false;
        Changes = 0;
        PlayerName = name;
        this.GraveyardObj = graveyard;
    }

    readonly CardDatabase cards = new CardDatabase();
    
    
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

        //Add user cards
        foreach (Card card in CardConverter.myCardsPerFactions[Faction])
        {
            PlayerDeck.Add(card.Duplicate());
        }

        //Assign Leader property
        for (int i = 0; i < PlayerDeck.Count; i++)
        {
            if (PlayerDeck[i] is Card.LeaderCard leader)
            {
                Leader = leader;
                Leader.Owner = this;
                PlayerDeck.RemoveAt(i);
            }
        }
    }
    
}
