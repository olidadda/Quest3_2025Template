using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil
{
    public class Ring : PPCircularBase
    {
        public float radius1 = 1;
        public float radius2 = 0.5f;
        public int segments = 3;

        protected override void CreateMesh()
        {
            m_mesh.name = "Ring";

            radius1 = Mathf.Clamp(radius1, 0.00001f, 10000.0f);
            radius2 = Mathf.Clamp(radius2, 0.00001f, radius1);
            segments = Mathf.Clamp(segments, 1, 100);
            sides = Mathf.Clamp(sides, 3, 100);
            sliceFrom = Mathf.Clamp(sliceFrom, 0.0f, 360.0f);
            sliceTo = Mathf.Clamp(sliceTo, sliceFrom, 360.0f);

            m_builder.CreateSurfaceRing(pivotOffset, m_forward, m_right, radius1, radius2, sides, segments, sliceOn, sliceFrom, sliceTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
        }
    }
}