using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UltimateProceduralPrimitives
{
  [System.Serializable]
  public class SphereIco : AbstractGenerator
  {
    public SurfaceType surfaceType = SurfaceType.Smooth;
    public Direction direction = Direction.Y_Axis;
    public PivotPosition pivotPosition = PivotPosition.Center;

    public float radius = 1.5f;
    public int subdivision = 2;
    public UVPattern uVPattern = UVPattern.BothSidesButOtherSideFlip;

    public bool flipNormals = false;


    public SphereIco() { }

    public override void Generate(Mesh mesh, MeshCutProducer _meshCutProducer)
    {
      var parameter = new SphereIcoParameters()
      {
        SurfaceType = this.surfaceType,
        Direction = this.direction,
        PivotPosition = this.pivotPosition,

        Radius = this.radius,
        Subdivision = this.subdivision,
        UVPattern = this.uVPattern,

        FlipNormals = this.flipNormals,
      };

      var myMeshInfo = new FormulaSphereIco().CalculateMyMeshInfo(parameter);
      Finishing(mesh, myMeshInfo, surfaceType, _meshCutProducer, flipNormals, pivotPosition);
    }


  }
}