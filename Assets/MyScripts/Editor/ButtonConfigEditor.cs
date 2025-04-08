using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ButtonType))]
public class ButtonConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ButtonType config = (ButtonType)target;

        config.buttonLibrary = (ButtonLibrary)EditorGUILayout.ObjectField("Button Library", config.buttonLibrary, typeof(ButtonLibrary), false);

        config.buttonTypeSelection = (ButtonType.ButtonTypeSelection)EditorGUILayout.EnumPopup("Button Type", config.buttonTypeSelection);

        if (config.buttonLibrary != null)
        {
            List<GameObject> matchingPrefabs = new List<GameObject>();
            List<string> prefabNames = new List<string>();

            foreach (GameObject prefab in config.buttonLibrary.buttonPrefabs)
            {
                if (config.PrefabMatchesType(prefab))
                {
                    matchingPrefabs.Add(prefab);
                    prefabNames.Add(prefab.name);
                }
            }

            if (matchingPrefabs.Count > 0)
            {
                config.selectedPrefabIndex = EditorGUILayout.Popup("Select Prefab", config.selectedPrefabIndex, prefabNames.ToArray());
            }
            else
            {
                EditorGUILayout.LabelField("No prefabs available for this type.");
            }

            if (GUILayout.Button("Update Button Prefab"))
            {
                config.UpdateButtonPrefab();
            }
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(config);
        }
    }
}
