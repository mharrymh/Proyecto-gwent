using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject GameBoard;
    public GameObject cardPrefab; // The card prefab
    public Transform parentTransform; // The location of the object where you want to instantiate the card
    public Transform LeaderPlayer1;
    public Transform LeaderPlayer2;

    public int Round;
    public Player currentPlayer;
    public bool GameOver;

    //The players
    public Player player1;
    public Player player2;
    //The points in the UI
    public TMP_Text PowerPlayer1;
    public TMP_Text PowerPlayer2; 

    //References
    CardDatabase cartas = new CardDatabase();
    Board board = Board.Instance;

    void Start()
    {
        Round = 0;
        player1 = new Player(CardFaction.Dark, "player1");
        player2 = new Player(CardFaction.Light, "player2");
        currentPlayer = GetStarterPlayer();
        StartCoroutine(InstanciarCartas(8, currentPlayer));
    }

    private void Update()
    {
        SetPower(player1);
        SetPower(player2);
    }

    IEnumerator InstanciarCartas(int n, Player player)
    {
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
            GameObject instanciaCarta = Instantiate(cardPrefab, parentTransform);
            //Get the DisplayCard component of the new Card
            DisplayCard disp = instanciaCarta.GetComponent<DisplayCard>();
            //Assign the card to it
            disp.card = card;
            //Add card to player hand
            player.AssignHand(card);
            card.Owner = player;

            if (!player.LeaderPlayed)
            {
                if (player == player1) instanciaCarta = Instantiate(cardPrefab, LeaderPlayer1);
                else if (player == player2) instanciaCarta = Instantiate(cardPrefab, LeaderPlayer2);

                disp = instanciaCarta.GetComponent<DisplayCard>();
                disp.card = player.Leader;
                player.LeaderPlayed = true;
            }
        }
        else
        {
            Debug.Log("El mazo esta vacio");
        }


    }
    public void SetPower(Player player)
    {
        if (player == player1)
        {
            if (GetPower(player) == 0) PowerPlayer1.text = "";
            else PowerPlayer1.text = GetPower(player).ToString();
        }
        else
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
                foreach (Card.UnityCard card in RangeSection)
                {
                    sum += card.Power;
                }
            }
        }

        player.Score = sum;
        return sum;
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
        int n = Random.Range(0, 1);
        return (n == 0) ? player1 : player2;
    }


    
    public void ChangeTurn()
    {
        RotateObject(GameBoard);

        if (currentPlayer == player1) currentPlayer = player2;
        else currentPlayer = player1;
        Debug.Log("Se cambio el turno");
    }

    //Rotar los demas objetos
    public void RotateObject(GameObject objectToRotate)
    {
        // Rota el objeto especificado 180 grados alrededor del eje Y
        objectToRotate.transform.Rotate(0, 0, 180);
    }




    //public Board Board { get; set; }
    //public int Round { get; set; }
    //public Player player1 { get; set; }
    //public Player player2 { get; set; }
    //public Player CurrentPlayer { get; set; }
    //public Player OpponentPlayer { get; set; }
    //public bool GameOver { get; set; }


    //public GameManager()
    //{
    //    Board = Board.Instance;
    //    Round = 1;
    //    player1 = new Player(CardFaction.Light, "player1");
    //    player2 = new Player(CardFaction.Dark, "player2");
    //    CurrentPlayer = StarterPlayer(player1, player2);
    //    OpponentPlayer = SecondPlayer(player1, player2, CurrentPlayer);
    //    GameOver = false;
    //}

    //public Player StarterPlayer(Player player1, Player player2)
    //{
    //    Random rand = new Random();
    //    return rand.Next(2) == 0 ? player1 : player2;
    //}

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

    //public void StartRound()
    //{
    //    if (Round == 1)
    //    {
    //        player1.Hand = player1.AssignHand();
    //        player2.Hand = player2.AssignHand();
    //    }
    //    else
    //    {
    //        CleanBoard();
    //        player1.DrawCard(2);
    //        player2.DrawCard(2);
    //    }
    //}

    //public void CleanBoard()
    //{
    //    var AllSections = Board.sections;
    //    foreach (var PlayerSection in AllSections)
    //    {
    //        var RangeSection = PlayerSection.Value;
    //        foreach (var Cards in RangeSection.Values)
    //        {
    //            foreach (Card card in Cards)
    //            {
    //                card.Owner.GraveYard.Add(card);
    //                Cards.Remove(card);
    //            }
    //        }
    //    }

    //    for (int i = 0; i < Board.climate_section.Length; i++)
    //    {
    //        if (Board.climate_section[i] != null)
    //        {
    //            Card card = Board.climate_section[i];
    //            card.Owner.GraveYard.Add(card);
    //            card = null;
    //        }
    //    }
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
