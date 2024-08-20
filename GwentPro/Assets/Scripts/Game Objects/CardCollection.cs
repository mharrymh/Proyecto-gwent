using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
#nullable enable

public class CardCollection : IList<Card>
{
    //Get the game manager script
    GameManager? gm; 
    //Get the board
    Board? board;
    private List<Card> Cards = new List<Card>();

    public string? GameListName {get; set;}
    public Player? Player { get; set; }

    public CardCollection(string? gameListName = null, Player? player = null)
    {
        if (gameListName != null)
            GameListName = gameListName;
        if (player != null)
            Player = player;
    }

    public CardCollection Find(Predicate predicate, IExecuteScope scope) 
    {
        CardCollection aux = new CardCollection();

        foreach (Card card in this)
        {
            //If the card fulfill 
            if (predicate.Execute(card, scope.CreateChildScope()))
            {
                Debug.Log("entro");
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
            if (GameListName == "board")
            {
                GameListName = "field";
                Remove(card, true);
                return;
            }
            else if (GameListName == "hand" || GameListName == "field")
            {
                gm.CardBeaten(card);
            }
        }
        Cards.Remove(card);
    }
    //TODO: ELIMINAR
    // public void Remove(Card card, bool transpilerCall)
    // {   
    //     if (transpilerCall) {

    //         gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    //         board = Board.Instance;
    //         //Change the game list and call the function again
    //         if (GameListName == "board") {
    //             GameListName = "field";
    //             Remove(card, true);
    //             return;
    //         }
    //         //The card does not exists in the list so it is not necessary to remove anything
    //         if (GameListName != card.Source) return;


    //         else if (GameListName == "hand") {
    //             gm.CardBeaten(card);
    //         }
    //         else if (GameListName == "field") {
    //             gm.CardBeaten(card);
    //             board.RemoveFromBoard(card);
    //         }
    //         if (GameListName != "graveyard")
    //             Player.GraveYard.Cards.Add(card);
    //     }
    //     Cards.Remove(card);
        
    // }

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
            board = Board.Instance;
            //Change the game list and call the function again
            if (GameListName == "board") {
                gm.CleanBoard();
            }

            if (GameListName == "deck") {
                foreach (Card card in Player.PlayerDeck)
                {
                    Player.GraveYard.Add(card);
                }
            }
            else if (GameListName == "hand") {
                foreach (Card card in Player.Hand) {
                    gm.CardBeaten(card);
                    Player.GraveYard.Add(card);
                }
            }
            else if (GameListName == "field") {
                CardCollection cardsInField = Player.Field.Copy();
                foreach (Card card in cardsInField)
                {
                    gm.CardBeaten(card);
                    Player.GraveYard.Add(card);
                }
                board.ClearField(Player);
                gm.SetPower();
            }
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
            board = Board.Instance;
            //Change the game list and call the function again
            if (GameListName == "board") {
                GameListName = "field";
                RemoveAt(index);
                return;
            }
            Card card;
            if (GameListName == "deck") {
                card = Player.PlayerDeck[index];
            }
            else if (GameListName == "hand") {
                card = Player.Hand[index];
                gm.CardBeaten(Player.Hand[index]);
            }
            else if (GameListName == "field") {
                card = Player.Field[index];
                gm.CardBeaten(card);
                board.RemoveFromBoard(card);
            }
            else //is from graveyard 
                return;
            
            //Add card to the player graveyard
            Player.GraveYard.Add(card);
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
            gm = GameObject.Find("GameManager").GetComponent<GameManager>();
            string source = item.Source;
            if (source == "board") {
                BoardSource(item);
                return;
            }
            //Add it in the field
            if (GameListName == "board") {
                this.GameListName = "field";
                Insert(index, item);
            }
            else if (GameListName == "deck")
            {
                //Delete player from the battlefield
                if (source == "field") Player.Field.Remove(item); 
                //Add card to the top of the deck
                else if (source == "deck") Player.PlayerDeck.Remove(item);
                //Restore card from default and add it to the deck
                item.Restore();
            }
            else if (GameListName == "hand")
            {
                //Already added
                if (source == "hand") return;
                else if (source == "graveyard") {
                    //Restore its values
                    item.Restore();
                }
                Remove(source, item, transpilerCall);
                //Instantiate the card in the player hand
                gm.InstantiateInHand(item, Player);
            }
            else if (GameListName == "field")
            {
                if (source == "field") return;

                if (source == "graveyard") item.Restore();
                Remove(source, item, transpilerCall);

                //TODO: this has to play the card in the backend too
                gm.InstantiateAndPlay(item, Player);
            }
            else // if (GameListName == "graveyard")
            {
                //Remmove the card from its source 
                Remove(source, item, true);
            }
        }
        Cards.Insert(index, item);
        
    }
    /// <summary>
    /// Remove card from its source
    /// </summary>
    /// <param name="source"></param>
    /// <param name="item"></param>
    private void Remove(string source, Card item, bool transpilerCall)
    {
        if (source == "deck") Player.PlayerDeck.Remove(item, transpilerCall);
        else if (source == "hand") Player.Hand.Remove(item, transpilerCall);
        else if (source == "field") Player.Field.Remove(item, transpilerCall);
        else //is from graveyard
            Player.GraveYard.Remove(item, true);
    }

    /// <summary>
    /// Validate if the source is from the board
    /// </summary>
    /// <param name="item"></param>
    private void BoardSource(Card item)
    {
        if (GameListName == "board") return; //Is already added
        //This is thrown just if the card is from the other player
        //else the source will be "field"
        else {
            //TODO: 
            throw new Exception("Se trato de asignar una carta del otro jugador ");
        }
    }
    public void Add(Card item)
    {
        Add(item, false);
    }
    public void Add(Card item, bool transpilerCall)
    {   
        if (transpilerCall)
        {
            string source = item.Source;
            gm = GameObject.Find("GameManager").GetComponent<GameManager>();
            if (source == "board") {
                BoardSource(item);
                return;
            }
            //Add it in the field
            if (GameListName == "board") {
                this.GameListName = "field";
                Add(item);
            }
            else if (GameListName == "deck")
            {
                //Delete player from the battlefield
                if (source == "field") Player.Field.Remove(item); 
                //Add card to the top of the deck
                else if (source == "deck") Player.PlayerDeck.Remove(item);
                //Restore card from default and add it to the deck
                item.Restore();
            }
            else if (GameListName == "hand")
            {
                //Already added
                if (source == "hand") return;
                else if (source == "graveyard") {
                    //Restore its values
                    item.Restore();
                }
                Remove(source, item, transpilerCall);
                //Instantiate the card in the player hand
                //TODO: add it in the backend
                gm.InstantiateInHand(item, Player);
            }
            else if (GameListName == "field")
            {
                if (source == "field") return;

                if (source == "graveyard") item.Restore();
                Remove(source, item, transpilerCall);

                //TODO: this has to play the card in the backend too
                gm.InstantiateAndPlay(item, Player);
            }
            else // if (GameListName == "graveyard")
            {
                //Remmove the card from its source 
                Remove(source, item, transpilerCall);
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

