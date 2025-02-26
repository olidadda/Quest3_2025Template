using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using g3;
using UnityEngine;
using UnityEngine.Rendering;

namespace UltimateProceduralPrimitives
{
  public static class UtilsG3
  {

    /// <summary>
    /// Convert DMesh3 to unity Mesh
    /// </summary>
    public static Mesh DMeshToUnityMesh(DMesh3 m, bool bLimitTo64k = false)
    {
      Mesh unityMesh = new Mesh();

      if (bLimitTo64k && (m.MaxVertexID > 65535 || m.MaxTriangleID > 65535))
      {
        unityMesh.indexFormat = IndexFormat.UInt32;
        // Debug.Log("g3UnityUtils.DMeshToUnityMesh: attempted to convert DMesh larger than 65535 verts/tris, not supported by Unity!");
        // return null;
      }

      unityMesh.vertices = dvector_to_vector3(m.VerticesBuffer);
      if (m.HasVertexNormals)
        unityMesh.normals = (m.HasVertexNormals) ? dvector_to_vector3(m.NormalsBuffer) : null;
      if (m.HasVertexColors)
        unityMesh.colors = dvector_to_color(m.ColorsBuffer);
      if (m.HasVertexUVs)
        unityMesh.uv = dvector_to_vector2(m.UVBuffer);
      unityMesh.triangles = dvector_to_int(m.TrianglesBuffer); // indices also created.

      if (m.HasVertexNormals == false)
        unityMesh.RecalculateNormals();

      return unityMesh;
    }


    /// <summary>
    /// Convert unity Mesh to a g3.DMesh3. Ignores UV's.
    /// </summary>
    public static DMesh3 UnityMeshToDMesh(Mesh mesh)
    {
      Vector3[] mesh_vertices = mesh.vertices;
      Vector3f[] dmesh_vertices = new Vector3f[mesh_vertices.Length];
      for (int i = 0; i < mesh.vertexCount; ++i)
        dmesh_vertices[i] = new Vector3f(mesh_vertices[i].x, mesh_vertices[i].y, mesh_vertices[i].z);

      Vector3[] mesh_normals = mesh.normals;
      if (mesh_normals != null)
      {
        Vector3f[] dmesh_normals = new Vector3f[mesh_vertices.Length];
        for (int i = 0; i < mesh.vertexCount; ++i)
          dmesh_normals[i] = new Vector3f(mesh_normals[i].x, mesh_normals[i].y, mesh_normals[i].z);

        return DMesh3Builder.Build(dmesh_vertices, mesh.triangles, dmesh_normals);

      }
      else
      {
        return DMesh3Builder.Build<Vector3f, int, Vector3f>(dmesh_vertices, mesh.triangles, null, null);
      }
    }

    // per-type conversion functions
    public static Vector3[] dvector_to_vector3(DVector<double> vec)
    {
      int nLen = vec.Length / 3;
      Vector3[] result = new Vector3[nLen];
      for (int i = 0; i < nLen; ++i)
      {
        result[i].x = (float)vec[3 * i];
        result[i].y = (float)vec[3 * i + 1];
        result[i].z = (float)vec[3 * i + 2];
      }
      return result;
    }
    public static Vector3[] dvector_to_vector3(DVector<float> vec)
    {
      int nLen = vec.Length / 3;
      Vector3[] result = new Vector3[nLen];
      for (int i = 0; i < nLen; ++i)
      {
        result[i].x = vec[3 * i];
        result[i].y = vec[3 * i + 1];
        result[i].z = vec[3 * i + 2];
      }
      return result;
    }
    public static Vector2[] dvector_to_vector2(DVector<float> vec)
    {
      int nLen = vec.Length / 2;
      Vector2[] result = new Vector2[nLen];
      for (int i = 0; i < nLen; ++i)
      {
        result[i].x = vec[2 * i];
        result[i].y = vec[2 * i + 1];
      }
      return result;
    }
    public static Color[] dvector_to_color(DVector<float> vec)
    {
      int nLen = vec.Length / 3;
      Color[] result = new Color[nLen];
      for (int i = 0; i < nLen; ++i)
      {
        result[i].r = vec[3 * i];
        result[i].g = vec[3 * i + 1];
        result[i].b = vec[3 * i + 2];
      }
      return result;
    }
    public static int[] dvector_to_int(DVector<int> vec)
    {
      // todo this could be faster because we can directly copy chunks...
      int nLen = vec.Length;
      int[] result = new int[nLen];
      for (int i = 0; i < nLen; ++i)
        result[i] = vec[i];
      return result;
    }

    //
    //
    //

    public static Mesh DMesh3ToMesh(DMesh3 dMesh)
    {
      var _mesh = new Mesh();

      var _myVtx = new List<Vector3>();
      var _myNrm = new List<Vector3>();
      var _myUVs = new List<Vector2>();
      var _myIdx = new List<int>();
      var _myTri = new List<int>();


      _myVtx = dvector_to_vector3(dMesh.VerticesBuffer).ToList();

      for (int i = 0; i < dMesh.TriangleCount; i++)
      {
        var tri = dMesh.Triangles().ToList();
        _myTri.Add(tri.ToArray()[i].a);
        _myTri.Add(tri.ToArray()[i].b);
        _myTri.Add(tri.ToArray()[i].c);
        _myIdx.Add(tri.ToArray()[i].a);
        _myIdx.Add(tri.ToArray()[i].b);
        _myIdx.Add(tri.ToArray()[i].c);
      }

      // Debug.Log($"myVtx.Count: {_myVtx.Count}");
      // Debug.Log($"myTri.Count: {_myTri.Count}");
      // Debug.Log($"myIdx.Count: {_myIdx.Count}");

      if (_myVtx.Count > 65535) _mesh.indexFormat = IndexFormat.UInt32;
      _mesh.SetVertices(_myVtx);
      _mesh.SetNormals(_myNrm); // empty
      _mesh.SetUVs(0, _myUVs); // empty
      _mesh.SetIndices(_myIdx, MeshTopology.Triangles, 0);
      _mesh.SetTriangles(_myTri, 0);
      _mesh.RecalculateNormals();

      return _mesh;
    }

    // g3 related function
    // generateMeshF() meshes the input implicit function at
    // the given cell resolution, and writes out the resulting mesh
    public static Action<BoundedImplicitFunction3d, int, string> generateMeshF = (root, numcells, path) =>
    {
      MarchingCubes c = new MarchingCubes();
      c.Implicit = root;
      c.RootMode = MarchingCubes.RootfindingModes.LerpSteps;      // cube-edge convergence method
      c.RootModeSteps = 5;                                        // number of iterations
      c.Bounds = root.Bounds();
      c.CubeSize = c.Bounds.MaxDim / numcells;
      c.Bounds.Expand(3 * c.CubeSize);                            // leave a buffer of cells
      c.Generate();
      MeshNormals.QuickCompute(c.Mesh);                           // generate normals
      StandardMeshWriter.WriteMesh(path, c.Mesh, WriteOptions.Defaults);   // write mesh
    };

    // g3 related function
    // return DMesh3
    public static Func<BoundedImplicitFunction3d, int, string, DMesh3> generateMeshF_genDMesh3 = (root, numcells, path) =>
    {
      MarchingCubes c = new MarchingCubes();
      c.Implicit = root;
      c.RootMode = MarchingCubes.RootfindingModes.LerpSteps;      // cube-edge convergence method
      c.RootModeSteps = 5;                                        // number of iterations
      c.Bounds = root.Bounds();
      c.CubeSize = c.Bounds.MaxDim / numcells;
      c.Bounds.Expand(3 * c.CubeSize);                            // leave a buffer of cells
      c.Generate();
      MeshNormals.QuickCompute(c.Mesh);                           // generate normals
                                                                  // StandardMeshWriter.WriteMesh(path, c.Mesh, WriteOptions.Defaults);   // write mesh
                                                                  // Debug.Log($"Bounds: {c.Bounds}");
                                                                  // Debug.Log($"cubeSize: {c.CubeSize}");
      return c.Mesh;
    };

    // g3 related function
    // meshToImplicitF() generates a narrow-band distance-field and
    // returns it as an implicit surface, that can be combined with other implicits
    public static Func<DMesh3, int, double, BoundedImplicitFunction3d> meshToImplicitF = (meshIn, numcells, max_offset) =>
    {
      double meshCellsize = meshIn.CachedBounds.MaxDim / numcells;
      MeshSignedDistanceGrid levelSet = new MeshSignedDistanceGrid(meshIn, meshCellsize);
      levelSet.ExactBandWidth = (int)(max_offset / meshCellsize) + 1;
      levelSet.Compute();
      return new DenseGridTrilinearImplicit(levelSet.Grid, levelSet.GridOrigin, levelSet.CellSize);
    };

    // g3 related function
    public static ImplicitSphere3d CreateImplicitSphere3d(double radius, Vector3d position)
    {
      ImplicitSphere3d g3Sphere = new ImplicitSphere3d()
      {
        Radius = radius,
        Origin = position,
      };
      return g3Sphere;
    }


  }
}
