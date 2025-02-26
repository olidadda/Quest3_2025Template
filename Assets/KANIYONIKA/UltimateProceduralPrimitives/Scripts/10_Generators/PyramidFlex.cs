using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UltimateProceduralPrimitives
{
  [System.Serializable]
  public class PyramidFlex : AbstractGenerator
  {
    public SurfaceType surfaceType = SurfaceType.Smooth;
    public Direction direction = Direction.Y_Axis;
    public PivotPosition pivotPosition = PivotPosition.Center;

    public float width = 3.0f;
    public float height = 2.5f;
    public float depth = 3.0f;

    public Vector2 segmentsSides = new Vector2(10.0f, 10.0f);
    public Vector2 segmentsBottom = new Vector2(5.0f, 5.0f);

    public VertexOffsets vertexOffsets;

    public bool flipNormals = false;


    public PyramidFlex() { }


    public override void Generate(Mesh mesh, MeshCutProducer _meshCutProducer)
    {
      var parameter = new PyramidFlexParameters()
      {
        SurfaceType = this.surfaceType,
        Direction = this.direction,
        PivotPosition = this.pivotPosition,

        Width = this.width,
        Height = this.height,
        Depth = this.depth,

        SegmentsSides = this.segmentsSides,
        SegmentsBottom = this.segmentsSides,

        OffsetUpVtx = this.vertexOffsets.upVtx,
        OffsetDownLeftForwardVtx = this.vertexOffsets.downLeftForwardVtx,
        OffsetDownRightForwardVtx = this.vertexOffsets.downRightForwardVtx,
        OffsetDownLeftBackwardVtx = this.vertexOffsets.downLeftBackwardVtx,
        OffsetDownRightBackwardVtx = this.vertexOffsets.downRightBackwardVtx,

        FlipNormals = this.flipNormals,
      };

      var myMeshInfo = new FormulaPyramidFlex().CalculateMyMeshInfo(parameter);
      Finishing(mesh, myMeshInfo, surfaceType, _meshCutProducer, flipNormals, pivotPosition);
    }

    [System.Serializable]
    public class VertexOffsets
    {
      public Vector3 upVtx = new Vector3(1.5f, 0, -1.5f);
      public Vector3 downLeftForwardVtx = new Vector3(-1.5f, -2.0f, -0.5f);
      public Vector3 downRightForwardVtx;
      public Vector3 downLeftBackwardVtx;
      public Vector3 downRightBackwardVtx;
    }
  }
}