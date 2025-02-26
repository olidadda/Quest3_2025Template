using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputConfirmationHandler : MonoBehaviour
{
    public InputHandler inputHandler;

    private void Start()
    {
        if (inputHandler != null)
        {
            inputHandler.OnInputConfirmed += HandleConfirmation;
            inputHandler.OnInputCancelled += HandleCancellation;
        }
    }

    private void OnDestroy()
    {
        if (inputHandler != null)
        {
            inputHandler.OnInputConfirmed -= HandleConfirmation;
            inputHandler.OnInputCancelled -= HandleCancellation;
        }
    }

    private void HandleConfirmation(string confirmedInput)
    {
        Debug.Log("Confirmed Input: " + confirmedInput);
        // Move to Game Choice Menu
    }

    private void HandleCancellation()
    {
        Debug.Log("Input Canceled.");
        // Return to Previous Menu
    }
}
