using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UltimateProceduralPrimitives
{
  [System.Serializable]
  public class PlaneFlex : AbstractGenerator
  {
    public SurfaceType surfaceType = SurfaceType.Smooth;
    public Orientation orientation = Orientation.Up;
    public Direction direction = Direction.Y_Axis;
    public PivotPosition pivotPosition = PivotPosition.Center;
    public bool doubleSided = true;

    public Vector2 faceSegments = new Vector2(3, 3);
    public float width = 3.0f;
    public float height = 3.0f;

    public VertexOffsets vertexOffsets;

    public bool flipNormals = false;


    public PlaneFlex() { }


    public override void Generate(Mesh mesh, MeshCutProducer _meshCutProducer)
    {
      var parameter = new PlaneFlexParameters()
      {
        SurfaceType = this.surfaceType,
        Orientation = this.orientation,
        Direction = this.direction,
        PivotPosition = this.pivotPosition,
        DoubleSided = this.doubleSided,

        Segments = this.faceSegments,

        Width = this.width,
        Height = this.height,

        OffsetLeftForwardVtxForOrientationUp = this.vertexOffsets.forOrientationUp.leftForwardVtx,
        OffsetRightForwardVtxForOrientationUp = this.vertexOffsets.forOrientationUp.rightForwardVtx,
        OffsetLeftBackwardVtxForOrientationUp = this.vertexOffsets.forOrientationUp.leftBackwardVtx,
        OffsetRightBackwardVtxForOrientationUp = this.vertexOffsets.forOrientationUp.rightBackwardVtx,

        OffsetLeftForwardVtxForOrientationDown = this.vertexOffsets.forOrientationDown.leftForwardVtx,
        OffsetRightForwardVtxForOrientationDown = this.vertexOffsets.forOrientationDown.rightForwardVtx,
        OffsetLeftBackwardVtxForOrientationDown = this.vertexOffsets.forOrientationDown.leftBackwardVtx,
        OffsetRightBackwardVtxForOrientationDown = this.vertexOffsets.forOrientationDown.rightBackwardVtx,

        OffsetUpForwardVtxForOrientationLeft = this.vertexOffsets.forOrientationLeft.upForwardVtx,
        OffsetUpBackwardVtxForOrientationLeft = this.vertexOffsets.forOrientationLeft.upBackwardVtx,
        OffsetDownForwardVtxForOrientationLeft = this.vertexOffsets.forOrientationLeft.downForwardVtx,
        OffsetDownBackwardVtxForOrientationLeft = this.vertexOffsets.forOrientationLeft.downBackwardVtx,

        OffsetUpForwardVtxForOrientationRight = this.vertexOffsets.forOrientationRight.upForwardVtx,
        OffsetUpBackwardVtxForOrientationRight = this.vertexOffsets.forOrientationRight.upBackwardVtx,
        OffsetDownForwardVtxForOrientationRight = this.vertexOffsets.forOrientationRight.downForwardVtx,
        OffsetDownBackwardVtxForOrientationRight = this.vertexOffsets.forOrientationRight.downBackwardVtx,

        OffsetUpLeftVtxForOrientationForward = this.vertexOffsets.forOrientationForward.upLeftVtx,
        OffsetUpRightVtxForOrientationForward = this.vertexOffsets.forOrientationForward.upRightVtx,
        OffsetDownLeftVtxForOrientationForward = this.vertexOffsets.forOrientationForward.downLeftVtx,
        OffsetDownRightVtxForOrientationForward = this.vertexOffsets.forOrientationForward.downRightVtx,

        OffsetUpLeftVtxForOrientationBackward = this.vertexOffsets.forOrientationBackward.upLeftVtx,
        OffsetUpRightVtxForOrientationBackward = this.vertexOffsets.forOrientationBackward.upRightVtx,
        OffsetDownLeftVtxForOrientationBackward = this.vertexOffsets.forOrientationBackward.downLeftVtx,
        OffsetDownRightVtxForOrientationBackward = this.vertexOffsets.forOrientationBackward.downRightVtx,

        FlipNormals = this.flipNormals,
      };

      var myMeshInfo = new FormulaPlaneFlex().CalculateMyMeshInfo(parameter);
      Finishing(mesh, myMeshInfo, surfaceType, _meshCutProducer, flipNormals, pivotPosition);
    }

    [System.Serializable]
    public class VertexOffsets
    {
      public VertexOffsetsForOrientationUp forOrientationUp;
      public VertexOffsetsForOrientationDown forOrientationDown;
      public VertexOffsetsForOrientationLeft forOrientationLeft;
      public VertexOffsetsForOrientationRight forOrientationRight;
      public VertexOffsetsForOrientationForward forOrientationForward;
      public VertexOffsetsForOrientationBackward forOrientationBackward;
    }

    [System.Serializable]
    public class VertexOffsetsForOrientationUp
    {
      public Vector3 leftForwardVtx = new Vector3(0.0f, -1.0f, 0.0f);
      public Vector3 rightForwardVtx = new Vector3(0.0f, 1.0f, 0.0f);
      public Vector3 leftBackwardVtx = new Vector3(0.0f, 1.0f, 0.0f);
      public Vector3 rightBackwardVtx = new Vector3(0.0f, -1.0f, 0.0f);
    }
    [System.Serializable]
    public class VertexOffsetsForOrientationDown
    {
      public Vector3 leftForwardVtx;
      public Vector3 rightForwardVtx;
      public Vector3 leftBackwardVtx;
      public Vector3 rightBackwardVtx;
    }
    [System.Serializable]
    public class VertexOffsetsForOrientationLeft
    {
      public Vector3 upForwardVtx;
      public Vector3 upBackwardVtx;
      public Vector3 downForwardVtx;
      public Vector3 downBackwardVtx;
    }
    [System.Serializable]
    public class VertexOffsetsForOrientationRight
    {
      public Vector3 upForwardVtx;
      public Vector3 upBackwardVtx;
      public Vector3 downForwardVtx;
      public Vector3 downBackwardVtx;
    }
    [System.Serializable]
    public class VertexOffsetsForOrientationForward
    {
      public Vector3 upLeftVtx;
      public Vector3 upRightVtx;
      public Vector3 downLeftVtx;
      public Vector3 downRightVtx;
    }
    [System.Serializable]
    public class VertexOffsetsForOrientationBackward
    {
      public Vector3 upLeftVtx;
      public Vector3 upRightVtx;
      public Vector3 downLeftVtx;
      public Vector3 downRightVtx;
    }


  }
}