using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using System.Collections.Generic;
#if UNITY_2020_2_OR_NEWER
using UnityEditor.AssetImporters;
#else
using UnityEditor.Experimental.AssetImporters;
#endif

namespace UltimateProceduralPrimitives
{
  [ScriptedImporter(1, "upp")]
  public sealed class UPPImporter : ScriptedImporter
  {
    [SerializeField] Shape _shape = Shape.BoxSuperEllipsoid;

    [SerializeField] Plane _plane = new Plane();
    [SerializeField] PlaneFlex _planeFlex = new PlaneFlex();
    [SerializeField] PlaneSuperEllipse _planeSuperEllipse = new PlaneSuperEllipse();
    [SerializeField] Box _box = new Box();
    [SerializeField] BoxFlex _boxFlex = new BoxFlex();
    [SerializeField] BoxRounded _boxRounded = new BoxRounded();
    [SerializeField] BoxSuperEllipsoid _boxSuperEllipsoid = new BoxSuperEllipsoid();
    [SerializeField] BoxSuperEllipsoidPlus _boxSuperEllipsoidPlus = new BoxSuperEllipsoidPlus();
    [SerializeField] Pyramid _pyramid = new Pyramid();
    [SerializeField] PyramidFlex _pyramidFlex = new PyramidFlex();
    [SerializeField] PyramidPerfectTriangularFlex _pyramidPerfectTriangularFlex = new PyramidPerfectTriangularFlex();
    [SerializeField] Sphere _sphere = new Sphere();
    [SerializeField] SphereIco _sphereIco = new SphereIco();
    [SerializeField] SphereFibonacci _sphereFibonacci = new SphereFibonacci();
    [SerializeField] TearDrop _tearDrop = new TearDrop();
    [SerializeField] Cylinder _cylinder = new Cylinder();
    [SerializeField] Cone _cone = new Cone();
    [SerializeField] Clouds _clouds = new Clouds();
    [SerializeField] Supershape _supershape = new Supershape();

    [SerializeField] MeshCutProducer _meshCut = new MeshCutProducer();


    public override void OnImportAsset(AssetImportContext context)
    {
      var gameObject = new GameObject();
      var mesh = ImportAsMesh(context.assetPath);

      var meshFilter = gameObject.AddComponent<MeshFilter>();
      meshFilter.sharedMesh = mesh;

      var pipelineAsset = GraphicsSettings.currentRenderPipeline;
      var baseMaterial = pipelineAsset ? pipelineAsset.defaultMaterial : AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat");

      var meshRenderer = gameObject.AddComponent<MeshRenderer>();
      List<Material> mats = new List<Material>();
      mats.Add(baseMaterial);
      for (int i = 0; i < _meshCut.numOfCuts.Count; i++)
      {
        var meshCutParameters = _meshCut.numOfCuts[i];
        if (meshCutParameters.doMeshCut && meshCutParameters.capMaterial && meshCutParameters.caps)
        {
          mats.Add(_meshCut.numOfCuts[i].capMaterial);
        }
      }
      meshRenderer.materials = mats.ToArray();

      context.AddObjectToAsset("prefab", gameObject);
      if (mesh != null) context.AddObjectToAsset("mesh", mesh);

      context.SetMainObject(gameObject);
    }

    Mesh ImportAsMesh(string path)
    {
      var mesh = new Mesh();
      mesh.name = $"UPP_{_shape.ToString()}";

      // timer
      Utils.TimerStart();

      // Generate mesh
      switch (_shape)
      {
        case Shape.Plane: _plane.Generate(mesh, _meshCut); break;
        case Shape.PlaneFlex: _planeFlex.Generate(mesh, _meshCut); break;
        case Shape.PlaneSuperEllipse: _planeSuperEllipse.Generate(mesh, _meshCut); break;
        case Shape.Box: _box.Generate(mesh, _meshCut); break;
        case Shape.BoxFlex: _boxFlex.Generate(mesh, _meshCut); break;
        case Shape.BoxRounded: _boxRounded.Generate(mesh, _meshCut); break;
        case Shape.BoxSuperEllipsoid: _boxSuperEllipsoid.Generate(mesh, _meshCut); break;
        case Shape.BoxSuperEllipsoidPlus: _boxSuperEllipsoidPlus.Generate(mesh, _meshCut); break;
        case Shape.Pyramid: _pyramid.Generate(mesh, _meshCut); break;
        case Shape.PyramidFlex: _pyramidFlex.Generate(mesh, _meshCut); break;
        case Shape.PyramidPerfectTriangularFlex: _pyramidPerfectTriangularFlex.Generate(mesh, _meshCut); break;
        case Shape.Sphere: _sphere.Generate(mesh, _meshCut); break;
        case Shape.SphereIco: _sphereIco.Generate(mesh, _meshCut); break;
        case Shape.SphereFibonacci: _sphereFibonacci.Generate(mesh, _meshCut); break;
        case Shape.TearDrop: _tearDrop.Generate(mesh, _meshCut); break;
        case Shape.Cylinder: _cylinder.Generate(mesh, _meshCut); break;
        case Shape.Cone: _cone.Generate(mesh, _meshCut); break;
        case Shape.Clouds: _clouds.Generate(mesh, _meshCut); break;
        case Shape.Supershape: _supershape.Generate(mesh, _meshCut); break;
      }

      // RecalculateBounds
      mesh.RecalculateBounds();

      //Results
      Debug.Log($"UPP Results : mesh.vertices.Length = {mesh.vertices.Length},  mesh.GetIndexCount(0) = {mesh.GetIndexCount(0)},  mesh.triangles.Length = {mesh.triangles.Length}");

      // Timer
      Utils.TimerStop(_shape);

      // return
      return mesh;
    }
  }
}