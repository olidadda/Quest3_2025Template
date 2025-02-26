using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public event Action<string> OnInputUpdated;
    public event Action<string> OnInputConfirmed;
    public event Action OnInputCancelled;

    //[Tooltip("0 = No limit on input length. Otherwise, input is capped at this value.")]
    //[SerializeField] private int maxLength = 3;
    //[Tooltip("0 = No automatic line breaks. Otherwise, inserts a line break every X characters.")]
    //[SerializeField] private int autoLineBreakInterval = 5; // Auto break interval

    private string currentInput = "";

    public void ReceiveInput(string input)
    {
        if (!string.IsNullOrEmpty(input))
        {
            ReceiveInput(input[0]); // Calls the char version
        }
    }
    public void ReceiveInput(char input)
    { 
            currentInput += input;
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
