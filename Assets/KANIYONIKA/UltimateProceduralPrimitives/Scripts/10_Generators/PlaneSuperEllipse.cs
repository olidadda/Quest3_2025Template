using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UltimateProceduralPrimitives
{
  [System.Serializable]
  public class PlaneSuperEllipse : AbstractGenerator
  {
    public SurfaceType surfaceType = SurfaceType.Smooth;
    public Orientation orientation = Orientation.Up;
    public Direction direction = Direction.Y_Axis;
    public PivotPosition pivotPosition = PivotPosition.Center;
    public bool doubleSided = true;

    public float width = 3.0f;
    public float height = 2.0f;

    public float n1 = 0.8f;
    public float n2 = 0.0f;
    public float n3 = 0.0f;
    public float n4 = 0.8f;

    public int segments = 100;

    public bool flipNormals = false;


    public PlaneSuperEllipse() { }

    public override void Generate(Mesh mesh, MeshCutProducer _meshCutProducer)
    {
      var parameter = new PlaneSuperEllipseParameters()
      {
        SurfaceType = this.surfaceType,
        Orientation = this.orientation,
        Direction = this.direction,
        PivotPosition = this.pivotPosition,
        DoubleSided = this.doubleSided,

        Width = this.width,
        Height = this.height,
        N1 = this.n1,
        N2 = this.n2,
        N3 = this.n3,
        N4 = this.n4,

        Segments = this.segments,
        FlipNormals = this.flipNormals,
      };

      var myMeshInfo = new FormulaPlaneSuperEllipse().CalculateMyMeshInfo(parameter);
      Finishing(mesh, myMeshInfo, surfaceType, _meshCutProducer, flipNormals, pivotPosition);
    }
  }
}