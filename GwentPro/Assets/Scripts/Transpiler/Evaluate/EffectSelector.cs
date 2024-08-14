using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSelector 
{
    string Source {get; }
    bool Single {get; }
    Predicate Predicate {get; } 
    public EffectSelector(string source, bool single, Predicate predicate)
    {
        Source = source;
        Single = single;
        Predicate = predicate;
    }

    public CardCollection GetTargets(DeclaredEffect parent)
    {
        //Get the cards of the source
        CardCollection source = GetSourceCollection(Source, parent);
        
        if (Single)
        {
            //Instantiate the new card collection that will be returned as a collection with just a single card
            CardCollection selector = new CardCollection{source.Find(Predicate, new ExecuteScope())[0]};
            return selector;
        }
        else {
            return source.Find(Predicate, new ExecuteScope());
        }
    }

    //This is called in execution time to get the cardCollection of the source
    private CardCollection GetSourceCollection(string source, DeclaredEffect parent)
    {
        //Get reference to the context 
        Context context = new Context();

        //If source is parent logic
        if (source == "parent")
        {
            if (parent == null)
            {
                //TODO: El efecto no es un post efecto
                throw new Exception("");
            }
            else
            {
                return parent.Targets.GetTargets(parent.Parent);
            }
        }

        Dictionary<string, CardCollection> relateSource = new Dictionary<string, CardCollection>()
        {
            {"board", context.BoardCards},
            {"deck", context.Deck},
            {"hand", context.Hand},
            {"field", context.Field},
            {"graveyard", context.Graveyard},
            {"otherdeck", context.DeckOfPlayer(context.Enemy)},
            {"otherhand", context.HandOfPlayer(context.Enemy)},
            {"otherfield", context.FieldOfPlayer(context.Enemy)},
            {"othergraveyard", context.GraveyardOfPlayer(context.Enemy)},
        };
        return relateSource[source];
    }
}   
