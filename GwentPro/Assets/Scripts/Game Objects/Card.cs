using System.Collections.Generic;
using UnityEngine;
#nullable enable
[System.Serializable] 

public enum CardFaction
{
    Dark,
    Light
}


/// <summary>
/// Represent a card in its most generic form
/// </summary>
public class Card : ScriptableObject
{
    /// <summary>
    /// Relate each effect type with a strings that represents the description of the card
    /// </summary>
    /// <value></value>
    public readonly Dictionary<string, string> effectDescriptions = new Dictionary<string, string>
    {
        { "DeleteMostPowerCard", "Delete the silver card with most power in the board" },
        { "IncrementFile", "Affects only silver cards of this file, increase its power by one" },
        { "DeleteLessPowerCard", "Delete the less powerful silver card of enemy field" },
        { "TakeCardFromDeck", "Draw a card from deck" },
        { "TakeCardFromGraveYard", "Draw most powerful card from graveyard" },
        { "AssignProm", "Assign to all cards on board the promedy of power on board" },
        { "TimesTwins", "Multiply its damage by all the cards with its same name on board" },
        { "CleanFile", "Clear all cards from the file with less cards on" },
        { "Climate", "Affects only silver cards of this range" },
        { "Clearance", "Clear all climate cards from board" },
        { "Decoy", "Drop it in a unity card to return it to the hand" },
        { "AddClimateCard", "Add a climate card" },
        { "DrawExtraCard", "This leader allows you to draw an extra card between rounds" },
        { "KeepRandomCard", "This leader allows you to keep a unity card between rounds" },
    };

    public Sprite CardImage {get; }
    public string? Description { get; private set;}
    public string Name { get;}
    public CardFaction CardFaction { get;}
    public string Faction {
        get {
            Debug.Log(CardFaction.ToString());
            return CardFaction.ToString();
        }
    }
    /// <summary>
    /// Card effect
    /// </summary>
    /// <value></value>
    public Effect? EffectType { get;}
    /// <summary>
    /// Owner of the card
    /// </summary>
    /// <value></value>
    public Player Owner { get; set; }
    /// <summary>
    /// Check if the card has been played in the game
    /// </summary>
    /// <value></value>
    public bool IsPlayed { get; set; }
    /// <summary>
    /// Represent the card as a GameObject in Unity
    /// </summary>
    /// <value></value>
    public GameObject CardPrefab { get; set; }
    /// <summary>
    /// The effects for the User cards
    /// </summary>
    /// <value></value>
    public List<DeclaredEffect>? UserCardEffects {get;}

    public string Type {get; private set;}

    
    /// <summary>
    /// Represents a card in its basic form 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="cardFaction"></param>
    /// <param name="effectType"></param>
    /// <param name="CardImage"></param>
    public Card(string name, CardFaction cardFaction, Effect effectType, Sprite CardImage, List<DeclaredEffect>? userCardEffects = null)
    {
        Name = name;
        CardFaction = cardFaction;
        this.EffectType = effectType;
        this.CardImage = CardImage;

        //Set description of the card
        if (effectType != null && effectDescriptions.ContainsKey(effectType.ToString())) {
            Description = effectDescriptions[effectType.ToString()];
        }
        else if (userCardEffects != null)
        {
            //Declare it null so it will be modified in the child constructor
            Description = null;
        }
        else Description = "No effect";

        this.UserCardEffects = userCardEffects;
    }
    public Card Duplicate()
    {
        return (Card)this.MemberwiseClone();
    }

    /// <summary>
    /// Represents a leader card
    /// </summary>
    public class LeaderCard : Card
    {
        /// <summary>
        /// Represent if the leader card has been placed or not
        /// </summary>
        /// <value>It returns False by default</value>
        public bool Placed { get; set; }
        /// <summary>
        /// Check if the effect of the leader has been played 
        /// </summary>
        /// <value></value>
        public bool Played {get; set;}

        public LeaderCard(string name, CardFaction cardFaction, Effect effectType, Sprite CardImage, List<DeclaredEffect>? userCardEffects = null)
            : base(name, cardFaction, effectType,  CardImage, userCardEffects)
        {
            //Set the IsPlayed property to true cause it wont interact with de event triggers
            IsPlayed = true;
            this.Type = "Leader";
            //This is the user card created in the dsl
            Description ??= "Your leader card";
            Played = false;
        }
    }
    #region Special Cards
    /// <summary>
    /// Represents a special card
    /// </summary>
    public abstract class SpecialCard : Card
    {
        public SpecialCard(string name, CardFaction cardFaction, Effect effectType, Sprite CardImage, List<DeclaredEffect>? userCardEffects = null)
        : base(name, cardFaction, effectType, CardImage, userCardEffects) {}
    }
    /// <summary>
    /// Represents a climate special card
    /// </summary>
    public class ClimateCard : SpecialCard
    {
        public string Range {get;}
        public ClimateCard(string name, CardFaction cardFaction, Effect effectType, Sprite CardImage, string range, List<DeclaredEffect>? userCardEffects = null) 
        : base(name, cardFaction, effectType, CardImage, userCardEffects)
        {
            this.Range = range;
            this.Type = "Climate";
            //This is the user card created in the dsl
            Description ??= "Your climate card";
        }
    }
    /// <summary>
    /// Represents a increment special card
    /// </summary>
    public class IncrementCard : SpecialCard
    {
        public string Range {get;}
        public IncrementCard(string name, CardFaction cardFaction, Effect effectType, Sprite CardImage, string range, List<DeclaredEffect>? userCardEffects = null) 
        : base(name, cardFaction, effectType, CardImage, userCardEffects)
        {
            this.Range = range;
            this.Type = "Increment";
            //This is the user card created in the dsl
            Description ??= "Your increment card";
        }
    }

    /// <summary>
    /// Represents a cleareance special card
    /// </summary>
    public class CleareanceCard : SpecialCard
    {
        public CleareanceCard(string name, CardFaction cardFaction, Effect effectType, Sprite CardImage, List<DeclaredEffect>? userCardEffects = null) 
        : base(name, cardFaction, effectType, CardImage, userCardEffects)
        {
            this.Type = "Cleareance";
            //This is the user card created in the dsl
            Description ??= "Your cleareance card";
        }
    }
    /// <summary>
    /// Represents a decoy special card
    /// </summary>
    public class DecoyCard : SpecialCard
    {
        public DecoyCard(string name, CardFaction cardFaction, Effect effectType, Sprite CardImage, List<DeclaredEffect>? userCardEffects = null) 
        : base(name, cardFaction, effectType, CardImage, userCardEffects)
        {
            this.Type = "Decoy";
            //This is the user card created in the dsl
            Description ??= "Your decoy card";
        }
    }
    #endregion
    #region Unity Cards
    /// <summary>
    /// Represents a unity card
    /// </summary>
    public abstract class UnityCard : Card
    {
        public string Range { get; }
        public int OriginalPower { get;}
        public int Power { get; set; }

        public UnityCard(string name, CardFaction cardFaction, Effect effectType, string Range, int power,
        Sprite CardImage, List<DeclaredEffect>? userCardEffects = null) : base(name, cardFaction, effectType, CardImage, userCardEffects)
        {
            this.Range = Range;
            OriginalPower = power;
            Power = power;
        }
    }
    /// <summary>
    /// Represent a silver unity card
    /// </summary>
    public class SilverCard : UnityCard
    {
        public SilverCard(string name, CardFaction cardFaction, Effect effectType, string Range, 
        int power, Sprite CardImage, List<DeclaredEffect>? userCardEffects = null) 
        : base(name, cardFaction, effectType, Range, power, CardImage, userCardEffects)
        {
            this.Type = "Silver";
            //This is the user card created in the dsl
            Description ??= "Your silver card";
        }
    }
    /// <summary>
    /// Represent a gold unity card
    /// </summary>
    public class GoldCard : UnityCard
    {
        public GoldCard(string name, CardFaction cardFaction, Effect effectType, string Range, 
        int power, Sprite CardImage, List<DeclaredEffect>? userCardEffects = null) 
        : base(name, cardFaction, effectType, Range, power, CardImage, userCardEffects)
        {
            this.Type = "Gold";
            //This is the user card created in the dsl
            Description ??= "Your gold card";
        }
    }


    #endregion
}