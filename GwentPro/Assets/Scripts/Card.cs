using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable] 

public enum EffectType
{
    //Aumenta el poder en la fila(Cartas aumento)
    IncrementFile,
    //Elimina la carta con mas poder del campo(propio o del rival)
    DeleteMostPowerCard,
    //Elimina la carta mas poderosa del rival
    DeleteLessPowerCard,
    //Roba una carta del Deck
    TakeCardFromDeck,
    //Roba una carta del ceementerio
    TakeCardFromGraveYard,
    //Multiplica por n su ataque siendo n la cantidad de cartas
    //iguales a ella en el campo
    TimesTwins,
    //Elimina todas las cartas de la fila con menos cartas(propia o del rival)
    //Si filas.count coinciden elimina la del rival
    CleanFile,
    //Calcula el promedio de todas las cartas del campo
    //y las iguala a ese poder
    AssignProm,
    //Carta clima
    Climate,
    //Despeje
    Clearance,
    //Decoy
    Decoy,
    //Add Climate Card
    AddClimateCard,
    //Leader effects
    //Mantener una carta aleatoria en el campo por ronda
    KeepRandomCard,
    //Robar una carta extra en la segunda ronda
    DrawExtraCard,
    //Ningun efecto
    None
}
public enum CardFaction
{
    Dark,
    Light
}
public enum SpecialType
{
    Climate,
    Clearance,
    Increment,
    Decoy,
}
public enum UnityType
{
    Gold,
    Silver
}

public class Card : ScriptableObject
{
    public Sprite CardImage;
    public string Description { get; set; }
    public string Name { get; private set; }
    public CardFaction Faction { get; private set; }
    public EffectType effectType { get; private set; }
    public Player Owner { get; set; }
    public bool IsPlayed { get; set; }
    public GameObject CardPrefab { get; set; }

    public Card(string name, CardFaction cardFaction, EffectType effectType, Sprite CardImage)
    {
        Name = name;
        Faction = cardFaction;
        this.effectType = effectType;
        this.CardImage = CardImage;
        CardPrefab = null;

        //Prueba
        Description = SetDescription(effectType, this);
    }

    public class LeaderCard : Card
    {
        public bool Played { get; set; }

        public LeaderCard(string name, CardFaction cardFaction, EffectType effectType, Sprite CardImage)
            : base(name, cardFaction, effectType, CardImage)
        {
            Played = false;

            //Set the is played because it wont interact with de event triggers
            IsPlayed = true;
        }

    }
    public class SpecialCard : Card
    {
        public SpecialType Type { get; private set; }
        public string Range { get; private set; }

        public SpecialCard(string name, CardFaction cardFaction, EffectType effectType, SpecialType Type, string range, 
            Sprite CardImage)
            : base(name, cardFaction, effectType, CardImage)
        {
            this.Type = Type;
            Range = range;
        }
    }
    public class UnityCard : Card
    {
        public string Range { get; private set; }
        public UnityType UnityType { get; private set; }
        public int OriginalPower { get; private set; }
        public int Power { get; set; }

        public UnityCard(string name, CardFaction cardFaction, EffectType effectType, string Range, UnityType Type, int power,
            Sprite CardImage)
            : base(name, cardFaction, effectType, CardImage)
        {
            this.Range = Range;
            UnityType = Type;
            OriginalPower = power;
            Power = power;
        }
    }

    string SetDescription(EffectType efecto, Card card)
    {
        if (efecto is EffectType.IncrementFile) return "Carta incremento (afecta solo a las cartas plata)";
        else if (efecto is EffectType.DeleteMostPowerCard) return "Elimina la carta con más poder en el campo";
        else if (efecto is EffectType.DeleteLessPowerCard) return "Elimina la carta con menos poder en el campo rival";
        else if (efecto is EffectType.TakeCardFromDeck) return "Roba una carta del deck";
        else if (efecto is EffectType.TakeCardFromGraveYard) return "Roba la carta más poderosa del cementerio";
        else if (efecto is EffectType.AssignProm) return "Asigna a todas las cartas del campo el promedio de poder general";
        else if (efecto is EffectType.TimesTwins) return "Multiplica su daño por la cantidad de cartas iguales a ella en el campo";
        else if (efecto is EffectType.CleanFile) return "Elimina todas las cartas de la fila con menos cartas del campo";
        else if (efecto is EffectType.Climate) return "Carta clima";
        else if (efecto is EffectType.DrawExtraCard) return "Este lider te permite robar una carta extra en el resto de rondad";
        else if (efecto is EffectType.KeepRandomCard) return "Este lider te permite mantener una carta del campo entre rondas";
        else if (efecto is EffectType.Clearance) return "Carta despeje";
        else if (efecto is EffectType.Decoy) return "Se coloca sobre una carta unidad propia para regresarla" +
                "a la mano";
        else if (efecto is EffectType.AddClimateCard) return "Añade (si hay espacio, y existe) una carta clima propia al campo";
        return "Sin efecto";
    }
}