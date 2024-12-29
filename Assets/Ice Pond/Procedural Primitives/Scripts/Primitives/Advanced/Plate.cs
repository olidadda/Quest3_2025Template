using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil
{
    public class Plate : PPCircularBase
    {
        public float radius = 0.5f;
        public float fillet = 0.2f;
        public float thickness = 0.02f;
        public bool cap = false;
        public bool flatChamfer = false;
        public int capSegs = 2;
        public int filletSegs = 3;

        protected override void CreateMesh()
        {
            m_mesh.name = "Plate";

            radius = Mathf.Clamp(radius, 0.00001f, 10000.0f);
            fillet = Mathf.Clamp(fillet, 0.00001f, radius);
            thickness = Mathf.Clamp(thickness, 0.00001f, fillet);
            sides = Mathf.Clamp(sides, 3, 100);
            capSegs = Mathf.Clamp(capSegs, 1, 100);
            filletSegs = Mathf.Clamp(filletSegs, 1, 100);
            sliceFrom = Mathf.Clamp(sliceFrom, 0.0f, 360.0f);
            sliceTo = Mathf.Clamp(sliceTo, sliceFrom, 360.0f);

            m_builder.CreatePlate(pivotOffset, m_forward, m_right, false, radius, fillet, cap, true, flatChamfer, sides, filletSegs, capSegs, sliceOn, sliceFrom, sliceTo, cap, cap, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth);
            
            if (!cap)
            {
                float filletHalf = fillet * 0.5f;

                m_builder.CreatePlate(pivotOffset + m_up * thickness * 0.5f, m_forward, m_right, false, radius - thickness, fillet - thickness, cap, true, flatChamfer, sides, filletSegs, capSegs, sliceOn, sliceFrom, sliceTo, cap, cap, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, !flipNormals, smooth);
                m_builder.CreateSurfaceRing(pivotOffset + m_up * filletHalf, m_forward, m_right, radius, radius - thickness, sides, 1, sliceOn, sliceFrom, sliceTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);

                if (sliceOn)
                {
                    int segSlice = flatChamfer ? 1 : filletSegs;
                    float planeWidth = radius - fillet;
                    float tilingSliceX = realWorldMapSize ? 1.0f : planeWidth / radius;
                    float tilingSliceY = realWorldMapSize ? 1.0f : thickness / fillet;
                    float tilingCircleX = realWorldMapSize ? 1.0f : fillet / radius;
                    float tilingCircleY = realWorldMapSize ? 1.0f : fillet * 2.0f / fillet;
                    float offsetSliceX = realWorldMapSize ? fillet : fillet / (radius * 2);

                    Vector2 UVTilingSlice = new Vector2(UVTiling.x * tilingSliceX, UVTiling.y * tilingSliceY);
                    Vector2 UVTilingCircle = new Vector2(UVTiling.x * tilingCircleX, UVTiling.y * tilingCircleY);
                    Vector2 UVOffsetSlice = UVOffset + new Vector2(UVTiling.x * offsetSliceX, 0.0f);

                    m_builder.CreateSliceFromToPlane(pivotOffset + m_down * ((fillet - thickness) * 0.5f), m_forward, m_right, planeWidth * 0.5f, planeWidth, thickness, capSegs, 1, sliceFrom, sliceTo, true, true, generateMappingCoords, realWorldMapSize, UVOffsetSlice, UVTilingSlice, flipNormals);
                    m_builder.CreateSliceFromToRing(pivotOffset + m_up * filletHalf, m_forward, m_right, radius - fillet, fillet, 1.0f - thickness / fillet, segSlice, 1, true, sliceFrom, sliceTo, true, true, 180.0f, 270.0f, generateMappingCoords, realWorldMapSize, UVOffset, UVTilingCircle, flipNormals);
                }
            }
        }
    }
}