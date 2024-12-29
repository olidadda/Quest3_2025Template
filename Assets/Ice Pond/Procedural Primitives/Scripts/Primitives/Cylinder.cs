using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil
{
    public class Cylinder : PPCircularBase
    {
        public float radius = 0.5f;
        public float height = 1.0f;
        public int capSegs = 2;
        public int heightSegs = 2;

        protected override void CreateMesh()
        {
            m_mesh.name = "Cylinder";

            radius = Mathf.Clamp(radius, 0.00001f, 10000.0f);
            height = Mathf.Clamp(height, 0.00001f, 10000.0f);
            sides = Mathf.Clamp(sides, 3, 100);
            capSegs = Mathf.Clamp(capSegs, 1, 100);
            heightSegs = Mathf.Clamp(heightSegs, 1, 100);
            sliceFrom = Mathf.Clamp(sliceFrom, 0.0f, 360.0f);
            sliceTo = Mathf.Clamp(sliceTo, sliceFrom, 360.0f);

            m_builder.CreateCylinder(pivotOffset, m_forward, m_right, height, radius, true, true, sides, capSegs, heightSegs, sliceOn, sliceFrom, sliceTo, true, true, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth);
        }
    }
}