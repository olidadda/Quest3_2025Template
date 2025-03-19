using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ActivateAllMeshRenderersInChildren : MonoBehaviour
{
   

    public void ActivateAllRenderersInDescendants(bool activated)
    {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers)
        {
            if (activated) { renderer.enabled = true; }
            else { renderer.enabled = false; }
            
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ActivateAllMeshRenderersInChildren))]
    public class ActivateAllMeshRenderersInChildrenEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector(); // Draws the default convertor inspector

            ActivateAllMeshRenderersInChildren myScript = (ActivateAllMeshRenderersInChildren)target;
            if (GUILayout.Button("Activate Renderers"))
            {
                myScript.ActivateAllRenderersInDescendants(true);
            }
            if (GUILayout.Button("Deactivate Renderers"))
            {
                myScript.ActivateAllRenderersInDescendants(false);
            }
        }
    }
#endif
}
