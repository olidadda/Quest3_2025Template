using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil
{
    public class Tube : PPCircularBase
    {
        public float radius1 = 0.5f;
        public float radius2 = 0.3f;
        public float height = 1.0f;
        public bool cap1 = false;
        public float capThickness1 = 0.2f;
        public bool cap2 = false;
        public float capThickness2 = 0.2f;
        public int capSegs = 2;
        public int heightSegs = 2;

        protected override void CreateMesh()
        {
            m_mesh.name = "Tube";

            radius1 = Mathf.Clamp(radius1, 0.00001f, 10000.0f);
            radius2 = Mathf.Clamp(radius2, 0.00001f, radius1);
            height = Mathf.Clamp(height, 0.00001f, 10000.0f);
            if (cap1)
            {
                capThickness1 = Mathf.Clamp(capThickness1, 0.00001f, height);
                capThickness2 = Mathf.Clamp(capThickness2, 0.00001f, height - capThickness1);
            }
            else
            {
                capThickness1 = Mathf.Clamp(capThickness1, 0.00001f, height);
                capThickness2 = Mathf.Clamp(capThickness2, 0.00001f, height);
            }
            sides = Mathf.Clamp(sides, 3, 100);
            capSegs = Mathf.Clamp(capSegs, 1, 100);
            heightSegs = Mathf.Clamp(heightSegs, 1, 100);
            sliceFrom = Mathf.Clamp(sliceFrom, 0.0f, 360.0f);
            sliceTo = Mathf.Clamp(sliceTo, sliceFrom, 360.0f);

            float heightHalf = height * 0.5f;
            float capHeightDown = cap1 ? capThickness1 : 0.0f;
            float capHeightUp = cap2 ? capThickness2 : 0.0f;
            float heightMid = height - capHeightDown - capHeightUp;
            float tilingMidY = realWorldMapSize ? 1.0f : heightMid / height;
            float tilingDownY = realWorldMapSize ? 1.0f : capHeightDown / height;
            float tilingUpY = realWorldMapSize ? 1.0f : capHeightUp / height;
            float offsetMidY = realWorldMapSize ? capHeightDown : capHeightDown / height;
            float offsetUpY = realWorldMapSize ? heightMid + capHeightDown : (heightMid + capHeightDown) / height;

            Vector3 midOffset = m_up * (capHeightDown - capHeightUp) * 0.5f;
            Vector2 UVTilingMid = new Vector2(UVTiling.x, UVTiling.y * tilingMidY);
            Vector2 UVTilingDown = new Vector2(UVTiling.x, UVTiling.y * tilingDownY);
            Vector2 UVTilingUp = new Vector2(UVTiling.x, UVTiling.y * tilingUpY);
            Vector2 UVOffsetMid = UVOffset + new Vector2(0.0f, UVTiling.y * offsetMidY);
            Vector2 UVOffsetUp = UVOffset + new Vector2(0.0f, UVTiling.y * offsetUpY);

            m_builder.CreateTube(pivotOffset + midOffset, m_forward, m_right, heightMid, radius1, radius2, false, false, sides, capSegs, heightSegs, sliceOn, sliceFrom, sliceTo, true, true, generateMappingCoords, realWorldMapSize, UVOffsetMid, UVTilingMid, flipNormals, smooth);

            if (cap1)
            {
                m_builder.CreateCylinder(pivotOffset + m_down * (heightHalf - capHeightDown * 0.5f), m_forward, m_right, capHeightDown, radius1, false, false, sides, capSegs, 1, sliceOn, sliceFrom, sliceTo, true, true, generateMappingCoords, realWorldMapSize, UVOffset, UVTilingDown, flipNormals, smooth);
                m_builder.CreateSurfaceCircle(pivotOffset + m_down * heightHalf, m_forward, m_left, radius1, sides, capSegs, sliceOn, -sliceTo, -sliceFrom, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
                m_builder.CreateSurfaceCircle(pivotOffset + m_down * (heightHalf - capHeightDown), m_forward, m_left, radius2, sides, capSegs, sliceOn, -sliceTo, -sliceFrom, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, !flipNormals);
            }
            else m_builder.CreateSurfaceRing(pivotOffset + m_down * heightHalf, m_forward, m_left, radius1, radius2, sides, capSegs, sliceOn, -sliceTo, -sliceFrom, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);

            if (cap2)
            {
                m_builder.CreateCylinder(pivotOffset + m_up * (heightHalf - capHeightUp * 0.5f), m_forward, m_right, capHeightUp, radius1, false, false, sides, capSegs, 1, sliceOn, sliceFrom, sliceTo, true, true, generateMappingCoords, realWorldMapSize, UVOffsetUp, UVTilingUp, flipNormals, smooth);
                m_builder.CreateSurfaceCircle(pivotOffset + m_up * heightHalf, m_forward, m_right, radius1, sides, capSegs, sliceOn, sliceFrom, sliceTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
                m_builder.CreateSurfaceCircle(pivotOffset + m_up * (heightHalf - capHeightUp), m_forward, m_right, radius2, sides, capSegs, sliceOn, sliceFrom, sliceTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, !flipNormals);
            }
            else m_builder.CreateSurfaceRing(pivotOffset + m_up * heightHalf, m_forward, m_right, radius1, radius2, sides, capSegs, sliceOn, sliceFrom, sliceTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
        }
    }
}