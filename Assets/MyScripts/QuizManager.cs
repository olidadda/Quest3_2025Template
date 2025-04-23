using UnityEngine;
using System.Collections; // Required for Coroutines (IEnumerator)

[RequireComponent(typeof(AudioSource))] // Ensures an AudioSource component is present
public class QuizManager : MonoBehaviour
{
    public enum QuizState
    {       
        SolvingQ1, 
        SolvingQ2, 
        SolvingQ3,
        SolvingQ4,
        SolvingQ5,
        Finished
    }

    public QuizState quizState;

    [SerializeField] BoolCondition CorrectAnswer1;
    [SerializeField] BoolCondition CorrectAnswer2;
    [SerializeField] BoolCondition CorrectAnswer3;
    [SerializeField] BoolCondition CorrectAnswer4;
    [SerializeField] BoolCondition CorrectAnswer5;
    [SerializeField] float boolConditionDelay = 2.5f;

    [Header("Feedback Objects")]
    [Tooltip("The GameObject with the red cross image/element")]
    public GameObject redCrossObject;

    [Tooltip("The GameObject with the green tick image/element")]
    public GameObject greenTickObject;

    [Header("Feedback Sounds")]
    [Tooltip("Sound to play for an incorrect answer")]
    public AudioClip errorSound;

    [Tooltip("Sound to play for a correct answer")]
    public AudioClip successSound;

    [Header("Flashing Settings")]
    [Tooltip("How long the red cross stays visible the first time")]
    public float firstFlashDuration = 0.5f;

    [Tooltip("How long the red cross stays visible the second time")]
    public float secondFlashDuration = 1.0f;

    // Private variables
    private AudioSource audioSource;
    private Coroutine flashingCoroutine = null; // To keep track of the flashing effect
    private Coroutine boolConditionDelayCO = null;

    void Awake()
    {
        quizState = QuizState.SolvingQ1;
        // Get the AudioSource component attached to this GameObject
        audioSource = GetComponent<AudioSource>();

        // Ensure feedback objects are initially inactive
        if (redCrossObject != null)
            redCrossObject.SetActive(false);
        if (greenTickObject != null)
            greenTickObject.SetActive(false);

        // --- Safety Checks (Optional but Recommended) ---
        if (redCrossObject == null)
            Debug.LogError("QuizManager: Red Cross Object is not assigned in the Inspector!");
        if (greenTickObject == null)
            Debug.LogError("QuizManager: Green Tick Object is not assigned in the Inspector!");
        if (errorSound == null)
            Debug.LogWarning("QuizManager: Error Sound is not assigned.");
        if (successSound == null)
            Debug.LogWarning("QuizManager: Success Sound is not assigned.");
        // -------------------------------------------------
    }


    public void ProcessButtonInput(int buttonNum)
    {
        // Route the call based on the current quiz state
        switch (quizState)
        {
            case QuizState.SolvingQ1:
                AnswerQuestion1(buttonNum);
                break;
            case QuizState.SolvingQ2:
                AnswerQuestion2(buttonNum);
                break;
            case QuizState.SolvingQ3:
                AnswerQuestion3(buttonNum);
                break;
            case QuizState.SolvingQ4:
                AnswerQuestion4(buttonNum);
                break;
            case QuizState.SolvingQ5:
                AnswerQuestion5(buttonNum);
                break;
            case QuizState.Finished:
                Debug.Log("Quiz is finished. Input ignored.");
                // Or add specific behavior for clicking after finishing
                break;
            default:
                Debug.LogWarning($"Received button input in unhandled state: {quizState}");
                break;
        }
    }


    // --- Public Methods for Each Question ---

    public void AnswerQuestion1(int buttonNum)
    {
        bool isCorrect;
        if(buttonNum == 2) 
        {
            isCorrect = true;
            quizState = QuizState.SolvingQ2;
        }
        else 
        {
            isCorrect = false;
        }

        HandleAnswer(isCorrect, CorrectAnswer1);
    }

    public void AnswerQuestion2(int buttonNum)
    {
        bool isCorrect;
        if (buttonNum == 2)
        {
            isCorrect = true;
            quizState = QuizState.SolvingQ3;
        }
        else
        {
            isCorrect = false;
        }

        HandleAnswer(isCorrect, CorrectAnswer2);
    }

    public void AnswerQuestion3(int buttonNum)
    {
        bool isCorrect;
        if (buttonNum == 3)
        {
            isCorrect = true;
            quizState = QuizState.SolvingQ4;
        }
        else
        {
            isCorrect = false;
        }

        HandleAnswer(isCorrect, CorrectAnswer3);
    }

    public void AnswerQuestion4(int buttonNum)
    {
        bool isCorrect;
        if (buttonNum == 3)
        {
            isCorrect = true;
            quizState = QuizState.SolvingQ5;
        }
        else
        {
            isCorrect = false;
        }

        HandleAnswer(isCorrect, CorrectAnswer4);
    }

    public void AnswerQuestion5(int buttonNum)
    {
        bool isCorrect;
        if (buttonNum == 3)
        {
            isCorrect = true;
            quizState = QuizState.Finished;
        }
        else
        {
            isCorrect = false;
        }

        HandleAnswer(isCorrect, CorrectAnswer5);
    }

    // --- Core Logic ---

    private void HandleAnswer(bool isCorrect, BoolCondition boolCondition)
    {
        // Stop any previous feedback routines and hide objects
        ResetFeedback();

        if (isCorrect)
        {
            // --- Correct Answer Logic ---
            Debug.Log("Answer Correct!");

            // Play success sound
            if (successSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(successSound);
            }

            // Activate green tick
            if (greenTickObject != null)
            {
                greenTickObject.SetActive(true);
                // Optional: Deactivate after a delay if needed
                // Invoke(nameof(HideGreenTick), 2f); // Example: Hide after 2 seconds
                boolConditionDelayCO = StartCoroutine(DelayBoolCondition(boolCondition));
            }
        }
        else
        {
            // --- Incorrect Answer Logic ---
            Debug.Log("Answer Incorrect!");

            // Play error sound
            if (errorSound != null && audioSource != null)
            {
                //audioSource.PlayOneShot(errorSound);
            }

            // Start flashing red cross
            if (redCrossObject != null && audioSource != null && errorSound != null)
            {
                // Start the flashing coroutine and store its reference
                flashingCoroutine = StartCoroutine(FlashRedCross());
            }
        }
    }

    // --- Coroutine for Flashing Effect ---

   
    private IEnumerator DelayBoolCondition( BoolCondition boolCondition)
    {
        yield return new WaitForSeconds(boolConditionDelay);
        boolCondition.conditionMet = true;
        boolConditionDelayCO = null;
    }


    private IEnumerator FlashRedCross()
    {
        if (redCrossObject == null) yield break; // Exit if object is not assigned

        // First Flash
        redCrossObject.SetActive(true);
        audioSource.PlayOneShot(errorSound);
        yield return new WaitForSeconds(firstFlashDuration);
        redCrossObject.SetActive(false);

        // Small delay between flashes (optional, can be 0)
        yield return null; // Waits for one frame, helps ensure visual separation

        // Second Flash
        redCrossObject.SetActive(true);
        audioSource.PlayOneShot(errorSound);
        yield return new WaitForSeconds(secondFlashDuration);
        redCrossObject.SetActive(false);

        // Coroutine finished
        flashingCoroutine = null;
    }

    // --- Helper Methods ---

    // Resets the feedback state before showing new feedback
    private void ResetFeedback()
    {
        // Stop the flashing if it's currently running
        if (flashingCoroutine != null)
        {
            StopCoroutine(flashingCoroutine);
            flashingCoroutine = null;
        }

        // Ensure both feedback objects are hidden
        if (redCrossObject != null)
            redCrossObject.SetActive(false);
        if (greenTickObject != null)
            greenTickObject.SetActive(false);
    }

    // Optional: Call this if you want the green tick to hide automatically
    // private void HideGreenTick()
    // {
    //     if (greenTickObject != null)
    //         greenTickObject.SetActive(false);
    // }
}
