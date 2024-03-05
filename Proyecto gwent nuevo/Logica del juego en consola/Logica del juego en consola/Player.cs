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
        public CardFaction Faction { get; set; }
        public List<Card> PlayerDeck { get; set; }
        public List<Card> Hand { get; set; }
        public int Score { get; set; }
        public Card Leader { get; set; }
        public List<Card> GraveYard { get; set; }
        public bool IsPlaying { get; set; }
        public bool HasPlayed { get; set; }
        public bool Passed { get; set; }
        public int RoundsWon { get; set; }

        //Constructor
        public Player(CardFaction Faction, string ID)
        {            
            this.Faction = Faction;
            PlayerDeck = GetPlayerDeck(Faction);
            Leader = GetLeaderCard(Faction);
            Hand = AssignHand();
            this.ID = ID;
            Score = 0;
            RoundsWon = 0;
            IsPlaying = false;
            HasPlayed = false;
            Passed = false;
        }

        CardDatabase cartas = new CardDatabase();
        
        public Card GetLeaderCard(CardFaction faction)
        {
            if (faction == CardFaction.Light)
            {
                Card leaderCard = PlayerDeck.Find(card => card.Name == "Zeus");
                PlayerDeck.Remove(leaderCard);
                return leaderCard;
            }
            else
            {
                Card leaderCard = PlayerDeck.Find(card => card.Name == "Hades");
                PlayerDeck.Remove(leaderCard);
                return leaderCard;
            }
        }
        public List<Card> GetPlayerDeck(CardFaction Faction)
        {
            if (Faction == CardFaction.Light)
            {
                List<Card> PlayerDeck = new List<Card>();
                PlayerDeck = cartas.GetLightDeck();
            }
            else
            {
                List<Card> PlayerDeck = new List<Card>();
                PlayerDeck = cartas.GetDarkDeck();
            }
            return PlayerDeck;
        }
        public List<Card> AssignHand()
        {
            Hand = new List<Card>();
            PlayerDeck = Shuffle(PlayerDeck);
            
            for (int i = 0; i < 10; i++)
            {
                Card card = PlayerDeck[i];
                card.Owner = this;
                Hand.Add(card);
                //Assign owner property to card         
            }
            //remove hand cards from the player deck
            PlayerDeck = PlayerDeck.Except(Hand).ToList();

            return Hand;
        }

        //Shuffle deck method
        public static List<Card> Shuffle(List<Card> playerDeck)
        {
            Random rand = new Random();
            return playerDeck = playerDeck.OrderBy(x => rand.Next()).ToList();
        }

    }
}
