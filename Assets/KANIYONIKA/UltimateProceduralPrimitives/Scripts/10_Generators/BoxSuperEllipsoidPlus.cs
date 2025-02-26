using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UltimateProceduralPrimitives
{
  [System.Serializable]
  public class BoxSuperEllipsoidPlus : AbstractGenerator
  {
    public SurfaceType surfaceType = SurfaceType.Smooth;
    public Direction direction = Direction.Y_Axis;
    public PivotPosition pivotPosition = PivotPosition.Center;

    public float width = 3.0f;
    public float height = 2.5f;
    public float depth = 3.0f;
    public float n1_TopEdges;
    public float n1_BottomEdges;
    public float n2_RightFrontEdge;
    public float n2_RightBackEdge;
    public float n2_LeftFrontEdge;
    public float n2_LeftBackEdge;
    public int segments = 50;

    public bool flipNormals = false;


    public BoxSuperEllipsoidPlus() { }

    public override void Generate(Mesh mesh, MeshCutProducer _meshCutProducer)
    {
      var parameter = new BoxSuperEllipsoidPlusParameters()
      {
        SurfaceType = this.surfaceType,
        Direction = this.direction,
        PivotPosition = this.pivotPosition,

        Width = this.width,
        Height = this.height,
        Depth = this.depth,

        N1_TopEdges = this.n1_TopEdges,
        N1_BottomEdges = this.n1_BottomEdges,
        N2_RightFrontEdge = this.n2_RightFrontEdge,
        N2_RightBackEdge = this.n2_RightBackEdge,
        N2_LeftFrontEdge = this.n2_LeftFrontEdge,
        N2_LeftBackEdge = this.n2_LeftBackEdge,

        Segments = this.segments,

        FlipNormals = this.flipNormals,
      };

      var myMeshInfo = new FormulaBoxSuperEllipsoidPlus().CalculateMyMeshInfo(parameter);
      Finishing(mesh, myMeshInfo, surfaceType, _meshCutProducer, flipNormals, pivotPosition);
    }
  }
}