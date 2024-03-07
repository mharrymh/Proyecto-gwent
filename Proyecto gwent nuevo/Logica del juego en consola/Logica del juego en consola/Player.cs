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
        public Card.LeaderCard Leader { get; set; }
        public List<Card> GraveYard { get; set; }
        public bool Passed { get; set; }
        public int RoundsWon { get; set; }
        public Board board { get; set; }
        
        public Player(CardFaction Faction, string ID)
        {            
            this.Faction = Faction;
            GetPlayerDeck(Faction);
            this.ID = ID;
            Score = 0;
            RoundsWon = 0;
            Passed = false;
            board = Board.Instance;
        }

        CardDatabase cartas = new CardDatabase();
        public void GetLeaderCard()
        {    
            foreach (Card card in PlayerDeck)
            {
                if (card is Card.LeaderCard leader_card)
                {
                    Leader = leader_card;
                }
            }
        }
        public void GetPlayerDeck(CardFaction Faction)
        {
            if (Faction == CardFaction.Light)
            {
                PlayerDeck = cartas.GetLightDeck();
            }
            else
            {
                PlayerDeck = cartas.GetDarkDeck();
            }
            //Assign Leader property
            GetLeaderCard();
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
        public static List<Card> Shuffle(List<Card> Cards)
        {
            Random rand = new Random();
            return Cards = Cards.OrderBy(x => rand.Next()).ToList();
        }

        public void DrawCard(int n)
        {
            if (PlayerDeck.Count > n)
            {
                for (int i = 0; i <= n; i++)
                {
                    Hand.Add(PlayerDeck[i]);
                }
            }
        }
    }
}
