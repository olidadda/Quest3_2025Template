using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil.Demo
{
    public class DemoHelper : MonoBehaviour
    {
        protected PPBase baseObj;
        protected DemoWireframe wireframe;

        private void Start()
        {
            wireframe = GetComponentInChildren<DemoWireframe>(true);
            wireframe.RefreshMesh(baseObj.mesh);
        }

        public void SetRotationX(float x) 
        { 
            baseObj.rotation.x = x;
            Apply();
        }

        public void SetRotationY(float x) 
        {
            baseObj.rotation.y = x;
            Apply();
        }

        public void SetRotationZ(float x) 
        {
            baseObj.rotation.z = x;
            Apply();
        }

        public void SetMaterial(int x)
        {
            GetComponent<MeshRenderer>().sharedMaterial = DemoManager.Instance.demoMaterials[x];
        }

        protected void Apply()
        {
            baseObj.Apply();
            wireframe.RefreshMesh(baseObj.mesh);
        }
    }
}