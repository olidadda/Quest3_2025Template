using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil
{
    public class Torus : PPCircularBase
    {
        public float radius1 = 0.5f;
        public float radius2 = 0.1f;
        public int segments = 12;

        protected override void CreateMesh()
        {
            m_mesh.name = "Torus";

            radius1 = Mathf.Clamp(radius1, 0.0f, 10000.0f);
            radius2 = Mathf.Clamp(radius2, 0.0f, 10000.0f);
            sides = Mathf.Clamp(sides, 3, 100);
            segments = Mathf.Clamp(segments, 3, 100);
            sliceFrom = Mathf.Clamp(sliceFrom, 0.0f, 360.0f);
            sliceTo = Mathf.Clamp(sliceTo, sliceFrom, 360.0f);
            int segCap = Mathf.Clamp(segments / 6, 1, segments);

            m_builder.CreateTorus(pivotOffset, m_forward, m_right, radius1, radius2, sides, segments, segCap, sliceOn, sliceFrom, sliceTo, true, true, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth);
        }
    }
}