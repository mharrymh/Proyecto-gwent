using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
#nullable enable

public class CardCollection : IList<Card>
{
    //Get the game manager script
    GameManager? gm; 
    //Get the board
    Board? board;
    public List<Card> Cards = new List<Card>();

    public string? GameListName {get; set;}
    public Player? Player { get; set; }

    public CardCollection(string? gameListName = null, Player? player = null)
    {
        if (gameListName != null) GameListName = gameListName;
        if (player != null) Player = player;
    }

    public CardCollection Find(Predicate predicate, IExecuteScope scope) 
    {
        CardCollection aux = new CardCollection();

        foreach (Card card in this)
        {
            //If the card fulfill 
            if (predicate.Execute(card, scope.CreateChildScope()))
            {
                aux.Add(card);
            }
        }
        return aux;
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

    public void Push(Card card, bool transpilerCall)
    {
        Add(card, transpilerCall);
    }

    public void SendBottom(Card card, bool transpilerCall)
    {
        Insert(0, card, transpilerCall);
    }

    public Card Pop(bool transpilerCall)
    {
        if (Cards.Count == 0)
        {
            //TODO:
            throw new IndexOutOfRangeException();
        }
        Card last = Cards[^1];
        RemoveAt(Cards.Count-1, transpilerCall);
        
        return last;
    }
    public void Remove(Card card)
    {
        Remove(card, false);
    }

    public void Remove(Card card, bool transpilerCall)
    {
        if (transpilerCall) {
            gm = GameObject.Find("GameManager").GetComponent<GameManager>();
            //Delete the cardPrefab of the board
            if (card.CardPrefab != null) {
                gm.CardBeaten(card);
            }
        }
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
    public bool IsReadOnly => false;

    public int Count => Cards.Count;


    Card IList<Card>.this[int index] {
        get => Cards[index]; 
        set => Cards[index] = value;
    }

    public Card this[int index]
    {
        get
        {
            if (index < 0 || index >= Cards.Count)
            {
                //TODO: 
                throw new IndexOutOfRangeException("El índice está fuera de rango.");
            }
            return Cards[index];
        }
        set
        {
            if (index < 0 || index >= Cards.Count)
            {
                //TODO: 
                throw new IndexOutOfRangeException("El índice está fuera de rango.");
            }
            Cards[index] = value;
        }
    }
    public void Clear()
    {
        Clear(false);
    }

    public void Clear(bool transpilerCall)
    {
        if (transpilerCall) {
            gm = GameObject.Find("GameManager").GetComponent<GameManager>();
            //Delete all cards that have a prefab assigned to it 
            foreach (Card card in this)
            {
                if (card.CardPrefab != null)
                {
                    gm.CardBeaten(card);
                }
            }
            gm.SetPower();
        }
        Cards.Clear();
    }

    public void RemoveAt(int index)
    {
        RemoveAt(index, false);
    }
    public void RemoveAt(int index, bool transpilerCall)
    {
        if (index >= Cards.Count)
        {
            //TODO:
            throw new IndexOutOfRangeException();
        }

        if (transpilerCall) {
            gm = GameObject.Find("GameManager").GetComponent<GameManager>();

            Card cardToRemove = this[index];

            if (cardToRemove.CardPrefab != null) {
                gm.CardBeaten(cardToRemove);
            }
        }
        Cards.RemoveAt(index);
    }


    public int IndexOf(Card item)
    {
        return Cards.IndexOf(item);
    }
    public void Insert(int index, Card item)
    {
        Insert(index, item, false);
    }
    public void Insert(int index, Card item, bool transpilerCall)
    {   
        if (transpilerCall) {
            item = item.Duplicate();
            gm = GameObject.Find("GameManager").GetComponent<GameManager>();
            //Check that the card is not from another player
            if (Player == null)
            {
                Player = item.Owner;
                GameListName = "Field";
                Add(item, true);
                return;
            }
            if (item.Owner != Player)
            {
                item.Owner = Player;
            }
            //Add it in the field
            if (GameListName == "board" || GameListName == "hand" || GameListName == "field") {
                //In this case is the same anyway
                if (GameListName == "board") //Assign owner and change the gameList
                { GameListName = "field"; Player = this[index].Owner;}

                if (GameListName == "field")
                {
                    //Instantiate the card on the board
                    gm.InstantiateAndPlay(item, Player);
                }
                else //Is to the hand
                {
                    //Instantiate the card in the player hand
                    gm.InstantiateInHand(item, Player);
                }
            }
        }
        Cards.Insert(index, item);
    }
    //TODO: Que hacer con este metodo
    /// <summary>
    /// Validate if the source is from the board
    /// </summary>
    /// <param name="item"></param>
    public void Add(Card item)
    {
        Add(item, false);
    }
    public void Add(Card item, bool transpilerCall)
    {   
        
        if (transpilerCall)
        {
            item = item.Duplicate();
            //Check that the card is not from another player
            if (Player == null)
            {
                Player = item.Owner;
                GameListName = "Field";
                Add(item, true);
                return;
            }
            if (item.Owner != Player)
            {
                item.Owner = Player;
            }
            gm = GameObject.Find("GameManager").GetComponent<GameManager>();
            //Add it in the field
            if (GameListName == "board" || GameListName == "hand" || GameListName == "field") {
                //In this case is the same anyway
                if (GameListName == "board") //Assign owner and change the gameList
                { GameListName = "field"; Player = item.Owner;}

                if (GameListName == "field")
                {
                    //Instantiate the card on the board
                    gm.InstantiateAndPlay(item, Player);
                }
                else //Is to the hand
                {
                    //Instantiate the card in the player hand
                    gm.InstantiateInHand(item, Player);
                }
            }
        }
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

