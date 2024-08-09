public interface ICard 
{
    string Name { get; }
    string Type {get; }
    //TODO: Cambiarlo para que en vez de un string sea el enum CardFaction
    string Faction {get; }
    int Power {get; }
    string Range {get;}
    List<IEffect> Effects {get;}
}

public interface IEffect
{
    
}