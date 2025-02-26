using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ButtonLibrary", menuName = "UI/Button Library")]
public class ButtonLibrary : ScriptableObject
{
    public List<GameObject> buttonPrefabs; // Each prefab has a DigitInfo subclass attached
}
