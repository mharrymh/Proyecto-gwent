using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCollection : IList<Card>
{
    private List<Card> Cards = new List<Card>();

    public CardCollection Find() 
    {
        // TODO: Completar con un predicado
        throw new NotImplementedException();
    }

    public CardCollection Copy()
    {
        CardCollection aux = new();

        foreach (var card in Cards)
        {
            aux.Add(card);
        }
        return aux;
    }

    public void Push(Card card)
    {
        Cards.Add(card);
    }

    public void SendBottom(Card card)
    {
        Cards.Insert(0, card);
    }

    public Card Pop()
    {
        Card last = Cards[^1];
        Cards.RemoveAt(Cards.Count-1);
        return last;
    }

    public void Remove(Card card)
    {
        Cards.Remove(card);
    }

    public void Shuffle()
    {
        int n = 0;
        Card aux;

        for (int i = 0; i < Cards.Count; i++)
        {
            n = UnityEngine.Random.Range(0, Cards.Count);

            aux = Cards[n];
            Cards[n] = Cards[i];
            Cards[i] = aux;
        }
    }

    public bool IsFixedSize => false;

    public bool IsReadOnly => false;

    public int Count => Cards.Count;

    public bool IsSynchronized => false;

    object SyncRoot => null;

    Card IList<Card>.this[int index] {
        get => Cards[index]; 
        set => Cards[index] = value;
    }

    public Card this[int index]
    {
        get => Cards[index];
        set => Cards[index] = value;
    }

    public void Clear()
    {
        Cards.Clear();
    }

    public void RemoveAt(int index)
    {
        Cards.RemoveAt(index);
    }

    public int IndexOf(Card item)
    {
        return Cards.IndexOf(item);
    }

    public void Insert(int index, Card item)
    {
        Cards.Insert(index, item);
    }

    public void Add(Card item)
    {
        Cards.Add(item);
    }

    public bool Contains(Card item)
    {
        return Cards.Contains(item);
    }

    public void CopyTo(Card[] array, int arrayIndex)
    {
        Cards.CopyTo(array, arrayIndex);
    }

    bool ICollection<Card>.Remove(Card item)
    {
        return Cards.Remove(item);
    }

    IEnumerator<Card> IEnumerable<Card>.GetEnumerator()
    {
        return Cards.GetEnumerator();
    }

    public IEnumerator GetEnumerator()
    {
        return Cards.GetEnumerator();
    }
}

