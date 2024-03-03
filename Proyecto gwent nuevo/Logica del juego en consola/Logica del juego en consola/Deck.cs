using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using UnityEngine.SocialPlatforms;

namespace Assets.Scripts
{
    public class Deck
    {
        //Create two factions
        public List<Card> LightDeck { get; set; }
        public List<Card> DarkDeck { get; set; }


        //Constructor
        public Deck()
        {
            LightDeck = new List<Card>();
            DarkDeck = new List<Card>();
            CreateLigthDeck();
            CreateDarkDeck();
        }



        //Crear las cartas en una clase database
        //Light faction cards
        public void CreateLigthDeck()
        {
            //Carta lider
            LightDeck.Add(new Card(1, "Zeus", null, null, CardType.Leader, CardFaction.Light));

            //Cartas oro
            LightDeck.Add(new Card(2, "Dragon Blanco", 6, "MRS", CardType.Gold, CardFaction.Light));
            LightDeck.Add(new Card(3, "Pegaso", 5, "S", CardType.Gold, CardFaction.Light));
            LightDeck.Add(new Card(4, "Kitsune", 6, "M", CardType.Gold, CardFaction.Light));

            //Cartas plata
            LightDeck.Add(new Card(5, "Ra", 4, "SR", CardType.Silver, CardFaction.Light));
            LightDeck.Add(new Card(6, "Fénix", 4, "SR", CardType.Silver, CardFaction.Light));
            LightDeck.Add(new Card(7, "Sirena", 3, "M", CardType.Silver, CardFaction.Light));
            LightDeck.Add(new Card(8, "Centauro", 3, "MR", CardType.Silver, CardFaction.Light));
            LightDeck.Add(new Card(9, "Yeti", 3, "R", CardType.Silver, CardFaction.Light));
            LightDeck.Add(new Card(10, "Unicornio", 3, "R", CardType.Silver, CardFaction.Light));
            LightDeck.Add(new Card(11, "Salamandra", 3, "M", CardType.Silver, CardFaction.Light));
            LightDeck.Add(new Card(12, "Manticora", 4, "SR", CardType.Silver, CardFaction.Light));
            LightDeck.Add(new Card(13, "Quimera", 3, "MS", CardType.Silver, CardFaction.Light));
            LightDeck.Add(new Card(14, "Sátiro", 3, "R", CardType.Silver, CardFaction.Light));
            LightDeck.Add(new Card(15, "Leprechaun", 3, "M", CardType.Silver, CardFaction.Light));
            LightDeck.Add(new Card(16, "Banshee", 3, "SR", CardType.Silver, CardFaction.Light));

            //Cartas clima
            LightDeck.Add(new Card(17, "Notos", null, "MS", CardType.Climate, CardFaction.Light));
            LightDeck.Add(new Card(17, "Notos", null, "MS", CardType.Climate, CardFaction.Light));
            LightDeck.Add(new Card(18, "Zéfiro", null, "R", CardType.Climate, CardFaction.Light));
            LightDeck.Add(new Card(18, "Zéfiro", null, "R", CardType.Climate, CardFaction.Light));
            LightDeck.Add(new Card(19, "Sol Radiante", null, "", CardType.Climate, CardFaction.Light));
            LightDeck.Add(new Card(19, "Sol Radiante", null, "", CardType.Climate, CardFaction.Light));

            //Cartas señuelo
            LightDeck.Add(new Card(20, "Espejismo de apolo", null, null, CardType.Decoy, CardFaction.Light));
            LightDeck.Add(new Card(20, "Espejismo de apolo", null, null, CardType.Decoy, CardFaction.Light));
            LightDeck.Add(new Card(21, "Reflejo de Atenea", null, null, CardType.Decoy, CardFaction.Light));
            LightDeck.Add(new Card(21, "Reflejo de Atenea", null, null, CardType.Decoy, CardFaction.Light));
        }
        //Dark faction cards
        public void CreateDarkDeck()
        {
            //Carta lider
            DarkDeck.Add(new Card(31, "Hades", null, "", CardType.Leader, CardFaction.Dark));

            //Cartas oro
            DarkDeck.Add(new Card(32, "Dragon Negro", 6, "SR", CardType.Gold, CardFaction.Dark));
            DarkDeck.Add(new Card(33, "Gárgola", 5, "S", CardType.Gold, CardFaction.Dark));
            DarkDeck.Add(new Card(34, "Cerbero", 6, "M", CardType.Gold, CardFaction.Dark));

            //Cartas plata
            DarkDeck.Add(new Card(35, "Medusa", 4, "SM", CardType.Silver, CardFaction.Dark));
            DarkDeck.Add(new Card(36, "Kraken", 4, "R", CardType.Silver, CardFaction.Dark));
            DarkDeck.Add(new Card(37, "Harpía", 3, "MS", CardType.Silver, CardFaction.Dark));
            DarkDeck.Add(new Card(38, "Grifo", 3, "MS", CardType.Silver, CardFaction.Dark));
            DarkDeck.Add(new Card(39, "Ciclope", 3, "SR", CardType.Silver, CardFaction.Dark));
            DarkDeck.Add(new Card(40, "Hidra", 2, "RS", CardType.Silver, CardFaction.Dark));
            DarkDeck.Add(new Card(41, "Súcubo", 3, "S", CardType.Silver, CardFaction.Dark));
            DarkDeck.Add(new Card(42, "Espectro", 3, "SR", CardType.Silver, CardFaction.Dark));
            DarkDeck.Add(new Card(43, "Basilisco", 3, "MS", CardType.Silver, CardFaction.Dark));
            DarkDeck.Add(new Card(44, "Górgona", 3, "RS", CardType.Silver, CardFaction.Dark));
            DarkDeck.Add(new Card(45, "Golem", 3, "MR", CardType.Silver, CardFaction.Dark));
            DarkDeck.Add(new Card(46, "Minotauro", 3, "S", CardType.Silver, CardFaction.Dark));

            //Cartas clima
            DarkDeck.Add(new Card(47, "Niebla de olvido", null, "MS", CardType.Climate, CardFaction.Dark));
            DarkDeck.Add(new Card(47, "Niebla de olvido", null, "MS", CardType.Climate, CardFaction.Dark));
            DarkDeck.Add(new Card(48, "Tormenta de arena", null, "R", CardType.Climate, CardFaction.Dark));
            DarkDeck.Add(new Card(48, "Tormenta de arena", null, "R", CardType.Climate, CardFaction.Dark));
            DarkDeck.Add(new Card(49, "Claridad de luna", null, "", CardType.Climate, CardFaction.Dark));
            DarkDeck.Add(new Card(49, "Claridad de luna", null, "", CardType.Climate, CardFaction.Dark));

            //Cartas señuelo
            DarkDeck.Add(new Card(50, "Eco de érebo", null, "", CardType.Decoy, CardFaction.Dark));
            DarkDeck.Add(new Card(50, "Eco de érebo", null, "", CardType.Decoy, CardFaction.Dark));
            DarkDeck.Add(new Card(51, "Ilusion de Nyx", null, "", CardType.Decoy, CardFaction.Dark));
            DarkDeck.Add(new Card(51, "Ilusion de Nyx", null, "", CardType.Decoy, CardFaction.Dark));
        }

        //Shuffle deck method
        public static List<Card> Shuffle(List<Card> PlayerDeck)
        {
            //Quit leader card because it has to be always in the hand
            Card card_leader = PlayerDeck.Find(Card => Card.Type == CardType.Leader);
            PlayerDeck.Remove(card_leader);

            Random rand = new Random();
            return PlayerDeck = PlayerDeck.OrderBy(x => rand.Next()).ToList();
        }
    }
}
