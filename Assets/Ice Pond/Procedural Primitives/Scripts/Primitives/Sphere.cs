using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil
{
    public class Sphere : PPCircularBase
    {
        public float radius = 0.5f;
        public SphereCutOption cutType = SphereCutOption.None;
        public float cutFrom = 0.0f;
        public float cutTo = 1.0f;

        protected override void CreateMesh()
        {
            m_mesh.name = "Sphere";

            radius = Mathf.Clamp(radius, 0.00001f, 10000.0f);
            sides = Mathf.Clamp(sides, 4, 100);
            sliceFrom = Mathf.Clamp(sliceFrom, 0.0f, 360.0f);
            sliceTo = Mathf.Clamp(sliceTo, sliceFrom, 360.0f);
            cutFrom = Mathf.Clamp01(cutFrom);
            cutTo = Mathf.Clamp(cutTo, cutFrom, 1.0f);
            int segCap = Mathf.Clamp(sides / 6, 1, sides);

            m_builder.CreateSphere(pivotOffset, m_forward, m_right, radius, cutType, sides, sides / 2, segCap, sliceOn, sliceFrom, sliceTo, true, true, cutType != SphereCutOption.None, cutFrom, cutTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth);
        }
    }
}