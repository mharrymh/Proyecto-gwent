using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PointerOnCard : MonoBehaviour
{
    public GameObject ShowPanel;
    public GameObject cardPrefab;
    public GameObject GreaterInstance;
    DisplayCard disp;
    DragAndDrop Drag;

    public void Start()
    {
        ShowPanel = GameObject.Find("ShowPanel");
        Drag = cardPrefab.GetComponent<DragAndDrop>();
    }
    public void OnPointerEnter()
    {
        if (!Drag.isDragging)
        {
            GreaterInstance = Instantiate(cardPrefab, ShowPanel.transform);
            Collider2D collider2D = GreaterInstance.GetComponent<Collider2D>();
            collider2D.enabled = false;
            GreaterInstance.transform.localScale = new Vector2(2.5f, 2.5f);
            disp = cardPrefab.GetComponent<DisplayCard>();
            disp.ShowCard();

            // Start listening the drag event
            Drag.OnDragStart += HandleDragStart;
        }

        
    }

    private void HandleDragStart()
    {
        // When drag start
        Destroy(GreaterInstance);

        // Stop listening Drag 
        Drag.OnDragStart -= HandleDragStart;
    }

    public void OnPointerExit()
    {
        if (!Drag.isDragging)
        {
            Destroy(GreaterInstance);
        }
    }
}
