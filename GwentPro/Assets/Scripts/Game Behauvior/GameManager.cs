using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public VisualManager visualManager;
    public GameObject GameBoard;
    public GameObject CardZones;
    public GameObject PassButton;
    public GameObject ScorePlayer1;
    public GameObject ScorePlayer2;
    public GameObject LivesPlayer1;
    public GameObject LivesPlayer2;
    public GameObject Graveyard1;
    public GameObject Graveyard2;
    //The amount of effects applied in one single turn
    public int CardsInstantiated;


    public GameObject cardPrefab; // The card prefab
    public Transform HandPanel;
    #region Board Zones
    public Transform MeleePlayer1;
    public Transform RangePlayer1;
    public Transform SiegePlayer1;
    public Transform MeleePlayer2;
    public Transform RangePlayer2;
    public Transform SiegePlayer2;
    #endregion
    #region Increment Zones
    public Transform IncrementMeleePlayer1;
    public Transform IncrementRangePlayer1;
    public Transform IncrementSiegePlayer1;
    public Transform IncrementMeleePlayer2;
    public Transform IncrementRangePlayer2;
    public Transform IncrementSiegePlayer2;
    #endregion

    public Transform ClimateZone;
    //The leader panels
    public Transform LeaderPlayer1;
    public GameObject LeaderButtonPlayer1;
    public Transform LeaderPlayer2;
    public GameObject LeaderButtonPlayer2;

    //Initialize the card counter
    public int Round;
    public bool GivingCards = false;
    public bool RoundOverAux = false;
    public Player currentPlayer;
    //Amount of cards to start with
    public int StartingAmount = 8;
    //public bool GameOver;

    //The players
    public Player player1;
    public Player player2;

    public Player NotCurrentPlayer
    {
        get
        {
            return GetNotCurrentPlayer();
        }
    }

    public Player winner = null;
    //The points in the UI
    public TMP_Text PowerPlayer1;
    public TMP_Text PowerPlayer2;


    public GameObject panelChangeTurn;
    public TMP_Text TurnOfText;
    DragAndDrop dragAndDrop;
    readonly Board board = Board.Instance;
    DisplayCard disp;

    /// <summary>
    /// This is called when game manager object is instantiated
    /// </summary>
    void Start()
    {
        CardsInstantiated = 0;
        visualManager = GameObject.Find("VisualManager").GetComponent<VisualManager>();
        //Start the round at 1
        Round = 1;

        // Instantiate the players
        player1 = new Player(PlayerData.FactionPlayer1, "player1", PlayerData.Player1Name, Graveyard1);
        player2 = new Player(PlayerData.FactionPlayer2, "player2", PlayerData.Player2Name, Graveyard2);

        //Set the power at 0
        SetPower();

        currentPlayer = GetStarterPlayer();
        //If the starter player is player2 rotate the scene
        if (currentPlayer == player2) RotateObjects();
        //Draw cards to both players
        StartCoroutine(InstanciarCartas(StartingAmount, currentPlayer));

        //This text will be showed in the game when the game starts
        visualManager.Add("Before returning any card, you can return two of them to the player deck and change it.");
        visualManager.DisplayAuxiliarText();

        //Show visually whose the turn is 
        StartCoroutine(VisualTurn());
    }

    //Close application if esc key is selected at any moment
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (visualManager.IsDisplaying && !visualManager.PointerInsideAuxPanel && Input.GetMouseButtonDown(0))
        {
            visualManager.CloseAuxPanel();
        }
    }

    IEnumerator InstanciarCartas(int drawAmount, Player player)
    {
        //Leader effect
        if (player.Leader.EffectType != null && player.Leader.EffectType.ToString() == "DrawExtraCard" && Round > 1) 
            drawAmount++;

        player.Ready = true;
        if (player.PlayerDeck.Count < drawAmount) drawAmount = player.PlayerDeck.Count;
        player.PlayerDeck.Shuffle();

        //Do not change turns while instantiating
        GivingCards = true;
        for (int i = 0; i < drawAmount; i++)
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
            //Add card to player hand (Adding the card remove it from its parent source)
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
                player.Leader.CardPrefab = CardInstance;
                //Show the apply effect button of the leader
                if (player.Leader.UserCardEffects != null) {
                    Dictionary<Player, GameObject> leaderButtons = new Dictionary<Player, GameObject>
                    {
                        {player1, LeaderButtonPlayer1},
                        {player2, LeaderButtonPlayer2}
                    };

                    leaderButtons[player].SetActive(true);
                }
            }
        }
    }

    public void InstantiateInHand(Card card, Player player)
    {
        if (player.Hand.Count <= 10)
        {
            InstantiateIn(card, HandPanel, player);
        }
        //Move card to the graveyard direcrtly 
        else 
        {
            card.Owner = player;
            player.GraveYard.Add(card);
        }
    }
    private Player GetNotCurrentPlayer()
    {
        return (currentPlayer == player1) ? player2 : player1;
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

    public void SetPower()
    {
        SetPower(player1);
        SetPower(player2);
    }

    public void SetPower(Player player)
    {
        //Get the power of the player 
        int powerOfPlayer = GetPower(player);
        if (player == player1)
        {
            if (powerOfPlayer == 0) PowerPlayer1.text = "";
            else PowerPlayer1.text = powerOfPlayer.ToString();
        }
        else if (player == player2)
        {
            if (powerOfPlayer == 0) PowerPlayer2.text = "";
            else PowerPlayer2.text = powerOfPlayer.ToString();
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


                        //Update card power
                        if (unity.CardPrefab != null)
                        {
                            var displayCard = unity.CardPrefab.GetComponent<DisplayCard>();
                            displayCard.PowerText.text = unity.Power.ToString();
                        }
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
    /// <summary>
    /// Get starter player by random
    /// </summary>
    /// <returns></returns>
    private Player GetStarterPlayer()
    {
        int random = UnityEngine.Random.Range(0, 2);
        return (random == 0) ? player2 : player1;
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
    #region Change Turn Management
    public void ChangeTurn()
    {
        CardsInstantiated = 0;
        //Don't change cards if the giving cards coroutine is running 
        if (GivingCards) return;

        Player perspective = currentPlayer;


        Dictionary<Player, GameObject> leaderButtons = new Dictionary<Player, GameObject>
        {
            {player1, LeaderButtonPlayer1},
            {player2, LeaderButtonPlayer2}
        };
        //Set inactive the not current player leader effect
        if (currentPlayer.Leader.UserCardEffects != null && !currentPlayer.Leader.Played)
            leaderButtons[currentPlayer].SetActive(false);


        //Change the current player
        currentPlayer = (currentPlayer == player1)? player2 : player1; 

        //Set active the apply effect button
        if (currentPlayer.Leader.UserCardEffects != null && !currentPlayer.Leader.Played)
            leaderButtons[currentPlayer].SetActive(true);


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
        SetPower();

        StartCoroutine(VisualTurn());
        //The visual of the effects applied etc
        visualManager.DisplayAuxiliarText();
    }

    IEnumerator VisualTurn()
    {
        if (RoundOverAux)
        { 
            yield return StartCoroutine(VisualSayWinner());
            RoundOverAux = false;
        }


        TurnOfText.text = "Turn of " + currentPlayer.PlayerName;
        panelChangeTurn.SetActive(true);

        //Wait 2 second
        yield return new WaitForSeconds(2);

        panelChangeTurn.SetActive(false);
    }

    public int GetTotalAmountOfCardsPlayed()
    {
        Context context = new Context();
        return context.BoardCards.Count;
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
        if (winner != null) TurnOfText.text = $"\"{winner.PlayerName}\" wins the round.";
        else TurnOfText.text = "Tie";

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
    public void CleanBoard()
    {
        //Effect of the leader
        //Keep a random card on the board between rounds
        List<Player> KeepRandomPlayer = GetPlayerWithLeader("KeepRandomCard");
        List<Card.UnityCard> Keepers = new List<Card.UnityCard>();

        if (KeepRandomPlayer.Count > 0)
        {
            foreach (Player player in KeepRandomPlayer)
            {
                Keepers.Add(GetRandomUnityCardOnBoard(player)); 
            }
            foreach (Card.UnityCard unityCard in Keepers)
            {
                unityCard.Power = unityCard.OriginalPower;
            }
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
                    if (Cards[i] is not Card.UnityCard unity || !Keepers.Contains(unity))
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
            n = UnityEngine.Random.Range(0, aux.Count);
            return aux[n];
        }
        return null;
    }

    List<Player> GetPlayerWithLeader(string keepRandomCard)
    {
        List<Player> players = new List<Player>();
        if (player1.Leader.EffectType.ToString() == keepRandomCard) players.Add(player1);
        if (player2.Leader.EffectType.ToString() == keepRandomCard) players.Add(player2);
        return players;
    }
    #endregion

    #region InstantiateAndPlay
    private void CheckAndIncreaseEffectsApplied()
    {
        if (++CardsInstantiated >= 3)
        {
            ExecutionError OverCardsApplied = new OverCardsApplied();
            throw OverCardsApplied;
        }
    }
    public void InstantiateAndPlay(Card item, Player player)
    {
        CheckAndIncreaseEffectsApplied();
        if (item is Card.UnityCard unityCard)
        {
            InstantiateUnityCard(unityCard, player);
        }
        else if (item is Card.ClimateCard climateCard)
        {
            InstantiateClimateCard(climateCard, climateCard.Range, player);
        }
        else if (item is Card.CleareanceCard cleareanceCard)
        {
            InstantiateClearanceCard(cleareanceCard, player);
        }
        else if (item is Card.IncrementCard incrementCard)
        {
            InstantiateIncrementCard(incrementCard, incrementCard.Range, player);
        }
       //Decoy cards cant be instantiated, error thrown before in execution
    }

    private void InstantiateClearanceCard(Card.CleareanceCard cleareanceCard, Player player)
    {
        //Play the card
        dragAndDrop.PlayCardFromEffect(cleareanceCard,"");
    }   

    private void InstantiateIncrementCard(Card.IncrementCard incrementCard, string range, Player player)
    {
        bool added = true;
        string rangeAux = range;
        if (range.Contains("M") && board.increment_section[player.ID][0] == null) { board.climate_section[0] = incrementCard;     rangeAux = "M";}
        else if (range.Contains("R") && board.increment_section[player.ID][1] == null) {board.climate_section[1] = incrementCard; rangeAux = "R";}
        else if (range.Contains("S") && board.increment_section[player.ID][2] == null) {board.climate_section[2] = incrementCard; rangeAux = "S"; }
        else // nothing was added
            added = false;

        if (!added) return;

        Dictionary<(string, string), Transform> relateDropZone = new Dictionary<(string, string), Transform>()
        {
            {("M", "player1"), IncrementMeleePlayer1},
            {("R", "player1"), IncrementRangePlayer1},
            {("S", "player1"), IncrementSiegePlayer1},
            {("M", "player2"), IncrementMeleePlayer2},
            {("R", "player2"), IncrementRangePlayer2},
            {("S", "player2"), IncrementSiegePlayer2},
        };

        Transform DropZone = relateDropZone[(rangeAux, player.ID)];

        InstantiateIn(incrementCard, DropZone , player);

        //Play the card
        //set drag and drop instance
        dragAndDrop = incrementCard.CardPrefab.GetComponent<DragAndDrop>();
        dragAndDrop.PlayCardFromEffect(incrementCard, rangeAux, DropZone);
    }

    private void InstantiateClimateCard(Card.ClimateCard climateCard, string range, Player player)
    {
        bool added = true;
        if (range.Contains("M") && board.climate_section[0] == null) { board.climate_section[0] = climateCard;     range = "M";}
        else if (range.Contains("R") && board.climate_section[1] == null) {board.climate_section[1] = climateCard; range = "R";}
        else if (range.Contains("S") && board.climate_section[2] == null) {board.climate_section[2] = climateCard; range = "S"; }
        else // nothing was added
            added = false;

        if (!added) return;

        InstantiateIn(climateCard, ClimateZone, player);

        //Play the card
        //set drag and drop instance
        dragAndDrop = climateCard.CardPrefab.GetComponent<DragAndDrop>();
        dragAndDrop.PlayCardFromEffect(climateCard, range, ClimateZone);
    }

    void InstantiateUnityCard(Card.UnityCard unityCard, Player player)
    {        
        string range = null;
        foreach (string rangeSection in board.sections[player.ID].Keys)
        {
            if (unityCard.Range.Contains(rangeSection))
            {
                range = rangeSection;
                board.sections[player.ID][range].Add(unityCard);
                break;
            }
        }

        Dictionary<(string, string), Transform> relateDropZone = new Dictionary<(string, string), Transform>()
        {
            {("M", "player1"), MeleePlayer1},
            {("R", "player1"), RangePlayer1},
            {("S", "player1"), SiegePlayer1},
            {("M", "player2"), MeleePlayer2},
            {("R", "player2"), RangePlayer2},
            {("S", "player2"), SiegePlayer2},
        };

        //Range is not null here
        Transform dropZone = relateDropZone[(range, player.ID)];

        InstantiateIn(unityCard, dropZone, player);

        //set drag and drop instance
        dragAndDrop = unityCard.CardPrefab.GetComponent<DragAndDrop>();
        //Play the card
        dragAndDrop.PlayCardFromEffect(unityCard, range, dropZone);
    }

    private void InstantiateIn(Card card, Transform dropZone, Player owner)
    {
        //Crate new instance of the card on the playerhand
        GameObject CardInstance = Instantiate(cardPrefab, dropZone);
        //Get the DisplayCard component of the new Card
        disp = CardInstance.GetComponent<DisplayCard>();
        //Assign the card to it
        disp.card = card;
        //Assign owner
        card.Owner = owner;
        //Assign tag to Instance of the card for decoy effect implementation
        CardInstance.tag = owner.ID;
        //Assign the cardPrefab to the card
        card.CardPrefab = CardInstance;
        
        if (player1.Field.Count > 0 && player1.Field[0].CardPrefab == null)
        {
            Debug.Log("es nulo " + player1.ID);
            Debug.Log("InstantiateIn");
        }
        if (player2.Field.Count > 0 && player2.Field[0].CardPrefab == null)
        {
            Debug.Log("es nulo " + player2.ID);
            Debug.Log("InstantiateIn");
        }
        //Set that the card hasnt been played
        if (dropZone == HandPanel)
            card.IsPlayed = false;
        //Display card
        disp.ShowCard();
    }

    #endregion

    #region Apply Leader Effect
    /// <summary>
    /// This is called by pressing the button under the leader card of player (if the leader card is a customized one)
    /// </summary>
    public void ApplyLeaderEffect()
    {
        Card.LeaderCard leader = currentPlayer.Leader;
        //Set drag and drop instance
        dragAndDrop = leader.CardPrefab.GetComponent<DragAndDrop>();
        //Apply effects of leader card
        dragAndDrop.ApplyLeaderEffect(leader);
        //The leader has been played 
        leader.Played = true;
    }

    #endregion
}
