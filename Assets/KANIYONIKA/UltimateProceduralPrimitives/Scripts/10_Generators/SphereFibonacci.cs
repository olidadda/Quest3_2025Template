using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateProceduralPrimitives
{
  [System.Serializable]
  public class SphereFibonacci : AbstractGenerator
  {
    public SurfaceType surfaceType = SurfaceType.Smooth;
    public Direction direction = Direction.Y_Axis;
    public PivotPosition pivotPosition = PivotPosition.Center;

    public float radius = 1.5f;
    public int vertices = 1000;
    public UVPattern uVPattern = UVPattern.BothSidesButOtherSideFlip;

    public bool flipNormals = false;


    public SphereFibonacci() { }


    public override void Generate(Mesh mesh, MeshCutProducer _meshCutProducer)
    {
      var parameter = new SphereFibonacciParameters
      {
        SurfaceType = surfaceType,
        Direction = direction,
        PivotPosition = pivotPosition,
        Radius = radius,
        Vertices = vertices,
        FlipNormals = flipNormals,
        UVPattern = uVPattern,
      };

      var myMeshInfo = new FormulaSphereFibonacci().CalculateMyMeshInfo(parameter);
      Finishing(mesh, myMeshInfo, surfaceType, _meshCutProducer, flipNormals, pivotPosition);
    }
  }

}