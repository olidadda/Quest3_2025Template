using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using g3;


namespace UltimateProceduralPrimitives
{
  [System.Serializable]
  public class Clouds : AbstractGenerator
  {
    [Header("- Save the scene before use. -")]
    public CloudsTargetSelect target = CloudsTargetSelect.Preset1;
    [Space(5)]
    public bool useGlobalResolution = false;
    public int globalResolution = 64;
    [Space(5)]
    public TargetSet cloudsPreset1 = new TargetSet(CloudsTargetSelect.Preset1);
    public TargetSet cloudsPreset2 = new TargetSet(CloudsTargetSelect.Preset2);
    public TargetSet cloudsPreset3 = new TargetSet(CloudsTargetSelect.Preset3);
    public TargetSet cloudsPreset4 = new TargetSet(CloudsTargetSelect.Preset4);
    public TargetSet cloudsPreset5 = new TargetSet(CloudsTargetSelect.Preset5);
    public TargetSet cloudsPreset6 = new TargetSet(CloudsTargetSelect.Preset6);
    public TargetSet makeFromScratch = new TargetSet(CloudsTargetSelect.MakeFromScratch);


    public override void Generate(Mesh mesh, MeshCutProducer _meshCutProducer)
    {
      var parameter = new CloudsParameters();
      TargetSet targetSet;

      if (target == CloudsTargetSelect.Preset1)
        targetSet = cloudsPreset1;
      else if (target == CloudsTargetSelect.Preset2)
        targetSet = cloudsPreset2;
      else if (target == CloudsTargetSelect.Preset3)
        targetSet = cloudsPreset3;
      else if (target == CloudsTargetSelect.Preset4)
        targetSet = cloudsPreset4;
      else if (target == CloudsTargetSelect.Preset5)
        targetSet = cloudsPreset5;
      else if (target == CloudsTargetSelect.Preset6)
        targetSet = cloudsPreset6;
      else
        targetSet = makeFromScratch;

      // create List
      var _cloudSpheres = new List<CloudSpheres>();
      foreach (var item in targetSet.numberOfSpheres)
      {
        var _cloudSphere = new CloudSpheres();
        _cloudSphere.RadiusOfSphere = item.radius;
        _cloudSphere.PositionOfSphere = new Vector3d(item.position.x, item.position.y, item.position.z);
        _cloudSphere.BlendWithNextSphere = item.blendWithNextSphere;

        _cloudSpheres.Add(_cloudSphere);
      }

      // set parameters
      parameter.SurfaceType = targetSet.surfaceType;
      parameter.Direction = targetSet.direction;
      parameter.PivotPosition = targetSet.pivotPosition;

      parameter.MarchingCubeCells = targetSet.resolution;
      parameter.CloudSpheres = _cloudSpheres;
      parameter.Roundness = targetSet.roundness;

      parameter.FlipNormals = targetSet.flipNormals;

      if (useGlobalResolution)
      {
        parameter.MarchingCubeCells = globalResolution;
      }


      var myMeshInfo = new FormulaClouds().CalculateMyMeshInfo(parameter);
      Finishing(mesh, myMeshInfo, parameter.SurfaceType, _meshCutProducer, targetSet.flipNormals, targetSet.pivotPosition);
    }

    [System.Serializable]
    public struct CloudSphere
    {
      public double radius;
      public Vector3 position;
      public double blendWithNextSphere;
    }

    [System.Serializable]
    public class TargetSet
    {
      public SurfaceType surfaceType;
      public Direction direction;
      public PivotPosition pivotPosition;
      public bool flipNormals;

      public int resolution;
      [HideInInspector] public double roundness;
      public List<CloudSphere> numberOfSpheres;


      public TargetSet(CloudsTargetSelect cloudsTargetSelect)
      {
        if (cloudsTargetSelect == CloudsTargetSelect.Preset1)
        {
          List<CloudSphere> cloudSpheres = new List<CloudSphere>();
          AddCloudSpheres(cloudSpheres, 0.30, new Vector3(-1.25f, +0.00f, +0.00f), 64.00);
          AddCloudSpheres(cloudSpheres, 0.50, new Vector3(-0.45f, +0.00f, +0.00f), 64.00);
          AddCloudSpheres(cloudSpheres, 0.50, new Vector3(+0.45f, +0.00f, +0.00f), 64.00);
          AddCloudSpheres(cloudSpheres, 0.30, new Vector3(+1.25f, +0.00f, +0.00f), 64.00);

          surfaceType = SurfaceType.Smooth;
          direction = Direction.Y_Axis;
          pivotPosition = PivotPosition.Center;

          resolution = 64;
          numberOfSpheres = cloudSpheres;
          roundness = 0.0;

          flipNormals = false;
        }
        else if (cloudsTargetSelect == CloudsTargetSelect.Preset2)
        {
          List<CloudSphere> cloudSpheres = new List<CloudSphere>();
          AddCloudSpheres(cloudSpheres, 0.70, new Vector3(-1.00f, +0.00f, +0.25f), 0.2);
          AddCloudSpheres(cloudSpheres, 1.00, new Vector3(-0.50f, +0.50f, +0.00f), 0.2);
          AddCloudSpheres(cloudSpheres, 0.70, new Vector3(+0.00f, +0.10f, -0.50f), 0.2);
          AddCloudSpheres(cloudSpheres, 0.80, new Vector3(+0.80f, +0.30f, -0.10f), 0.2);

          surfaceType = SurfaceType.Smooth;
          direction = Direction.Y_Axis;
          pivotPosition = PivotPosition.Center;

          resolution = 64;
          numberOfSpheres = cloudSpheres;
          roundness = 0.0;

          flipNormals = false;

        }
        else if (cloudsTargetSelect == CloudsTargetSelect.Preset3)
        {
          List<CloudSphere> cloudSpheres = new List<CloudSphere>();
          AddCloudSpheres(cloudSpheres, 0.50, new Vector3(-0.60f, 0.0f, 0), 1.0);
          AddCloudSpheres(cloudSpheres, 0.70, new Vector3(+0.00f, 0.2f, 0), 1.0);
          AddCloudSpheres(cloudSpheres, 0.50, new Vector3(+0.60f, 0.0f, 0), 1.0);

          surfaceType = SurfaceType.Smooth;
          direction = Direction.Y_Axis;
          pivotPosition = PivotPosition.Center;

          resolution = 64;
          numberOfSpheres = cloudSpheres;
          roundness = 0.0;

          flipNormals = false;
        }
        else if (cloudsTargetSelect == CloudsTargetSelect.Preset4)
        {
          List<CloudSphere> cloudSpheres = new List<CloudSphere>();
          AddCloudSpheres(cloudSpheres, 0.25, new Vector3(-0.20f, 0.0f, 0), 1.0);
          AddCloudSpheres(cloudSpheres, 0.40, new Vector3(+0.20f, 0.1f, 0), 1.0);

          surfaceType = SurfaceType.Smooth;
          direction = Direction.Y_Axis;
          pivotPosition = PivotPosition.Center;

          resolution = 32;
          numberOfSpheres = cloudSpheres;
          roundness = 0.0;

          flipNormals = false;
        }
        else if (cloudsTargetSelect == CloudsTargetSelect.Preset5)
        {
          List<CloudSphere> cloudSpheres = new List<CloudSphere>();
          AddCloudSpheres(cloudSpheres, 0.80, new Vector3(-2.30f, +0.00f, +0.00f), 16.00);
          AddCloudSpheres(cloudSpheres, 0.80, new Vector3(-1.30f, +0.00f, +0.00f), 16.00);
          AddCloudSpheres(cloudSpheres, 1.00, new Vector3(+0.00f, +0.00f, +0.00f), 16.00);
          AddCloudSpheres(cloudSpheres, 1.50, new Vector3(+1.50f, +0.40f, +0.00f), 16.00);
          AddCloudSpheres(cloudSpheres, 1.00, new Vector3(+3.00f, +0.10f, +0.00f), 16.00);

          surfaceType = SurfaceType.Smooth;
          direction = Direction.Y_Axis;
          pivotPosition = PivotPosition.Center;

          resolution = 64;
          numberOfSpheres = cloudSpheres;
          roundness = 0.0;

          flipNormals = false;
        }
        else if (cloudsTargetSelect == CloudsTargetSelect.Preset6)
        {
          List<CloudSphere> cloudSpheres = new List<CloudSphere>();
          AddCloudSpheres(cloudSpheres, 0.50, new Vector3(-1.50f, +0.00f, +0.01f), 0.00);
          AddCloudSpheres(cloudSpheres, 0.70, new Vector3(-0.90f, +0.00f, +0.00f), 0.00);
          AddCloudSpheres(cloudSpheres, 0.80, new Vector3(-0.60f, +0.40f, +0.50f), 0.00);
          AddCloudSpheres(cloudSpheres, 0.80, new Vector3(+0.00f, +0.00f, +0.00f), 0.00);
          AddCloudSpheres(cloudSpheres, 0.70, new Vector3(+0.50f, +0.30f, +0.60f), 0.00);
          AddCloudSpheres(cloudSpheres, 0.50, new Vector3(+0.90f, +0.00f, +0.01f), 0.00);

          surfaceType = SurfaceType.Flat;
          direction = Direction.Y_Axis;
          pivotPosition = PivotPosition.Center;

          resolution = 32;
          numberOfSpheres = cloudSpheres;
          roundness = 0.0;

          flipNormals = false;
        }
        else if (cloudsTargetSelect == CloudsTargetSelect.MakeFromScratch)
        {
          List<CloudSphere> cloudSpheres = new List<CloudSphere>();
          AddCloudSpheres(cloudSpheres, 0.50, new Vector3(-0.25f, 0.00f, 0.00f), 1.0);
          AddCloudSpheres(cloudSpheres, 0.50, new Vector3(+0.25f, 0.00f, 0.00f), 1.0);

          surfaceType = SurfaceType.Smooth;
          direction = Direction.Y_Axis;
          pivotPosition = PivotPosition.Center;

          resolution = 32;
          numberOfSpheres = cloudSpheres;
          roundness = 0.0;

          flipNormals = false;
        }

      }

      void AddCloudSpheres(List<CloudSphere> ref_cloudSpheres, double _radius, Vector3 _position, double _blendWithNextSphere)
      {
        ref_cloudSpheres.Add(new CloudSphere { radius = _radius, position = _position, blendWithNextSphere = _blendWithNextSphere });
      }

    }
  }
}