using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDatabase
{
    //Create two factions
    public static CardCollection LightDeck { get; set; }
    public static CardCollection DarkDeck { get; set; }

    //Constructor
    public CardDatabase()
    {
        LightDeck = new();
        DarkDeck = new(); 
    }

    /// <summary>
    /// Add cards to the LightDeck list
    /// </summary>
    private void CreateLigthDeck()
    {
        //Leader card
        LightDeck.Add(new Card.LeaderCard("Zeus", CardFaction.Light, new KeepRandomCard(), Resources.Load<Sprite>("0")));

        //Gold cards
        LightDeck.Add(new Card.GoldCard("Dragon Blanco", CardFaction.Light, new AddClimateCard(), "RS", 5, Resources.Load<Sprite>("1")));
        LightDeck.Add(new Card.GoldCard("Pegaso", CardFaction.Light, new AssignProm(), "S",  6, Resources.Load<Sprite>("2")));
        LightDeck.Add(new Card.GoldCard("Kitsune", CardFaction.Light, new AddClimateCard(), "M",  6, Resources.Load<Sprite>("3")));

        //Silver cards
        LightDeck.Add(new Card.SilverCard("Ra", CardFaction.Light, new DeleteLessPowerCard(), "RS", 4, Resources.Load<Sprite>("4")));
        LightDeck.Add(new Card.SilverCard("F�nix", CardFaction.Light, null, "RS", 4, Resources.Load<Sprite>("5")));
        // LightDeck.Add(new Card.SilverCard("Sirena", CardFaction.Light, new DeleteLessPowerCard(), "M", 3, Resources.Load<Sprite>("6")));
        // LightDeck.Add(new Card.SilverCard("Sirena", CardFaction.Light, new TakeCardFromGraveYard(), "M", 3, Resources.Load<Sprite>("6")));
        // LightDeck.Add(new Card.SilverCard("Centauro", CardFaction.Light, new TakeCardFromGraveYard(), "MR", 3, Resources.Load<Sprite>("7")));
        // LightDeck.Add(new Card.SilverCard("Centauro", CardFaction.Light, new TakeCardFromGraveYard(), "MR", 3, Resources.Load<Sprite>("7")));
        // LightDeck.Add(new Card.SilverCard("Yeti", CardFaction.Light, new DeleteMostPowerCard(), "R", 3, Resources.Load<Sprite>("8")));
        // LightDeck.Add(new Card.SilverCard("Unicornio", CardFaction.Light, null, "R", 4, Resources.Load<Sprite>("9")));
        // LightDeck.Add(new Card.SilverCard("Salamandra", CardFaction.Light, new TakeCardFromDeck(), "M", 4, Resources.Load<Sprite>("10")));
        // LightDeck.Add(new Card.SilverCard("Manticora", CardFaction.Light, new TimesTwins(), "RS", 4, Resources.Load<Sprite>("11")));
        // LightDeck.Add(new Card.SilverCard("Manticora", CardFaction.Light, new TimesTwins(), "RS", 4, Resources.Load<Sprite>("11")));
        // LightDeck.Add(new Card.SilverCard("Manticora", CardFaction.Light, new TimesTwins(), "RS", 4, Resources.Load<Sprite>("11")));
        // LightDeck.Add(new Card.SilverCard("Quimera", CardFaction.Light, new TimesTwins(), "MRS", 3, Resources.Load<Sprite>("12")));
        // LightDeck.Add(new Card.SilverCard("Quimera", CardFaction.Light, new TimesTwins(), "MRS", 3, Resources.Load<Sprite>("12")));
        // LightDeck.Add(new Card.SilverCard("S�tiro", CardFaction.Light, new AddClimateCard(), "R", 3, Resources.Load<Sprite>("13")));
        // LightDeck.Add(new Card.SilverCard("Leprechaun", CardFaction.Light, null, "M", 4, Resources.Load<Sprite>("14")));
        // LightDeck.Add(new Card.SilverCard("Banshee", CardFaction.Light, new TakeCardFromGraveYard(), "MRS", 3, Resources.Load<Sprite>("15")));
        // LightDeck.Add(new Card.SilverCard("Banshee", CardFaction.Light, null, "MRS", 3, Resources.Load<Sprite>("15")));

        // //Climate cards
        // LightDeck.Add(new Card.ClimateCard("Notos", CardFaction.Light, new Climate(), Resources.Load<Sprite>("16"), "M"));
        // LightDeck.Add(new Card.ClimateCard("Tormenta de medianoche", CardFaction.Light, new Climate(),  Resources.Load<Sprite>("18"), "R"));
        // LightDeck.Add(new Card.ClimateCard("Z�firo", CardFaction.Light, new Climate(), Resources.Load<Sprite>("20"), "S"));

        // //Cleareance cards
        // LightDeck.Add(new Card.CleareanceCard("Sol Radiante", CardFaction.Light, new Clearance(), Resources.Load<Sprite>("22")));

        // //Decoy cards
        // LightDeck.Add(new Card.DecoyCard("Espejismo de apolo", CardFaction.Light, null, Resources.Load<Sprite>("24")));
        // LightDeck.Add(new Card.DecoyCard("Reflejo de Atenea", CardFaction.Light, null, Resources.Load<Sprite>("25")));

        // //Increment card
        // LightDeck.Add(new Card.IncrementCard("Anfisbena", CardFaction.Light, new IncrementFile(), Resources.Load<Sprite>("26"), "M"));
        // LightDeck.Add(new Card.IncrementCard("Artemisa", CardFaction.Light, new IncrementFile(), Resources.Load<Sprite>("27"), "R"));
        // LightDeck.Add(new Card.IncrementCard("Helios", CardFaction.Light, new IncrementFile(),  Resources.Load<Sprite>("28"), "S"));
    }
    /// <summary>
    /// Add cards to the DarkDeck list
    /// </summary>
    private void CreateDarkDeck()
    {
        //Leader card
        DarkDeck.Add(new Card.LeaderCard("Hades", CardFaction.Dark, new DrawExtraCard(), Resources.Load<Sprite>("29")));

        //Gold cards
        DarkDeck.Add(new Card.GoldCard("Dragon Negro", CardFaction.Dark, new AssignProm(), "RS", 5, Resources.Load<Sprite>("30")));
        DarkDeck.Add(new Card.GoldCard("G�rgola", CardFaction.Dark, new AddClimateCard(), "R", 6, Resources.Load<Sprite>("31")));
        DarkDeck.Add(new Card.GoldCard("Cerbero", CardFaction.Dark, new AddClimateCard(), "M", 6, Resources.Load<Sprite>("32")));

        //Silver cards
        DarkDeck.Add(new Card.SilverCard("Medusa", CardFaction.Dark, new TakeCardFromDeck(), "RS", 4, Resources.Load<Sprite>("33")));
        // DarkDeck.Add(new Card.SilverCard("Kraken", CardFaction.Dark, new DeleteMostPowerCard(), "RS", 4, Resources.Load<Sprite>("34")));
        // DarkDeck.Add(new Card.SilverCard("Medusa", CardFaction.Dark, new DeleteLessPowerCard(), "RS", 4, Resources.Load<Sprite>("33")));
        // DarkDeck.Add(new Card.SilverCard("Kraken", CardFaction.Dark, new DeleteMostPowerCard(), "RS", 4, Resources.Load<Sprite>("34")));
        // DarkDeck.Add(new Card.SilverCard("Harp�a", CardFaction.Dark, new CleanFile(), "M", 3, Resources.Load<Sprite>("35")));
        // DarkDeck.Add(new Card.SilverCard("Grifo", CardFaction.Dark, new DeleteLessPowerCard(), "MS", 3, Resources.Load<Sprite>("36")));
        // DarkDeck.Add(new Card.SilverCard("Ciclope", CardFaction.Dark, new TakeCardFromDeck(), "R", 3, Resources.Load<Sprite>("37")));
        // DarkDeck.Add(new Card.SilverCard("Hidra", CardFaction.Dark, new TimesTwins(), "R", 4, Resources.Load<Sprite>("38")));
        // DarkDeck.Add(new Card.SilverCard("Hidra", CardFaction.Dark, new TimesTwins(), "R", 4, Resources.Load<Sprite>("38")));
        // DarkDeck.Add(new Card.SilverCard("Hidra", CardFaction.Dark, new TimesTwins(), "R", 4, Resources.Load<Sprite>("38")));
        // DarkDeck.Add(new Card.SilverCard("Leon de Nemea", CardFaction.Dark, new TakeCardFromDeck(), "M", 4, Resources.Load<Sprite>("39")));
        // DarkDeck.Add(new Card.SilverCard("Espectro", CardFaction.Dark, new TakeCardFromGraveYard(), "RS", 4, Resources.Load<Sprite>("40")));
        // DarkDeck.Add(new Card.SilverCard("Basilisco", CardFaction.Dark, null, "MRS", 3, Resources.Load<Sprite>("41")));
        // DarkDeck.Add(new Card.SilverCard("G�rgona", CardFaction.Dark, new TimesTwins(), "R", 3, Resources.Load<Sprite>("42")));
        // DarkDeck.Add(new Card.SilverCard("G�rgona", CardFaction.Dark, new TimesTwins(), "R", 3, Resources.Load<Sprite>("42")));
        // DarkDeck.Add(new Card.SilverCard("Golem", CardFaction.Dark, new TakeCardFromDeck(), "M", 4, Resources.Load<Sprite>("43")));
        // DarkDeck.Add(new Card.SilverCard("Minotauro", CardFaction.Dark, null, "MRS", 3, Resources.Load<Sprite>("44")));
        // DarkDeck.Add(new Card.SilverCard("Minotauro", CardFaction.Dark, null, "MRS", 3, Resources.Load<Sprite>("44")));

        // //Climate cards
        // DarkDeck.Add(new Card.ClimateCard("Niebla de olvido", CardFaction.Dark, new Climate(), Resources.Load<Sprite>("45"), "M"));
        // DarkDeck.Add(new Card.ClimateCard("Lluvia de desesperacion", CardFaction.Dark, new Climate(), Resources.Load<Sprite>("47"), "R"));
        // DarkDeck.Add(new Card.ClimateCard("Tormenta de arena", CardFaction.Dark, new Climate(),  Resources.Load<Sprite>("49"), "S"));

        // //Cleareance cards
        // DarkDeck.Add(new Card.CleareanceCard("Claridad de luna", CardFaction.Dark, new Clearance(), Resources.Load<Sprite>("51")));

        // //Decoy cards
        // DarkDeck.Add(new Card.DecoyCard("Eco de �rebo", CardFaction.Dark, null, Resources.Load<Sprite>("53")));
        // DarkDeck.Add(new Card.DecoyCard("Ilusion de Nyx", CardFaction.Dark, null, Resources.Load<Sprite>("54")));

        // //Increment cards
        // DarkDeck.Add(new Card.IncrementCard("Ares", CardFaction.Dark, new IncrementFile(), Resources.Load<Sprite>("55"), "M"));
        // DarkDeck.Add(new Card.IncrementCard("Eur�ale", CardFaction.Dark, new IncrementFile(), Resources.Load<Sprite>("56"), "R"));
        // DarkDeck.Add(new Card.IncrementCard("T�natos", CardFaction.Dark, new IncrementFile(),  Resources.Load<Sprite>("57"), "S"));
    }

    public CardCollection GetLightDeck()
    {
        CreateLigthDeck();
        // Return a copy of the deck to prevent modification
        return LightDeck.Copy();
    }

    public CardCollection GetDarkDeck()
    {
        CreateDarkDeck();
        // Return a copy of the deck to prevent modification
        return DarkDeck.Copy();
    }
}