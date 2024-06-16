using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public GameObject GameBoard;
    public GameObject CardZones;
    public GameObject PassButton;
    public GameObject ScorePlayer1;
    public GameObject ScorePlayer2;
    public GameObject LivesPlayer1;
    public GameObject LivesPlayer2;
    public GameObject Graveyard1;
    public GameObject Graveyard2;


    public GameObject cardPrefab; // The card prefab
    public Transform HandPanel;
    //The leader panels
    public Transform LeaderPlayer1;
    public Transform LeaderPlayer2;

    public int Round;
    public bool GivingCards = false;
    public bool RoundOverAux = false;
    public int RoundDraws;
    public Player currentPlayer;
    //Amount of cards to start with
    public int StartingAmount = 8;
    //public bool GameOver;

    //The players
    public Player player1;
    public Player player2;
    public Player winner = null;
    //The points in the UI
    public TMP_Text PowerPlayer1;
    public TMP_Text PowerPlayer2;


    public GameObject panelChangeTurn;
    public GameObject panelAux;
    public TMP_Text TurnOfText;
    public TMP_Text AuxText;



    //References
    CardDatabase cartas = new CardDatabase();
    Effects CardEffects;
    Board board = Board.Instance;
    DisplayCard disp;

    void Start()
    {
        CardEffects = new Effects();

        Round = 1;
        player1 = new Player(PlayerData.FactionPlayer1, "player1", PlayerData.Player1Name);
        player1.GraveyardObj = Graveyard1;
        player2 = new Player(PlayerData.FactionPlayer2, "player2",PlayerData.Player2Name);
        player2.GraveyardObj = Graveyard2;

        currentPlayer = GetStarterPlayer();
        //If the starter player is player2 rotate the scene
        if (currentPlayer == player2) RotateObjects();
        StartCoroutine(InstanciarCartas(StartingAmount, currentPlayer));

        //Start the power at 0
        SetPower(player1);
        SetPower(player2);

        StartCoroutine(SetAuxText("Antes de jugar cualquier carta, puedes devolver " +
            "hasta dos cartas no deseadas y cambiarlas simplemente arrastr�ndolas al deck"));

        StartCoroutine(VisualChangeTurn());
    }

    void Update()
    {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }

    IEnumerator InstanciarCartas(int n, Player player)
    {
        //Leader effect
        if (player.Leader.effectType == EffectType.DrawExtraCard && Round > 1) n++;

        player.Ready = true;
        if (player.PlayerDeck.Count < n) n = player.PlayerDeck.Count;
        Shuffle(player.PlayerDeck);

        //Do not change turns while instantiating
        GivingCards = true;
        for (int i = 0; i < n; i++)
        {
            if (player.Hand.Count < 10)
            {
                Instanciar(player);
                player.PlayerDeck.RemoveAt(0);
                yield return new WaitForSeconds(0.2f); // Wait .2 seconds before instantiate the new card
            }
            else
            {
                //If the hand is full send the card directly to the graveyard
                player.GraveYard.Add(player.PlayerDeck[0]);
                if (player.GraveYard.Count == 1)
                {
                    player.GraveyardObj.SetActive(true);
                }
                player.PlayerDeck.RemoveAt(0);
            }
        }
        GivingCards = false;
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
            player.Hand.Add(card);
            //Assign owner of the card
            card.Owner = player;
            //Assign tag to Instance of the card for decoy effect implementation
            CardInstance.tag = card.Owner.ID;
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
    }

    public void ChangeCard(Card card, Player player)
    {
        //Destroy card prefab
        CardBeaten(card);

        //Instantiate new card

        InstantiateCard(player.PlayerDeck[0], HandPanel);
        //Add new card to the player hand
        player.Hand.Add(player.PlayerDeck[0]);
        //Remove changed card from the player hand 
        player.Hand.Remove(card);
        //Remove card from player deck
        player.PlayerDeck.RemoveAt(0);
        //Insert changed card to the player deck in the last position 
        player.PlayerDeck.Insert(player.PlayerDeck.Count - 1, card);
        //Increment the property changes of the player 
        player.Changes++;
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
                for (int i = RangeSection.Count - 1; i >= 0; i--)
                {
                    if (RangeSection[i] is Card.UnityCard unity)
                    {
                        sum += unity.Power;

                        //If the card power is 0
                        //Destroy it
                        if (unity.Power <= 0)
                        {
                            unity.Owner.GraveYard.Add(unity);
                            RangeSection.RemoveAt(i);

                            if (unity.Owner.GraveYard.Count == 1)
                            {
                                unity.Owner.GraveyardObj.SetActive(true);
                            }
                            CardBeaten(unity);
                        }
                    }
                }


            }
        }
        player.Score = sum;
        return sum;
    }
    //Destroy prefab
    public void CardBeaten(Card card)
    {
        //Restore to the original power
        if (card is Card.UnityCard unity) unity.Power = unity.OriginalPower;
        card.IsPlayed = false;
        
        Destroy(card.CardPrefab);

        SetPower(player1);
        SetPower(player2);
    }
    //Shuffle deck
    private void Shuffle(List<Card> cards)
    {
        int n = 0;
        Card aux;

        for (int i = 0; i < cards.Count; i++)
        {
            n = Random.Range(0, cards.Count);

            aux = cards[n];
            cards[n] = cards[i];
            cards[i] = aux;
        }
    }
    private Player GetStarterPlayer()
    {
        int n = Random.Range(0, 2);
        return (n == 0) ? player2 : player1;
    }

    public void ChangeTurn()
    {
        if (GivingCards) return;

        Player perspective = currentPlayer;
        //Change the current player
        if (currentPlayer == player1) currentPlayer = player2;
        else currentPlayer = player1;

        //End the round if both of the players passed
        if (player1.Passed && player2.Passed)
        {
            RoundOver();
            RoundOverAux = true;
        }

        //Rotate the scene if the current player changed
        if (perspective != currentPlayer)
        {
            RotateObjects();
        }
        //Change the hand panel to the other player
        ChangeHandPanel();

        //Set Power for both players
        SetPower(player1);
        SetPower(player2);

        StartCoroutine(VisualChangeTurn());
    }

    IEnumerator VisualChangeTurn()
    {
        if (RoundOverAux)
        { 
            yield return StartCoroutine(VisualSayWinner());
            RoundOverAux = false;
        }


        TurnOfText.text = "Turno de " + currentPlayer.PlayerName;
        panelChangeTurn.SetActive(true);

        //Wait 2 second
        yield return new WaitForSeconds(2);

        panelChangeTurn.SetActive(false);
    }

    public IEnumerator SetAuxText(string text)
    {
        AuxText.text = text;

        panelAux.SetActive(true);

        yield return new WaitForSeconds(4);

        panelAux.SetActive(false);
    }


    public void ChangeHandPanel()
    {
        foreach (RectTransform card in HandPanel)
        {
            Destroy(card.gameObject);
        }

        //Instantiate hands
        if (!currentPlayer.Ready)
        {
            if (Round == 1)
            {
                StartCoroutine(InstanciarCartas(StartingAmount, currentPlayer));
            }
            else
            {
                InstantiateCurrentHand();
                StartCoroutine(InstanciarCartas(2, currentPlayer));
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
            InstantiateCard(card, HandPanel);
        }
    }
    public void InstantiateCard(Card card, Transform DropZone)
    {
        if (card.Owner == null) card.Owner = currentPlayer;


        GameObject CardInstance = Instantiate(cardPrefab, DropZone);
        disp = CardInstance.GetComponent<DisplayCard>();
        //Reset the cardPrefab property to the new instance
        card.CardPrefab = CardInstance;
        CardInstance.tag = card.Owner.ID;
        disp.card = card;
        disp.ShowCard();
        
    }

    public void RotateObjects()
    {
        RotateObject(GameBoard);
        RotateObject(PassButton);
        RotateObject(ScorePlayer1);
        RotateObject(ScorePlayer2);

        //Rotate all cardZones childs
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
        player1.Passed = false;
        player2.Passed = false;
        //Get the winner of the round
        winner = GetRoundWinner();
        if (winner != null)
        {
            //The winner start the next round
            currentPlayer = winner;
            //Add round to the player property
            if (winner == player1) player1.RoundsWon++;
            else player2.RoundsWon++;
        }        
        else
        {
            player1.RoundsWon++;
            player2.RoundsWon++;
        }


        //Check if the game is over
        if (player1.RoundsWon == 2 || player2.RoundsWon == 2 || Round == 3)
        {
            if (player1.RoundsWon > player2.RoundsWon) winner = player1;
            else if (player2.RoundsWon > player1.RoundsWon) winner = player2;
            else
            {
                winner = null;
            }
            CleanBoard();
            GameOver();
        }
        else
        {
            CleanBoard();
            SetVisualWinner(winner);
            Round++;
        }

       
    }

    IEnumerator VisualSayWinner()
    {
        if (winner != null) TurnOfText.text = "Gan� " + winner.PlayerName;
        else TurnOfText.text = "Empate";

        panelChangeTurn.SetActive(true);

        //Wait 2 second
        yield return new WaitForSeconds(2);

        panelChangeTurn.SetActive(false);
    }
    void SetVisualWinner(Player winner)
    {

        if (winner == player1) Destroy(LivesPlayer2);
        else if (winner == player2) Destroy(LivesPlayer1);
        else
        {
            Destroy(LivesPlayer1);
            Destroy(LivesPlayer2);
        }
        
    }
    public Player GetRoundWinner()
    {
        if (player1.Score > player2.Score) return player1;
        else if (player2.Score > player1.Score) return player2;
        return null;
    }

    public void GameOver()
    {
        if (winner != null)
        {
            PlayerData.Winner = winner.PlayerName;
        }
        //Change scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    void CleanBoard()
    {
        //Effect of the leader
        //Keep a random card on the board between rounds
        Player KeepRandomPlayer = GetPlayerWithLeader(EffectType.KeepRandomCard);
        Card.UnityCard Keeper = null;

        if (KeepRandomPlayer != null)
        {
            Keeper = GetRandomUnityCardOnBoard(KeepRandomPlayer);
            if (Keeper != null && Keeper) Keeper.Power = Keeper.OriginalPower;
        }


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
                    if (Keeper != Cards[i])
                    {
                        //Erase card from board and add it to the player graveyard
                        Cards[i].Owner.GraveYard.Add(Cards[i]);
                        if (Cards[i].Owner.GraveYard.Count == 1)
                        {
                            Cards[i].Owner.GraveyardObj.SetActive(true);
                        }
                        //Set the IsPlayed to false so it can be played again
                        Cards[i].IsPlayed = false;
                        //Destroy the instance (object)
                        Destroy(Cards[i].CardPrefab);
                        Cards.RemoveAt(i);
                    }
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
                if (card.Owner.GraveYard.Count == 1)
                {
                    card.Owner.GraveyardObj.SetActive(true);
                }
                //Destroy the object
                CardBeaten(card);
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
                    if (card.Owner.GraveYard.Count == 1)
                    {
                        card.Owner.GraveyardObj.SetActive(true);
                    }
                    //Destroy the object
                    CardBeaten(cards[i]);
                    cards[i] = null;
                }
            }
        }
    }

    Card.UnityCard GetRandomUnityCardOnBoard(Player player)
    {
        List<Card.UnityCard> aux = new List<Card.UnityCard>();
        int n;

        foreach (var RangeSection in board.sections[player.ID])
        {
            foreach (Card card in RangeSection.Value)
            {
                if (card is Card.UnityCard unity)
                {
                    aux.Add(unity);
                }
            }
        }

        if (aux.Count > 0)
        {
            n = Random.Range(0, aux.Count);
            return aux[n];
        }
        return null;
    }

    Player GetPlayerWithLeader(EffectType keepRandomCard)
    {
        if (player1.Leader.effectType == keepRandomCard) return player1;
        else if (player2.Leader.effectType == keepRandomCard) return player2;

        else return null;
    }
}