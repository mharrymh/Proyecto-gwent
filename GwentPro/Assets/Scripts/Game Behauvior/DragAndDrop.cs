using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class DragAndDrop : MonoBehaviour
{
    /// <summary>
    /// Returns true if the object is moving
    /// </summary>
    public bool isDragging = false;
    //FIXME: Eliminar comentario?
    // public bool isOverDropZone = false;

    //It saves the panel where the card is dropped
    public GameObject DropZone;
    //Save the start position
    public Vector2 startPosition;
    //Gets the card
    public Card MovingCard;
    //TODO: ELIMINAR ESTO HACIENDOLO CON ARRAYS EN EL BOARD.CS
    //Max amount of cards allowed per range section
    public int max_sections = 6;

    /// <summary>
    /// Relate the range with the position in the climate section
    /// </summary>
    readonly Dictionary<string, int> relate = new()
    {
        {"M", 0}, {"R", 1}, {"S", 2}
    };

    //Gets references
    readonly Board board = Board.Instance;
    GameManager gm;
    SoundManager soundM;

    // Event Declaration
    /// <summary>
    /// Event declaration that represent when a drag starts
    /// </summary>
    public event Action OnDragStart;

    /// <summary>
    /// Function called by Unity, it is called when an instance of the script is loaded
    /// </summary>
    private void Awake()
    {
        //Get the GameManager and the sound manager script
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        soundM = GameObject.Find("AudioSourceEffects").GetComponent<SoundManager>();
    }

    /// <summary>
    /// Function called by Unity, it is called in every frame (60 times per second)
    /// </summary>//  
    void Update()
    {
        if (isDragging)
        {
            //Move the object to the position where the mouse is
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
    }

    /// <summary>
    /// It is called when my card collides with an object 
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Saves the object that is colliding with the moving card
        DropZone = collision.gameObject;
    }

    /// <summary>
    /// It is called when the card stops colliding with an object
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        DropZone = null;
    }

    /// <summary>
    /// It is called when the drag starts
    /// </summary>
    public void StartDrag()
    {
        //Get the display card script
        DisplayCard disp = gameObject.GetComponent<DisplayCard>();
        //Saves the card
        MovingCard = disp.card;

        //Just start the drag if the card hasn't been played and is not already being dragged
        if (!MovingCard.IsPlayed && !isDragging)
        {
            startPosition = gameObject.transform.position;
            isDragging = true;

            // Shoot the event when drag starts
            OnDragStart?.Invoke();
        }


        //TODO: Por que necesito guardar la posicion inicial para la carta lider?
        //Save the start position to the leader card
        else if (MovingCard is Card.LeaderCard leader)
        {
            startPosition = gameObject.transform.position;
        }
    }
    public void EndDrag()
    {
        //FIXME: ELIMINAR COMENTARIOS?
        // if (!MovingCard.IsPlayed)
        // {
        isDragging = false;

        if (DropZone != null) DropCard(MovingCard);
        else this.transform.position = startPosition;
        // }
    }


    //FIXME: Create validate method
    void DropCard(Card card)
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
        else if (card is Card.ClimateCard climate_card && DropZone.name == "ClimateZone")
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
        else if (card is Card.IncrementCard increment_card)
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
        else if (card is Card.CleareanceCard cleareance
        //TODO: CREAR UN METODO QUE TE VERIFIQUE EN EL DROPZONE ACTUAL O EN EL DEL PARENT
            && (DropZone.name == "ClimateZone" || DropZone.transform.parent.gameObject.name == "ClimateZone"))
        {
            PlayCard(cleareance);
        }
        else if (card is Card.DecoyCard decoy && DropZone.tag == card.Owner.ID
            && DropZone.transform.parent != gm.HandPanel && DropZone.transform.parent.name != "ClimateZone"
            && !DropZone.transform.parent.name.Contains("Increment"))
        {
            PlayCard(card);
        }
        else
        {
            transform.position = startPosition;
        }
    }
    public void PlayCard(Card card, string range = "")
    {
        //Play a sound when a card is dropped
        soundM.PlayCardSound();

        //Set that owner already played
        card.Owner.HasPlayed = true;

        //Set the card to played so that it 
        //wont interact anymore with the drag and drop
        card.IsPlayed = true;

        //Remove card from the player hand in the backend
        card.Owner.Hand.Remove(card);

        //Drop card
        transform.SetParent(DropZone.transform, false);

        //Disable passed property if you play a card 
        gm.currentPlayer.Passed = false;

        //Apply effect
        if (card.EffectType is IActiveEffect effect)
        {
            effect.Invoke(card);
        }

        //Add card in backend
        //FIXME:  CREAR UN DICCIONARIO
        if (card is Card.UnityCard unity_card)
        {
            board.sections[unity_card.Owner.ID][range].Add(unity_card);
            //Check if there are special cards on the board
            if (range == "M")
            {
                if (board.climate_section[0] != null) unity_card.Power--;
                else if (board.increment_section[unity_card.Owner.ID][0] != null
                    && unity_card is Card.SilverCard) unity_card.Power++;
            }
            else if (range == "R")
            {
                if (board.climate_section[1] != null) unity_card.Power--;
                else if (board.increment_section[unity_card.Owner.ID][1] != null
                    && unity_card is Card.SilverCard) unity_card.Power++;
            }
            else if (range == "S")
            {
                if (board.climate_section[2] != null) unity_card.Power--;
                else if (board.increment_section[unity_card.Owner.ID][2] != null
                    && unity_card is Card.SilverCard) unity_card.Power++;
            }

        }
        else if (card is Card.ClimateCard climate_card)
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
        else if (card is Card.IncrementCard increment_card)
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
        else if (card is Card.CleareanceCard cleareance )
        {
            cleareance.Owner.GraveYard.Add(cleareance);
            cleareance.IsPlayed = false;
            gm.CardBeaten(cleareance);
        }
        else if (card is Card.DecoyCard decoy)
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

            if (BackendZone != null)
            {
                Decoy eff = new();
                eff.Invoke(CardToMove.name, BackendZone, PlayerZone, decoy);
            }
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
        gm.SetPower();

        //Change turn
        gm.ChangeTurn();      
    }
}


