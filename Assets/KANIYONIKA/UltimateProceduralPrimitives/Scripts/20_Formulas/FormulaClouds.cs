/*
 * The following code was studied and constructed based on https://github.com/keijiro/Metamesh
 */

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using g3;
using UnityEngine;


namespace UltimateProceduralPrimitives
{
  public class FormulaClouds : AbstractFormula
  {
    public MyMeshInfo CalculateMyMeshInfo(CloudsParameters parameter)
    {

      ImplicitBlend3d blendedSpheres = new ImplicitBlend3d();

      // Let's Blend
      for (int i = 0; i < parameter.CloudSpheres.Count - 1; i++)
      {
        // Debug.Log($"i: {i}");
        if (parameter.CloudSpheres.Count == 0) { break; }

        if (i == 0)
        {
          blendedSpheres = new ImplicitBlend3d()
          {
            A = UtilsG3.CreateImplicitSphere3d(parameter.CloudSpheres[i].RadiusOfSphere, parameter.CloudSpheres[i].PositionOfSphere),
            B = UtilsG3.CreateImplicitSphere3d(parameter.CloudSpheres[i + 1].RadiusOfSphere, parameter.CloudSpheres[i + 1].PositionOfSphere),
            Blend = parameter.CloudSpheres[i].BlendWithNextSphere,
          };
        }
        else
        {
          blendedSpheres = new ImplicitBlend3d()
          {
            A = blendedSpheres,
            B = UtilsG3.CreateImplicitSphere3d(parameter.CloudSpheres[i + 1].RadiusOfSphere, parameter.CloudSpheres[i + 1].PositionOfSphere),
            Blend = parameter.CloudSpheres[i].BlendWithNextSphere,
          };
        }
      }

      // Create dMesh
      var dMesh = UtilsG3.generateMeshF_genDMesh3(new ImplicitOffset3d() { A = blendedSpheres, Offset = parameter.Roundness }, parameter.MarchingCubeCells, "dummyPath");

      // ReMesh
      if (parameter.SurfaceType == SurfaceType.Smooth)
      {
        // Debug.Log($"Before-dMeshTriCount:  {dMesh.TriangleCount * 3}");
        // Debug.Log($"Before-dMeshVtxCount:  {dMesh.VertexCount}");
        // Debug.Log($"Before-dMeshVtBCount:  {dMesh.VerticesBuffer.Count() / 3}");

        double min_edge_len, max_edge_len, avg_edge_len;
        MeshQueries.EdgeLengthStats(dMesh, out min_edge_len, out max_edge_len, out avg_edge_len);
        // Debug.Log($"avg_edge_len: {avg_edge_len}");

        Remesher reMesh = new Remesher(dMesh);
        reMesh.PreventNormalFlips = true;
        reMesh.SetTargetEdgeLength(avg_edge_len);
        for (int k = 0; k < 5; ++k)
          reMesh.BasicRemeshPass();

        // Debug.Log($"After-dMeshTriCount:  {dMesh.TriangleCount * 3}");
        // Debug.Log($"After-dMeshVtxCount:  {dMesh.VertexCount}");
        // Debug.Log($"After-dMeshVtBCount:  {dMesh.VerticesBuffer.Count() / 3}");
      }

      // convert dMesh to mesh
      var mesh = UtilsG3.DMesh3ToMesh(dMesh);
      // var mesh = UtilsG3.DMeshToUnityMesh(dMesh); // doesn't work


      // create UV
      float minX = mesh.vertices[0].x;
      float maxX = mesh.vertices[0].x;
      float minY = mesh.vertices[0].y;
      float maxY = mesh.vertices[0].y;

      foreach (var item in mesh.vertices)
      {
        if (item.x < minX) { minX = item.x; }
        if (item.x > maxX) { maxX = item.x; }
        if (item.y < minY) { minY = item.y; }
        if (item.y > maxY) { maxY = item.y; }
      }

      // Debug.Log($"x: {minX.ToString("0.0000")}  {maxX.ToString("0.0000")}  y: {minY.ToString("0.0000")}  {maxY.ToString("0.0000")}");
      foreach (var item in mesh.vertices)
      {
        float uvX = Mathf.InverseLerp(minX, maxX, item.x);
        float uvY = Mathf.InverseLerp(minY, maxY, item.y);
        myUVs.Add(new Vector2(uvX, uvY));
      }


      myVtx = new List<Vector3>(mesh.vertices);
      // myUVs = new List<Vector2>(mesh.uv); // already created above
      myIdx = new List<int>(mesh.GetIndices(0));

      FlipNormals(parameter.FlipNormals, myVtx, myUVs, myIdx);
      DirectionCollection(parameter.Direction, myVtx);
      // SetPivotPosition(parameter.PivotPosition, myVtx);
      return CreateMyMeshInfoStruct(myVtx, myUVs, myIdx);
    }
  }
}