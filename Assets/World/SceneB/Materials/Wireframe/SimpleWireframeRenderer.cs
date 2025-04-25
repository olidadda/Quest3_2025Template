using UnityEngine;
using System.Collections.Generic; // Needed for List

[RequireComponent(typeof(Renderer))] // Works on MeshRenderer or SkinnedMeshRenderer
public class SimpleWireframeRenderer : MonoBehaviour
{
    // Assign the Material using the "Custom/SimpleWireframeSPI" shader here in the Inspector
    public Material wireframeMaterial;

    private Renderer meshRenderer;
    private Material[] originalMaterials;
    private bool materialsReplaced = false;

    void Awake()
    {
        meshRenderer = GetComponent<Renderer>();
        if (meshRenderer != null)
        {
            // Store original materials safely
            originalMaterials = meshRenderer.sharedMaterials;
        }
        else
        {
            Debug.LogError("SimpleWireframeRenderer requires a Renderer component.", this);
            enabled = false;
        }

        if (wireframeMaterial == null)
        {
            Debug.LogError("Wireframe Material is not assigned in the Inspector!", this);
            enabled = false;
        }
    }

    void OnEnable()
    {
        // Apply wireframe material only if needed and possible
        if (!materialsReplaced && meshRenderer != null && wireframeMaterial != null)
        {
            // Create a new array containing only the wireframe material
            Material[] wireframeMats = new Material[originalMaterials.Length];
            for (int i = 0; i < wireframeMats.Length; i++)
            {
                wireframeMats[i] = wireframeMaterial; // Use the same wireframe material for all submeshes
            }
            // Use sharedMaterials to avoid creating material instances if not needed
            meshRenderer.sharedMaterials = wireframeMats;
            materialsReplaced = true;
        }
    }

    void OnDisable()
    {
        // Restore original materials only if they were replaced and we have them
        if (materialsReplaced && meshRenderer != null && originalMaterials != null)
        {
            meshRenderer.sharedMaterials = originalMaterials;
            materialsReplaced = false;
        }
    }

    // Optional: Restore materials if the script component is destroyed
    void OnDestroy()
    {
        // Check if meshRenderer still exists (might be destroyed already)
        if (meshRenderer != null && materialsReplaced && originalMaterials != null)
        {
            meshRenderer.sharedMaterials = originalMaterials;
            materialsReplaced = false;
        }
    }
}