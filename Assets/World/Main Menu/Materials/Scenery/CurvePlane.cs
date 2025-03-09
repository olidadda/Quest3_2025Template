using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[ExecuteInEditMode] // This makes it work in the editor
public class CurvePlane : MonoBehaviour
{
    public float curvature = 0.001f;
    public bool curveX = true;
    public bool curveZ = true;
    public Vector2 planeSize = new Vector2(10f, 10f);
    public int resolution = 20; // Grid resolution (vertices per side)

    private Mesh mesh;
    private Vector3[] originalVertices;
    private Vector3[] modifiedVertices;

    private float lastCurvature;
    private bool lastCurveX;
    private bool lastCurveZ;
    private Vector2 lastPlaneSize;
    private int lastResolution;

    void OnEnable()
    {
        // Always create a unique mesh instance for this object
        CreateUniqueMesh();
    }

    void OnDestroy()
    {
        // Clean up the mesh when the object is destroyed
        if (Application.isEditor && !Application.isPlaying)
        {
            if (mesh != null)
            {
                DestroyImmediate(mesh);
            }
        }
    }

    void CreateUniqueMesh()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();

        // Create a new mesh instance
        mesh = new Mesh();
        mesh.name = "CurvedPlaneMesh_" + GetInstanceID();
        meshFilter.sharedMesh = mesh;

        GenerateCurvedPlane();
        StoreCurrentSettings();
    }

    void Update()
    {
        // Check if any properties have changed
        if (HasSettingsChanged())
        {
            GenerateCurvedPlane();
            StoreCurrentSettings();
        }
    }

    bool HasSettingsChanged()
    {
        return lastCurvature != curvature ||
               lastCurveX != curveX ||
               lastCurveZ != curveZ ||
               lastPlaneSize != planeSize ||
               lastResolution != resolution;
    }

    void StoreCurrentSettings()
    {
        lastCurvature = curvature;
        lastCurveX = curveX;
        lastCurveZ = curveZ;
        lastPlaneSize = planeSize;
        lastResolution = resolution;
    }

    void GenerateCurvedPlane()
    {
        // Clear existing mesh data
        mesh.Clear();

        // Initialize arrays to hold vertices and triangles
        Vector3[] vertices = new Vector3[(resolution + 1) * (resolution + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[resolution * resolution * 6];

        // Calculate the size of each grid cell
        float cellSizeX = planeSize.x / resolution;
        float cellSizeZ = planeSize.y / resolution;

        // Generate vertices and UVs
        for (int z = 0; z <= resolution; z++)
        {
            for (int x = 0; x <= resolution; x++)
            {
                // Calculate the position of this vertex in local space
                float xPos = x * cellSizeX - planeSize.x / 2;
                float zPos = z * cellSizeZ - planeSize.y / 2;

                // Apply curvature based on distance from center
                float distSquared = 0f;
                if (curveX) distSquared += xPos * xPos;
                if (curveZ) distSquared += zPos * zPos;

                float yOffset = -distSquared * curvature;

                // Set the vertex position
                int index = z * (resolution + 1) + x;
                vertices[index] = new Vector3(xPos, yOffset, zPos);

                // Set the UV coordinate (normalized from 0 to 1)
                uv[index] = new Vector2((float)x / resolution, (float)z / resolution);
            }
        }

        // Generate triangles
        int triangleIndex = 0;
        for (int z = 0; z < resolution; z++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int bottomLeft = z * (resolution + 1) + x;
                int bottomRight = bottomLeft + 1;
                int topLeft = (z + 1) * (resolution + 1) + x;
                int topRight = topLeft + 1;

                // First triangle (bottom left - top left - top right)
                triangles[triangleIndex++] = bottomLeft;
                triangles[triangleIndex++] = topLeft;
                triangles[triangleIndex++] = topRight;

                // Second triangle (bottom left - top right - bottom right)
                triangles[triangleIndex++] = bottomLeft;
                triangles[triangleIndex++] = topRight;
                triangles[triangleIndex++] = bottomRight;
            }
        }

        // Assign to mesh
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        // Recalculate normals and bounds for proper lighting and culling
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // Store vertices for later modifications
        originalVertices = new Vector3[vertices.Length];
        System.Array.Copy(vertices, originalVertices, vertices.Length);
        modifiedVertices = new Vector3[vertices.Length];
    }

    // Method to allow runtime updates from other scripts
    public void UpdateCurvature(float newCurvature)
    {
        curvature = newCurvature;
        // This will trigger regeneration in Update()
    }

#if UNITY_EDITOR
    // This will ensure the mesh updates when you modify values in the Inspector
    void OnValidate()
    {
        if (mesh != null)
        {
            // Schedule an update for the next frame
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this != null) // Check if the object still exists
                {
                    GenerateCurvedPlane();
                    StoreCurrentSettings();
                }
            };
        }
    }
#endif
}