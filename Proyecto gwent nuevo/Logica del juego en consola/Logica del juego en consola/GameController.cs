using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;

public class GameController /*: MonoBehaviour*/
{
    public int Rounds { get; set; }

    public void Play(Card card)
    {
        
    }
}
    //Player player1;
    //Player player2;
    //Player currentPlayer;

     

    //public int round = 1;

    
    //void Start()
    //{     

    //    player1 = new Player();
    //    player2 = new Player();

    //    //Se le asigna una mano del deck a cada jugador 

    //    //Elige el jugador que empieza al azar
    //    if (Random.Range(0,2) == 0)
    //    {
    //        currentPlayer = player1;
    //    }
    //    else
    //    {
    //        currentPlayer = player2;
    //    }

    //    currentPlayer.IsPlaying = true;
    //}

//    void ChooseFaction(Player player)
//    {
//        //Las cartas las elige un jugador al azar
//        // Implementar la lógica para que el jugador elija su facción.
//        // Mostrar un menú o una interfaz de usuario que permita al jugador hacer su elección.

//        CardFaction chosenFaction = /* la facción elegida por el jugador */;

//        if (chosenFaction == CardFaction.Light)
//        {
//            DealCards(player, lightdeck);
//        }
//        else
//        {
//            DealCards(player, darkdeck);
//        }
//    }

//    void DealCards(Player player, Deck deck)
//    {
//        for (int i = 0; i < 10; i++ )
//        {
//            AssignHand(player, deck);
//        }
//    }

//    void AssignHand(Player player, Deck deck)
//    {
//        if (player == player1)
//        {
//            if (deck == lightdeck)
//            {
//                AddCards(player1, lightdeck);
//            }
//            else
//            {
//                AddCards(player1, darkdeck);
//            }
//        }
//        else
//        {
//            if (deck == lightdeck)
//            {
//                AddCards(player2, lightdeck);
//            }
//            else
//            {
//                AddCards(player2, darkdeck);
//            }
//        }

//        //AddCards(player, deck);
//    }

//    void AddCards(Player player, Deck deck)
//    {
        

//    }

//    void PlayerPasses()
//    {
//        if (!player1.HasPlayed && !player2.HasPlayed)
//        {
//            round ++;
//            EndRound();
//        }

//        currentPlayer.IsPlaying = false;
//        currentPlayer = (currentPlayer == player1) ? player2 : player1;
//        currentPlayer.IsPlaying = true;

//        //Girar la camara
//    }

//    void EndRound()
//    {
//        if (player1.Score > player2.Score)
//        {
//            player1.RoundsWon++;
//            currentPlayer = player1;
//        }
//        else if (player2.Score > player1.Score)
//        {
//            player2.RoundsWon++;
//            currentPlayer = player2;
//        }

        

//        if (player1.RoundsWon >= 2 || player2.RoundsWon >= 2)
//        {
//            EndGame();
//        }

//        player1.Score = 0;
//        player2.Score = 0;

//        round++;
//        if (round > 3)
//        {
//            EndGame();
//        }
//        player1.HasPlayed = false;
//        player2.HasPlayed = false;
        
//    }

//    void EndGame()
//    {
//        Player gameWinner = (player1.RoundsWon > player2.RoundsWon) ? player1 : player2;
//        //Determina quien es el ganador

//        // Cambia a una escena de fin de juego 
//        // SceneManager.LoadScene("EndGameScene");
//    }
//    void Update()
//    {
        
//    }


//}
