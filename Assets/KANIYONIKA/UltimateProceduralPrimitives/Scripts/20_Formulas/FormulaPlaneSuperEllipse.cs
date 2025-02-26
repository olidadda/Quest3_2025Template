using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UltimateProceduralPrimitives
{
  public class FormulaPlaneSuperEllipse : AbstractFormula
  {
    public MyMeshInfo CalculateMyMeshInfo(PlaneSuperEllipseParameters parameter)
    {
      CreateVertexesAndUVs(parameter, myVtx, myUVs, myGlobeVtx_For2D, myGlobeUVs_For2D, myIdx);

      FlipNormals(parameter.FlipNormals, myVtx, myUVs, myIdx);
      DirectionCollection(parameter.Direction, myVtx);
      // SetPivotPosition(parameter.PivotPosition, myVtx);
      var myMeshInfo = CreateMyMeshInfoStruct(myVtx, myUVs, myIdx);

      if (parameter.DoubleSided)
      {
        myVtx.Clear();
        myUVs.Clear();
        myIdx.Clear();


        if (parameter.DoubleSided)
        {
          var parameterForDoubleSided = parameter;
          if (parameter.Orientation == Orientation.Up) parameterForDoubleSided.Orientation = Orientation.Down;
          if (parameter.Orientation == Orientation.Down) parameterForDoubleSided.Orientation = Orientation.Up;
          if (parameter.Orientation == Orientation.Left) parameterForDoubleSided.Orientation = Orientation.Right;
          if (parameter.Orientation == Orientation.Right) parameterForDoubleSided.Orientation = Orientation.Left;
          if (parameter.Orientation == Orientation.Forward) parameterForDoubleSided.Orientation = Orientation.Backward;
          if (parameter.Orientation == Orientation.Backward) parameterForDoubleSided.Orientation = Orientation.Forward;
          CreateVertexesAndUVs(parameterForDoubleSided, myVtx, myUVs, myGlobeVtx_For2D_ForDoubleSided, myGlobeUVs_For2D_ForDoubleSided, myIdx);
        }

        FlipNormals(parameter.FlipNormals, myVtx, myUVs, myIdx);
        DirectionCollection(parameter.Direction, myVtx);
        // SetPivotPosition(parameter.PivotPosition, myVtx);
        var myMeshInfo_ForDoubleSided = CreateMyMeshInfoStruct(myVtx, myUVs, myIdx);

        var newMyVtx = new List<Vector3>();
        foreach (var item in myMeshInfo.myVtx)
          newMyVtx.Add(item);
        foreach (var item in myMeshInfo_ForDoubleSided.myVtx)
          newMyVtx.Add(item);

        var newMyUVs = new List<Vector2>();
        foreach (var item in myMeshInfo.myUVs)
          newMyUVs.Add(item);
        foreach (var item in myMeshInfo_ForDoubleSided.myUVs)
          newMyUVs.Add(item);

        var newMyIdx = new List<int>();
        foreach (var item in myMeshInfo.myIdx)
          newMyIdx.Add(item);
        foreach (var item in myMeshInfo_ForDoubleSided.myIdx)
          newMyIdx.Add(item + myMeshInfo.myVtx.Count);

        return CreateMyMeshInfoStruct(newMyVtx, newMyUVs, newMyIdx);
      }
      else
      {
        return myMeshInfo;
      }
    }

    // List is Reference Passing
    void CreateVertexesAndUVs(PlaneSuperEllipseParameters _parameter, List<Vector3> ref_myVtx, List<Vector2> ref_myUVs, List<Vector3> ref_myGlobeVtx_For2D, List<Vector2> ref_myGlobeUVs_For2D, List<int> ref_myIdx)
    {
      //
      // Create myVtx
      //
      for (int i = 0; i < _parameter.Segments + 1; i++)
      {
        float theta = 0.0f;

        float cosTheta = 0.0f;
        float sinTheta = 0.0f;
        float Abs_cosTheta = 0.0f;
        float Abs_sinTheta = 0.0f;

        float n = 0.0f;
        float x = 0.0f;
        float y = 0.0f;
        float z = 0.0f;

        // Set theta
        switch (_parameter.Orientation)
        {
          case Orientation.Up:
          case Orientation.Right:
          case Orientation.Backward:
            theta = Mathf.Lerp(0.0f, Mathf.PI * 2.0f, (float)i / (float)_parameter.Segments);
            SetThetaRelatedParameter();
            break;
          case Orientation.Left:
          case Orientation.Down:
          case Orientation.Forward:
            theta = Mathf.Lerp(0.0f, Mathf.PI * 2.0f, 1.0f - (float)i / (float)_parameter.Segments); // To Flip triangles
            SetThetaRelatedParameter();
            break;

          default:
            break;
        }
        void SetThetaRelatedParameter()
        {
          cosTheta = Mathf.Cos(theta);
          sinTheta = Mathf.Sin(theta);
          Abs_cosTheta = Mathf.Abs(Mathf.Cos(theta));
          Abs_sinTheta = Mathf.Abs(Mathf.Sin(theta));

          // Set N
          if (cosTheta < 0 && sinTheta >= 0) { n = _parameter.N1; }
          if (cosTheta >= 0 && sinTheta >= 0) { n = _parameter.N2; }
          if (cosTheta < 0 && sinTheta < 0) { n = _parameter.N3; }
          if (cosTheta >= 0 && sinTheta < 0) { n = _parameter.N4; }
        }


        // Set myVtx
        switch (_parameter.Orientation)
        {
          case Orientation.Up:
          case Orientation.Down:
            // Create myVtx
            x = _parameter.Width / 2.0f * Mathf.Pow(Abs_cosTheta, n);
            z = _parameter.Height / 2.0f * Mathf.Pow(Abs_sinTheta, n);
            if (cosTheta < 0) { x = x * -1.0f; }
            if (sinTheta < 0) { z = z * -1.0f; }
            break;

          case Orientation.Left:
          case Orientation.Right:
            // Create myVtx
            z = _parameter.Width / 2.0f * Mathf.Pow(Abs_cosTheta, n);
            y = _parameter.Height / 2.0f * Mathf.Pow(Abs_sinTheta, n);
            if (cosTheta < 0) { z = z * -1.0f; }
            if (sinTheta < 0) { y = y * -1.0f; }
            break;

          case Orientation.Forward:
          case Orientation.Backward:
            // Create myVtx
            x = _parameter.Width / 2.0f * Mathf.Pow(Abs_cosTheta, n);
            y = _parameter.Height / 2.0f * Mathf.Pow(Abs_sinTheta, n);
            if (cosTheta < 0) { x = x * -1.0f; }
            if (sinTheta < 0) { y = y * -1.0f; }
            break;

          default:
            break;
        }

        ref_myGlobeVtx_For2D.Add(new Vector3(x, y, z));
        // Debug.Log($"i: {i.ToString("000")} ------- theta: {theta.ToString("000.0000000")} ------- cosTheta: {cosTheta.ToString("000.0000000")} ------- sinTheta:{sinTheta.ToString("000.0000000")} --||---||-- x: {ref_globeVertexes_For2D[i].x.ToString("000.0000000")} ------- y: {ref_globeVertexes_For2D[i].y.ToString("000.0000000")} ------- z: {ref_globeVertexes_For2D[i].z.ToString("000.0000000")}");
      }



      //
      // Create myUVs
      //
      var uvX = 0.0f;
      var uvY = 0.0f;

      var maximumX = ref_myGlobeVtx_For2D[0].x;
      var minimumX = ref_myGlobeVtx_For2D[0].x;
      var maximunY = ref_myGlobeVtx_For2D[0].y;
      var minimumY = ref_myGlobeVtx_For2D[0].y;
      var maximunZ = ref_myGlobeVtx_For2D[0].z;
      var minimumZ = ref_myGlobeVtx_For2D[0].z;

      foreach (var item in ref_myGlobeVtx_For2D)
      {
        if (item.x > maximumX) { maximumX = item.x; }
        if (item.x < minimumX) { minimumX = item.x; }
        if (item.y > maximunY) { maximunY = item.y; }
        if (item.y < minimumY) { minimumY = item.y; }
        if (item.z > maximunZ) { maximunZ = item.z; }
        if (item.z < minimumZ) { minimumZ = item.z; }
      }
      var diffX = maximumX - minimumX;
      var diffY = maximunY - minimumY;
      var diffZ = maximunZ - minimumZ;


      foreach (var item in ref_myGlobeVtx_For2D)
      {
        switch (_parameter.Orientation)
        {
          case Orientation.Up:
            uvX = Mathf.Lerp(0.0f, 1.0f, (item.x - minimumX) / diffX);
            uvY = Mathf.Lerp(0.0f, 1.0f, (item.z - minimumZ) / diffZ);
            break;
          case Orientation.Down:
            uvX = Mathf.Lerp(0.0f, 1.0f, 1.0f - (item.x - minimumX) / diffX); // To put back inverted texture.
            uvY = Mathf.Lerp(0.0f, 1.0f, (item.z - minimumZ) / diffZ);
            break;

          case Orientation.Left:
            uvX = Mathf.Lerp(0.0f, 1.0f, 1.0f - (item.z - minimumZ) / diffZ);// To put back inverted texture.
            uvY = Mathf.Lerp(0.0f, 1.0f, (item.y - minimumY) / diffY);
            break;
          case Orientation.Right:
            uvX = Mathf.Lerp(0.0f, 1.0f, (item.z - minimumZ) / diffZ);
            uvY = Mathf.Lerp(0.0f, 1.0f, (item.y - minimumY) / diffY);
            break;

          case Orientation.Forward:
            uvX = Mathf.Lerp(0.0f, 1.0f, 1.0f - (item.x - minimumX) / diffX); // To put back inverted texture.
            uvY = Mathf.Lerp(0.0f, 1.0f, (item.y - minimumY) / diffY);
            break;
          case Orientation.Backward:
            uvX = Mathf.Lerp(0.0f, 1.0f, (item.x - minimumX) / diffX);
            uvY = Mathf.Lerp(0.0f, 1.0f, (item.y - minimumY) / diffY);
            break;

          default:
            break;
        }
        ref_myGlobeUVs_For2D.Add(new Vector2(uvX, uvY));
        // Debug.Log($"UVx: {uvX.ToString("000.0000000")} ------- UVy: {uvY.ToString("000.0000000")}")
      }


      //
      // Create vertexes and uvs
      //
      var centerVertex = new Vector3(0, 0, 0);
      var centerUVs = new Vector2(0.5f, 0.5f);

      ref_myVtx.Add(centerVertex);
      ref_myUVs.Add(centerUVs);

      for (int i = 0; i < ref_myGlobeVtx_For2D.Count; i++)
      {
        ref_myVtx.Add(ref_myGlobeVtx_For2D[i]);
        ref_myUVs.Add(ref_myGlobeUVs_For2D[i]); // same count
      }

      for (int i = 0; i < _parameter.Segments + 1; i++) // +1 is since centerVertex is added.
      {
        ref_myIdx.Add(0);
        ref_myIdx.Add(i + 1);
        ref_myIdx.Add(i);
      }


    }
  }
}