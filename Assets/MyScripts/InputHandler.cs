using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public event Action<string> OnInputUpdated;
    public event Action<string> OnInputConfirmed;
    public event Action OnInputCancelled;

    [Tooltip("0 or negative => No limit on input length; otherwise, input is capped at this value.")]
    [SerializeField]
    private int maxLength = 3;

    private string currentInput = "";

    public void ReceiveCharacter(string character)
    {
        // If for some reason Unity passes an empty or multi-length string, you handle it:
        if (string.IsNullOrEmpty(character))
            return; // do nothing

        // The first char is really all we care about
        char c = character[0];

        // Enforce length if needed
        if (maxLength <= 0 || currentInput.Length < maxLength)
        {
            currentInput += c;
            OnInputUpdated?.Invoke(currentInput);
        }
    }

    public void ReceiveString(string input)
    {
        if (string.IsNullOrEmpty(input)) return;

        foreach (char c in input)
        {
            // Stop if at max length
            if (maxLength > 0 && currentInput.Length >= maxLength)
                break;
            currentInput += c;
        }

        OnInputUpdated?.Invoke(currentInput);
    }


    public void CancelInput()
    {        
        OnInputCancelled?.Invoke();
        ClearInput();
    }
    public void LineBreak()
    {
        if (currentInput.Length > 0) // Prevents accidental new lines
        {
            currentInput += "\n"; // Manual line break
            OnInputUpdated?.Invoke(currentInput);
        }
    }
    public void DeleteLastCharacter()
    {
        if (currentInput.Length > 0)
        {
            currentInput = currentInput.Substring(0, currentInput.Length - 1);
            OnInputUpdated?.Invoke(currentInput);
        }
    }

    public void ConfirmInput() //Enter pressed for example
    {
        if (currentInput.Length > 0)
        {
            //Debug.Log("Input confirmed: " + currentInput);
            OnInputConfirmed?.Invoke(currentInput);
            ClearInput();  // Call the reusable method
        }
    }

    public void ClearInput()
    {
        currentInput = "";
        OnInputUpdated?.Invoke(currentInput);
    }
}
