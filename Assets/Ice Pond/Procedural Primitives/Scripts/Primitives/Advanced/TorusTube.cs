using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil
{
    public class TorusTube : PPCircularBase
    {
        public float radius1 = 0.5f;
        public float radius2 = 0.1f;
        public float radius3 = 0.05f;
        public int segments = 12;

        protected override void CreateMesh()
        {
            m_mesh.name = "Torus";

            radius1 = Mathf.Clamp(radius1, 0.0f, 10000.0f);
            radius2 = Mathf.Clamp(radius2, 0.0f, 10000.0f);
            radius3 = Mathf.Clamp(radius3, 0.0f, radius2);
            sides = Mathf.Clamp(sides, 3, 100);
            segments = Mathf.Clamp(segments, 3, 100);
            sliceFrom = Mathf.Clamp(sliceFrom, 0.0f, 360.0f);
            sliceTo = Mathf.Clamp(sliceTo, sliceFrom, 360.0f);

            m_builder.CreateSurfaceTorus(pivotOffset, m_forward, m_right, radius1, radius2, sides, segments, sliceOn, sliceFrom, sliceTo, 0.0f, 360.0f, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth);
            m_builder.CreateSurfaceTorus(pivotOffset, m_forward, m_right, radius1, radius3, sides, segments, sliceOn, sliceFrom, sliceTo, 0.0f, 360.0f, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, !flipNormals, smooth);
            if (sliceOn) m_builder.CreateSliceFromToRing(pivotOffset, m_forward, m_right, radius1, radius2, radius3 / radius2, segments, 1, sliceOn, sliceFrom, sliceTo, true, true, 0.0f, 360.0f, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, true);
        }
    }
}