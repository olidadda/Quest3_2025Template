using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil
{
    public class Circle : PPCircularBase
    {
        public float radius = 1;
        public int segments = 5;

        protected override void CreateMesh()
        {
            m_mesh.name = "Circle";

            radius = Mathf.Clamp(radius, 0.00001f, 10000.0f);
            segments = Mathf.Clamp(segments, 1, 100);
            sides = Mathf.Clamp(sides, 3, 100);
            sliceFrom = Mathf.Clamp(sliceFrom, 0.0f, 360.0f);
            sliceTo = Mathf.Clamp(sliceTo, sliceFrom, 360.0f);

            m_builder.CreateSurfaceCircle(pivotOffset, m_forward, m_right, radius, sides, segments, sliceOn, sliceFrom, sliceTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
        }
    }
}