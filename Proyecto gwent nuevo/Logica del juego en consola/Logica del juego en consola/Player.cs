using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Unity.VisualScripting.Antlr3.Runtime.Tree;
//using UnityEngine.Rendering;
//using UnityEngine.SocialPlatforms.Impl;

namespace Assets.Scripts
{
    public class Player
    {
        public string ID { get; set; }
        public List<Card> PlayerDeck { get; set; }
        public List<Card> Hand { get; set; }
        public int Score { get; set; }
        public Card Leader { get; set; }
        public List<Card> GraveYard { get; set; }
        public bool IsPlaying { get; set; }
        public bool HasPlayed { get; set; }
        public int RoundsWon { get; set; }

        //Constructor
        public Player(List<Card> Faction, string ID)
        {
            //***
            PlayerDeck = Faction;
            Leader = PlayerDeck.Find(Card => Card.Type == CardType.Leader);
            Hand = AssignHand();
            this.ID = ID;
            Score = 0;
            RoundsWon = 0;
        }
        public List<Card> AssignHand()
        {
            Hand = new List<Card>();
            PlayerDeck = Deck.Shuffle(PlayerDeck);
            
            for (int i = 0; i < 10; i++)
            {
                Card card = PlayerDeck[i];
                card.Owner = this;
                Hand.Add(card);
                //Agregarle el owner a la carta
                
            }
            //remove cards from the player deck
            PlayerDeck = PlayerDeck.Except(Hand).ToList();

            return Hand;
        }
    }
}
