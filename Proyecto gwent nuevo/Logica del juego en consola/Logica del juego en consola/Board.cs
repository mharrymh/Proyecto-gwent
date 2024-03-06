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
        public Dictionary<string, Card.SpecialCard[]> climate_section;
        

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

            climate_section = new Dictionary<string, Card.SpecialCard[]>()
            {
                {
                    "player2", new Card.SpecialCard[3]
                },
                {
                    "player 1", new Card.SpecialCard[3]
                }
            };
        }

        public bool ValidMove(Card card, string range)
        {
            
            foreach (char Range in range)
            {
                if (card is Card.UnityCard unity_card && sections[card.Owner.ID].ContainsKey(range.ToString()))
                {
                    return true;
                }    
                else if (card is Card.SpecialCard special_card && special_card.Type == SpecialType.Decoy)
                {
                    return true;
                }
                else if (card is Card.SpecialCard climate_card && climate_card.Type == SpecialType.Climate)
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
                //if (card is Card.SpecialCard climate_card && climate_card.Type == SpecialType.Climate)
                //{
                //    var PlayerSection = card.Owner.board.climate_section[card.Owner.ID];
                //    if (climate_card.Range = "M")

                //}
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

