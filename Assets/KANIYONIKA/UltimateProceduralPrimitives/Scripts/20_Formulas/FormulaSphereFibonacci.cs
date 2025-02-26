using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GK;

namespace UltimateProceduralPrimitives
{

  public class FormulaSphereFibonacci : AbstractFormula
  {
    public MyMeshInfo CalculateMyMeshInfo(SphereFibonacciParameters parameter)
    {

      //
      // Create myVtx
      //
      var points = new List<Vector3>();
      float phi = (float)(Mathf.PI * (3.0 - Mathf.Sqrt(5.0f)));

      points = GetFibonacciSpherePoints(parameter.Vertices, parameter.Radius);
      foreach (var item in points)
      {
        myVtx.Add(item);
      }

      // StereographicProjection
      var stereographicProjection = new StereographicProjection();
      stereographicProjection.ExecuteStereographicProjection(points, myVtx, parameter.Radius, Vector3.zero);

      // DelaunayCalculator
      var pointsStereographicProjection = new List<Vector2>();
      foreach (var item in points)
      {
        var vec2 = new Vector2(item.x, item.y);
        pointsStereographicProjection.Add(vec2);
      }
      var delaunayCalculator = new DelaunayCalculator();
      var delaunayTriangulationResult = delaunayCalculator.CalculateTriangulation(pointsStereographicProjection);

      // // Add to vertexes (faces for bottom). Because of the flat expansion in the stereographicProjection, there is a hole at the bottom.
      myIdx.Add(0); // triangle1
      myIdx.Add(2); // triangle1
      myIdx.Add(1); // triangle1
      myIdx.Add(1); // triangle2
      myIdx.Add(2); // triangle2
      myIdx.Add(4); // triangle2
      myIdx.Add(3); // triangle3 - sometimes needed
      myIdx.Add(0); // triangle3 - sometimes needed
      myIdx.Add(1); // triangle# - sometimes needed


      // Add to vertexes.
      foreach (var item in delaunayTriangulationResult.Triangles)
      {
        myIdx.Add(item);
      }

      // Adjustment. Because the stereographicProjection axis is x and y, (not x and z),
      DirectionCollection(Direction.Z_Axis, myVtx);


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


      // for (int i = 0; i < pointsOrigin.Count; i++)
      // {
      //   Debug.Log($"i: {i.ToString("000")} ------------- x: {pointsOrigin[i].x.ToString("000.000")}  y: {pointsOrigin[i].y.ToString("000.000")}  z: {pointsOrigin[i].z.ToString("000.000")}");
      // }
      // for (int i = 0; i < uvs.Count; i++)
      // {
      //   Debug.Log($"i: {i.ToString("000")} ------------- x: {uvs[i].x.ToString("0.000")}  y: {uvs[i].y.ToString("0.000")}  \n  -------------------------- x: {vertexes[i].x.ToString("000.000")}  y: {vertexes[i].y.ToString("000.000")}  z: {vertexes[i].z.ToString("000.000")}");
      // }



      FlipNormals(parameter.FlipNormals, myVtx, myUVs, myIdx);
      DirectionCollection(parameter.Direction, myVtx);
      // SetPivotPosition(parameter.PivotPosition, myVtx);
      return CreateMyMeshInfoStruct(myVtx, myUVs, myIdx);
    }


    public List<Vector3> GetFibonacciSpherePoints(int vertices, float sphereRadius)
    {
      var points = new List<Vector3>();
      float phi = (float)(Mathf.PI * (3.0 - Mathf.Sqrt(5.0f)));

      for (int i = 0; i < vertices; i++)
      {
        float theta = phi * i;
        float z = -1.0f + ((float)i / (vertices - 1.0f)) * 2.0f;
        float dist_z = Mathf.Sqrt(1.0f - Mathf.Pow(z, 2));
        float x = Mathf.Cos(theta) * dist_z;
        float y = Mathf.Sin(theta) * dist_z;
        points.Add(new Vector3(x * sphereRadius, y * sphereRadius, z * sphereRadius));
      }
      return points;
    }

  }

}