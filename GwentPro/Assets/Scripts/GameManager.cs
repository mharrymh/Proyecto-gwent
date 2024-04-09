using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.UIElements;




/// <summary>
/// Crear escenas de inicio y game over 
/// Implementar cementerio
/// Quitarle el efecto clima a la carta decoy o ponerlo para que solo afecte a las cartas de unidad
/// </summary>

public class GameManager : MonoBehaviour
{
    public GameObject GameBoard;
    public GameObject CardZones;
    public GameObject PassButton;
    public GameObject ScorePlayer1;
    public GameObject ScorePlayer2;


    public GameObject cardPrefab; // The card prefab
    public Transform HandPanel; 
    //The leader panels
    public Transform LeaderPlayer1;
    public Transform LeaderPlayer2;

    public int Round;
    //
    public int RoundDraws;
    public Player currentPlayer;
    //public bool GameOver;

    //The players
    public Player player1;
    public Player player2;
    //The points in the UI
    public TMP_Text PowerPlayer1;
    public TMP_Text PowerPlayer2; 

    //References
    CardDatabase cartas = new CardDatabase();
    Effects CardEffects;
    Board board = Board.Instance;
    DisplayCard disp;

    void Start()
    {
        CardEffects = new Effects();


        Round = 1;
        player1 = new Player(CardFaction.Dark, "player1");
        player2 = new Player(CardFaction.Light, "player2");
        currentPlayer = GetStarterPlayer();
        //If the starter player is player2 rotate the scene
        if (currentPlayer == player2) RotateObjects();
        StartCoroutine(InstanciarCartas(5, currentPlayer));
    }

    private void Update()
    {
        SetPower(player1);
        SetPower(player2);
    }

    IEnumerator InstanciarCartas(int n, Player player)
    {
        player.Ready = true;
        if (player.PlayerDeck.Count < n) n = player.PlayerDeck.Count;

        player.PlayerDeck = Shuffle(player.PlayerDeck);
        for (int i = 0; i < n; i++)
        {
            Instanciar(player);
            player.PlayerDeck.RemoveAt(0);
            yield return new WaitForSeconds(0.2f); // Wait .2 seconds before instantiate the new card
        }
    }

    public void Instanciar(Player player) 
    {
        if (player.PlayerDeck.Count > 0)
        {
            Card card = player.PlayerDeck[0];
            //Crate new instance of the card on the playerhand
            GameObject CardInstance = Instantiate(cardPrefab, HandPanel);
            //Get the DisplayCard component of the new Card
            disp = CardInstance.GetComponent<DisplayCard>();
            //Assign the card to it
            disp.card = card;
            //Add card to player hand
            player.AssignHand(card);
            //Assign owner of the card
            card.Owner = player;
            //Assign the cardPrefab to the card
            card.CardPrefab = CardInstance;
            //Display card
            disp.ShowCard();

            if (!player.LeaderPlayed)
            {
                if (player == player1) CardInstance = Instantiate(cardPrefab, LeaderPlayer1);
                else if (player == player2) CardInstance = Instantiate(cardPrefab, LeaderPlayer2);

                disp = CardInstance.GetComponent<DisplayCard>();
                disp.card = player.Leader;
                disp.ShowCard();
                player.LeaderPlayed = true;
            }
        }
        else
        {
            Debug.Log("Deck is empty");
        }
    }
    public void SetPower(Player player)
    {        
        if (player == player1)
        {
            if (GetPower(player) == 0) PowerPlayer1.text = "";
            else PowerPlayer1.text = GetPower(player).ToString();
        }
        else if (player == player2)
        {
            if (GetPower(player) == 0) PowerPlayer2.text = "";
            else PowerPlayer2.text = GetPower(player).ToString();
        }
    }

    public int GetPower(Player player)
    {
        int sum = 0;
        foreach (var RangeSection in board.sections[player.ID].Values)
        {
            if (RangeSection.Count != 0)
            {
                foreach (Card card in RangeSection)
                {
                    if (card is Card.UnityCard unity)
                    {
                        sum += unity.Power;

                        //If the card power is 0
                        //Destroy it
                        if (unity.Power <= 0)
                        {
                            unity.Owner.GraveYard.Add(unity);
                            CardBeaten(unity);
                        }
                    }
                }
            }
        }
        player.Score = sum;
        return sum;
    }

    public void CardBeaten(Card card)
    {
        Destroy(card.CardPrefab);
    }

    //Shuffle deck
    public List<Card> Shuffle(List<Card> cards)
    {
        List<Card> aux = new List<Card>();
        int n = 0;

        for (int i = 0; i < cards.Count; i++)
        {
            n = Random.Range(0, cards.Count);
            aux.Add(cards[n]);
            cards.RemoveAt(n);
        }

        return aux;
    }
    public Player GetStarterPlayer()
    {
        int n = Random.Range(0, 2);
        return (n == 0) ? player2 : player1;
    }

    public void ChangeTurn()
    {
        //End the round if both of the players passed
        if (player1.Passed && player2.Passed)
        {
            RoundOver();
        }

        //Rotate the scene
        RotateObjects();

        //Change the current player
        if (currentPlayer == player1) currentPlayer = player2;
        else currentPlayer = player1;
        Debug.Log("Se cambio el turno");

        //Change the hand panel to the other player
        ChangeHandPanel();
    }

    public void ChangeHandPanel()
    {
        foreach (RectTransform card in HandPanel)
        {
            Destroy(card.gameObject);
        }

        if (!currentPlayer.Ready)
        {
            if (Round == 1)
            {
                StartCoroutine(InstanciarCartas(5, currentPlayer));
            }
            else
            {
                StartCoroutine(InstanciarCartas(2, currentPlayer));
                InstantiateCurrentHand();
            }
        }
        else
        {
            InstantiateCurrentHand();
        }
    }
    public void InstantiateCurrentHand()
    {
        foreach (Card card in currentPlayer.Hand)
        {
            GameObject CardInstance = Instantiate(cardPrefab, HandPanel);
            disp = CardInstance.GetComponent<DisplayCard>();
            //Reset the cardPrefab property to the new instance
            card.CardPrefab = CardInstance;
            disp.card = card;
            disp.ShowCard();
        }
    }

    public void InstantiateCard(Card card)
    {
        GameObject CardInstance = Instantiate(cardPrefab, HandPanel);
        disp = CardInstance.GetComponent<DisplayCard>();
        //Reset the cardPrefab property to the new instance
        card.CardPrefab = CardInstance;
        disp.card = card;
        disp.ShowCard();
    }

    public void RotateObjects()
    {
        RotateObject(GameBoard);
        RotateObject(PassButton);
        RotateObject(ScorePlayer1);
        RotateObject(ScorePlayer2);

        //Rota a todos los objetos hijos de CardZones
        foreach (RectTransform child in CardZones.transform)
        {
            RotateObject(child.gameObject);
        }
        
    }
    
    public void RotateObject(GameObject objectToRotate)
    {
        // Rotate especified object 180 degrees around Z edge
        objectToRotate.transform.Rotate(0, 0, 180);
    }

    public void RoundOver()
    {
        player1.Ready = false;
        player2.Ready = false;
        //Get the winner of the round
        Player winner = GetRoundWinner();
        //Add the round
        Round++;

        //Add round won to player property
        if (winner != null)
        {
            if (winner == player1) player1.RoundsWon++;
            else player2.RoundsWon++;
        }

        //Check if the game is over
        if (player1.RoundsWon == 2 || player2.RoundsWon == 2 || Round == 3)
        {
            GameOver();
        }
        else
        {
            CleanBoard();
        }
    }

    public void DrawCards(int n)
    {

    }

    public Player GetRoundWinner()
    {
        if (player1.Score > player2.Score) return player1;
        else if (player2.Score > player1.Score) return player2;
        return null;
    }

    public void GameOver()
    {
        //Implementar mas tarde 
        Debug.Log("Se acabo el juego");
    }
    public void CleanBoard()
    {
        //Clean the backend board removing each card of the lists of cards
        //that represents the unity zones
        var AllSections = board.sections;
        foreach (var PlayerSection in AllSections)
        {
            var RangeSection = PlayerSection.Value;
            foreach (var Cards in RangeSection.Values)
            {
                for (int i = Cards.Count - 1; i >= 0; i--)
                {
                    //Erase card from board and add it to the player graveyard
                    Cards[i].Owner.GraveYard.Add(Cards[i]);
                    //Set the IsPlayed to false so it can be played again
                    Cards[i].IsPlayed = false;
                    //Destroy the instance (object)
                    Destroy(Cards[i].CardPrefab);
                    Cards.Remove(Cards[i]);
                }

            }
        }

        //Clean the backend climate section
        for (int i = 0; i < board.climate_section.Length; i++)
        {
            if (board.climate_section[i] != null)
            {
                Card card = board.climate_section[i];
                card.Owner.GraveYard.Add(card);
                //Destroy the object
                Destroy(card.CardPrefab);
                board.climate_section[i] = null;
            }
        }

        //Clean the increment section backend
        var IncrementSection = board.increment_section;
        foreach (Card[] cards in IncrementSection.Values)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                if (cards[i] != null)
                {
                    Card card = cards[i];
                    card.Owner.GraveYard.Add(card);
                    //Destroy the object
                    Destroy(cards[i].CardPrefab);
                    cards[i] = null;
                }
            }
        }
    }


    //public Board Board { get; set; }
    //public int Round { get; set; }
    //public Player player1 { get; set; }
    //public Player player2 { get; set; }
    //public Player CurrentPlayer { get; set; }
    //public Player OpponentPlayer { get; set; }
    //public bool GameOver { get; set; }




    //public Player SecondPlayer(Player player1, Player player2, Player CurrentPlayer)
    //{
    //    if (CurrentPlayer == player1)
    //    {
    //        return player2;
    //    }
    //    return player1;
    //}

    //public void ChangeTurn()
    //{
    //    //Rotar la camara
    //    if (CurrentPlayer == player1)
    //    {
    //        CurrentPlayer = player2;
    //        OpponentPlayer = player1;
    //    }
    //    else
    //    {
    //        CurrentPlayer = player1;
    //        OpponentPlayer = player2;
    //    }
    //}



    //public void CleanBoard()
    //{
    //    
    //}

    //public void EndRound()
    //{
    //    //Determine winner
    //    int player1_score = player1.Score;
    //    int player2_score = player2.Score;
    //    if (player1_score > player2_score)
    //    {
    //        player1.RoundsWon++;
    //        player1 = CurrentPlayer;
    //        player2 = OpponentPlayer;
    //    }
    //    else if (player2_score > player1_score)
    //    {
    //        player2.RoundsWon++;
    //        player2 = CurrentPlayer;
    //        player1 = OpponentPlayer;
    //    }
    //    else
    //    {
    //        if (player1.Leader.effectType == EffectType.TieIsWin && player1.Leader.Played == true)
    //        {
    //            player1.RoundsWon++;
    //            player1 = CurrentPlayer;
    //            player2 = OpponentPlayer;
    //        }
    //        else if (player2.Leader.effectType == EffectType.TieIsWin && player2.Leader.Played == true)
    //        {
    //            player2.RoundsWon++;
    //            player2 = CurrentPlayer;
    //            player1 = OpponentPlayer;
    //        }
    //    }
    //    //Check if game is over
    //    if (player1.RoundsWon == 2 || player2.RoundsWon == 2)
    //    {
    //        GameOver = true;
    //    }
    //    else
    //    {
    //        //Start next round
    //        Round++;
    //        StartRound();
    //    }
    //}

    //public bool ValidMove(Card card, string range)
    //{

    //    foreach (char Range in range)
    //    {
    //        if (card is Card.UnityCard unity_card && Board.sections[card.Owner.ID].ContainsKey(range.ToString()))
    //        {
    //            return true;
    //        }
    //        else if (card is Card.SpecialCard special_card && special_card.Type == SpecialType.Decoy)
    //        {
    //            return true;
    //        }
    //        else if (card is Card.SpecialCard climate_card && climate_card.Type == SpecialType.Climate)
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    //public void PlayCard(string range, Card card)
    //{
    //    //range is the place where the player drop the card
    //    bool is_valid = ValidMove(card, range);
    //    if (is_valid && card.Owner.Hand.Contains(card))
    //    {
    //        if (card is Card.SpecialCard climate_card)
    //        {
    //            if (climate_card.Range == "M" && Board.climate_section[0] != null)
    //            {
    //                Board.climate_section[0] = climate_card;
    //            }
    //            else if (climate_card.Range == "R" && Board.climate_section[1] != null)
    //            {
    //                Board.climate_section[1] = climate_card;
    //            }
    //            else if (climate_card.Range == "S" && Board.climate_section[2] != null)
    //            {
    //                Board.climate_section[2] = climate_card;
    //            }
    //        }
    //        else
    //        {
    //            //Add card to board
    //            Board.sections[card.Owner.ID][range].Add(card);
    //        }
    //        //Remove card from the player hand
    //        card.Owner.Hand.Remove(card);
    //        //Apply effects of the card
    //        Effect.Effects[card.effectType].Invoke(card);
    //        //Calculate player score
    //        int player_score = SumPowerInSection(card.Owner);
    //        card.Owner.Score = player_score;
    //        //Change turn 
    //        ChangeTurn();
    //    }
    //}


}
