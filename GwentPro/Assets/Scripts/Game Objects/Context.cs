using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Context
{
    //Get the game manager script
    public GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    //Get the board instance
    readonly Board board = Board.Instance;
    //Returns an string with the id player that trigger the effect, it is always the current player
    public Player TriggerPlayer {
        get {
            return gm.currentPlayer;
        }
    }

    public Player Enemy {
        get {
            return gm.NotCurrentPlayer;
        }
    }
    
    //Returns a list with all card in the board
    public CardCollection BoardCards {
        get {
            CardCollection aux = new CardCollection();
            //Get all the cards in the section and add them to the aux list
            foreach (var Range in board.sections.Values)
            {
                foreach (CardCollection Cards in Range.Values)
                {
                    foreach (Card card in Cards)
                    {
                        aux.Add(card);
                    }
                }
            }
            //Get the climate cards and add them to the cards list
            for (int i = 0; i < board.climate_section.Length; i++)
            {
                if (board.climate_section[i] != null) aux.Add(board.climate_section[i]);
            }
            //Get the increment cards and add them to the aux list
            foreach (var arr in board.increment_section.Values)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    if (arr[i] != null) aux.Add(arr[i]);
                }
            }
            // aux.Shuffle();
            return aux;
        }
    }

    public CardCollection HandOfPlayer(Player player) {
        return player.Hand;
    }

    public CardCollection Hand {
        get {
            return HandOfPlayer(TriggerPlayer);
        }
    }

    public CardCollection FieldOfPlayer(Player player) {
        return player.Field;
    }

    public CardCollection Field {
        get {
            return FieldOfPlayer(TriggerPlayer);
        }
    }

    public CardCollection GraveyardOfPlayer(Player player)
    {
        return player.GraveYard;
    }

    public CardCollection Graveyard {
        get {
            return GraveyardOfPlayer(TriggerPlayer);
        }
    }


    public CardCollection DeckOfPlayer(Player player)
    {
        return player.PlayerDeck;
    }

    public CardCollection Deck {
        get {
            return DeckOfPlayer(TriggerPlayer);
        }
    }

}
