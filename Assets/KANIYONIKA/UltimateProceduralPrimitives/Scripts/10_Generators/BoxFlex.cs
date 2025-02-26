using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UltimateProceduralPrimitives
{
  [System.Serializable]
  public class BoxFlex : AbstractGenerator
  {
    public SurfaceType surfaceType = SurfaceType.Smooth;
    public Direction direction = Direction.Y_Axis;
    public PivotPosition pivotPosition = PivotPosition.Center;

    public float width = 3.0f;
    public float height = 3.0f;
    public float depth = 3.0f;

    public Segments segments;
    public VertexOffsets vertexOffsets;

    public bool flipNormals = false;


    public BoxFlex() { }


    public override void Generate(Mesh mesh, MeshCutProducer _meshCutProducer)
    {
      var parameter = new BoxFlexParameters()
      {
        SurfaceType = this.surfaceType,
        Direction = this.direction,
        PivotPosition = this.pivotPosition,

        Width = this.width,
        Height = this.height,
        Depth = this.depth,

        SegmentsUp = this.segments.up,
        SegmentsDown = this.segments.down,
        SegmentsLeft = this.segments.left,
        SegmentsRight = this.segments.right,
        SegmentsForward = this.segments.forward,
        SegmentsBackward = this.segments.backward,

        OffsetUpLeftForwardVtx = this.vertexOffsets.upLeftForwardVtx,
        OffsetUpRightForwardVtx = this.vertexOffsets.upRightForwardVtx,
        OffsetUpLeftBackwardVtx = this.vertexOffsets.upLeftBackwardVtx,
        OffsetUpRightBackwardVtx = this.vertexOffsets.upRightBackwardVtx,

        OffsetDownLeftForwardVtx = this.vertexOffsets.downLeftForwardVtx,
        OffsetDownRightForwardVtx = this.vertexOffsets.downRightForwardVtx,
        OffsetDownLeftBackwardVtx = this.vertexOffsets.downLeftBackwardVtx,
        OffsetDownRightBackwardVtx = this.vertexOffsets.downRightBackwardVtx,

        FlipNormals = this.flipNormals,
      };

      var myMeshInfo = new FormulaBoxFlex().CalculateMyMeshInfo(parameter);
      Finishing(mesh, myMeshInfo, surfaceType, _meshCutProducer, flipNormals, pivotPosition);
    }


    [System.Serializable]
    public class Segments
    {
      public Vector2 up = new Vector2(5, 5);
      public Vector2 down = new Vector2(5, 5);
      public Vector2 left = new Vector2(10, 10);
      public Vector2 right = new Vector2(10, 10);
      public Vector2 forward = new Vector2(10, 10);
      public Vector2 backward = new Vector2(10, 10);
    }
    [System.Serializable]
    public class VertexOffsets
    {
      public Vector3 upLeftForwardVtx;
      public Vector3 upRightForwardVtx;
      public Vector3 upLeftBackwardVtx;
      public Vector3 upRightBackwardVtx;

      public Vector3 downLeftForwardVtx;
      public Vector3 downRightForwardVtx;
      public Vector3 downLeftBackwardVtx;
      public Vector3 downRightBackwardVtx = new Vector3(1, -0.2f, 1.5f);
    }
  }
}