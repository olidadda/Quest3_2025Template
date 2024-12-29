using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil
{
    public class RectTube : PPBase
    {
        public float width1 = 1.0f;
        public float width2 = 0.6f;
        public float length1 = 1.0f;
        public float length2 = 0.6f;
        public float height = 1.0f;
        public bool cap1 = false;
        public float capThickness1 = 0.2f;
        public bool cap2 = false;
        public float capThickness2 = 0.2f;
        public int widthSegs = 2;
        public int lengthSegs = 2;
        public int heightSegs = 2;

        protected override void CreateMesh()
        {
            m_mesh.name = "RectTube";

            length1 = Mathf.Clamp(length1, 0.00001f, 10000.0f);
            width1 = Mathf.Clamp(width1, 0.00001f, 10000.0f);
            length2 = Mathf.Clamp(length2, 0.00001f, length1);
            width2 = Mathf.Clamp(width2, 0.00001f, width1);
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
            lengthSegs = Mathf.Clamp(lengthSegs, 1, 100);
            widthSegs = Mathf.Clamp(widthSegs, 1, 100);
            heightSegs = Mathf.Clamp(heightSegs, 1, 100);

            float heightHalf = height * 0.5f;
            float capHeightDown = cap1 ? capThickness1 : 0.0f;
            float capHeightUp = cap2 ? capThickness2 : 0.0f;
            float heighMid = height - capHeightDown - capHeightUp;
            float tilingMidY = realWorldMapSize ? 1.0f : heighMid / height;
            float tilingDownY = realWorldMapSize ? 1.0f : capHeightDown / height;
            float tilingUpY = realWorldMapSize ? 1.0f : capHeightUp / height;
            float offsetMidY = realWorldMapSize ? capHeightDown : capHeightDown / height;
            float offsetUpY = realWorldMapSize ? heighMid + capHeightDown : (heighMid + capHeightDown) / height;
            Vector3 midOffset = m_up * (capHeightDown - capHeightUp) * 0.5f;

            Vector2 UVTilingMid = new Vector2(UVTiling.x, UVTiling.y * tilingMidY);
            Vector2 UVTilingDown = new Vector2(UVTiling.x, UVTiling.y * tilingDownY);
            Vector2 UVTilingUp = new Vector2(UVTiling.x, UVTiling.y * tilingUpY);
            Vector2 UVOffsetMid = UVOffset + new Vector2(0.0f, UVTiling.y * offsetMidY);
            Vector2 UVOffsetUp = UVOffset + new Vector2(0.0f, UVTiling.y * offsetUpY);

            m_builder.CreateBox(pivotOffset + midOffset, m_forward, m_right, width1, length1, heighMid, widthSegs, lengthSegs, heightSegs, generateMappingCoords, realWorldMapSize, UVOffsetMid, UVTilingMid, flipNormals, false, false, true, true, true, true);
            m_builder.CreateBox(pivotOffset + midOffset, m_forward, m_right, width2, length2, heighMid, widthSegs, lengthSegs, heightSegs, generateMappingCoords, realWorldMapSize, UVOffsetMid, UVTilingMid, !flipNormals, false, false, true, true, true, true);

            if (cap1)
            {
                m_builder.CreateBox(pivotOffset + m_down * (heightHalf - capHeightDown * 0.5f), m_forward, m_right, width1, length1, capHeightDown, widthSegs, lengthSegs, 1, generateMappingCoords, realWorldMapSize, UVOffset, UVTilingDown, flipNormals, false, false, true, true, true, true);
                m_builder.CreateSurfacePlane(pivotOffset + m_down * heightHalf, m_forward, m_left, width1, length1, widthSegs, lengthSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
                m_builder.CreateSurfacePlane(pivotOffset + m_down * (heightHalf - capHeightDown), m_forward, m_left, width2, length2, widthSegs, lengthSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, !flipNormals);
            }
            else m_builder.CreateSurfaceRectRing(pivotOffset + m_down * heightHalf, m_forward, m_left, width1, length1, width2, length2, 1, widthSegs, lengthSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);

            if (cap2)
            {
                m_builder.CreateBox(pivotOffset + m_up * (heightHalf - capHeightUp * 0.5f), m_forward, m_right, width1, length1, capHeightUp, widthSegs, lengthSegs, 1, generateMappingCoords, realWorldMapSize, UVOffsetUp, UVTilingUp, flipNormals, false, false, true, true, true, true);
                m_builder.CreateSurfacePlane(pivotOffset + m_up * heightHalf, m_forward, m_right, width1, length1, widthSegs, lengthSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
                m_builder.CreateSurfacePlane(pivotOffset + m_up * (heightHalf - capHeightUp), m_forward, m_right, width2, length2, widthSegs, lengthSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, !flipNormals);
            }
            else m_builder.CreateSurfaceRectRing(pivotOffset + m_up * heightHalf, m_forward, m_right, width1, length1, width2, length2, 1, widthSegs, lengthSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
        }
    }
}