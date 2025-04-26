using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Required for Tuple comparison if not using newer C#/.NET

[RequireComponent(typeof(MeshFilter))] // Ensure the GameObject has a MeshFilter
public class WireFrameLineBased : MonoBehaviour
{
    public Material lineMaterial; // Material for the LineRenderer
    public Color lineColor = Color.white;
    public float lineWidth = 0.01f;
    public bool generateOnStart = true;

    private LineRenderer lineRenderer;
    private MeshFilter meshFilter;

    void Start()
    {
        if (generateOnStart)
        {
            GenerateWireframe();
        }
    }

    // Optional: Provide a context menu item to generate/update in the editor
    [ContextMenu("Generate Wireframe")]
    public void GenerateWireframe()
    {
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null || meshFilter.sharedMesh == null)
        {
            Debug.LogError("WireframeRenderer requires a MeshFilter with a valid mesh.", this);
            return;
        }

        // Get or add the LineRenderer component
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // Configure the LineRenderer
        SetupLineRenderer();

        // Generate the wireframe points
        DrawWireframe(meshFilter.sharedMesh);
    }

    void SetupLineRenderer()
    {
        lineRenderer.useWorldSpace = false; // Use local space relative to this GameObject
        lineRenderer.loop = false;          // Lines are segments, not loops
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;

        // Assign the material
        if (lineMaterial == null)
        {
            // Create a simple default material if none is assigned
            lineMaterial = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
            // Or use a more modern unlit shader:
            // lineMaterial = new Material(Shader.Find("Unlit/Color"));
            // lineMaterial.color = lineColor; // Set color on material if using Unlit/Color
            Debug.LogWarning("No material assigned to WireframeRenderer. Creating a default material.", this);
        }
        lineRenderer.material = lineMaterial;

        // Important for rendering order, especially with transparency
        lineRenderer.sortingOrder = 1;
    }

    void DrawWireframe(Mesh mesh)
    {
        var vertices = mesh.vertices;
        var triangles = mesh.triangles;

        // Use a HashSet to store unique edges (pairs of vertex indices)
        // Store edges with the smaller index first to ensure uniqueness regardless of order
        var edges = new HashSet<(int, int)>();

        // Iterate through triangles and add edges
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int v1 = triangles[i];
            int v2 = triangles[i + 1];
            int v3 = triangles[i + 2];

            // Add edges, ensuring smaller index comes first
            AddEdge(edges, v1, v2);
            AddEdge(edges, v2, v3);
            AddEdge(edges, v3, v1);
        }

        // Prepare points for the LineRenderer
        // Each edge needs two points (start and end)
        Vector3[] linePoints = new Vector3[edges.Count * 2];
        int pointIndex = 0;

        foreach (var edge in edges)
        {
            linePoints[pointIndex++] = vertices[edge.Item1]; // Start point of edge
            linePoints[pointIndex++] = vertices[edge.Item2]; // End point of edge
        }

        // Set the points on the LineRenderer
        lineRenderer.positionCount = linePoints.Length;
        lineRenderer.SetPositions(linePoints);
    }

    void AddEdge(HashSet<(int, int)> edges, int index1, int index2)
    {
        // Ensure the edge is stored with the smaller index first
        var edge = (Mathf.Min(index1, index2), Mathf.Max(index1, index2));
        edges.Add(edge);
    }

    // Optional: Clean up the LineRenderer if the component is removed
    void OnDestroy()
    {
        // If the LineRenderer was potentially added by this script, destroy it
        if (lineRenderer != null)
        {
            // Check if the component was added at runtime by checking its hideFlags
            // A more robust check might involve tracking if *we* added it.
            // For simplicity, we just destroy it if it exists. Adjust if needed.
            if (Application.isPlaying) // Only destroy if running
            {
                // Destroy(lineRenderer); // Careful: This might remove a user-added LineRenderer too.
                // Safer: Disable it or only destroy if a flag indicates this script added it.
                // Let's just disable it to be safe:
                // lineRenderer.enabled = false;
            }
            // If you *always* want this script to manage the LineRenderer, uncomment Destroy.
        }
        // Clean up the default material if we created it
        if (lineMaterial != null && lineMaterial.name.Contains("Default-Material")) // Basic check
        {
            if (Application.isPlaying) Destroy(lineMaterial); else DestroyImmediate(lineMaterial);
        }
    }

    // Optional: Call this if the mesh changes at runtime
    public void UpdateWireframe()
    {
        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            DrawWireframe(meshFilter.sharedMesh);
        }
    }

    // Optional: Update properties dynamically if changed in inspector at runtime
    void Update()
    {
        if (lineRenderer != null)
        {
            if (lineRenderer.startWidth != lineWidth) lineRenderer.startWidth = lineWidth;
            if (lineRenderer.endWidth != lineWidth) lineRenderer.endWidth = lineWidth;
            if (lineRenderer.startColor != lineColor) lineRenderer.startColor = lineColor;
            if (lineRenderer.endColor != lineColor) lineRenderer.endColor = lineColor;
            // Note: Changing material at runtime might require careful handling
            if (lineRenderer.material != lineMaterial) lineRenderer.material = lineMaterial;
        }
    }

   


}