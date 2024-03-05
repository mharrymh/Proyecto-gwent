using Logica_del_juego_en_consola;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
//using static UnityEditor.Experimental.GraphView.GraphView;

namespace Assets.Scripts
{
    public class Board
    {
        private static Board _instance;

        public static Board Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Board();
                }
                return _instance;
            }
        }


        GameController gamecontroller = new GameController();
        //The board with its sections
        public Dictionary<string, Dictionary<string, List<Card>>> sections;
        //Climate Section
        
        

        private Board()
        {
            sections = new Dictionary<string, Dictionary<string, List<Card>>>()
            {
                {
                    //section of player 2
                    "player2", new Dictionary<string, List<Card>>()
                    {
                        {"S", new List<Card>() },
                        {"R", new List<Card>() },
                        {"M", new List<Card>() }
                    }
                },
                {
                    //section of player 1
                    "player1", new Dictionary<string, List<Card>>()
                    {
                        {"M", new List<Card>() },
                        {"R", new List<Card>() },
                        {"S", new List<Card>() }
                    }
                }
            };
        }

        public bool ValidMove(Card card, string range)
        {
            
            foreach (char Range in range)
            {
                if (sections[card.Owner.ID].ContainsKey(range.ToString()))
                {
                    return true;
                }    
            }
            return false;
        }

        public void PlayCard(string range, Card card)
        {
            //range is the place where the player unclick the mouse
            bool is_valid = ValidMove(card, range);

            if (is_valid && card.Owner.Hand.Contains(card))
            {
                //Add card to board
                sections[card.Owner.ID][range].Add(card);
                //remove card from the hand
                card.Owner.Hand.Remove(card);
                //Calculate player score
                int player_score = SumPowerInSection(card);
                card.Owner.Score = player_score;
                Effect.Effects[card.effectType].Invoke(card);
            }
        }

        //Sum all the points in player section
        public int SumPowerInSection(Card card)
        {
            int sum = 0;
            foreach (var section in sections[card.Owner.ID])
            {
                foreach(Card.UnityCard UnityCard in section.Value)
                {
                    int cardpower = UnityCard.Power;
                    sum += cardpower;
                }
            }
            return sum;
        }
    }
}

