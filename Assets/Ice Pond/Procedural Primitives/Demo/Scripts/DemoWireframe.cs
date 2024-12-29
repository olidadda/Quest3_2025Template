using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil.Demo
{
    public class DemoWireframe : MonoBehaviour
    {
        MeshFilter m_meshFilter;
        Mesh m_mesh;

        private void Awake()
        {
            m_meshFilter = GetComponent<MeshFilter>();
            m_mesh = new Mesh();
        }

        public void RefreshMesh(Mesh mesh)
        {
            var maximumNumberOfVertices = 65534; //Since unity uses a 16-bit indices, not sure if this is still the case. http://answers.unity3d.com/questions/255405/vertex-limit.html
            var meshTriangles = mesh.triangles;
            var meshVertices = mesh.vertices;
            var meshNormals = mesh.normals;
            var boneWeights = mesh.boneWeights;

            var numberOfVerticesRequiredForTheProcessedMesh = meshTriangles.Length;
            if (numberOfVerticesRequiredForTheProcessedMesh > maximumNumberOfVertices)
            {
                Debug.LogError("Wireframe renderer can't safely create the processed mesh it needs because the resulting number of vertices would surpass unity vertex limit!");
                return;
            }

            var processedVertices = new Vector3[numberOfVerticesRequiredForTheProcessedMesh];
            var processedUVs = new Vector2[numberOfVerticesRequiredForTheProcessedMesh];
            var processedTriangles = new int[meshTriangles.Length];
            var processedNormals = new Vector3[numberOfVerticesRequiredForTheProcessedMesh];

            var boneWeigthsArraySize = boneWeights.Length > 0 ? numberOfVerticesRequiredForTheProcessedMesh : 0;
            var processedBoneWeigths = new BoneWeight[boneWeigthsArraySize]; //The size of the array is either the same as vertexCount or empty.

            for (var i = 0; i < meshTriangles.Length; i += 3)
            {
                processedVertices[i] = meshVertices[meshTriangles[i]];
                processedVertices[i + 1] = meshVertices[meshTriangles[i + 1]];
                processedVertices[i + 2] = meshVertices[meshTriangles[i + 2]];

                processedUVs[i] = new Vector2(0f, 0f);
                processedUVs[i + 1] = new Vector2(1f, 0f);
                processedUVs[i + 2] = new Vector2(0f, 1f);

                processedTriangles[i] = i;
                processedTriangles[i + 1] = i + 1;
                processedTriangles[i + 2] = i + 2;

                processedNormals[i] = meshNormals[meshTriangles[i]];
                processedNormals[i + 1] = meshNormals[meshTriangles[i + 1]];
                processedNormals[i + 2] = meshNormals[meshTriangles[i + 2]];

                if (processedBoneWeigths.Length > 0)
                {
                    processedBoneWeigths[i] = boneWeights[meshTriangles[i]];
                    processedBoneWeigths[i + 1] = boneWeights[meshTriangles[i + 1]];
                    processedBoneWeigths[i + 2] = boneWeights[meshTriangles[i + 2]];
                }
            }

            if (m_mesh == null) m_mesh = new Mesh();
            m_mesh.Clear();
            m_mesh.vertices = processedVertices;
            m_mesh.uv = processedUVs;
            m_mesh.triangles = processedTriangles;
            m_mesh.normals = processedNormals;
            m_mesh.bindposes = mesh.bindposes;
            m_mesh.boneWeights = processedBoneWeigths;

            if (m_meshFilter == null) m_meshFilter = GetComponent<MeshFilter>();
            if (m_meshFilter != null) m_meshFilter.sharedMesh = m_mesh;
        }
    }
}