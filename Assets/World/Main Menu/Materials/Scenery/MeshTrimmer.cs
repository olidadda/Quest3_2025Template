using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class MeshTrimmer : MonoBehaviour
{
    [Header("Trim Boundaries")]
    [Range(-10f, 10f)] public float minX = -10f;
    [Range(-10f, 10f)] public float maxX = 10f;
    [Range(-10f, 10f)] public float minY = -10f;
    [Range(-10f, 10f)] public float maxY = 10f;
    [Range(-10f, 10f)] public float minZ = -10f;
    [Range(-10f, 10f)] public float maxZ = 10f;

    [Header("Original Mesh Bounds")]
    public Vector3 originalMin = Vector3.zero;
    public Vector3 originalMax = Vector3.zero;

    [Header("Settings")]
    public bool preserveUVs = true;
    public bool preserveNormals = true;
    public bool updateInRealtime = false;
    public bool useLocalSpace = true;
    public bool clipEdges = true;

    private Mesh originalMesh;
    private Mesh workingMesh;
    private MeshFilter meshFilter;
    private Vector3[] originalVertices;
    private Vector3[] workingVertices;
    private Vector2[] originalUVs;
    private Vector3[] originalNormals;
    private int[] originalTriangles;

    private float lastMinX, lastMaxX, lastMinY, lastMaxY, lastMinZ, lastMaxZ;
    private bool initializedBounds = false;

    void OnEnable()
    {
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter.sharedMesh != null)
        {
            // Store the original mesh
            originalMesh = meshFilter.sharedMesh;

            // Create a working copy
            workingMesh = new Mesh();
            workingMesh.name = "Trimmed_" + originalMesh.name;
            meshFilter.sharedMesh = workingMesh;

            // Store original mesh data
            originalVertices = originalMesh.vertices;
            originalTriangles = originalMesh.triangles;

            if (originalMesh.uv != null && originalMesh.uv.Length > 0)
                originalUVs = originalMesh.uv;

            if (originalMesh.normals != null && originalMesh.normals.Length > 0)
                originalNormals = originalMesh.normals;

            // Initialize bounds if not already done
            if (!initializedBounds)
            {
                Bounds bounds = originalMesh.bounds;
                originalMin = bounds.min;
                originalMax = bounds.max;

                // Set initial trim values to match original bounds
                minX = originalMin.x;
                maxX = originalMax.x;
                minY = originalMin.y;
                maxY = originalMax.y;
                minZ = originalMin.z;
                maxZ = originalMax.z;

                initializedBounds = true;
            }

            // Do initial trimming
            TrimMesh();
        }
    }

    void OnDisable()
    {
        // Restore original mesh when disabled
        if (meshFilter && originalMesh)
            meshFilter.sharedMesh = originalMesh;
    }

    void OnDestroy()
    {
        // Clean up the working mesh to prevent memory leaks
        if (Application.isEditor && !Application.isPlaying)
        {
            if (workingMesh != null)
                DestroyImmediate(workingMesh);
        }
    }

    void Update()
    {
        // Only update in real-time if the option is selected
        if (updateInRealtime && HasBoundsChanged())
        {
            TrimMesh();
            UpdateLastBounds();
        }
    }

    bool HasBoundsChanged()
    {
        return lastMinX != minX || lastMaxX != maxX ||
               lastMinY != minY || lastMaxY != maxY ||
               lastMinZ != minZ || lastMaxZ != maxZ;
    }

    void UpdateLastBounds()
    {
        lastMinX = minX;
        lastMaxX = maxX;
        lastMinY = minY;
        lastMaxY = maxY;
        lastMinZ = minZ;
        lastMaxZ = maxZ;
    }

    public void TrimMesh()
    {
        if (originalMesh == null || workingMesh == null)
            return;

        // Clear the working mesh
        workingMesh.Clear();

        // Lists to hold new mesh data
        List<Vector3> newVertices = new List<Vector3>();
        List<int> newTriangles = new List<int>();
        List<Vector2> newUVs = new List<Vector2>();
        List<Vector3> newNormals = new List<Vector3>();

        // Process triangles
        for (int i = 0; i < originalTriangles.Length; i += 3)
        {
            int idx1 = originalTriangles[i];
            int idx2 = originalTriangles[i + 1];
            int idx3 = originalTriangles[i + 2];

            Vector3 v1 = originalVertices[idx1];
            Vector3 v2 = originalVertices[idx2];
            Vector3 v3 = originalVertices[idx3];

            // Convert to world space if needed
            if (!useLocalSpace)
            {
                v1 = transform.TransformPoint(v1);
                v2 = transform.TransformPoint(v2);
                v3 = transform.TransformPoint(v3);
            }

            // Check if all vertices are within bounds
            bool v1Inside = IsPointInBounds(v1);
            bool v2Inside = IsPointInBounds(v2);
            bool v3Inside = IsPointInBounds(v3);

            // Only include triangles if at least one vertex is inside the bounds
            if (v1Inside || v2Inside || v3Inside)
            {
                // If we're not clipping edges and any vertex is outside, skip the triangle
                if (!clipEdges && (!v1Inside || !v2Inside || !v3Inside))
                    continue;

                // Handle simple case where all vertices are inside
                if (v1Inside && v2Inside && v3Inside)
                {
                    // Add triangle as-is
                    int newIdx1 = AddOrGetVertexIndex(newVertices, v1, idx1, newUVs, newNormals);
                    int newIdx2 = AddOrGetVertexIndex(newVertices, v2, idx2, newUVs, newNormals);
                    int newIdx3 = AddOrGetVertexIndex(newVertices, v3, idx3, newUVs, newNormals);

                    newTriangles.Add(newIdx1);
                    newTriangles.Add(newIdx2);
                    newTriangles.Add(newIdx3);
                }
                else if (clipEdges)
                {
                    // This is where you'd implement a more complex clipping algorithm
                    // For simplicity in this implementation, we'll just include partial triangles
                    // A full implementation would clip the triangle against each boundary plane
                    // and generate new vertices at the intersection points

                    // For now, we'll do a simple inclusion of triangles that have any vertex inside
                    int newIdx1 = AddOrGetVertexIndex(newVertices, v1, idx1, newUVs, newNormals);
                    int newIdx2 = AddOrGetVertexIndex(newVertices, v2, idx2, newUVs, newNormals);
                    int newIdx3 = AddOrGetVertexIndex(newVertices, v3, idx3, newUVs, newNormals);

                    newTriangles.Add(newIdx1);
                    newTriangles.Add(newIdx2);
                    newTriangles.Add(newIdx3);
                }
            }
        }

        // Assign new data to the working mesh
        workingMesh.vertices = newVertices.ToArray();
        workingMesh.triangles = newTriangles.ToArray();

        if (preserveUVs && newUVs.Count > 0)
            workingMesh.uv = newUVs.ToArray();

        if (preserveNormals && newNormals.Count > 0)
            workingMesh.normals = newNormals.ToArray();
        else
            workingMesh.RecalculateNormals();

        workingMesh.RecalculateBounds();
    }

    int AddOrGetVertexIndex(List<Vector3> vertices, Vector3 vertex, int originalIndex,
                           List<Vector2> uvs, List<Vector3> normals)
    {
        // Add vertex
        int index = vertices.Count;
        vertices.Add(vertex);

        // Add UV if available
        if (preserveUVs && originalUVs != null && originalUVs.Length > originalIndex)
            uvs.Add(originalUVs[originalIndex]);
        else if (preserveUVs)
            uvs.Add(Vector2.zero); // Add default UV if original not available

        // Add normal if available
        if (preserveNormals && originalNormals != null && originalNormals.Length > originalIndex)
            normals.Add(originalNormals[originalIndex]);

        return index;
    }

    bool IsPointInBounds(Vector3 point)
    {
        return point.x >= minX && point.x <= maxX &&
               point.y >= minY && point.y <= maxY &&
               point.z >= minZ && point.z <= maxZ;
    }

    // Reset trim bounds to original mesh bounds
    public void ResetBounds()
    {
        minX = originalMin.x;
        maxX = originalMax.x;
        minY = originalMin.y;
        maxY = originalMax.y;
        minZ = originalMin.z;
        maxZ = originalMax.z;

        TrimMesh();
        UpdateLastBounds();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (meshFilter != null && meshFilter.sharedMesh != null && !updateInRealtime)
        {
            // Delay call to avoid issues with OnValidate
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this != null) // Check if object still exists
                {
                    TrimMesh();
                    UpdateLastBounds();
                }
            };
        }
    }

    // Custom Editor for better UI and buttons
    [UnityEditor.CustomEditor(typeof(MeshTrimmer))]
    public class MeshTrimmerEditor : UnityEditor.Editor
    {
        SerializedProperty minX, maxX, minY, maxY, minZ, maxZ;
        SerializedProperty originalMin, originalMax;
        SerializedProperty preserveUVs, preserveNormals, updateInRealtime, useLocalSpace, clipEdges;

        void OnEnable()
        {
            minX = serializedObject.FindProperty("minX");
            maxX = serializedObject.FindProperty("maxX");
            minY = serializedObject.FindProperty("minY");
            maxY = serializedObject.FindProperty("maxY");
            minZ = serializedObject.FindProperty("minZ");
            maxZ = serializedObject.FindProperty("maxZ");

            originalMin = serializedObject.FindProperty("originalMin");
            originalMax = serializedObject.FindProperty("originalMax");

            preserveUVs = serializedObject.FindProperty("preserveUVs");
            preserveNormals = serializedObject.FindProperty("preserveNormals");
            updateInRealtime = serializedObject.FindProperty("updateInRealtime");
            useLocalSpace = serializedObject.FindProperty("useLocalSpace");
            clipEdges = serializedObject.FindProperty("clipEdges");
        }

        public override void OnInspectorGUI()
        {
            MeshTrimmer trimmer = (MeshTrimmer)target;
            serializedObject.Update();

            // Draw settings section
            UnityEditor.EditorGUILayout.LabelField("Trimmer Settings", UnityEditor.EditorStyles.boldLabel);
            UnityEditor.EditorGUILayout.PropertyField(preserveUVs);
            UnityEditor.EditorGUILayout.PropertyField(preserveNormals);
            UnityEditor.EditorGUILayout.PropertyField(updateInRealtime);
            UnityEditor.EditorGUILayout.PropertyField(useLocalSpace);
            UnityEditor.EditorGUILayout.PropertyField(clipEdges);

            UnityEditor.EditorGUILayout.Space();

            // Draw information about the original mesh
            UnityEditor.EditorGUILayout.LabelField("Original Mesh Information", UnityEditor.EditorStyles.boldLabel);
            GUI.enabled = false;
            UnityEditor.EditorGUILayout.Vector3Field("Min Bounds", trimmer.originalMin);
            UnityEditor.EditorGUILayout.Vector3Field("Max Bounds", trimmer.originalMax);

            if (trimmer.originalVertices != null)
                UnityEditor.EditorGUILayout.LabelField("Vertex Count", trimmer.originalVertices.Length.ToString());
            GUI.enabled = true;

            UnityEditor.EditorGUILayout.Space();

            // Draw trimming sliders
            UnityEditor.EditorGUILayout.LabelField("Trim Boundaries", UnityEditor.EditorStyles.boldLabel);

            UnityEditor.EditorGUILayout.LabelField("X Axis", UnityEditor.EditorStyles.boldLabel);
            UnityEditor.EditorGUILayout.BeginHorizontal();
            UnityEditor.EditorGUILayout.PropertyField(minX, new GUIContent("Min X"));
            UnityEditor.EditorGUILayout.PropertyField(maxX, new GUIContent("Max X"));
            UnityEditor.EditorGUILayout.EndHorizontal();

            UnityEditor.EditorGUILayout.LabelField("Y Axis", UnityEditor.EditorStyles.boldLabel);
            UnityEditor.EditorGUILayout.BeginHorizontal();
            UnityEditor.EditorGUILayout.PropertyField(minY, new GUIContent("Min Y"));
            UnityEditor.EditorGUILayout.PropertyField(maxY, new GUIContent("Max Y"));
            UnityEditor.EditorGUILayout.EndHorizontal();

            UnityEditor.EditorGUILayout.LabelField("Z Axis", UnityEditor.EditorStyles.boldLabel);
            UnityEditor.EditorGUILayout.BeginHorizontal();
            UnityEditor.EditorGUILayout.PropertyField(minZ, new GUIContent("Min Z"));
            UnityEditor.EditorGUILayout.PropertyField(maxZ, new GUIContent("Max Z"));
            UnityEditor.EditorGUILayout.EndHorizontal();

            UnityEditor.EditorGUILayout.Space();

            // Quick trimming buttons
            UnityEditor.EditorGUILayout.LabelField("Quick Trimming Actions", UnityEditor.EditorStyles.boldLabel);

            UnityEditor.EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Trim Left (X-)"))
            {
                minX.floatValue = Mathf.Lerp(trimmer.originalMin.x, trimmer.originalMax.x, 0.25f);
            }

            if (GUILayout.Button("Trim Right (X+)"))
            {
                maxX.floatValue = Mathf.Lerp(trimmer.originalMin.x, trimmer.originalMax.x, 0.75f);
            }
            UnityEditor.EditorGUILayout.EndHorizontal();

            UnityEditor.EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Trim Bottom (Y-)"))
            {
                minY.floatValue = Mathf.Lerp(trimmer.originalMin.y, trimmer.originalMax.y, 0.25f);
            }

            if (GUILayout.Button("Trim Top (Y+)"))
            {
                maxY.floatValue = Mathf.Lerp(trimmer.originalMin.y, trimmer.originalMax.y, 0.75f);
            }
            UnityEditor.EditorGUILayout.EndHorizontal();

            UnityEditor.EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Trim Back (Z-)"))
            {
                minZ.floatValue = Mathf.Lerp(trimmer.originalMin.z, trimmer.originalMax.z, 0.25f);
            }

            if (GUILayout.Button("Trim Front (Z+)"))
            {
                maxZ.floatValue = Mathf.Lerp(trimmer.originalMin.z, trimmer.originalMax.z, 0.75f);
            }
            UnityEditor.EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Reset All Trimming"))
            {
                trimmer.ResetBounds();
            }

            if (GUILayout.Button("Apply Trim (Create New Mesh)"))
            {
                CreateNewTrimmedMesh(trimmer);
            }

            serializedObject.ApplyModifiedProperties();
        }

        void CreateNewTrimmedMesh(MeshTrimmer trimmer)
        {
            // Create a permanent copy of the current trimmed mesh
            if (trimmer.workingMesh != null)
            {
                // Create a copy of the current working mesh
                Mesh newMesh = new Mesh();
                newMesh.vertices = trimmer.workingMesh.vertices;
                newMesh.triangles = trimmer.workingMesh.triangles;
                newMesh.uv = trimmer.workingMesh.uv;
                newMesh.normals = trimmer.workingMesh.normals;
                newMesh.RecalculateBounds();

                // Generate a unique name
                string path = UnityEditor.EditorUtility.SaveFilePanelInProject(
                    "Save Trimmed Mesh",
                    "Trimmed_" + trimmer.originalMesh.name,
                    "asset",
                    "Save the trimmed mesh as an asset"
                );

                if (!string.IsNullOrEmpty(path))
                {
                    // Save the mesh as an asset
                    UnityEditor.AssetDatabase.CreateAsset(newMesh, path);
                    UnityEditor.AssetDatabase.SaveAssets();

                    // Assign the new mesh to the MeshFilter
                    trimmer.meshFilter.sharedMesh = newMesh;

                    // Disable the trimmer component
                    trimmer.enabled = false;

                    UnityEditor.EditorUtility.DisplayDialog(
                        "Mesh Saved",
                        "The trimmed mesh has been saved as an asset and assigned to the MeshFilter.",
                        "OK"
                    );
                }
            }
        }
    }
#endif
}