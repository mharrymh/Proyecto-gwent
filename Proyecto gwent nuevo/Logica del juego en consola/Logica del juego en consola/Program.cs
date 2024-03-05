using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logica_del_juego_en_consola
{
    internal class Program
    {       
        static void Main(string[] args)
        {
            Player player1 = new Player(CardFaction.Light, "player1");
            Player player2 = new Player(CardFaction.Dark, "player2");


            for (int i = 0; i < player1.PlayerDeck.Count; i++)
            {
                Console.WriteLine(player1.PlayerDeck[i].Name);
            }
            Console.WriteLine();
            for (int i = 0; i < player1.Hand.Count; i++)
            {
                Console.WriteLine(player1.Hand[i].Name);
            }

            Board board = Board.Instance;
            board.PlayCard("M", player2.Hand[1]);    
        }

    }
}
