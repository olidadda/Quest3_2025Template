using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IDNumberDisplay : MonoBehaviour
{
    public InputHandler inputHandler;
    public GameObject[] digitSlots; // Assign N parent GameObjects, each containing 0-9 models.

    private string displayedNumber = "";

    private void Start()
    {
        inputHandler.OnInputUpdated += UpdateDisplay;
    }

    private void UpdateDisplay(string newInput)
    {
        // Trim input to fit within available digit slots
        if (newInput.Length > digitSlots.Length)
        {
            newInput = newInput.Substring(0, digitSlots.Length);
        }

        displayedNumber = newInput;
        UpdateVisuals();
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
                DeactivateAll(digitSlots[i]); // Hide unused digit slots
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

    private void DeactivateAll(GameObject slot)
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
}
