using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IDNumberDisplay : MonoBehaviour
{
    [Header("References")]
    public InputHandler inputHandler;
    public GameObject[] digitSlots; 
    public BoolCondition conditionObject; // The bool condition to reflect "all digits entered"

    private string displayedNumber = "";

    private void OnEnable()
    {
        if (inputHandler != null)
            inputHandler.OnInputUpdated += UpdateDisplay;
    }

    private void UpdateDisplay(string newInput)
    {
        //// Trim input to fit within available digit slots
        //if (newInput.Length > digitSlots.Length)
        //{
        //    newInput = newInput.Substring(0, digitSlots.Length);
        //}

        displayedNumber = newInput;
        UpdateVisuals();

        // Check if all digit slots are filled
        if (conditionObject != null)
        {
            bool allDigitsFilled = (displayedNumber.Length == digitSlots.Length);
            conditionObject.conditionMet = allDigitsFilled;
        }
    }

    private void UpdateVisuals()
    {
        for (int i = 0; i < digitSlots.Length; i++)
        {
            if (i < displayedNumber.Length)
            {
                int digit = int.Parse(displayedNumber[i].ToString());
                ActivateDigit(digitSlots[i], digit);
            }
            else
            {
                DeactivateSlotDigits(digitSlots[i]); // Hide unused digit slots
            }
        }
    }

    private void ActivateDigit(GameObject slot, int digit)
    {
        for (int j = 0; j < slot.transform.childCount; j++)
        {
            slot.transform.GetChild(j).gameObject.SetActive(j == digit);
        }
    }

    private void DeactivateSlotDigits(GameObject slot)
    {
        foreach (Transform child in slot.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void ConfirmChoice()
    {
        Debug.Log("User confirmed code: " + displayedNumber);
    }

    private void OnDisable()
    {
        if (inputHandler != null)
            inputHandler.OnInputUpdated -= UpdateDisplay;
    }
}
