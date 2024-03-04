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

        public bool ValidSection(Player player, string ranges)
        {
            foreach (char range in ranges)
            {
                if (sections[player.ID].ContainsKey(range.ToString()))
                {
                    return true;
                }    
            }
            return false;
        }

        public void AddCard(string range /*donde el jugador suelte el mouse*/, Card card)
        {
            //range = "donde el jugador suelte el mouse"
            bool is_valid = ValidSection(card.Owner, range);
            if (is_valid && card.Owner.Hand.Contains(card))
            {
                //Add card to board
                sections[card.Owner.ID][range].Add(card);
                //remove card from the hand
                card.Owner.Hand.Remove(card);
                //play card
                gamecontroller.Play(card);
                //Calculate player score
                int player_score = SumPowerSection(card.Owner);
                card.Owner.Score = player_score;
                Effect.Effects[card.effectType].Invoke(card);
            }
        }

        //Sum all the points in player section
        //**
        public int SumPowerSection(Player player)
        {
            int sum = 0;
            foreach (var section in sections[player.ID])
            {
                foreach(Card card in section.Value)
                {
                    int cardpower = card.Power ?? 0;
                    sum += cardpower;
                }
            }
            return sum;
        }


        public void PrintBoard()
        {
            foreach (var player in sections)
            {
                Console.WriteLine($"Jugador: {player.Key}");
                foreach (var section in player.Value)
                {
                    Console.WriteLine($"  Sección: {section.Key}");
                    foreach (var card in section.Value)
                    {
                        Console.WriteLine($"    Carta: {card.Name}, Poder: {card.Power}");
                    }
                }
            }
        }
    }
}

