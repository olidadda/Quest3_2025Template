using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ButtonType : MonoBehaviour
{
    public ButtonLibrary buttonLibrary;

    [SerializeField] private int number;  // Only for digits (0-9)
    [SerializeField] private char character;  // chars
    [SerializeField] private SpecialCharInfo specialCharacter;  // Only for special characters
    [SerializeField] private string word;  // For letters or words
    public enum ButtonTypeSelection
    {
        None,
        Number,
        SpecialCharacter,
        Character,
        Word
    }

    public ButtonTypeSelection buttonTypeSelection;

    public int selectedPrefabIndex;  // Selected prefab from the dropdown
    private GameObject currentPrefabInstance;

    public void UpdateButtonPrefab()
    {
        GameObject prefabToSpawn = null;

        foreach (GameObject prefab in buttonLibrary.buttonPrefabs)
        {
            if (PrefabMatchesType(prefab))
            {
                if (selectedPrefabIndex == 0)
                {
                    prefabToSpawn = prefab;
                    break;
                }
                selectedPrefabIndex--;
            }
        }

        if (prefabToSpawn != null)
        {
            AssignPrefab(prefabToSpawn);
        }
    }

    public bool PrefabMatchesType(GameObject prefab)
    {
        switch (buttonTypeSelection)
        {
            case ButtonTypeSelection.Word: return prefab.GetComponent<WordInfo>() != null;
            case ButtonTypeSelection.Character: return prefab.GetComponent<CharInfo>() != null;
            case ButtonTypeSelection.Number: return prefab.GetComponent<NumberInfo>() != null;
            case ButtonTypeSelection.SpecialCharacter: return prefab.GetComponent<SpecialCharInfo>() != null;
            default: return false;
        }
    }

    private void AssignPrefab(GameObject prefab)
    {
        if (currentPrefabInstance != null)
        {
            DestroyImmediate(currentPrefabInstance);
        }

        currentPrefabInstance = Instantiate(prefab, transform);
    }
}
    
   




