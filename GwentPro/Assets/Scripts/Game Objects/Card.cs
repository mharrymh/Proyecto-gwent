using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
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
    //TODO: Cambiar los strings para que sean en ingles
    //TODO: Hacerlo con el effect.ToString();
    public readonly Dictionary<string, string> effectDescriptions = new Dictionary<string, string>
    {
        { "DeleteMostPowerCard", "Elimina la carta plata con m�s poder en el campo" },
        { "IncrementFile", "Carta incremento (afecta solo a las cartas plata)" },
        { "DeleteLessPowerCard", "Elimina la carta plata con menos poder en el campo rival" },
        { "TakeCardFromDeck", "Roba una carta del deck" },
        { "TakeCardFromGraveYard", "Roba la carta m�s poderosa del cementerio" },
        { "AssignProm", "Asigna a todas las cartas plata del campo el promedio de poder general" },
        { "TimesTwins", "Multiplica su da�o por la cantidad de cartas iguales a ella en el campo" },
        { "CleanFile", "Elimina todas las cartas de la fila con menos cartas del campo" },
        { "Climate", "Carta clima (afecta solo a las cartas plata)" },
        { "Clearance", "Carta despeje" },
        { "Decoy", "(Se�uelo) Se coloca sobre una carta unidad propia para regresarla a la mano" },
        { "AddClimateCard", "A�ade (si hay espacio, y existe) una carta clima propia al campo" },
        { "DrawExtraCard", "Este lider te permite robar una carta extra en el resto de rondad" },
        { "KeepRandomCard", "Este lider te permite mantener una carta del campo entre rondas" },
    };

    public Sprite CardImage {get; }
    public string Description { get; private set;}
    public string Name { get;}
    public CardFaction Faction { get;}
    /// <summary>
    /// Card effect
    /// </summary>
    /// <value></value>
    public Effect EffectType { get;}
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
    /// Represents a card in its basic form 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="cardFaction"></param>
    /// <param name="effectType"></param>
    /// <param name="CardImage"></param>
    public Card(string name, CardFaction cardFaction, Effect effectType, Sprite CardImage)
    {
        Name = name;
        Faction = cardFaction;
        this.EffectType = effectType;
        this.CardImage = CardImage;

        //Set description of the card
        if (effectType != null && effectDescriptions.ContainsKey(effectType.ToString())) {
            Description = effectDescriptions[effectType.ToString()];
        }
        else Description = "Sin efecto";
    }

    /// <summary>
    /// Represents a leader card
    /// </summary>
    public class LeaderCard : Card
    {
        readonly Dictionary<string, string> leaderDescription = new()
        {
            
        };
        /// <summary>
        /// Represent if the leader card has been placed or not
        /// </summary>
        /// <value>It returns False by default</value>
        public bool Placed { get; set; }

        public LeaderCard(string name, CardFaction cardFaction, Effect effectType, Sprite CardImage)
            : base(name, cardFaction, effectType,  CardImage)
        {
            //Set the IsPlayed property to true cause it wont interact with de event triggers
            IsPlayed = true;
        }
    }
    #region Special Cards
    /// <summary>
    /// Represents a special card
    /// </summary>
    public abstract class SpecialCard : Card
    {
        public SpecialCard(string name, CardFaction cardFaction, Effect effectType, Sprite CardImage)
        : base(name, cardFaction, effectType, CardImage) {}
    }
    /// <summary>
    /// Represents a climate special card
    /// </summary>
    public class ClimateCard : SpecialCard
    {
        public string Range {get;}
        public ClimateCard(string name, CardFaction cardFaction, Effect effectType, Sprite CardImage, string range) 
        : base(name, cardFaction, effectType, CardImage)
        {
            this.Range = range;
        }
    }
    /// <summary>
    /// Represents a increment special card
    /// </summary>
    public class IncrementCard : SpecialCard
    {
        public string Range {get;}
        public IncrementCard(string name, CardFaction cardFaction, Effect effectType, Sprite CardImage, string range) 
        : base(name, cardFaction, effectType, CardImage)
        {
            this.Range = range;
        }
    }
    /// <summary>
    /// Represents a cleareance special card
    /// </summary>
    public class CleareanceCard : SpecialCard
    {
        public CleareanceCard(string name, CardFaction cardFaction, Effect effectType, Sprite CardImage) 
        : base(name, cardFaction, effectType, CardImage)
        {
        }
    }
    /// <summary>
    /// Represents a decoy special card
    /// </summary>
    public class DecoyCard : SpecialCard
    {
        public DecoyCard(string name, CardFaction cardFaction, Effect effectType, Sprite CardImage) 
        : base(name, cardFaction, effectType, CardImage)
        {
            this.Description = "Carta señuelo, colocala sobre una de tus cartas para que esta vuelva a tu mano";
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
        Sprite CardImage) : base(name, cardFaction, effectType, CardImage)
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
        int power, Sprite CardImage) 
        : base(name, cardFaction, effectType, Range, power, CardImage)
        {
        }
    }
    /// <summary>
    /// Represent a gold unity card
    /// </summary>
    public class GoldCard : UnityCard
    {
        public GoldCard(string name, CardFaction cardFaction, Effect effectType, string Range, 
        int power, Sprite CardImage) 
        : base(name, cardFaction, effectType, Range, power, CardImage)
        {
        }
    }
    #endregion
}