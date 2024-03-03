using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using static UnityEditor.Experimental.GraphView.GraphView;

namespace Assets.Scripts
{
    public class Board
    {
        GameController gamecontroller = new GameController();
        //The board with its sections
        public Dictionary<string, Dictionary<string, List<Card>>> sections;
        //Climate Section
        
        

        public Board(string player1, string player2)
        {
            sections = new Dictionary<string, Dictionary<string, List<Card>>>()
            {
                {
                    //section of player 2
                    player2, new Dictionary<string, List<Card>>()
                    {
                        {"S", new List<Card>() },
                        {"R", new List<Card>() },
                        {"M", new List<Card>() }
                    }
                },
                {
                    //section of player 1
                    player1, new Dictionary<string, List<Card>>()
                    {
                        {"M", new List<Card>() },
                        {"R", new List<Card>() },
                        {"S", new List<Card>() }
                    }
                }
            };
        }

        public bool ValidSection(string player, string ranges)
        {
            foreach (char range in ranges)
            {
                if (sections[player].ContainsKey(range.ToString()))
                {
                    return true;
                }    
            }
            return false;
        }

        public void AddCard(Player player, string range /*donde el jugador suelte el mouse*/, Card card)
        {
            //range = "donde el jugador suelte el mouse"
            bool is_valid = ValidSection(player.ID, range);
            if (is_valid && player.Hand.Contains(card))
            {
                sections[player.ID][range].Add(card);
                player.Hand.Remove(card);
                gamecontroller.Play(card); 
            }
        }

        //public void PrintBoard()
        //{
        //    foreach (var player in sections)
        //    {
        //        Console.WriteLine($"Jugador: {player.Key}");
        //        foreach (var section in player.Value)
        //        {
        //            Console.WriteLine($"  Sección: {section.Key}");
        //            foreach (var card in section.Value)
        //            {
        //                Console.WriteLine($"    Carta: {card.Name}, Poder: {card.Power}");
        //            }
        //        }
        //    }
        //}
    }
}

