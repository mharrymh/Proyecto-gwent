using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class DragAndDrop : MonoBehaviour
{
    public bool isDragging = false;
    public bool isOverDropZone = false;
    //Gets the panels 
    public GameObject DropZone;
    //Save the start position
    public Vector2 startPosition;
    //Gets the card
    public Card card;
    //The max of cards allow per panel
    public int max_sections = 6;
    //Relate the range with the position in climate_section
    Dictionary<string, int> relate = new Dictionary<string, int>()
        {
            {"M", 0}, {"R", 1}, {"S", 2}
        };

    //Gets references
    Board board = Board.Instance;
    Effects CardEffects;
    public GameManager gm;
    public SoundManager soundM;

    // Event Declaration
    public event Action OnDragStart;

    private void Awake()
    {
        CardEffects = new Effects();
        //Get the GameManagerObject
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        soundM = GameObject.Find("AudioSourceEffects").GetComponent<SoundManager>();
    }

    void Update()
    {
        if (isDragging)
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
    }

    //This method is called when my card collides with an object 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        DropZone = collision.gameObject;
    }

    //This method is called when stops card stops colliding with an object
    private void OnTriggerExit2D(Collider2D collision)
    {
        DropZone = null;
    }


    public void StartDrag()
    {
        DisplayCard disp = gameObject.GetComponent<DisplayCard>();
        card = disp.card;
        if (!card.IsPlayed && !isDragging)
        {
            startPosition = gameObject.transform.position;
            isDragging = true;

            // Shoot the event when drag starts
            OnDragStart?.Invoke();
        }

        //Save the pos to the leader card
        else if (card is Card.LeaderCard leader)
        {
            startPosition = gameObject.transform.position;
        }
    }
    public void EndDrag()
    {
        if (!card.IsPlayed)
        {
            isDragging = false;

            if (DropZone != null) DropCard(card);
            else transform.position = startPosition;
        }
    }

    public void DropCard(Card card)
    {
        if (DropZone.name.Contains("Deck") && gm.Round == 1)
        {
            if (DropZone.name.Contains("1") && gm.currentPlayer == gm.player1 && !gm.player1.HasPlayed && gm.player1.Changes < 2
                && gm.player1.PlayerDeck.Count > 0) gm.ChangeCard(card, gm.player1);
            else if (DropZone.name.Contains("2") && gm.currentPlayer == gm.player2 && !gm.player2.HasPlayed && gm.player2.Changes < 2
                && gm.player2.PlayerDeck.Count > 0) gm.ChangeCard(card, gm.player2);
            else transform.position = startPosition;

        }
        //Check for each dropable card type
        else if (card is Card.UnityCard unity_card)
        {
            if (unity_card.Range.Contains("M") && unity_card.Owner.ID == "player1"
                && DropZone.name == "MeleeZonePlayer1" && board.sections[card.Owner.ID]["M"].Count < max_sections)
            {
                PlayCard(unity_card, "M");
            }
            else if (unity_card.Range.Contains("M") && unity_card.Owner.ID == "player2"
                && DropZone.name == "MeleeZonePlayer2" && board.sections[card.Owner.ID]["M"].Count < max_sections)
            {
                PlayCard(unity_card, "M");
            }
            else if (unity_card.Range.Contains("R") && unity_card.Owner.ID == "player1"
                && DropZone.name == "RangeZonePlayer1" && board.sections[card.Owner.ID]["R"].Count < max_sections)
            {
                PlayCard(unity_card, "R");
            }
            else if (unity_card.Range.Contains("R") && unity_card.Owner.ID == "player2"
                && DropZone.name == "RangeZonePlayer2" && board.sections[card.Owner.ID]["R"].Count < max_sections)
            {
                PlayCard(unity_card, "R");
            }
            else if (unity_card.Range.Contains("S") && unity_card.Owner.ID == "player1"
                && DropZone.name == "SiegeZonePlayer1" && board.sections[card.Owner.ID]["S"].Count < max_sections)
            {
                PlayCard(unity_card, "S");
            }
            else if (unity_card.Range.Contains("S") && unity_card.Owner.ID == "player2"
                && DropZone.name == "SiegeZonePlayer2" && board.sections[card.Owner.ID]["S"].Count < max_sections)
            {
                PlayCard(unity_card, "S");
            }
            else
            {
                //If not come back to player hand
                transform.position = startPosition;
            }
        }
        else if (card is Card.SpecialCard climate_card && climate_card.Type is SpecialType.Climate
            && DropZone.name == "ClimateZone")
        {
            if (climate_card.Range == "M" && board.climate_section[0] == null)
            {
                PlayCard(climate_card, "M");
            }
            else if (climate_card.Range == "R" && board.climate_section[1] == null)
            {
                PlayCard(climate_card, "R");
            }
            else if (climate_card.Range == "S" && board.climate_section[2] == null)
            {
                PlayCard(climate_card, "S");
            }
            else
            {
                //If not come back to player hand
                transform.position = startPosition;
            }
        }
        else if (card is Card.SpecialCard increment_card && increment_card.Type is SpecialType.Increment)
        {
            if (increment_card.Range == "M" && increment_card.Owner.ID == "player1"
                && DropZone.name == "IncrementMeleePlayer1"
                && board.increment_section[card.Owner.ID][0] == null)
            {
                PlayCard(increment_card, "M");
            }
            else if (increment_card.Range == "M" && increment_card.Owner.ID == "player2"
                && DropZone.name == "IncrementMeleePlayer2"
                && board.increment_section[card.Owner.ID][0] == null)
            {
                PlayCard(increment_card, "M");
            }
            else if (increment_card.Range == "R" && increment_card.Owner.ID == "player1"
                && DropZone.name == "IncrementRangePlayer1"
                && board.increment_section[card.Owner.ID][1] == null)
            {
                PlayCard(increment_card, "R");
            }
            else if (increment_card.Range == "R" && increment_card.Owner.ID == "player2"
                && DropZone.name == "IncrementRangePlayer2"
                && board.increment_section[card.Owner.ID][1] == null)
            {
                PlayCard(increment_card, "R");
            }
            else if (increment_card.Range == "S" && increment_card.Owner.ID == "player1"
                && DropZone.name == "IncrementSiegePlayer1"
                && board.increment_section[card.Owner.ID][2] == null)
            {
                PlayCard(increment_card, "S");
            }
            else if (increment_card.Range == "S" && increment_card.Owner.ID == "player2"
                && DropZone.name == "IncrementSiegePlayer2"
                && board.increment_section[card.Owner.ID][2] == null)
            {
                PlayCard(increment_card, "S");
            }
            else
            {
                //If not come back to player hand
                transform.position = startPosition;
            }
        }
        else if (card is Card.SpecialCard cleareance && cleareance.Type is SpecialType.Clearance
            && (DropZone.name == "ClimateZone" || DropZone.transform.parent.gameObject.name == "ClimateZone"))
        {
            PlayCard(cleareance);
        }
        else if (card is Card.SpecialCard decoy && decoy.Type is SpecialType.Decoy && DropZone.tag == card.Owner.ID
            && DropZone.transform.parent != gm.HandPanel && DropZone.transform.parent.name != "ClimateZone"
            && !DropZone.transform.parent.name.Contains("Increment"))
        {
            Debug.Log(DropZone.tag);
            PlayCard(card);
        }
        else
        {
            transform.position = startPosition;
        }
    }

    public void PlayCard(Card card, string range = "")
    {
        soundM.PlayCardSound();


        card.Owner.HasPlayed = true;
        //Set the card to true so that it 
        //wont interact anymore with the drag and drop
        card.IsPlayed = true;
        //Remove card from the player hand
        card.Owner.Hand.Remove(card);
        //Drop card
        transform.SetParent(DropZone.transform, false);
        //Disable passed property if you play a card 
        gm.currentPlayer.Passed = false;
        //Apply effect
        CardEffects.CardEffects[card.effectType].Invoke(card);

        //Add card in backend
        if (card is Card.UnityCard unity_card)
        {
            board.sections[unity_card.Owner.ID][range].Add(unity_card);
            //Check if there are special cards on the board
            if (range == "M")
            {
                if (board.climate_section[0] != null) unity_card.Power--;
                else if (board.increment_section[unity_card.Owner.ID][0] != null
                    && unity_card.UnityType == UnityType.Silver) unity_card.Power++;
            }
            else if (range == "R")
            {
                if (board.climate_section[1] != null) unity_card.Power--;
                else if (board.increment_section[unity_card.Owner.ID][1] != null
                    && unity_card.UnityType == UnityType.Silver) unity_card.Power++;
            }
            else if (range == "S")
            {
                if (board.climate_section[2] != null) unity_card.Power--;
                else if (board.increment_section[unity_card.Owner.ID][2] != null
                    && unity_card.UnityType == UnityType.Silver) unity_card.Power++;
            }

        }
        else if (card is Card.SpecialCard climate_card && climate_card.Type == SpecialType.Climate)
        {
            string key = range;
            int value;
            if (relate.TryGetValue(key, out value))
            {
                board.climate_section[value] = climate_card;
            }

            if (range == "M") range = "Melee";
            else if (range == "R") range = "Ranged";
            else range = "Siege";

            gm.StartCoroutine(gm.SetAuxText("Se aplic� un clima en la zona " + range));
        }
        else if (card is Card.SpecialCard increment_card && increment_card.Type == SpecialType.Increment)
        {
            string key = range;
            int value;
            if (relate.TryGetValue(key, out value))
            {
                board.increment_section[increment_card.Owner.ID][value] = increment_card;
            }

            if (range == "M") range = "Melee";
            else if (range == "R") range = "Ranged";
            else range = "Siege";

            gm.StartCoroutine(gm.SetAuxText("Se aplic� un incremento en la zona " + range + " de " + gm.currentPlayer.PlayerName));
        }
        else if (card is Card.SpecialCard cleareance && cleareance.Type is SpecialType.Clearance)
        {
            cleareance.Owner.GraveYard.Add(cleareance);
            cleareance.IsPlayed = false;
            gm.CardBeaten(cleareance);
        }
        else if (card is Card.SpecialCard decoy && decoy.Type is SpecialType.Decoy)
        {
            //Get the card to move
            GameObject CardToMove = decoy.CardPrefab.transform.parent.gameObject;
            //Move the decoy card to the parent transform
            decoy.CardPrefab.transform.SetParent(CardToMove.transform.parent.transform, false);

            string Zone = CardToMove.transform.parent.name;
            string BackendZone = null;
            string PlayerZone = card.Owner.ID;

            if (Zone.Contains("Melee"))
            {
                BackendZone = "M";
            }
            else if (Zone.Contains("Range"))
            {
                BackendZone = "R";
            }
            else if (Zone.Contains("Siege"))
            {
                BackendZone = "S";
            }

            if (BackendZone != null) CardEffects.Decoy(CardToMove.name, BackendZone, PlayerZone, decoy);
            else
            {
                transform.position = startPosition;
                decoy.IsPlayed = false;
                //Add card to the player hand
                decoy.Owner.Hand.Add(card);
            }
            //Move the card to the player hand 
            CardToMove.transform.SetParent(gm.HandPanel.transform, false);
        }

        //Update the power
        gm.SetPower(gm.player1);
        gm.SetPower(gm.player2);

        
        //Change turn
        gm.ChangeTurn();      
    }
}


