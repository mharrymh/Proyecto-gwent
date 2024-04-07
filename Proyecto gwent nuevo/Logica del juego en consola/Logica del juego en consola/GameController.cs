using Assets.Scripts;
using Logica_del_juego_en_consola;
using System.Collections;
using System.Collections.Generic;
using System.Text;
//using Unity.VisualScripting;
//using UnityEngine;

public class GameManager /*: MonoBehaviour*/
{
    public Board Board { get; set; }
    public int Round { get; set; }
    public Player player1 { get; set; }
    public Player player2 { get; set; }
    public Player CurrentPlayer { get; set; }
    public Player OpponentPlayer { get; set; }
    public bool GameOver { get; set; }

    
    public GameManager()
    {
        Board = Board.Instance;
        Round = 1;
        player1 = new Player(CardFaction.Light, "player1");
        player2 = new Player(CardFaction.Dark, "player2");
        CurrentPlayer = StarterPlayer(player1, player2);
        OpponentPlayer = SecondPlayer(player1, player2, CurrentPlayer);
        GameOver = false;
    }

    public Player StarterPlayer(Player player1, Player player2)
    {
        Random rand = new Random();
        return rand.Next(2) == 0 ? player1 : player2;
    }

    public Player SecondPlayer(Player player1, Player player2, Player CurrentPlayer )
    {
        if (CurrentPlayer == player1)
        {
            return player2;
        }
        return player1;
    }

    public void ChangeTurn()
    {
        //Rotar la camara
        if (CurrentPlayer == player1)
        {
            CurrentPlayer = player2;
            OpponentPlayer = player1;
        }
        else
        {
            CurrentPlayer = player1;
            OpponentPlayer = player2;
        }
    }

    public void StartRound()
    {
        if (Round == 1)
        {
            player1.Hand = player1.AssignHand();
            player2.Hand = player2.AssignHand();
        }
        else
        {
            CleanBoard();
            player1.DrawCard(2);
            player2.DrawCard(2);
        }
    }
    
    public void CleanBoard()
    {
        var AllSections = Board.sections;
        foreach (var PlayerSection in AllSections)
        {
            var RangeSection = PlayerSection.Value;
            foreach (var Cards in RangeSection.Values)
            {
                foreach(Card card in Cards)
                {
                    card.Owner.GraveYard.Add(card);
                    Cards.Remove(card);
                }
            }
        }

        for (int i = 0; i < Board.climate_section.Length; i++)
        {
            if (Board.climate_section[i] != null)
            {
                Card card = Board.climate_section[i];
                card.Owner.GraveYard.Add(card);
                card = null;
            }
        }
    }

    public void EndRound()
    {
        //Determine winner
        int player1_score = player1.Score;
        int player2_score = player2.Score;
        if (player1_score > player2_score)
        {
            player1.RoundsWon++;
            player1 = CurrentPlayer;
            player2 = OpponentPlayer;
        }
        else if (player2_score > player1_score)
        {
            player2.RoundsWon++;
            player2 = CurrentPlayer;
            player1 = OpponentPlayer;
        }
        else
        {
            if (player1.Leader.effectType == EffectType.TieIsWin && player1.Leader.Played == true)
            {
                player1.RoundsWon++;
                player1 = CurrentPlayer;
                player2 = OpponentPlayer;
            }
            else if (player2.Leader.effectType == EffectType.TieIsWin && player2.Leader.Played == true)
            {
                player2.RoundsWon++;
                player2 = CurrentPlayer;
                player1 = OpponentPlayer;
            }
        }
        //Check if game is over
        if (player1.RoundsWon == 2 || player2.RoundsWon == 2)
        {
            GameOver = true;
        }
        else
        {
            //Start next round
            Round++;
            StartRound();
        }
    }

    public bool ValidMove(Card card, string range)
    {

        foreach (char Range in range)
        {
            if (card is Card.UnityCard unity_card && Board.sections[card.Owner.ID].ContainsKey(range.ToString()))
            {
                return true;
            }
            else if (card is Card.SpecialCard special_card && special_card.Type == SpecialType.Decoy)
            {
                return true;
            }
            else if (card is Card.SpecialCard climate_card && climate_card.Type == SpecialType.Climate)
            {
                return true;
            }
        }
        return false;
    }

    public void PlayCard(string range, Card card)
    {
        //range is the place where the player drop the card
        bool is_valid = ValidMove(card, range);
        if (is_valid && card.Owner.Hand.Contains(card))
        {
            if (card is Card.SpecialCard climate_card)
            {
                if (climate_card.Range == "M" && Board.climate_section[0] != null)
                {
                    Board.climate_section[0] = climate_card;
                }
                else if (climate_card.Range == "R" && Board.climate_section[1] != null)
                {
                    Board.climate_section[1] = climate_card;
                }
                else if (climate_card.Range == "S" && Board.climate_section[2] != null)
                {
                    Board.climate_section[2] = climate_card;
                }
            }
            else
            {
                //Add card to board
                Board.sections[card.Owner.ID][range].Add(card);
            }
            //Remove card from the player hand
            card.Owner.Hand.Remove(card);
            //Apply effects of the card
            Effect.Effects[card.effectType].Invoke(card);
            //Calculate player score
            int player_score = SumPowerInSection(card.Owner);
            card.Owner.Score = player_score;
            //Change turn 
            ChangeTurn();
        }
    }
    
    



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

//    



//       

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
