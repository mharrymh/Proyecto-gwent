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
        LightDeck.Add(new Card.GoldCard("White Dragon", CardFaction.Light, new AddClimateCard(), "RS", 5, Resources.Load<Sprite>("1")));
        LightDeck.Add(new Card.GoldCard("Pegaso", CardFaction.Light, new AssignProm(), "S",  6, Resources.Load<Sprite>("2")));
        LightDeck.Add(new Card.GoldCard("Kitsune", CardFaction.Light, new AddClimateCard(), "M",  6, Resources.Load<Sprite>("3")));

        //Silver cards
        LightDeck.Add(new Card.SilverCard("Ra", CardFaction.Light, new DeleteLessPowerCard(), "RS", 4, Resources.Load<Sprite>("4")));
        LightDeck.Add(new Card.SilverCard("Fenix", CardFaction.Light, null, "RS", 4, Resources.Load<Sprite>("5")));
        LightDeck.Add(new Card.SilverCard("Siren", CardFaction.Light, new DeleteLessPowerCard(), "M", 3, Resources.Load<Sprite>("6")));
        LightDeck.Add(new Card.SilverCard("Siren", CardFaction.Light, new TakeCardFromGraveYard(), "M", 3, Resources.Load<Sprite>("6")));
        LightDeck.Add(new Card.SilverCard("Centaur", CardFaction.Light, new TakeCardFromGraveYard(), "MR", 3, Resources.Load<Sprite>("7")));
        LightDeck.Add(new Card.SilverCard("Centaur", CardFaction.Light, new TakeCardFromGraveYard(), "MR", 3, Resources.Load<Sprite>("7")));
        LightDeck.Add(new Card.SilverCard("Yeti", CardFaction.Light, new DeleteMostPowerCard(), "R", 3, Resources.Load<Sprite>("8")));
        LightDeck.Add(new Card.SilverCard("Unicorn", CardFaction.Light, null, "R", 4, Resources.Load<Sprite>("9")));
        LightDeck.Add(new Card.SilverCard("Salamander", CardFaction.Light, new TakeCardFromDeck(), "M", 4, Resources.Load<Sprite>("10")));
        LightDeck.Add(new Card.SilverCard("Manticor", CardFaction.Light, new TimesTwins(), "RS", 4, Resources.Load<Sprite>("11")));
        LightDeck.Add(new Card.SilverCard("Manticor", CardFaction.Light, new TimesTwins(), "RS", 4, Resources.Load<Sprite>("11")));
        LightDeck.Add(new Card.SilverCard("Manticor", CardFaction.Light, new TimesTwins(), "RS", 4, Resources.Load<Sprite>("11")));
        LightDeck.Add(new Card.SilverCard("Chimera", CardFaction.Light, new TimesTwins(), "MRS", 3, Resources.Load<Sprite>("12")));
        LightDeck.Add(new Card.SilverCard("Chimera", CardFaction.Light, new TimesTwins(), "MRS", 3, Resources.Load<Sprite>("12")));
        LightDeck.Add(new Card.SilverCard("Satyr", CardFaction.Light, new AddClimateCard(), "R", 3, Resources.Load<Sprite>("13")));
        LightDeck.Add(new Card.SilverCard("Leprechaun", CardFaction.Light, null, "M", 4, Resources.Load<Sprite>("14")));
        LightDeck.Add(new Card.SilverCard("Banshee", CardFaction.Light, new TakeCardFromGraveYard(), "MRS", 3, Resources.Load<Sprite>("15")));
        LightDeck.Add(new Card.SilverCard("Banshee", CardFaction.Light, null, "MRS", 3, Resources.Load<Sprite>("15")));

        //Climate cards
        LightDeck.Add(new Card.ClimateCard("Notos", CardFaction.Light, new Climate(), Resources.Load<Sprite>("16"), "M"));
        LightDeck.Add(new Card.ClimateCard("Midnight storm", CardFaction.Light, new Climate(),  Resources.Load<Sprite>("18"), "R"));
        LightDeck.Add(new Card.ClimateCard("Sapphire", CardFaction.Light, new Climate(), Resources.Load<Sprite>("20"), "S"));

        //Cleareance cards
        LightDeck.Add(new Card.CleareanceCard("Radiant sun", CardFaction.Light, new Clearance(), Resources.Load<Sprite>("22")));

        //Decoy cards
        LightDeck.Add(new Card.DecoyCard("Apollo mirage", CardFaction.Light, null, Resources.Load<Sprite>("24")));
        LightDeck.Add(new Card.DecoyCard("Atenea reflex", CardFaction.Light, null, Resources.Load<Sprite>("25")));

        //Increment card
        LightDeck.Add(new Card.IncrementCard("Amphisbaena", CardFaction.Light, new IncrementFile(), Resources.Load<Sprite>("26"), "M"));
        LightDeck.Add(new Card.IncrementCard("Artemisa", CardFaction.Light, new IncrementFile(), Resources.Load<Sprite>("27"), "R"));
        LightDeck.Add(new Card.IncrementCard("Helios", CardFaction.Light, new IncrementFile(),  Resources.Load<Sprite>("28"), "S"));
    }
    /// <summary>
    /// Add cards to the DarkDeck list
    /// </summary>
    private void CreateDarkDeck()
    {
        //Leader card
        DarkDeck.Add(new Card.LeaderCard("Hades", CardFaction.Dark, new DrawExtraCard(), Resources.Load<Sprite>("29")));

        //Gold cards
        DarkDeck.Add(new Card.GoldCard("Black dragon", CardFaction.Dark, new AssignProm(), "RS", 5, Resources.Load<Sprite>("30")));
        DarkDeck.Add(new Card.GoldCard("Gargoyle", CardFaction.Dark, new AddClimateCard(), "R", 6, Resources.Load<Sprite>("31")));
        DarkDeck.Add(new Card.GoldCard("Cerberus", CardFaction.Dark, new AddClimateCard(), "M", 6, Resources.Load<Sprite>("32")));

        //Silver cards
        DarkDeck.Add(new Card.SilverCard("Jellyfish", CardFaction.Dark, new TakeCardFromDeck(), "RS", 4, Resources.Load<Sprite>("33")));
        DarkDeck.Add(new Card.SilverCard("Kraken", CardFaction.Dark, new DeleteMostPowerCard(), "RS", 4, Resources.Load<Sprite>("34")));
        DarkDeck.Add(new Card.SilverCard("Jellyfish", CardFaction.Dark, new DeleteLessPowerCard(), "RS", 4, Resources.Load<Sprite>("33")));
        DarkDeck.Add(new Card.SilverCard("Kraken", CardFaction.Dark, new DeleteMostPowerCard(), "RS", 4, Resources.Load<Sprite>("34")));
        DarkDeck.Add(new Card.SilverCard("Harpya", CardFaction.Dark, new CleanFile(), "M", 3, Resources.Load<Sprite>("35")));
        DarkDeck.Add(new Card.SilverCard("Griffin", CardFaction.Dark, new DeleteLessPowerCard(), "MS", 3, Resources.Load<Sprite>("36")));
        DarkDeck.Add(new Card.SilverCard("Cyclop", CardFaction.Dark, new TakeCardFromDeck(), "R", 3, Resources.Load<Sprite>("37")));
        DarkDeck.Add(new Card.SilverCard("Hydra", CardFaction.Dark, new TimesTwins(), "R", 4, Resources.Load<Sprite>("38")));
        DarkDeck.Add(new Card.SilverCard("Hydra", CardFaction.Dark, new TimesTwins(), "R", 4, Resources.Load<Sprite>("38")));
        DarkDeck.Add(new Card.SilverCard("Hydra", CardFaction.Dark, new TimesTwins(), "R", 4, Resources.Load<Sprite>("38")));
        DarkDeck.Add(new Card.SilverCard("Nemea lion", CardFaction.Dark, new TakeCardFromDeck(), "M", 4, Resources.Load<Sprite>("39")));
        DarkDeck.Add(new Card.SilverCard("Espectrus", CardFaction.Dark, new TakeCardFromGraveYard(), "RS", 4, Resources.Load<Sprite>("40")));
        DarkDeck.Add(new Card.SilverCard("Basilisk", CardFaction.Dark, null, "MRS", 3, Resources.Load<Sprite>("41")));
        DarkDeck.Add(new Card.SilverCard("Gorgon", CardFaction.Dark, new TimesTwins(), "R", 3, Resources.Load<Sprite>("42")));
        DarkDeck.Add(new Card.SilverCard("Gorgon", CardFaction.Dark, new TimesTwins(), "R", 3, Resources.Load<Sprite>("42")));
        DarkDeck.Add(new Card.SilverCard("Golem", CardFaction.Dark, new TakeCardFromDeck(), "M", 4, Resources.Load<Sprite>("43")));
        DarkDeck.Add(new Card.SilverCard("Minotaur", CardFaction.Dark, null, "MRS", 3, Resources.Load<Sprite>("44")));
        DarkDeck.Add(new Card.SilverCard("Minotaur", CardFaction.Dark, null, "MRS", 3, Resources.Load<Sprite>("44")));

        //Climate cards
        DarkDeck.Add(new Card.ClimateCard("Fog of oblivion", CardFaction.Dark, new Climate(), Resources.Load<Sprite>("45"), "M"));
        DarkDeck.Add(new Card.ClimateCard("Rain of despair", CardFaction.Dark, new Climate(), Resources.Load<Sprite>("47"), "R"));
        DarkDeck.Add(new Card.ClimateCard("Sand storm", CardFaction.Dark, new Climate(),  Resources.Load<Sprite>("49"), "S"));

        //Cleareance cards
        DarkDeck.Add(new Card.CleareanceCard("Moon clarity", CardFaction.Dark, new Clearance(), Resources.Load<Sprite>("51")));

        //Decoy cards
        DarkDeck.Add(new Card.DecoyCard("Erebo echo", CardFaction.Dark, null, Resources.Load<Sprite>("53")));
        DarkDeck.Add(new Card.DecoyCard("Nyx ilusion", CardFaction.Dark, null, Resources.Load<Sprite>("54")));

        //Increment cards
        DarkDeck.Add(new Card.IncrementCard("Ares", CardFaction.Dark, new IncrementFile(), Resources.Load<Sprite>("55"), "M"));
        DarkDeck.Add(new Card.IncrementCard("Euriale", CardFaction.Dark, new IncrementFile(), Resources.Load<Sprite>("56"), "R"));
        DarkDeck.Add(new Card.IncrementCard("Tanatos", CardFaction.Dark, new IncrementFile(),  Resources.Load<Sprite>("57"), "S"));
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