using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UltimateProceduralPrimitives
{
  public class FormulaBoxSuperEllipsoidPlus : AbstractFormula
  {
    public MyMeshInfo CalculateMyMeshInfo(BoxSuperEllipsoidPlusParameters parameter)
    {
      var globeUVsX = new List<float>();
      var globeUVsY = new List<float>();


      // j is　for latitude shape.  (Shape when cut vertically.)
      for (int j = 0; j < parameter.Segments + 1; j++)
      {
        // Debug.Log($"------- j:{j} loop ---------");
        float theta = Mathf.Lerp(-1.0f * Mathf.PI / 2.0f, Mathf.PI / 2.0f, (float)j / (float)parameter.Segments);

        // i is　for longitude shape.  (Shape when cut horizontally.)
        myGlobeVtx.Add(new List<Vector3>());
        for (int i = 0; i < parameter.Segments + 1; i++)
        {
          float beta = Mathf.Lerp(-1.0f * Mathf.PI, Mathf.PI, (float)i / (float)parameter.Segments);

          // Create Vertex Preparing
          var cosTheta = Mathf.Cos(theta);
          var cosBete = Mathf.Cos(beta);
          var sinTheta = Mathf.Sin(theta);
          var sinBete = Mathf.Sin(beta);

          var Abs_cosTheta = Mathf.Abs(cosTheta);
          var Abs_cosBete = Mathf.Abs(cosBete);
          var Abs_sinTheta = Mathf.Abs(sinTheta);
          var Abs_sinBete = Mathf.Abs(sinBete);

          var Sign_cosTheta = Utils.Sign(cosTheta);
          var Sign_cosBete = Utils.Sign(cosBete);
          var Sign_sinTheta = Utils.Sign(sinTheta);
          var Sign_sinBete = Utils.Sign(sinBete);

          // Create Vertex
          float n1 = 0.0f;
          float n2 = 0.0f;
          float x = 0.0f;
          float y = 0.0f;
          float z = 0.0f;

          void CreateN()
          {
            // Set n1
            if (cosTheta > 0 && theta > 0) { n1 = parameter.N1_TopEdges; }
            if (cosTheta > 0 && theta < 0) { n1 = parameter.N1_BottomEdges; }

            // Set n2
            if (cosTheta > 0 && Sign_cosBete > 0 && beta > 0) { n2 = parameter.N2_RightFrontEdge; }
            if (cosTheta > 0 && Sign_cosBete > 0 && beta < 0) { n2 = parameter.N2_RightBackEdge; }
            if (cosTheta > 0 && Sign_cosBete < 0 && beta > 0) { n2 = parameter.N2_LeftFrontEdge; }
            if (cosTheta > 0 && Sign_cosBete < 0 && beta < 0) { n2 = parameter.N2_LeftBackEdge; }
          }
          CreateN();

          //  x = parameter.Width / 2.0f * Mathf.Pow(Abs_cosTheta, parameter.N1) * Sign_cosTheta * Mathf.Pow(Abs_cosBete, parameter.N2) * Sign_cosBete;
          x = parameter.Width / 2.0f * Mathf.Pow(Abs_cosTheta, n1) * Sign_cosTheta * Mathf.Pow(Abs_cosBete, n2) * Sign_cosBete;
          // y = parameter.Height / 2.0f * Mathf.Pow(Abs_sinTheta, parameter.N1) * Sign_sinTheta;
          y = parameter.Height / 2.0f * Mathf.Pow(Abs_sinTheta, n1) * Sign_sinTheta;
          //  z = parameter.Depth / 2.0f * Mathf.Pow(Abs_cosTheta, parameter.N1) * Sign_cosTheta * Mathf.Pow(Abs_sinBete, parameter.N2) * Sign_sinBete;
          z = parameter.Depth / 2.0f * Mathf.Pow(Abs_cosTheta, n1) * Sign_cosTheta * Mathf.Pow(Abs_sinBete, n2) * Sign_sinBete;

          if (j == 0)
          {
            x = 0;
            y = -parameter.Height / 2.0f;
            z = 0;
          }
          if (j == parameter.Segments)
          {
            x = 0;
            y = parameter.Height / 2.0f;
            z = 0;
          }

          // to modify　unnecessary surface.
          // (when N2_Left Front Edge or N2_Left Back Edge equal 0, there are unnecessary faces.)
          if (i == 0 || i == parameter.Segments)
          {
            z = 0;
          }

          // Set the Vertexes
          myGlobeVtx[j].Add(new Vector3(x, y, z));


          Debug.Log($"j: {j.ToString("0000")}  i: {i.ToString("0000")} --- x: {x.ToString("00.000000")} --- y: {y.ToString("00.000000")} ---z: {z.ToString("00.000000")} ");
        }
      }

      // Set the UVx
      for (int i = 0; i < parameter.Segments + 1; i++)
      {
        // Use the middle globeVertex group as a sample
        var vec3 = myGlobeVtx[(parameter.Segments + 1) / (int)2][i];
        var rForUVx = Mathf.Atan2(-vec3.z, -vec3.x) >= 0 ? Mathf.Atan2(-vec3.z, -vec3.x) : Mathf.PI + Mathf.Atan2(-vec3.z, -vec3.x) + Mathf.PI;
        var tForUVx = rForUVx / (Mathf.PI * 2.0f);

        //　fixed value
        if (i == 0) { tForUVx = 0; }
        if (i == parameter.Segments) { tForUVx = 1; }

        // set
        float xUV = Mathf.Lerp(0.25f, 1.25f, tForUVx);
        globeUVsX.Add(xUV);
      }

      // Set the UVy
      for (int i = 0; i < parameter.Segments + 1; i++)
      {
        // Use the first globeVertex group as a sample
        var vec3 = myGlobeVtx[i][0];
        var rForUVy = Mathf.Atan2(-vec3.x, -vec3.y);
        var tForUVy = rForUVy / Mathf.PI;

        //　fixed value
        if (i == 0) { tForUVy = 0; }
        if (i == parameter.Segments) { tForUVy = 1; }

        // set
        float yUV = Mathf.Lerp(0.0f, 1.0f, tForUVy);
        globeUVsY.Add(yUV);
      }

      //Combine UV x y
      for (int j = 0; j < parameter.Segments + 1; j++)
      {
        myGlobeUVs.Add(new List<Vector2>());
        for (int i = 0; i < parameter.Segments + 1; i++)
        {
          var uv = new Vector2(globeUVsX[i], globeUVsY[j]);
          myGlobeUVs[j].Add(uv);
          // Debug.Log($"---j: {j.ToString("00")}  i: {i.ToString("00")}---- UVx: {(globeUVs[j][i].x).ToString("0.000")}---- UVy: {(globeUVs[j][i]].y).ToString("0.000")} ---- VTXx: {(globeVertexes[j][i]].x).ToString("0.000")} ---- VTXy: {(globeVertexes[j][i]].y).ToString("0.000")} ---- VTXz: {(globeVertexes[j][i]].z).ToString("0.000")}");
        }
      }


      CreateMyVtxUVsIdx_FlexPlane_Supershape_SuperEllipsoid_TearDrop(myVtx, myUVs, myGlobeVtx, myGlobeUVs, myIdx);

      FlipNormals(parameter.FlipNormals, myVtx, myUVs, myIdx);
      DirectionCollection(parameter.Direction, myVtx);
      // SetPivotPosition(parameter.PivotPosition, myVtx);
      return CreateMyMeshInfoStruct(myVtx, myUVs, myIdx);

    }
  }
}