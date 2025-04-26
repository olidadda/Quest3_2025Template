//using UnityEngine;

//[RequireComponent(typeof(MeshRenderer))]
//[RequireComponent(typeof(MeshFilter))]
//public class WireframeController : MonoBehaviour
//{

//    private Mesh _mesh;
//    private Material _wireMaterial;
//    public Color wireColor = Color.green;

//    void Start()
//    {
//        // Grab the mesh from the MeshFilter
//        _mesh = GetComponent<MeshFilter>().sharedMesh;

//        // Create a simple material for drawing lines
//        _wireMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
//    }

//    void OnDrawGizmos()
//    {
//        // In the Editor (Scene view), Unity calls OnDrawGizmos
//        // We'll draw the wireframe for visualization purposes here
//        Mesh mesh = GetComponent<MeshFilter>()?.sharedMesh;
//        if (mesh == null) return;

//        Gizmos.color = wireColor;

//        Vector3[] vertices = mesh.vertices;
//        int[] triangles = mesh.triangles;

//        for (int i = 0; i < triangles.Length; i += 3)
//        {
//            Vector3 v1 = transform.TransformPoint(vertices[triangles[i]]);
//            Vector3 v2 = transform.TransformPoint(vertices[triangles[i + 1]]);
//            Vector3 v3 = transform.TransformPoint(vertices[triangles[i + 2]]);

//            Gizmos.DrawLine(v1, v2);
//            Gizmos.DrawLine(v2, v3);
//            Gizmos.DrawLine(v3, v1);
//        }
//    }

//    void OnRenderObject()
//    {
//        // OnRenderObject gets called at runtime in both the editor and actual builds
//        // This will draw lines in Game view and in builds.
//        if (_mesh == null || _wireMaterial == null) return;

//        // Apply the line-drawing material
//        _wireMaterial.SetPass(0);

//        GL.PushMatrix();

//        GL.Begin(GL.LINES);
//        GL.Color(wireColor);

//        Vector3[] vertices = _mesh.vertices;
//        int[] triangles = _mesh.triangles;

//        // Draw each triangle edge
//        for (int i = 0; i < triangles.Length; i += 3)
//        {
//            Vector3 v1 = transform.TransformPoint(vertices[triangles[i]]);
//            Vector3 v2 = transform.TransformPoint(vertices[triangles[i + 1]]);
//            Vector3 v3 = transform.TransformPoint(vertices[triangles[i + 2]]);

//            GL.Vertex(v1);
//            GL.Vertex(v2);

//            GL.Vertex(v2);
//            GL.Vertex(v3);

//            GL.Vertex(v3);
//            GL.Vertex(v1);
//        }

//        GL.End();

//        GL.PopMatrix();
//    }

//    void OnDestroy()
//    {
//        if (_wireMaterial != null)
//        {
//            Destroy(_wireMaterial);
//            _wireMaterial = null;
//        }
//    }
//}

using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class WireframeController : MonoBehaviour
{

   private Mesh _mesh;
   private Material _wireMaterial;
   public Color wireColor = Color.green;

   void Start()
   {
       //// Grab the mesh from the MeshFilter
       //_mesh = GetComponent<MeshFilter>().sharedMesh;

       //// Create a simple material for drawing lines
       //_wireMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));


            _mesh = GetComponent<MeshFilter>().sharedMesh;
       Shader wireShader = Shader.Find("Hidden/Internal-Colored");
       if (wireShader == null) {
           Debug.LogError("Wireframe Shader 'Hidden/Internal-Colored' not found!");
           _wireMaterial = null; // Ensure it's null if shader not found
       } else {
           _wireMaterial = new Material(wireShader);
           Debug.Log("Wireframe Shader found and material created.");
       }
       // Check mesh too
       if (_mesh == null) {
            Debug.LogError("MeshFilter has no mesh assigned!");
       }

   }

   void OnDrawGizmos()
   {
       // In the Editor (Scene view), Unity calls OnDrawGizmos
       // We'll draw the wireframe for visualization purposes here
       Mesh mesh = GetComponent<MeshFilter>()?.sharedMesh;
       if (mesh == null) return;

       Gizmos.color = wireColor;

       Vector3[] vertices = mesh.vertices;
       int[] triangles = mesh.triangles;

       for (int i = 0; i < triangles.Length; i += 3)
       {
           Vector3 v1 = transform.TransformPoint(vertices[triangles[i]]);
           Vector3 v2 = transform.TransformPoint(vertices[triangles[i + 1]]);
           Vector3 v3 = transform.TransformPoint(vertices[triangles[i + 2]]);

           Gizmos.DrawLine(v1, v2);
           Gizmos.DrawLine(v2, v3);
           Gizmos.DrawLine(v3, v1);
       }
   }

   void OnRenderObject()
   {
        //Debug.Log($"OnRenderObject called by camera: {Camera.current?.name ?? "NULL"} for object: {gameObject.name}");

        // OnRenderObject gets called at runtime in both the editor and actual builds
        // This will draw lines in Game view and in builds.
        if (_mesh == null || _wireMaterial == null) return;

       // Apply the line-drawing material
       _wireMaterial.SetPass(0);

       GL.Begin(GL.LINES);
       GL.Color(wireColor);

       Vector3[] vertices = _mesh.vertices;
       int[] triangles = _mesh.triangles;

       // Draw each triangle edge
       for (int i = 0; i < triangles.Length; i += 3)
       {
           Vector3 v1 = transform.TransformPoint(vertices[triangles[i]]);
           Vector3 v2 = transform.TransformPoint(vertices[triangles[i + 1]]);
           Vector3 v3 = transform.TransformPoint(vertices[triangles[i + 2]]);

           GL.Vertex(v1);
           GL.Vertex(v2);

           GL.Vertex(v2);
           GL.Vertex(v3);

           GL.Vertex(v3);
           GL.Vertex(v1);
       }

       GL.End();
   }

}