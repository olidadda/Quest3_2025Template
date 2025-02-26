using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateProceduralPrimitives
{

  public class FormulaTearDrop : AbstractFormula
  {
    public MyMeshInfo CalculateMyMeshInfo(TearDropParameters parameter)
    {

      //
      // Create vertexes and uvs
      //
      // j is　for latitude shape.  (Shape when cut vertically.)
      for (int j = 0; j < parameter.Segments + 1; j++)
      {
        // Debug.Log($"------------- {j.ToString("000")} --- j Loop ------------------");
        var theta = Mathf.PI * (float)j / (float)parameter.Segments;

        // j is　for longitude shape.  (Shape when cut horizontally.)
        myGlobeVtx.Add(new List<Vector3>());
        myGlobeUVs.Add(new List<Vector2>());
        for (int i = 0; i < parameter.Segments + 1; i++)
        {
          // Debug.Log($"------------- {i.ToString("000")} --- i Loop ------------------");
          var phi = Mathf.PI * 2.0f * i / (float)parameter.Segments;
          myGlobeVtx[j].Add(AdditionalFormula(parameter.Width, parameter.Height, parameter.Depth, theta, phi));

          float uvX = Mathf.Lerp(0.25f, 1.25f, (float)i / (float)parameter.Segments);
          float uvY = Mathf.Lerp(0.0f, 1.0f, (float)j / (float)parameter.Segments);
          myGlobeUVs[j].Add(new Vector2(uvX, uvY));


          // Debug.Log($"x: {globeVertexes[j][i].x.ToString("00.00")}  y: {globeVertexes[j][i].y.ToString("00.00")}  z: {globeVertexes[j][i].z.ToString("00.00")}");
        }
      }

      CreateMyVtxUVsIdx_FlexPlane_Supershape_SuperEllipsoid_TearDrop(myVtx, myUVs, myGlobeVtx, myGlobeUVs, myIdx);
      FlipNormals(parameter.FlipNormals, myVtx, myUVs, myIdx);
      DirectionCollection(parameter.Direction, myVtx);
      // SetPivotPosition(parameter.PivotPosition, myVtx);
      return CreateMyMeshInfoStruct(myVtx, myUVs, myIdx);
    }

    Vector3 AdditionalFormula(float width, float height, float depth, float theta, float phi)
    {
      float cosTheta = Mathf.Cos(theta);
      float sinTheta = Mathf.Sin(theta);
      float cosPhi = Mathf.Cos(phi);
      float sinPhi = Mathf.Sin(phi);

      float x = width / 2.0f * (-1.0f - cosTheta) * sinTheta * cosPhi;
      float y = height / 2.0f * cosTheta;
      float z = depth / 2.0f * (-1.0f - cosTheta) * sinTheta * sinPhi;

      return new Vector3(x, -y, z);
    }
  }

}