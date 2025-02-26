using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace UltimateProceduralPrimitives
{
  public class AbstractGenerator
  {
    public virtual void Generate(Mesh mesh, MeshCutProducer _meshCutProducer) { }

    protected void Finishing(Mesh mesh, MyMeshInfo myMeshInfo, SurfaceType surfaceType, MeshCutProducer _meshCutProducer, bool flipNormalForMeshCut, PivotPosition pivotPosition)
    {
      mesh.Clear();

      //
      // Smooth or Flat
      //
      if (surfaceType == SurfaceType.Smooth)
      {
        if (myMeshInfo.myVtx.Count > 65535) mesh.indexFormat = IndexFormat.UInt32;
        mesh.SetVertices(myMeshInfo.myVtx);
        mesh.SetUVs(0, myMeshInfo.myUVs);
        mesh.SetIndices(myMeshInfo.myIdx, MeshTopology.Triangles, 0);
        NormalSolver.RecalculateNormals(mesh, 60);
        // mesh.RecalculateNormals();
      }
      if (surfaceType == SurfaceType.Flat)
      {
        var flatSurfaceMyMeshInfo = CreateFlatSurfaceMyMeshInfoStruct(myMeshInfo);
        if (flatSurfaceMyMeshInfo.myVtx.Count > 65535) mesh.indexFormat = IndexFormat.UInt32;
        mesh.SetVertices(flatSurfaceMyMeshInfo.myVtx);
        mesh.SetUVs(0, flatSurfaceMyMeshInfo.myUVs);
        mesh.SetIndices(flatSurfaceMyMeshInfo.myIdx, MeshTopology.Triangles, 0);
        mesh.RecalculateNormals();
      }

      //
      // MeshCutting. FlipNormal is in the Slice Function.
      //
      for (int i = 0; i < _meshCutProducer.numOfCuts.Count; i++)
      {
        if (_meshCutProducer.numOfCuts[i].doMeshCut)
        {
          var meshCut = new Yr.MeshCut();
          var unityPlane = new UnityEngine.Plane(_meshCutProducer.numOfCuts[i].cutPoint1, _meshCutProducer.numOfCuts[i].cutPoint2, _meshCutProducer.numOfCuts[i].cutPoint3);
          var unityPlaneAnchorValue = (_meshCutProducer.numOfCuts[i].cutPoint1 + _meshCutProducer.numOfCuts[i].cutPoint2 + _meshCutProducer.numOfCuts[i].cutPoint3) / 3.0f; // This is because UnityPlane move to (0,0,0) when anchor is not 0.
          var offset = _meshCutProducer.numOfCuts[i].offset;
          var isFillCaps = _meshCutProducer.numOfCuts[i].caps;
          var useCapMaterial = _meshCutProducer.numOfCuts[i].capMaterial ? true : false;

          Mesh[] afterCutMesh = meshCut.Slice(mesh, unityPlane.normal, unityPlaneAnchorValue + offset, isFillCaps, useCapMaterial, flipNormalForMeshCut);

          if (_meshCutProducer.numOfCuts[i].useSide == useSide.A)
          {
            mesh.Clear();
            mesh.SetVertices(afterCutMesh[1].vertices);
            mesh.SetNormals(afterCutMesh[1].normals);
            mesh.SetUVs(0, afterCutMesh[1].uv);

            mesh.SetTriangles(afterCutMesh[1].triangles, 0);

            mesh.subMeshCount = afterCutMesh[1].subMeshCount;
            for (int k = 0; k < afterCutMesh[1].subMeshCount; k++)
            {
              mesh.SetTriangles(afterCutMesh[1].GetTriangles(k), k);
              mesh.SetIndices(afterCutMesh[1].GetTriangles(k), MeshTopology.Triangles, k);
            }
          }
          if (_meshCutProducer.numOfCuts[i].useSide == useSide.B)
          {
            mesh.Clear();
            mesh.SetVertices(afterCutMesh[0].vertices);
            mesh.SetNormals(afterCutMesh[0].normals);
            mesh.SetUVs(0, afterCutMesh[0].uv);

            mesh.SetTriangles(afterCutMesh[0].triangles, 0);

            mesh.subMeshCount = afterCutMesh[0].subMeshCount;
            for (int k = 0; k < afterCutMesh[0].subMeshCount; k++)
            {
              mesh.SetTriangles(afterCutMesh[0].GetTriangles(k), k);
              mesh.SetIndices(afterCutMesh[0].GetTriangles(k), MeshTopology.Triangles, k);
            }
          }
          // Debug.Log(string.Join("_", mesh.vertices));
          // Debug.Log(string.Join("_", mesh.triangles));
          // Debug.Log(string.Join("_", mesh.uv));
        }
      }

      //
      // SetPivot
      //
      SetPivotPosition(mesh, pivotPosition);
    }

    //
    // SetPivot
    //
    void SetPivotPosition(Mesh mesh, PivotPosition pivotPosition)
    {
      if (pivotPosition == PivotPosition.Center) { return; }

      float offsetY = mesh.vertices[0].y;
      // ver 4.1 Start
      float offsetX = mesh.vertices[0].x;
      float offsetZ = mesh.vertices[0].z;
      // ver 4.1 End

      switch (pivotPosition)
      {
        case PivotPosition.Top:
          foreach (var item in mesh.vertices) { if (item.y > offsetY) { offsetY = item.y; } }
          offsetX = 0; offsetZ = 0; break;

        case PivotPosition.Bottom:
          foreach (var item in mesh.vertices) { if (item.y < offsetY) { offsetY = item.y; } }
          offsetX = 0; offsetZ = 0; break;

        // ver 5 Start
        case PivotPosition.TopLeft:
          foreach (var item in mesh.vertices)
          {
            if (item.y > offsetY) { offsetY = item.y; }
            if (item.x < offsetX) { offsetX = item.x; }
          }
          offsetZ = 0; break;

        case PivotPosition.TopRight:
          foreach (var item in mesh.vertices)
          {
            if (item.y > offsetY) { offsetY = item.y; }
            if (item.x > offsetX) { offsetX = item.x; }
          }
          offsetZ = 0; break;
        //----------------------------------------------------------------------------
        case PivotPosition.TopForward:
          foreach (var item in mesh.vertices)
          {
            if (item.y > offsetY) { offsetY = item.y; }
            if (item.z > offsetZ) { offsetZ = item.z; }
          }
          offsetX = 0; break;

        case PivotPosition.TopForwardLeft:
          foreach (var item in mesh.vertices)
          {
            if (item.y > offsetY) { offsetY = item.y; }
            if (item.z > offsetZ) { offsetZ = item.z; }
            if (item.x < offsetX) { offsetX = item.x; }
          }
          break;

        case PivotPosition.TopForwardRight:
          foreach (var item in mesh.vertices)
          {
            if (item.y > offsetY) { offsetY = item.y; }
            if (item.z > offsetZ) { offsetZ = item.z; }
            if (item.x > offsetX) { offsetX = item.x; }
          }
          break;
        //----------------------------------------------------------------------------
        case PivotPosition.TopBack:
          foreach (var item in mesh.vertices)
          {
            if (item.y > offsetY) { offsetY = item.y; }
            if (item.z < offsetZ) { offsetZ = item.z; }
          }
          offsetX = 0; break;

        case PivotPosition.TopBackLeft:
          foreach (var item in mesh.vertices)
          {
            if (item.y > offsetY) { offsetY = item.y; }
            if (item.z < offsetZ) { offsetZ = item.z; }
            if (item.x < offsetX) { offsetX = item.x; }
          }
          break;
        case PivotPosition.TopBackRight:
          foreach (var item in mesh.vertices)
          {
            if (item.y > offsetY) { offsetY = item.y; }
            if (item.z < offsetZ) { offsetZ = item.z; }
            if (item.x > offsetX) { offsetX = item.x; }
          }
          break;
        //----------------------------------------------------------------------------
        case PivotPosition.CenterLeft:
          foreach (var item in mesh.vertices) { if (item.x < offsetX) { offsetX = item.x; } }
          offsetY = 0; offsetZ = 0; break;

        case PivotPosition.CenterRight:
          foreach (var item in mesh.vertices) { if (item.x > offsetX) { offsetX = item.x; } }
          offsetY = 0; offsetZ = 0; break;
        //----------------------------------------------------------------------------
        case PivotPosition.CenterForward:
          foreach (var item in mesh.vertices) { if (item.z > offsetZ) { offsetZ = item.z; } }
          offsetX = 0; offsetY = 0; break;

        case PivotPosition.CenterForwardLeft:
          foreach (var item in mesh.vertices)
          {
            if (item.z > offsetZ) { offsetZ = item.z; }
            if (item.x < offsetX) { offsetX = item.x; }
          }
          offsetY = 0; break;
        case PivotPosition.CenterForwardRight:
          foreach (var item in mesh.vertices)
          {
            if (item.z > offsetZ) { offsetZ = item.z; }
            if (item.x > offsetX) { offsetX = item.x; }
          }
          offsetY = 0; break;
        //----------------------------------------------------------------------------
        case PivotPosition.CenterBack:
          foreach (var item in mesh.vertices) { if (item.z < offsetZ) { offsetZ = item.z; } }
          offsetX = 0; offsetY = 0; break;

        case PivotPosition.CenterBackLeft:
          foreach (var item in mesh.vertices)
          {
            if (item.z < offsetZ) { offsetZ = item.z; }
            if (item.x < offsetX) { offsetX = item.x; }
          }
          offsetY = 0; break;

        case PivotPosition.CenterBackRight:
          foreach (var item in mesh.vertices)
          {
            if (item.z < offsetZ) { offsetZ = item.z; }
            if (item.x > offsetX) { offsetX = item.x; }
          }
          offsetY = 0; break;
        //----------------------------------------------------------------------------
        case PivotPosition.BottomLeft:
          foreach (var item in mesh.vertices)
          {
            if (item.y < offsetY) { offsetY = item.y; }
            if (item.x < offsetX) { offsetX = item.x; }
          }
          offsetZ = 0; break;

        case PivotPosition.BottomRight:
          foreach (var item in mesh.vertices)
          {
            if (item.y < offsetY) { offsetY = item.y; }
            if (item.x > offsetX) { offsetX = item.x; }
          }
          offsetZ = 0; break;

        //----------------------------------------------------------------------------
        case PivotPosition.BottomForward:
          foreach (var item in mesh.vertices)
          {
            if (item.y < offsetY) { offsetY = item.y; }
            if (item.z > offsetZ) { offsetZ = item.z; }
          }
          offsetX = 0; break;

        case PivotPosition.BottomForwardLeft:
          foreach (var item in mesh.vertices)
          {
            if (item.y < offsetY) { offsetY = item.y; }
            if (item.z > offsetZ) { offsetZ = item.z; }
            if (item.x < offsetX) { offsetX = item.x; }
          }
          break;
        case PivotPosition.BottomForwardRight:
          foreach (var item in mesh.vertices)
          {
            if (item.y < offsetY) { offsetY = item.y; }
            if (item.z > offsetZ) { offsetZ = item.z; }
            if (item.x > offsetX) { offsetX = item.x; }
          }
          break;
        //----------------------------------------------------------------------------
        case PivotPosition.BottomBack:
          foreach (var item in mesh.vertices)
          {
            if (item.y < offsetY) { offsetY = item.y; }
            if (item.z < offsetZ) { offsetZ = item.z; }
          }
          offsetX = 0; break;

        case PivotPosition.BottomBackLeft:
          foreach (var item in mesh.vertices)
          {
            if (item.y < offsetY) { offsetY = item.y; }
            if (item.z < offsetZ) { offsetZ = item.z; }
            if (item.x < offsetX) { offsetX = item.x; }
          }
          break;
        case PivotPosition.BottomBackRight:
          foreach (var item in mesh.vertices)
          {
            if (item.y < offsetY) { offsetY = item.y; }
            if (item.z < offsetZ) { offsetZ = item.z; }
            if (item.x > offsetX) { offsetX = item.x; }
          }
          break;
        // ver 5 End


        default:
          break;
      }

      var newVertexes = new List<Vector3>();
      for (int i = 0; i < mesh.vertices.Length; i++)
      {
        newVertexes.Add(new Vector3(mesh.vertices[i].x - offsetX, mesh.vertices[i].y - offsetY, mesh.vertices[i].z - offsetZ));
      }
      mesh.SetVertices(newVertexes);
    }


    public MyMeshInfo CreateFlatSurfaceMyMeshInfoStruct(MyMeshInfo myMeshInfo)
    {
      var _myVtx = new List<Vector3>();
      var _myUVs = new List<Vector2>();
      var _myIdx = new List<int>();

      var i = 0;
      foreach (var idx in myMeshInfo.myIdx)
      {
        _myVtx.Add(myMeshInfo.myVtx[idx]);
        _myUVs.Add(myMeshInfo.myUVs[idx]);
        _myIdx.Add(i);
        i++;
      }

      var flatSurfaceMyMeshInfo = new MyMeshInfo()
      {
        myVtx = _myVtx,
        myUVs = _myUVs,
        myIdx = _myIdx,
      };
      return flatSurfaceMyMeshInfo;
    }
  }
}
