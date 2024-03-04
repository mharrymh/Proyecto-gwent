using Logica_del_juego_en_consola;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Assets.Scripts
{
    public enum EffectType
    {
        //Aumenta el poder en la fila(Cartas aumento)
        IncrementFile,
        //Anade una carta clima
        AddClimateCard,
        //Elimina la carta con mas poder del campo(propio o del rival)
        DeleteMostPowerCard,
        //Elimina la carta mas poderosa del rival
        DeleteLessPowerCard,
        //Roba una carta
        TakeCard,
        //Multiplica por n su ataque siendo n la cantidad de cartas
        //iguales a ella en el campo
        TimesTwins,
        //Elimina todas las cartas de la fila(propia o del rival)
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
        //Leader effects
        //Mantener una carta aleatoria en el campo por ronda
        KeepRandomCard,
        //Robar una carta extra en la segunda ronda
        DrawExtraCard,
        //Ganar en caso de empate
        TieIsWin,
        //Ningun efecto
        None
    }
    public enum CardFaction
    {
        Light,
        Dark
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

    public class Card
    {
        //IMAGEN Y DESCRIPCION
        public string Name { get; private set; }            
        public CardFaction Faction { get; private set; }
        public EffectType effectType { get; private set; }
        public Player Owner { get; set; }


        public Card(string name, CardFaction cardFaction, EffectType effectType)
        {           
            Name = name;          
            Faction = cardFaction;
            this.effectType = effectType;
        }

        public class LeaderCard : Card
        {
            public bool Played { get; set; }

            public LeaderCard(string name, CardFaction cardFaction, EffectType effectType)
                : base(name, cardFaction, effectType)
            {
                Played = false;
            }

        }
        public class SpecialCard : Card
        {
            public SpecialType Type { get; private set; }
            public string Range { get; private set; }

            public SpecialCard(string name, CardFaction cardFaction, EffectType effectType, SpecialType Type, string range)
                : base(name, cardFaction, effectType)
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

            public UnityCard(string name, CardFaction cardFaction, EffectType effectType, string Range, UnityType Type, int power)
                : base(name, cardFaction, effectType)
            {
                this.Range = Range;
                UnityType = Type;
                OriginalPower = power;
                Power = power;
            }
        }
    }



}
