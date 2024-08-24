using System;
using System.Collections.Generic;
using UnityEngine;

public static class CardConverter
{
    public static Dictionary<CardFaction, CardCollection> myCardsPerFactions = new Dictionary<CardFaction, CardCollection>
    {
        { CardFaction.Light, new CardCollection()},
        { CardFaction.Dark, new CardCollection()}
    };
    public static void SaveCards(List<ICard> myCards)
    {
        foreach (ICard card in myCards)
        {
            SaveCard(card);
        }
    }

    static void SaveCard(ICard card)
    {
        //Save the card faction
        CardFaction cardFaction = card.Faction;
        //Create the card
        Card convertedCard = relateType[card.Type](card);
        //Add card to the dictionary to save it 
        myCardsPerFactions[cardFaction].Add(convertedCard);
    }

    public readonly static Dictionary<string, Func<ICard, Card>> relateType = new Dictionary<string, Func<ICard, Card>>
    {
        {"Gold", GoldCard},
        {"Silver", SilverCard},
        {"Leader", LeaderCard},
        {"Increment", IncrementCard},
        {"Decoy", DecoyCard},
        {"Climate", ClimateCard},
        {"Clearance", CleareanceCard},
    };

    private static Card CleareanceCard(ICard card)
    {
        Sprite CardImage = Utils.DefaultCardImage;
        return new Card.CleareanceCard(card.Name, card.Faction, null, CardImage, card.Effects);
    }

    private static Card ClimateCard(ICard card)
    {
        Sprite CardImage = Utils.DefaultCardImage;
        return new Card.ClimateCard(card.Name, card.Faction, null, CardImage, card.Range, card.Effects);
    }

    private static Card DecoyCard(ICard card)
    {
        string name = card.Name;
        CardFaction faction = card.Faction;
        

        //TODO: 
        Sprite CardImage = Utils.DefaultCardImage;

        //Power is 0
        return new Card.DecoyCard(name, faction, null, CardImage, card.Effects);
    }

    private static Card IncrementCard(ICard card)
    {
        Sprite CardImage = Utils.DefaultCardImage;
        return new Card.IncrementCard(card.Name, card.Faction, null, CardImage, card.Range, card.Effects);
    }

    private static Card LeaderCard(ICard card)
    {
        Sprite CardImage = Utils.DefaultCardImage;
        return new Card.LeaderCard(card.Name, card.Faction, null, CardImage, card.Effects);
    }

    private static Card SilverCard(ICard card)
    {
        string name = card.Name;
        CardFaction faction = card.Faction;
        Effect effect = null;
        string range = card.Range;
        int power = card.Power;


        List<DeclaredEffect> cardEffects = card.Effects;
        
        Sprite CardImage = Utils.DefaultCardImage;

        return new Card.SilverCard(name, faction, effect, range, power, CardImage, cardEffects);
    }

    private static Card GoldCard(ICard card)
    {
        string name = card.Name;
        CardFaction faction = card.Faction;
        Effect effect = null;
        string range = card.Range;
        int power = card.Power;


        List<DeclaredEffect> cardEffects = card.Effects;
        


        //TODO: 
        Sprite CardImage = Utils.DefaultCardImage;

        return new Card.GoldCard(name, faction, effect, range, power, CardImage, cardEffects);
    }
} 


