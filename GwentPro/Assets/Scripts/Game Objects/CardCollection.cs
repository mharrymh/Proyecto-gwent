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
    GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    //Get the board
    Board board = Board.Instance;
    private List<Card> Cards = new List<Card>();

    /// <summary>
    /// Context reference
    /// </summary>
    /// <value></value>
    Context context = new Context();

    public String? GameListName {get; set;} = null;
    public Player Player {get; set;} = null;

    public CardCollection(string? gameListName = null, Player? player = null)
    {
        if (GameListName != null)
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

    public void Push(Card card)
    {
        Add(card);
    }

    public void SendBottom(Card card)
    {
        Insert(0, card);
    }

    public Card Pop()
    {
        if (Cards.Count == 0)
        {
            //TODO:
            throw new IndexOutOfRangeException();
        }
        Card last = Cards[^1];
        RemoveAt(Cards.Count-1);
        return last;
    }

    public void Remove(Card card)
    {   
        if (GameListName == null) {
            Cards.Remove(card);
            return;
        }
        //Change the game list and call the function again
        if (GameListName == "board") {
            GameListName = "field";
            Remove(card);
            return;
        }
        //The card does not exists in the list so it is not necessary to remove anything
        if (GameListName != card.Source) return;

        if (GameListName == "deck") {
            Player.PlayerDeck.Remove(card);
        }
        else if (GameListName == "hand") {
            gm.CardBeaten(card);
            Player.Hand.Remove(card);
        }
        else if (GameListName == "field") {
            gm.CardBeaten(card);
            board.RemoveFromBoard(card);
        }
        Player.GraveYard.Add(card);
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
        if (GameListName == null) {
            Cards.Clear();
            return;
        }

        //Change the game list and call the function again
        if (GameListName == "board") {
            gm.CleanBoard();
        }

        if (GameListName == "deck") {
            foreach (Card card in Player.PlayerDeck)
            {
                Player.GraveYard.Add(card);
            }
            Player.PlayerDeck.Clear();
        }
        else if (GameListName == "hand") {
            foreach (Card card in Player.Hand) {
                gm.CardBeaten(card);
                Player.GraveYard.Add(card);
            }
            Player.Hand.Clear();
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

    public void RemoveAt(int index)
    {
        if (index >= Cards.Count)
        {
            //TODO:
            throw new IndexOutOfRangeException();
        }


        if (GameListName == null) {
            Cards.RemoveAt(index);
            return;
        }


        //Change the game list and call the function again
        if (GameListName == "board") {
            GameListName = "field";
            RemoveAt(index);
            return;
        }
        Card card;
        if (GameListName == "deck") {
            card = Player.PlayerDeck[index];
            Player.PlayerDeck.RemoveAt(index);
        }
        else if (GameListName == "hand") {
            card = Player.Hand[index];
            gm.CardBeaten(Player.Hand[index]);
            Player.Hand.RemoveAt(index);
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

    public int IndexOf(Card item)
    {
        return Cards.IndexOf(item);
    }

    public void Insert(int index, Card item)
    {   if (GameListName == null) {
            Cards.Insert(index, item);
            return; 
        }

        string source = item.Source;
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
            //Add card to player deck
            Player.PlayerDeck.Insert(index, item);
        }
        else if (GameListName == "hand")
        {
            //Already added
            if (source == "hand") return;
            else if (source == "graveyard") {
                //Restore its values
                item.Restore();
            }
            Remove(source, item);
            //Instantiate the card in the player hand
            gm.InstantiateInHand(item, Player);
        }
        else if (GameListName == "field")
        {
            if (source == "field") return;

            if (source == "graveyard") item.Restore();
            Remove(source, item);

            //TODO: this has to play the card in the backend too
            gm.InstantiateAndPlay(item, Player);
        }
        else // if (GameListName == "graveyard")
        {
            //Remmove the card from its source 
            Remove(source, item);
            //Add it to the graveyard
            Player.GraveYard.Insert(index, item);
        }
        
    }
    /// <summary>
    /// Remove card from its source
    /// </summary>
    /// <param name="source"></param>
    /// <param name="item"></param>
    private void Remove(string source, Card item)
    {
        if (source == "deck") Player.PlayerDeck.Remove(item);
        else if (source == "hand") Player.Hand.Remove(item);
        else if (source == "field") Player.Field.Remove(item);
        else //is from graveyard
            Player.GraveYard.Remove(item);
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
    {   if (GameListName == null) {
            Cards.Add(item);
            return;
        }   

        string source = item.Source;
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
            //Add card to player deck
            Player.PlayerDeck.Add(item);
        }
        else if (GameListName == "hand")
        {
            //Already added
            if (source == "hand") return;
            else if (source == "graveyard") {
                //Restore its values
                item.Restore();
            }
            Remove(source, item);
            //Instantiate the card in the player hand
            //TODO: add it in the backend
            gm.InstantiateInHand(item, Player);
        }
        else if (GameListName == "field")
        {
            if (source == "field") return;

            if (source == "graveyard") item.Restore();
            Remove(source, item);

            //TODO: this has to play the card in the backend too
            gm.InstantiateAndPlay(item, Player);
        }
        else // if (GameListName == "graveyard")
        {
            //Remmove the card from its source 
            Remove(source, item);
            //Add it to the graveyard
            Player.GraveYard.Add(item);
        }
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

