using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VisualManager : MonoBehaviour
{
    public List<string> TextsToDisplay = new List<string>();
    public int ActualPosition = 0;
    public bool PointerInsideAuxPanel = false;
    public bool IsDisplaying = false;
    public GameObject PanelAux;
    public GameObject NextButton;
    public GameObject BackButton;
    public TMP_Text AuxText;
    
    public void Add(string text)
    {
        TextsToDisplay.Add(text);
    }

    public void DisplayAuxiliarText() {
        IsDisplaying = true;
        //Active the panel and display the text
        if (TextsToDisplay.Count > 0) {
            PanelAux.SetActive(true);
            AuxText.text = TextsToDisplay[ActualPosition];
        }

        if (TextsToDisplay.Count > 1 && ActualPosition < TextsToDisplay.Count-1)
        {
            NextButton.SetActive(true);
        }
    }

    public void GetNextText() {
        //Increase the position
        if (++ActualPosition == TextsToDisplay.Count-1)
        {
            //Delete the nextButton
            NextButton.SetActive(false);
        }
        BackButton.SetActive(true);
        DisplayAuxiliarText();
    }

    public void GetPreviousText() {
        //Decrease the position
        if (--ActualPosition == 0)
        {
            //Delete the backButton
            BackButton.SetActive(false);
        }
        NextButton.SetActive(true);
        DisplayAuxiliarText();
    }

    public void CloseAuxPanel() {
        TextsToDisplay.Clear();
        ActualPosition = 0;
        IsDisplaying = false;
        PanelAux.SetActive(false);
    }

    
    public void OnPointerEnter()
    {
        PointerInsideAuxPanel = true;
    }

    public void OnPointerExit()
    {
        PointerInsideAuxPanel = false;
    }

}
