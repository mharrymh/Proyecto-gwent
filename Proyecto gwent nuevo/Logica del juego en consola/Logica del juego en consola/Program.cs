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
            Deck cartas = new Deck();
            Player player1 = new Player(cartas.DarkDeck, "player1");
            Player player2 = new Player(cartas.LightDeck, "player2");

            player1.PlayerDeck = Deck.Shuffle(player1.PlayerDeck);
            for (int i = 0; i < player1.PlayerDeck.Count; i++)
            {
                Console.WriteLine(player1.PlayerDeck[i].Name);
            }
            Console.WriteLine();
            for (int i = 0; i < player1.Hand.Count; i++)
            {
                Console.WriteLine(player1.Hand[i].Name);
            }

            Board board = new Board(player1.ID, player2.ID);
            board.AddCard(player1, "M", player1.Hand[1]);
            //board.PrintBoard();
            
            
        }

    }
}
