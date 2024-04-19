using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDatabase
{
    //Create two factions
    public static List<Card> LightDeck { get; set; }
    public static List<Card> DarkDeck { get; set; }

    //Constructor
    public CardDatabase()
    {
        LightDeck = new List<Card>();
        DarkDeck = new List<Card>(); 
    }

    //Light faction cards
    private void CreateLigthDeck()
    {
        //Leader card
        LightDeck.Add(new Card.LeaderCard("Zeus", CardFaction.Light, EffectType.KeepRandomCard, Resources.Load<Sprite>("0")));
        //Mantener una carta aleatoria en el campo

        //Gold cards
        LightDeck.Add(new Card.UnityCard("Dragon Blanco", CardFaction.Light, EffectType.CleanFile, "RS", UnityType.Gold, 5, Resources.Load<Sprite>("1")));
        LightDeck.Add(new Card.UnityCard("Pegaso", CardFaction.Light, EffectType.DeleteMostPowerCard, "S", UnityType.Gold, 6, Resources.Load<Sprite>("2")));
        LightDeck.Add(new Card.UnityCard("Kitsune", CardFaction.Light, EffectType.TakeCardFromDeck, "M", UnityType.Gold, 6, Resources.Load<Sprite>("3")));

        //Silver cards
        LightDeck.Add(new Card.UnityCard("Ra", CardFaction.Light, EffectType.AddClimateCard, "RS", UnityType.Silver, 4, Resources.Load<Sprite>("4")));
        LightDeck.Add(new Card.UnityCard("Fénix", CardFaction.Light, EffectType.AssignProm, "RS", UnityType.Silver, 4, Resources.Load<Sprite>("5")));
        LightDeck.Add(new Card.UnityCard("Sirena", CardFaction.Light, EffectType.DeleteLessPowerCard, "M", UnityType.Silver, 3, Resources.Load<Sprite>("6")));
        LightDeck.Add(new Card.UnityCard("Sirena", CardFaction.Light, EffectType.TakeCardFromGraveYard, "M", UnityType.Silver, 3, Resources.Load<Sprite>("6")));
        LightDeck.Add(new Card.UnityCard("Centauro", CardFaction.Light, EffectType.TakeCardFromGraveYard, "MR", UnityType.Silver, 3, Resources.Load<Sprite>("7")));
        LightDeck.Add(new Card.UnityCard("Centauro", CardFaction.Light, EffectType.TakeCardFromGraveYard, "MR", UnityType.Silver, 3, Resources.Load<Sprite>("7")));
        LightDeck.Add(new Card.UnityCard("Yeti", CardFaction.Light, EffectType.DeleteMostPowerCard, "R", UnityType.Silver, 3, Resources.Load<Sprite>("8")));
        LightDeck.Add(new Card.UnityCard("Unicornio", CardFaction.Light, EffectType.AddClimateCard, "R", UnityType.Silver, 4, Resources.Load<Sprite>("9")));
        LightDeck.Add(new Card.UnityCard("Salamandra", CardFaction.Light, EffectType.AddClimateCard, "M", UnityType.Silver, 4, Resources.Load<Sprite>("10")));
        LightDeck.Add(new Card.UnityCard("Manticora", CardFaction.Light, EffectType.TimesTwins, "RS", UnityType.Silver, 4, Resources.Load<Sprite>("11")));
        LightDeck.Add(new Card.UnityCard("Manticora", CardFaction.Light, EffectType.TimesTwins, "RS", UnityType.Silver, 4, Resources.Load<Sprite>("11")));
        LightDeck.Add(new Card.UnityCard("Manticora", CardFaction.Light, EffectType.TimesTwins, "RS", UnityType.Silver, 4, Resources.Load<Sprite>("11")));
        LightDeck.Add(new Card.UnityCard("Quimera", CardFaction.Light, EffectType.TimesTwins, "MRS", UnityType.Silver, 3, Resources.Load<Sprite>("12")));
        LightDeck.Add(new Card.UnityCard("Quimera", CardFaction.Light, EffectType.TimesTwins, "MRS", UnityType.Silver, 3, Resources.Load<Sprite>("12")));
        LightDeck.Add(new Card.UnityCard("Sátiro", CardFaction.Light, EffectType.AddClimateCard, "R", UnityType.Silver, 3, Resources.Load<Sprite>("13")));
        LightDeck.Add(new Card.UnityCard("Leprechaun", CardFaction.Light, EffectType.None, "M", UnityType.Silver, 4, Resources.Load<Sprite>("14")));
        LightDeck.Add(new Card.UnityCard("Banshee", CardFaction.Light, EffectType.AssignProm, "MRS", UnityType.Silver, 3, Resources.Load<Sprite>("15")));
        LightDeck.Add(new Card.UnityCard("Banshee", CardFaction.Light, EffectType.AssignProm, "MRS", UnityType.Silver, 3, Resources.Load<Sprite>("15")));

        //Climate and clearance cards
        LightDeck.Add(new Card.SpecialCard("Notos", CardFaction.Light, EffectType.Climate, SpecialType.Climate, "M", Resources.Load<Sprite>("16")));
        LightDeck.Add(new Card.SpecialCard("Tormenta de medianoche", CardFaction.Light, EffectType.Climate, SpecialType.Climate, "R", Resources.Load<Sprite>("18")));
        LightDeck.Add(new Card.SpecialCard("Zéfiro", CardFaction.Light, EffectType.Climate, SpecialType.Climate, "S", Resources.Load<Sprite>("20")));
        LightDeck.Add(new Card.SpecialCard("Sol Radiante", CardFaction.Light, EffectType.Clearance, SpecialType.Clearance, "", Resources.Load<Sprite>("22")));

        //Decoy cards
        LightDeck.Add(new Card.SpecialCard("Espejismo de apolo", CardFaction.Light, EffectType.Decoy, SpecialType.Decoy, "", Resources.Load<Sprite>("24")));
        LightDeck.Add(new Card.SpecialCard("Reflejo de Atenea", CardFaction.Light, EffectType.Decoy, SpecialType.Decoy, "", Resources.Load<Sprite>("25")));

        //Increment card
        LightDeck.Add(new Card.SpecialCard("Anfisbena", CardFaction.Light, EffectType.IncrementFile, SpecialType.Increment, "M", Resources.Load<Sprite>("26")));
        LightDeck.Add(new Card.SpecialCard("Artemisa", CardFaction.Light, EffectType.IncrementFile, SpecialType.Increment, "R", Resources.Load<Sprite>("27")));
        LightDeck.Add(new Card.SpecialCard("Helios", CardFaction.Light, EffectType.IncrementFile, SpecialType.Increment, "S", Resources.Load<Sprite>("28")));
    }
    //Dark faction cards
    private void CreateDarkDeck()
    {
        //Leader card
        DarkDeck.Add(new Card.LeaderCard("Hades", CardFaction.Dark, EffectType.DrawExtraCard, Resources.Load<Sprite>("29")));
        //Robar una carta extra entre rondas

        //Gold cards
        DarkDeck.Add(new Card.UnityCard("Dragon Negro", CardFaction.Dark, EffectType.DeleteLessPowerCard, "RS", UnityType.Gold, 5, Resources.Load<Sprite>("30")));
        DarkDeck.Add(new Card.UnityCard("Gárgola", CardFaction.Dark, EffectType.AddClimateCard, "R", UnityType.Gold, 6, Resources.Load<Sprite>("31")));
        DarkDeck.Add(new Card.UnityCard("Cerbero", CardFaction.Dark, EffectType.AddClimateCard, "M", UnityType.Gold, 6, Resources.Load<Sprite>("32")));

        //Silver cards
        DarkDeck.Add(new Card.UnityCard("Medusa", CardFaction.Dark, EffectType.AddClimateCard, "RS", UnityType.Silver, 4, Resources.Load<Sprite>("33")));
        DarkDeck.Add(new Card.UnityCard("Medusa", CardFaction.Dark, EffectType.AddClimateCard, "RS", UnityType.Silver, 4, Resources.Load<Sprite>("33")));
        DarkDeck.Add(new Card.UnityCard("Kraken", CardFaction.Dark, EffectType.DeleteMostPowerCard, "RS", UnityType.Silver, 4, Resources.Load<Sprite>("34")));
        DarkDeck.Add(new Card.UnityCard("Kraken", CardFaction.Dark, EffectType.DeleteMostPowerCard, "RS", UnityType.Silver, 4, Resources.Load<Sprite>("34")));
        DarkDeck.Add(new Card.UnityCard("Harpía", CardFaction.Dark, EffectType.CleanFile, "M", UnityType.Silver, 3, Resources.Load<Sprite>("35")));
        DarkDeck.Add(new Card.UnityCard("Grifo", CardFaction.Dark, EffectType.DeleteLessPowerCard, "MS", UnityType.Silver, 3, Resources.Load<Sprite>("36")));
        DarkDeck.Add(new Card.UnityCard("Ciclope", CardFaction.Dark, EffectType.TakeCardFromDeck, "R", UnityType.Silver, 3, Resources.Load<Sprite>("37")));
        DarkDeck.Add(new Card.UnityCard("Hidra", CardFaction.Dark, EffectType.TimesTwins, "R", UnityType.Silver, 4, Resources.Load<Sprite>("38")));
        DarkDeck.Add(new Card.UnityCard("Hidra", CardFaction.Dark, EffectType.TimesTwins, "R", UnityType.Silver, 4, Resources.Load<Sprite>("38")));
        DarkDeck.Add(new Card.UnityCard("Hidra", CardFaction.Dark, EffectType.TimesTwins, "R", UnityType.Silver, 4, Resources.Load<Sprite>("38")));
        DarkDeck.Add(new Card.UnityCard("Leon de Nemea", CardFaction.Dark, EffectType.TakeCardFromDeck, "M", UnityType.Silver, 4, Resources.Load<Sprite>("39")));
        DarkDeck.Add(new Card.UnityCard("Espectro", CardFaction.Dark, EffectType.TakeCardFromDeck, "RS", UnityType.Silver, 4, Resources.Load<Sprite>("40")));
        DarkDeck.Add(new Card.UnityCard("Basilisco", CardFaction.Dark, EffectType.AssignProm, "MRS", UnityType.Silver, 3, Resources.Load<Sprite>("41")));
        DarkDeck.Add(new Card.UnityCard("Górgona", CardFaction.Dark, EffectType.TimesTwins, "R", UnityType.Silver, 3, Resources.Load<Sprite>("42")));
        DarkDeck.Add(new Card.UnityCard("Górgona", CardFaction.Dark, EffectType.TimesTwins, "R", UnityType.Silver, 3, Resources.Load<Sprite>("42")));
        DarkDeck.Add(new Card.UnityCard("Golem", CardFaction.Dark, EffectType.TakeCardFromDeck, "M", UnityType.Silver, 4, Resources.Load<Sprite>("43")));
        DarkDeck.Add(new Card.UnityCard("Minotauro", CardFaction.Dark, EffectType.AssignProm, "MRS", UnityType.Silver, 3, Resources.Load<Sprite>("44")));
        DarkDeck.Add(new Card.UnityCard("Minotauro", CardFaction.Dark, EffectType.AssignProm, "MRS", UnityType.Silver, 3, Resources.Load<Sprite>("44")));

        //Climate and clearance cards
        DarkDeck.Add(new Card.SpecialCard("Niebla de olvido", CardFaction.Dark, EffectType.Climate, SpecialType.Climate, "M", Resources.Load<Sprite>("45")));
        DarkDeck.Add(new Card.SpecialCard("Lluvia de desesperacion", CardFaction.Dark, EffectType.Climate, SpecialType.Climate, "R", Resources.Load<Sprite>("47")));
        DarkDeck.Add(new Card.SpecialCard("Tormenta de arena", CardFaction.Dark, EffectType.Climate, SpecialType.Climate, "S", Resources.Load<Sprite>("49")));
        DarkDeck.Add(new Card.SpecialCard("Claridad de luna", CardFaction.Dark, EffectType.Clearance, SpecialType.Clearance, "", Resources.Load<Sprite>("51")));

        //Decoy cards
        DarkDeck.Add(new Card.SpecialCard("Eco de érebo", CardFaction.Dark, EffectType.Decoy, SpecialType.Decoy, "", Resources.Load<Sprite>("53")));
        DarkDeck.Add(new Card.SpecialCard("Ilusion de Nyx", CardFaction.Dark, EffectType.Decoy, SpecialType.Decoy, "", Resources.Load<Sprite>("54")));

        //Increment cards
        DarkDeck.Add(new Card.SpecialCard("Ares", CardFaction.Dark, EffectType.IncrementFile, SpecialType.Increment, "M", Resources.Load<Sprite>("55")));
        DarkDeck.Add(new Card.SpecialCard("Euríale", CardFaction.Dark, EffectType.IncrementFile, SpecialType.Increment, "R", Resources.Load<Sprite>("56")));
        DarkDeck.Add(new Card.SpecialCard("Tánatos", CardFaction.Dark, EffectType.IncrementFile, SpecialType.Increment, "S", Resources.Load<Sprite>("57")));
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

