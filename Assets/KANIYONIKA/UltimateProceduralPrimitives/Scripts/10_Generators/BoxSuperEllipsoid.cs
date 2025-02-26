using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UltimateProceduralPrimitives
{
  [System.Serializable]
  public class BoxSuperEllipsoid : AbstractGenerator
  {
    public SurfaceType surfaceType = SurfaceType.Smooth;
    public Direction direction = Direction.Y_Axis;
    public PivotPosition pivotPosition = PivotPosition.Center;

    public float width = 3.0f;
    public float height = 2.5f;
    public float depth = 3.0f;
    public float n1 = 0.0f;
    public float n2 = 0.5f;
    public int segments = 50;

    public bool flipNormals = false;


    public BoxSuperEllipsoid() { }

    public override void Generate(Mesh mesh, MeshCutProducer _meshCutProducer)
    {
      var parameter = new BoxSuperEllipsoidParameters()
      {
        SurfaceType = this.surfaceType,
        Direction = this.direction,
        PivotPosition = this.pivotPosition,

        Width = this.width,
        Height = this.height,
        Depth = this.depth,
        N1 = this.n1,
        N2 = this.n2,
        Segments = this.segments,

        FlipNormals = this.flipNormals,
      };

      var myMeshInfo = new FormulaBoxSuperEllipsoid().CalculateMyMeshInfo(parameter);
      Finishing(mesh, myMeshInfo, surfaceType, _meshCutProducer, flipNormals, pivotPosition);
    }
  }
}