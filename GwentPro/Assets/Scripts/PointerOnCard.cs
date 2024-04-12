using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PointerOnCard : MonoBehaviour
{
    public GameObject ShowPanel;
    public GameObject cardPrefab;
    public GameObject GreaterInstance;
    DragAndDrop Drag;
    DisplayCard disp;

    public void Start()
    {
        ShowPanel = GameObject.Find("ShowPanel");
        Drag = GetComponent<DragAndDrop>();
    }
    public void OnPointerEnter()
    {
        if (!Drag.isDragging)
        {
            GreaterInstance = Instantiate(cardPrefab, ShowPanel.transform);
            GreaterInstance.transform.localScale = new Vector2(2.5f, 2.5f);
            disp = GreaterInstance.GetComponent<DisplayCard>();
            disp.ShowCard();
        }
    }
    public void OnPointerExit()
    {
        if (!Drag.isDragging)
        {
            foreach (RectTransform child in ShowPanel.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }


    

    
}
