using UnityEngine;
using System.Collections.Generic;

namespace UltimateProceduralPrimitives
{
  [System.Serializable]
  public class MeshCutProducer
  {
    public List<MeshCutParameters> numOfCuts = new List<MeshCutParameters>();

    public MeshCutProducer()
    {
      var meshCutParameters = new MeshCutParameters();
      numOfCuts.Add(meshCutParameters);
    }
  }

  [System.Serializable]
  public class MeshCutParameters
  {
    public bool doMeshCut = false;
    [Space(5)]
    public Vector3 cutPoint1 = new Vector3(0.0f, 0.0f, 0.0f);
    public Vector3 cutPoint2 = new Vector3(0.0f, 0.0f, 1.0f);
    public Vector3 cutPoint3 = new Vector3(-1.0f, 0.5f, 0.0f);
    [Space(5)]
    public Vector3 offset = new Vector3(0.0f, 0.0f, 0.0f);
    [Space(5)]
    public useSide useSide = useSide.A;
    [Space(5)]
    public bool caps = true;
    public Material capMaterial;
  }
}