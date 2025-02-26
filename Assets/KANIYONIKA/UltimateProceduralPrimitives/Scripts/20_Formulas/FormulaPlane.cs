using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace UltimateProceduralPrimitives
{
  public class FormulaPlane : AbstractFormula
  {
    public MyMeshInfo CalculateMyMeshInfo(PlaneBasicParameters parameter)
    {
      var planeParameter = new PlaneFlexParameters()
      {
        SurfaceType = parameter.SurfaceType,
        Orientation = parameter.Orientation,
        Direction = parameter.Direction,
        PivotPosition = parameter.PivotPosition,
        DoubleSided = parameter.DoubleSided,
        Segments = parameter.Segments,
        Width = parameter.Width,
        Height = parameter.Height,
        FlipNormals = parameter.FlipNormals,
      };

      var myMeshInfo = new FormulaPlaneFlex().CalculateMyMeshInfo(planeParameter);

      return myMeshInfo;
    }
  }
}