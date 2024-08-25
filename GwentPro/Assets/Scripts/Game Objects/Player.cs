using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Player
{
    readonly Board board = Board.Instance;
    
    public string PlayerName { get;}
    public string ID { get; set; }
    public CardFaction Faction { get; set; }
    public CardCollection PlayerDeck { get; private set;}
    public CardCollection Hand {get; }
    public int Score { get; set; }
    public Card.LeaderCard Leader { get; set; }
    public CardCollection GraveYard {get; }
    
    public CardCollection Field {
        get {
            //Get all the cards in the player board
            CardCollection field = new("field", this);
            foreach (CardCollection cards in board.sections[this.ID].Values)
            {
                foreach (Card card in cards)
                {
                    field.Add(card);
                }
            }
            foreach (Card.ClimateCard climate_card in board.climate_section)
            {
                if (climate_card != null)
                field.Add(climate_card);
            }
            foreach (Card.IncrementCard increment_card in board.increment_section[this.ID])
            {
                if (increment_card != null)
                field.Add(increment_card);
            }
            field.Shuffle();
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
        Hand = new("hand", this);
        GraveYard = new("graveyard", this);
        HasPlayed = false;
        Changes = 0;
        PlayerName = name;
        GraveyardObj = graveyard;
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
            if (card is Card.LeaderCard leader)
            {
                OverwriteLeader(leader.Duplicate());
            }
            else PlayerDeck.Add(card.Duplicate());
        }
        //Set owner to all cards
        foreach (Card card in PlayerDeck)
        {
            card.Owner = this;
        }

        //Assign properties to cardCollection
        PlayerDeck.GameListName = "deck";
        PlayerDeck.Player = this;

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

    void OverwriteLeader(Card newLeader)
    {
        for (int i = 0; i < PlayerDeck.Count; i++)
        {   
            if (PlayerDeck[i] is Card.LeaderCard oldLeader)
            {
                PlayerDeck[i] = newLeader;
                return;
            }
        }
    }
}
