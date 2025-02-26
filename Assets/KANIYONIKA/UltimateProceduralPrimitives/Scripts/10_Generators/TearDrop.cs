using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateProceduralPrimitives
{
  [System.Serializable]
  public class TearDrop : AbstractGenerator
  {
    public SurfaceType surfaceType = SurfaceType.Smooth;
    public Direction direction = Direction.Y_Axis;
    public PivotPosition pivotPosition = PivotPosition.Center;

    public float width = 3.0f;
    public float height = 6.0f;
    public float depth = 3.0f;

    public int segments = 30;

    public bool flipNormals = false;


    public TearDrop() { }


    public override void Generate(Mesh mesh, MeshCutProducer _meshCutProducer)
    {
      var parameter = new TearDropParameters
      {
        SurfaceType = this.surfaceType,
        Direction = this.direction,
        PivotPosition = this.pivotPosition,
        Width = this.width,
        Height = this.height,
        Depth = this.depth,
        Segments = this.segments,
        FlipNormals = this.flipNormals,
      };

      var myMeshInfo = new FormulaTearDrop().CalculateMyMeshInfo(parameter);
      Finishing(mesh, myMeshInfo, surfaceType, _meshCutProducer, flipNormals, pivotPosition);
    }
  }

}