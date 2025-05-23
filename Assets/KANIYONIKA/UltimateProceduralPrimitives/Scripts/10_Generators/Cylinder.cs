using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UltimateProceduralPrimitives
{
  [System.Serializable]
  public class Cylinder : AbstractGenerator
  {
    public SurfaceType surfaceType = SurfaceType.Smooth;
    public Direction direction = Direction.Y_Axis;
    public PivotPosition pivotPosition = PivotPosition.Center;

    public float topRadius = 1.0f;
    public float bottomRadius = 1.0f;
    public float Height = 3.0f;
    public int columns = 24; // 3
    public int rows = 12; // 1
    public bool caps = true;

    public bool flipNormals = false;


    public Cylinder() { }

    public override void Generate(Mesh mesh, MeshCutProducer _meshCutProducer)
    {
      var parameter = new CylinderParameters()
      {
        SurfaceType = this.surfaceType,
        Direction = this.direction,
        PivotPosition = this.pivotPosition,

        TopRadius = this.topRadius,
        BottomRadius = this.bottomRadius,
        Height = this.Height,
        Columns = this.columns,
        Rows = this.rows,
        Caps = this.caps,

        FlipNormals = this.flipNormals,
      };

      var myMeshInfo = new FormulaCylinder().CalculateMyMeshInfo(parameter);
      Finishing(mesh, myMeshInfo, surfaceType, _meshCutProducer, flipNormals, pivotPosition);
    }
  }
}