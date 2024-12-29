using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil
{
    public class Cone : PPCircularBase
    {
        public float radius1 = 0.5f;
        public float radius2 = 0.3f;
        public float height = 1.0f;
        public int capSegs = 2;
        public int heightSegs = 5;

        protected override void CreateMesh()
        {
            m_mesh.name = "Cone";

            radius1 = Mathf.Clamp(radius1, 0.00001f, 10000.0f);
            radius2 = Mathf.Clamp(radius2, 0.00001f, 10000.0f);
            height = Mathf.Clamp(height, 0.00001f, 10000.0f);
            sides = Mathf.Clamp(sides, 3, 100);
            capSegs = Mathf.Clamp(capSegs, 1, 100);
            heightSegs = Mathf.Clamp(heightSegs, 1, 100);
            sliceFrom = Mathf.Clamp(sliceFrom, 0.0f, 360.0f);
            sliceTo = Mathf.Clamp(sliceTo, sliceFrom, 360.0f);

            m_builder.CreateCone(pivotOffset, m_forward, m_right, height, radius1, radius2, true, true, sides, capSegs, heightSegs, sliceOn, sliceFrom, sliceTo, true, true, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth);
        }
    }
}