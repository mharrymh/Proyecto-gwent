using System.Collections.Generic;

public interface ICard {
    string Name {get; }
    string Type {get; }
    CardFaction Faction {get; }
    string Range {get; }
    int Power {get; }
    List<DeclaredEffect> Effects {get; }
}

public class MyCard : ICard
{
    public MyCard(string Name, string Type, CardFaction faction, string Range, int Power, List<DeclaredEffect> effects)
    {
        this.Name = Name;
        this.Type = Type;
        this.Faction = faction;
        this.Range = Range;
        this.Power = Power;
        this.Effects = effects;
    }

    public string Name {get; }

    public string Type {get; }

    public CardFaction Faction {get; }

    public string Range {get; }

    public int Power {get; }

    public List<DeclaredEffect> Effects {get; }
}