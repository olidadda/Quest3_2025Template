using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace UltimateProceduralPrimitives
{
  public class FormulaBox : AbstractFormula
  {
    public MyMeshInfo CalculateMyMeshInfo(BoxBasicParameters parameter)
    {
      var boxFlexParameter = new BoxFlexParameters()
      {
        SurfaceType = parameter.SurfaceType,
        Direction = parameter.Direction,
        PivotPosition = parameter.PivotPosition,

        Width = parameter.Width,
        Height = parameter.Height,
        Depth = parameter.Depth,

        SegmentsUp = parameter.Segments,
        SegmentsDown = parameter.Segments,
        SegmentsLeft = parameter.Segments,
        SegmentsRight = parameter.Segments,
        SegmentsForward = parameter.Segments,
        SegmentsBackward = parameter.Segments,

        OffsetUpLeftForwardVtx = Vector3.zero,
        OffsetUpRightForwardVtx = Vector3.zero,
        OffsetUpLeftBackwardVtx = Vector3.zero,
        OffsetUpRightBackwardVtx = Vector3.zero,

        OffsetDownLeftForwardVtx = Vector3.zero,
        OffsetDownRightForwardVtx = Vector3.zero,
        OffsetDownLeftBackwardVtx = Vector3.zero,
        OffsetDownRightBackwardVtx = Vector3.zero,

        FlipNormals = parameter.FlipNormals,
      };

      var myMeshInfo = new FormulaBoxFlex().CalculateMyMeshInfo(boxFlexParameter);
      return myMeshInfo;
    }
  }
}