using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable] // Allows each choice to be editable in the Inspector
public class Choice
{
    public string choiceName; // Displayed in Inspector
    public Button choiceButton; // UI Button or Interaction
    public TimelineEventBase nextEvent; // Event to transition to
}

public class PlayerChoiceEvent : TimelineEventBase
{
    [Header("Player Choices")]
    public List<Choice> choices = new List<Choice>();

    public override void Execute()
    {
        Debug.Log("Waiting for player choice: " + eventName);
        SetupPhase();

        foreach (var choice in choices)
        {
            if (choice.choiceButton != null)
            {
                choice.choiceButton.onClick.AddListener(() => OnChoiceSelected(choice));
            }
        }
    }

    private void OnChoiceSelected(Choice chosen)
    {
        Debug.Log("Player selected: " + chosen.choiceName);

        // Remove all button listeners to prevent multiple calls
        foreach (var choice in choices)
        {
            if (choice.choiceButton != null)
                choice.choiceButton.onClick.RemoveAllListeners();
        }

        TimeLine.Instance.TriggerEvent(chosen.nextEvent);
    }

    public override bool CheckCondition() => false; // Manually triggered by button clicks
}
