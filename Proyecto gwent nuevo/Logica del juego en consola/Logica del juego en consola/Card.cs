using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Assets.Scripts
{
    public enum CardFaction
    {
        Light,
        Dark
    }
    public enum CardType
    {
        Leader,
        Gold,
        Silver,
        Climate,
        Increment,
        Decoy,
        Normal
    }

    public class Card
    {
        //image y description
        public int ID { get; private set; }
        public string Name { get; private set; }
        public int? OriginalPower { get; private set; }
        public int? Power { get; set; }

        //public int OriginalPower {get; set;}
        public string Range { get; private set; }
        public CardType Type { get; private set; }
        public CardFaction Faction { get; private set; }

        public Card(int id, string name, int? power, string attackRange, CardType type, CardFaction cardFaction)
        {
            ID = id;
            Name = name;
            OriginalPower = power;
            Power = power;
            Range = attackRange;
            Type = type;
            Faction = cardFaction;
        }
    }



}
