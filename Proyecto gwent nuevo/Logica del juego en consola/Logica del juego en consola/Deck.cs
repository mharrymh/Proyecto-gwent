using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using UnityEngine.SocialPlatforms;

namespace Assets.Scripts
{
    public class CardDatabase
    {
        //Create two factions
        public List<Card> LightDeck { get; set; }
        public List<Card> DarkDeck { get; set; }

        //Constructor
        public CardDatabase()
        {
            LightDeck = new List<Card>();
            DarkDeck = new List<Card>();
        }

        //Light faction cards
        private void CreateLigthDeck()
        {
            //Carta lider
            LightDeck.Add(new Card.LeaderCard("Zeus", CardFaction.Light, EffectType.TieIsWin));

            //Cartas oro
            LightDeck.Add(new Card.UnityCard("Dragon Blanco", CardFaction.Light, EffectType.CleanMeleeFile, "RS", UnityType.Gold, 5));
            LightDeck.Add(new Card.UnityCard("Pegaso", CardFaction.Light, EffectType.TimesTwins, "S", UnityType.Gold, 6));
            LightDeck.Add(new Card.UnityCard("Kitsune", CardFaction.Light, EffectType.None, "M", UnityType.Gold, 6));

            //Cartas plata
            LightDeck.Add(new Card.UnityCard("Ra", CardFaction.Light, EffectType.None, "RS", UnityType.Silver, 4));
            LightDeck.Add(new Card.UnityCard("Fénix",CardFaction.Light, EffectType.None, "RS", UnityType.Silver, 4));
            LightDeck.Add(new Card.UnityCard("Sirena",CardFaction.Light, EffectType.CleanSiegeFile, "M", UnityType.Silver, 3));
            LightDeck.Add(new Card.UnityCard("Centauro", CardFaction.Light, EffectType.TakeCardFromGraveYard, "MR", UnityType.Silver, 3));
            LightDeck.Add(new Card.UnityCard("Yeti", CardFaction.Light, EffectType.DeleteMostPowerCard, "R", UnityType.Silver, 3));
            LightDeck.Add(new Card.UnityCard("Unicornio", CardFaction.Light, EffectType.None, "R", UnityType.Silver, 4));
            LightDeck.Add(new Card.UnityCard("Salamandra", CardFaction.Light, EffectType.None, "M", UnityType.Silver, 4));
            LightDeck.Add(new Card.UnityCard("Manticora", CardFaction.Light, EffectType.None, "RS", UnityType.Silver, 4));
            LightDeck.Add(new Card.UnityCard("Quimera", CardFaction.Light, EffectType.DeleteLessPowerCard, "MRS", UnityType.Silver, 3));
            LightDeck.Add(new Card.UnityCard("Sátiro", CardFaction.Light, EffectType.None, "R", UnityType.Silver, 3));
            LightDeck.Add(new Card.UnityCard("Leprechaun", CardFaction.Light, EffectType.None, "M", UnityType.Silver, 4));
            LightDeck.Add(new Card.UnityCard("Banshee", CardFaction.Light, EffectType.AssignProm, "MRS", UnityType.Silver, 3));

            //Cartas clima
            LightDeck.Add(new Card.SpecialCard("Notos", CardFaction.Light, EffectType.Climate, SpecialType.Climate, "M"));
            LightDeck.Add(new Card.SpecialCard("Notos", CardFaction.Light, EffectType.Climate, SpecialType.Climate, "M"));
            LightDeck.Add(new Card.SpecialCard("Tormenta de medianoche", CardFaction.Light, EffectType.Climate, SpecialType.Climate, "R"));
            LightDeck.Add(new Card.SpecialCard("Tormenta de medianoche", CardFaction.Light, EffectType.Climate, SpecialType.Climate, "R"));
            LightDeck.Add(new Card.SpecialCard("Zéfiro",  CardFaction.Light, EffectType.Climate, SpecialType.Climate, "S"));
            LightDeck.Add(new Card.SpecialCard("Zéfiro",  CardFaction.Light, EffectType.Climate, SpecialType.Climate, "S"));
            LightDeck.Add(new Card.SpecialCard("Sol Radiante", CardFaction.Light, EffectType.Clearance,SpecialType.Clearance,""));
            LightDeck.Add(new Card.SpecialCard("Sol Radiante", CardFaction.Light, EffectType.Clearance,SpecialType.Clearance,""));

            //Cartas señuelo
            LightDeck.Add(new Card.SpecialCard("Espejismo de apolo", CardFaction.Light, EffectType.Decoy, SpecialType.Decoy, ""));
            LightDeck.Add(new Card.SpecialCard("Reflejo de Atenea", CardFaction.Light, EffectType.Decoy, SpecialType.Decoy, ""));
        }
        //Dark faction cards
        private void CreateDarkDeck()
        {
            //Carta lider
            DarkDeck.Add(new Card.LeaderCard("Hades", CardFaction.Dark, EffectType.KeepRandomCard));

            //Cartas oro
            DarkDeck.Add(new Card.UnityCard("Dragon Negro", CardFaction.Dark, EffectType.DeleteLessPowerCard, "RS", UnityType.Gold, 5));
            DarkDeck.Add(new Card.UnityCard("Gárgola", CardFaction.Dark, EffectType.DrawExtraCard, "R", UnityType.Gold, 6));
            DarkDeck.Add(new Card.UnityCard("Cerbero", CardFaction.Dark, EffectType.None, "M", UnityType.Gold, 6));

            //Cartas plata
            DarkDeck.Add(new Card.UnityCard("Medusa",  CardFaction.Dark, EffectType.None, "RS", UnityType.Silver, 4));
            DarkDeck.Add(new Card.UnityCard("Kraken", CardFaction.Dark, EffectType.None,"RS", UnityType.Silver, 4));
            DarkDeck.Add(new Card.UnityCard("Harpía",  CardFaction.Dark, EffectType.IncrementFile, "M", UnityType.Silver, 3));
            DarkDeck.Add(new Card.UnityCard("Grifo",  CardFaction.Dark, EffectType.CleanRangedFile, "MS", UnityType.Silver, 3));
            DarkDeck.Add(new Card.UnityCard("Ciclope",  CardFaction.Dark, EffectType.None, "R", UnityType.Silver, 3));
            DarkDeck.Add(new Card.UnityCard("Hidra",  CardFaction.Dark, EffectType.None, "R", UnityType.Silver, 4));
            DarkDeck.Add(new Card.UnityCard("Súcubo", CardFaction.Dark, EffectType.None, "M", UnityType.Silver, 4));
            DarkDeck.Add(new Card.UnityCard("Espectro",  CardFaction.Dark, EffectType.None, "RS", UnityType.Silver, 4));
            DarkDeck.Add(new Card.UnityCard("Basilisco",  CardFaction.Dark, EffectType.AssignProm, "MRS", UnityType.Silver, 3));
            DarkDeck.Add(new Card.UnityCard("Górgona", CardFaction.Dark, EffectType.TakeCardFromDeck, "R", UnityType.Silver, 3));
            DarkDeck.Add(new Card.UnityCard("Golem",  CardFaction.Dark, EffectType.None, "M", UnityType.Silver, 4));
            DarkDeck.Add(new Card.UnityCard("Minotauro", CardFaction.Dark, EffectType.AssignProm, "MRS", UnityType.Silver, 3));

            //Cartas clima
            DarkDeck.Add(new Card.SpecialCard("Niebla de olvido", CardFaction.Dark, EffectType.Climate,SpecialType.Climate, "M"));
            DarkDeck.Add(new Card.SpecialCard("Niebla de olvido", CardFaction.Dark, EffectType.Climate,SpecialType.Climate, "M"));
            DarkDeck.Add(new Card.SpecialCard("Lluvia de desesperacion", CardFaction.Dark, EffectType.Climate,SpecialType.Climate, "R"));
            DarkDeck.Add(new Card.SpecialCard("Lluvia de desesperacion", CardFaction.Dark, EffectType.Climate,SpecialType.Climate, "R"));
            DarkDeck.Add(new Card.SpecialCard("Tormenta de arena",CardFaction.Dark, EffectType.Climate,SpecialType.Climate, "S"));
            DarkDeck.Add(new Card.SpecialCard("Tormenta de arena",CardFaction.Dark, EffectType.Climate,SpecialType.Climate, "S"));
            DarkDeck.Add(new Card.SpecialCard("Claridad de luna",CardFaction.Dark, EffectType.Clearance,SpecialType.Clearance, ""));
            DarkDeck.Add(new Card.SpecialCard("Claridad de luna", CardFaction.Dark, EffectType.Clearance,SpecialType.Clearance, ""));

            //Cartas señuelo
            DarkDeck.Add(new Card.SpecialCard("Eco de érebo", CardFaction.Dark, EffectType.Decoy,SpecialType.Decoy, ""));
            DarkDeck.Add(new Card.SpecialCard("Ilusion de Nyx",CardFaction.Dark, EffectType.Decoy,SpecialType.Decoy, ""));
        }

        public List<Card> GetLightDeck()
        {
            CreateLigthDeck();
            // Return a copy of the deck to prevent modification
            return new List<Card>(LightDeck);
        }

        public List<Card> GetDarkDeck()
        {
            CreateDarkDeck();
            // Return a copy of the deck to prevent modification
            return new List<Card>(DarkDeck);
        }
    }
}
