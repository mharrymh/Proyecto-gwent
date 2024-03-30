using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

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
    public GameManager gm;



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
        Debug.Log("Colisiono");
        isOverDropZone = true;
        DropZone = collision.gameObject;
    }

    //This method is called when stops card stops colliding with an object
    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("out");
        isOverDropZone = false;
        DropZone = null;
    }
    

    public void StartDrag()
    {
        DisplayCard disp = gameObject.GetComponent<DisplayCard>();
        card = disp.card;
        if (!card.IsPlayed)
        {
            startPosition = gameObject.transform.position;
            isDragging = true;
        }
        //Save the pos to the leader card
        else if (card is Card.LeaderCard leader)
        {
            startPosition = gameObject.transform.position;
        }
    }

    public void EndDrag()
    {
        isDragging = false;

        if (isOverDropZone && DropZone != null) DropCard(card);
        else transform.position = startPosition;
    }

    public void DropCard(Card card)
    {
        //Check for each dropable card type
        if (card is Card.UnityCard unity_card)
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
        else if (card is Card.SpecialCard climate_card && climate_card.Type is SpecialType.Climate)
        {
            if (climate_card.Range == "M" && DropZone.name == "ClimateMeleeZone" 
                && board.climate_section[0] == null)
            {
                PlayCard(climate_card, "M");
            }
            else if (climate_card.Range == "R" && DropZone.name == "ClimateRangeZone"
                && board.climate_section[1] == null)
            {
                PlayCard(climate_card, "R");
            }
            else if (climate_card.Range == "S" && DropZone.name == "ClimateSiegeZone"
                && board.climate_section[2] == null)
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
        else
        {
            transform.position = startPosition;
        }
    }



    public void PlayCard(Card card, string range)
    {
        card.IsPlayed = true;
        transform.SetParent(DropZone.transform, false);

        
        

        if (card is Card.UnityCard unity_card)
        {
            board.sections[unity_card.Owner.ID][range].Add(unity_card);
        }

        else if (card is Card.SpecialCard climate_card && climate_card.Type == SpecialType.Climate)
        {
            string key = range;
            int value;
            if (relate.TryGetValue(key, out value))
            {
                board.climate_section[value] = climate_card;
            }
        }

        else if (card is Card.SpecialCard increment_card && increment_card.Type == SpecialType.Increment)
        {
            string key = range;
            int value;
            if (relate.TryGetValue(key, out value))
            {
                board.increment_section[increment_card.Owner.ID][value] = increment_card;
            }
        }

        //Sets the power
        gm.SetPower(card.Owner);      
    }


}


