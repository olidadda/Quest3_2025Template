using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UltimateProceduralPrimitives
{
  [System.Serializable]
  public class PyramidPerfectTriangularFlex : AbstractGenerator
  {
    public SurfaceType surfaceType = SurfaceType.Smooth;
    public Direction direction = Direction.Y_Axis;
    public PivotPosition pivotPosition = PivotPosition.Center;

    public float length = 3.0f;

    public Vector2 segmentsSides = new Vector2(10.0f, 10.0f);
    public Vector2 segmentsBottom = new Vector2(5.0f, 5.0f);

    public VertexOffsets vertexOffsets;

    public bool flipNormals = false;


    public PyramidPerfectTriangularFlex() { }


    public override void Generate(Mesh mesh, MeshCutProducer _meshCutProducer)
    {
      var parameter = new PyramidPerfectTriangularFlexParameters()
      {
        SurfaceType = this.surfaceType,
        Direction = this.direction,
        PivotPosition = this.pivotPosition,

        Length = this.length,

        SegmentsSides = this.segmentsSides,
        SegmentsBottom = this.segmentsBottom,

        OffsetUpVtx = this.vertexOffsets.upVtx,
        OffsetDownForwardVtx = this.vertexOffsets.downForwardVtx,
        OffsetDownLeftBackwardVtx = this.vertexOffsets.downLeftBackwardVtx,
        OffsetDownRightBackwardVtx = this.vertexOffsets.downRightBackwardVtx,

        FlipNormals = this.flipNormals,
      };

      var myMeshInfo = new FormulaPyramidPerfectTriangularFlex().CalculateMyMeshInfo(parameter);
      Finishing(mesh, myMeshInfo, surfaceType, _meshCutProducer, flipNormals, pivotPosition);
    }


    [System.Serializable]
    public class VertexOffsets
    {
      public Vector3 upVtx;
      public Vector3 downForwardVtx;
      public Vector3 downLeftBackwardVtx;
      public Vector3 downRightBackwardVtx;
    }
  }
}