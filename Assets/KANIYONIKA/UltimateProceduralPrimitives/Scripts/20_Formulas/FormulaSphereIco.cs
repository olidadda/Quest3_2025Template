using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace UltimateProceduralPrimitives
{
  public class FormulaSphereIco : AbstractFormula
  {
    public MyMeshInfo CalculateMyMeshInfo(SphereIcoParameters parameter)
    {
      var builder = new IcosphereBuilder();
      for (var i = 1; i < parameter.Subdivision; i++)
        builder = new IcosphereBuilder(builder);

      var vtx = builder.Vertices.Select(v => (Vector3)(v * parameter.Radius));
      var idx = builder.Indices;


      //
      // Add vertexes
      //
      foreach (var item in vtx)
        myVtx.Add(item);

      //
      // Add to UVs
      //
      foreach (var item in myVtx)
      {
        float uvX = 0.0f;
        if (parameter.UVPattern == UVPattern.BothSidesButOtherSideFlip)
        {
          uvX = Mathf.Lerp(0.0f, 1.0f, (((float)item.x + (float)parameter.Radius)) / ((float)parameter.Radius * 2.0f));
        }
        else if (parameter.UVPattern == UVPattern.WrappingButOtherSideJaggies)
        {
          float work_uvX = 0.0f;
          if ((float)item.z > 0.0f && (float)item.x >= 0.0f) { work_uvX = (float)parameter.Radius - (float)item.x + (float)parameter.Radius * 3.0f; }
          else if ((float)item.z > 0.0f && (float)item.x <= 0.0f) { work_uvX = -(float)item.x; }
          else { work_uvX = (float)item.x + (float)parameter.Radius * 2; }
          work_uvX = (work_uvX / ((float)parameter.Radius * 4.0f));
          uvX = Mathf.Lerp(0.0f, 1.0f, work_uvX);
        }
        else if (parameter.UVPattern == UVPattern.OneSideButOtherSideJaggies)
        {
          uvX = Mathf.Lerp(0.0f, 1.0f, (((float)item.x + (float)parameter.Radius)) / ((float)parameter.Radius * 2.0f));
          if (item.z > 0 && item.x < 0) uvX = 0;
          if (item.z > 0 && item.x > 0) uvX = 1;
        }

        var uvY = Mathf.Lerp(0.0f, 1.0f, (((float)item.y + (float)parameter.Radius)) / ((float)parameter.Radius * 2.0f));
        myUVs.Add(new Vector2(uvX, uvY));
      }

      // Create myIdx
      foreach (var item in idx)
        myIdx.Add(item);


      FlipNormals(parameter.FlipNormals, myVtx, myUVs, myIdx);
      DirectionCollection(parameter.Direction, myVtx);
      // SetPivotPosition(parameter.PivotPosition, myVtx);
      return CreateMyMeshInfoStruct(myVtx, myUVs, myIdx);
    }
  }


  public sealed class IcosphereBuilder
  {
    public int VertexCount => _vertices.Count;
    public IEnumerable<Vector3> Vertices => _vertices;
    public IEnumerable<int> Indices => _triangles.SelectMany(it => new[] { it.i1, it.i2, it.i3 });

    public IcosphereBuilder() => BuildInitialInstance();
    public IcosphereBuilder(IcosphereBuilder source)
    {
      _vertices.AddRange(source._vertices);
      var midPoints = new MidpointTable(_vertices);

      foreach (var t in source._triangles)
      {
        var m1 = midPoints.GetMidpoint(t.i1, t.i2);
        var m2 = midPoints.GetMidpoint(t.i2, t.i3);
        var m3 = midPoints.GetMidpoint(t.i3, t.i1);

        _triangles.Add((t.i1, m1, m3));
        _triangles.Add((m1, t.i2, m2));
        _triangles.Add((m3, m2, t.i3));
        _triangles.Add((m1, m2, m3));
      }
    }

    List<Vector3> _vertices = new List<Vector3>();
    List<(int i1, int i2, int i3)> _triangles = new List<(int, int, int)>();

    void BuildInitialInstance()
    {
      var t = (1 + Mathf.Sqrt(5)) / 2;

      _vertices.Add(Vector3.Normalize(new Vector3(-1, +t, 0)));
      _vertices.Add(Vector3.Normalize(new Vector3(+1, +t, 0)));
      _vertices.Add(Vector3.Normalize(new Vector3(-1, -t, 0)));
      _vertices.Add(Vector3.Normalize(new Vector3(+1, -t, 0)));

      _vertices.Add(Vector3.Normalize(new Vector3(0, -1, +t)));
      _vertices.Add(Vector3.Normalize(new Vector3(0, +1, +t)));
      _vertices.Add(Vector3.Normalize(new Vector3(0, -1, -t)));
      _vertices.Add(Vector3.Normalize(new Vector3(0, +1, -t)));

      _vertices.Add(Vector3.Normalize(new Vector3(+t, 0, -1)));
      _vertices.Add(Vector3.Normalize(new Vector3(+t, 0, +1)));
      _vertices.Add(Vector3.Normalize(new Vector3(-t, 0, -1)));
      _vertices.Add(Vector3.Normalize(new Vector3(-t, 0, +1)));

      _triangles.Add((0, 11, 5));
      _triangles.Add((0, 5, 1));
      _triangles.Add((0, 1, 7));
      _triangles.Add((0, 7, 10));
      _triangles.Add((0, 10, 11));

      _triangles.Add((1, 5, 9));
      _triangles.Add((5, 11, 4));
      _triangles.Add((11, 10, 2));
      _triangles.Add((10, 7, 6));
      _triangles.Add((7, 1, 8));

      _triangles.Add((3, 9, 4));
      _triangles.Add((3, 4, 2));
      _triangles.Add((3, 2, 6));
      _triangles.Add((3, 6, 8));
      _triangles.Add((3, 8, 9));

      _triangles.Add((4, 9, 5));
      _triangles.Add((2, 4, 11));
      _triangles.Add((6, 2, 10));
      _triangles.Add((8, 6, 7));
      _triangles.Add((9, 8, 1));
    }


    class MidpointTable
    {
      List<Vector3> _vertices;
      Dictionary<int, int> _table;

      static int IndexPairToKey(int i1, int i2)
        => i1 < i2 ? i1 | (i2 << 16) : (i1 << 16) | i2;

      public MidpointTable(List<Vector3> vertices)
      {
        _vertices = vertices;
        _table = new Dictionary<int, int>();
      }

      public int GetMidpoint(int i1, int i2)
      {
        var key = IndexPairToKey(i1, i2);
        if (_table.ContainsKey(key)) return _table[key];
        var i = _vertices.Count;
        var mid = (_vertices[i1] + _vertices[i2]) / 2;
        _vertices.Add(Vector3.Normalize(mid));
        _table[key] = i;

        return i;
      }
    }
  }
}