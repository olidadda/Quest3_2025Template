using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil.Demo
{
    public class DemoManager : MonoBehaviour
    {
        static DemoManager instance;
        public static DemoManager Instance { get { if (instance == null) instance = FindObjectOfType<DemoManager>(); return instance; } }

        public Material[] demoMaterials;

        DemoHelper[] helpers;

        private void Awake()
        {
            helpers = GetComponentsInChildren<DemoHelper>(true);
        }

        public void SetMaterial(int x)
        {
            foreach (DemoHelper helper in helpers)
            {
                helper.GetComponent<MeshRenderer>().sharedMaterial = demoMaterials[x];
            }
        }

        public void ToggleWireframe(bool isOn)
        {
            foreach (DemoHelper helper in helpers)
            {
                helper.transform.GetChild(0).gameObject.SetActive(isOn);
            }
        }
    }
}