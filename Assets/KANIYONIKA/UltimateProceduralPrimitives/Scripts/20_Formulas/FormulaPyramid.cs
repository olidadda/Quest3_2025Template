using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace UltimateProceduralPrimitives
{
  public class FormulaPyramid : AbstractFormula
  {
    public MyMeshInfo CalculateMyMeshInfo(PyramidBasicParameters parameter)
    {

      var pyramidFlexParameter = new PyramidFlexParameters()
      {
        SurfaceType = parameter.SurfaceType,
        Direction = parameter.Direction,
        PivotPosition = parameter.PivotPosition,

        Width = parameter.Width,
        Height = parameter.Height,
        Depth = parameter.Depth,

        SegmentsSides = parameter.SegmentsSides,
        SegmentsBottom = parameter.SegmentsBottom,

        OffsetUpVtx = Vector3.zero,
        OffsetDownLeftForwardVtx = Vector3.zero,
        OffsetDownRightForwardVtx = Vector3.zero,
        OffsetDownLeftBackwardVtx = Vector3.zero,
        OffsetDownRightBackwardVtx = Vector3.zero,

        FlipNormals = parameter.FlipNormals,
      };

      var myMeshInfo = new FormulaPyramidFlex().CalculateMyMeshInfo(pyramidFlexParameter);
      return myMeshInfo;

    }
  }
}