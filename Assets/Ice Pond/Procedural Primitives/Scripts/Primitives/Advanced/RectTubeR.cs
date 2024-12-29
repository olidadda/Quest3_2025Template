using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil
{
    public class RectTubeR : PPBase
    {
        public float width = 1.0f;
        public float length = 1.0f;
        public float height = 1.0f;
        public float fillet = 0.2f;
        public float thickness = 0.05f;
        public bool cap1 = false;
        public bool cap2 = false;
        public int widthSegs = 2;
        public int lengthSegs = 2;
        public int heightSegs = 2;
        public int filletSegs = 3;

        protected override void CreateMesh()
        {
            m_mesh.name = "RectTubeR";

            length = Mathf.Clamp(length, 0.00001f, 10000.0f);
            width = Mathf.Clamp(width, 0.00001f, 10000.0f);
            height = Mathf.Clamp(height, 0.00001f, 10000.0f);
            float min = Mathf.Min(width * 0.5f, length * 0.5f, height);
            fillet = Mathf.Clamp(fillet, 0.00001f, min);
            thickness = Mathf.Clamp(thickness, 0.00001f, fillet);
            lengthSegs = Mathf.Clamp(lengthSegs, 1, 100);
            widthSegs = Mathf.Clamp(widthSegs, 1, 100);
            heightSegs = Mathf.Clamp(heightSegs, 1, 100);
            filletSegs = Mathf.Clamp(filletSegs, 1, 100);


            float lengthHalf = length * 0.5f;
            float widthHalf = width * 0.5f;
            float filletHalf = fillet * 0.5f;
            float heightHalf = height * 0.5f;
            float widthMid = width - fillet * 2.0f;
            float lengthMid = length - fillet * 2.0f;
            float filletMid = fillet - thickness;
            float widthMidHalf = widthMid * 0.5f;
            float lengthMidHalf = lengthMid * 0.5f;
            float filletDown = cap1 ? fillet : 0.0f;
            float filletUp = cap2 ? fillet : 0.0f;
            float thicknessDouble = thickness * 2.0f;

            float heighMid = height - filletDown - filletUp;
            float pirUp = filletUp * Mathf.PI * 0.5f;
            float pirDown = filletDown * Mathf.PI * 0.5f;
            float heightTotal = heighMid + pirUp + pirDown;
            float tilingMidY = realWorldMapSize ? 1.0f : heighMid / heightTotal;
            float tilingDownY = realWorldMapSize ? 1.0f : pirDown / heightTotal;
            float tilingUpY = realWorldMapSize ? 1.0f : pirUp / heightTotal;
            float offsetMidY = realWorldMapSize ? pirDown : pirDown / heightTotal;
            float offsetUpY = realWorldMapSize ? pirDown + heighMid : (pirDown + heighMid) / heightTotal;

            Vector3 midOffset = pivotOffset + m_up * (filletDown - filletUp) * 0.5f;
            Vector3 TR = m_forward * lengthMidHalf + m_right * widthMidHalf;
            Vector3 BR = m_back * lengthMidHalf + m_right * widthMidHalf;
            Vector3 BL = m_back * lengthMidHalf + m_left * widthMidHalf;
            Vector3 TL = m_forward * lengthMidHalf + m_left * widthMidHalf;

            Vector2 UVTilingMid = new Vector2(UVTiling.x, UVTiling.y * tilingMidY);
            Vector2 UVTilingDown = new Vector2(UVTiling.x, UVTiling.y * tilingDownY);
            Vector2 UVTilingUp = new Vector2(UVTiling.x, UVTiling.y * tilingUpY);
            Vector2 UVOffsetMid = UVOffset + new Vector2(0.0f, UVTiling.y * offsetMidY);
            Vector2 UVOffsetUp = UVOffset + new Vector2(0.0f, UVTiling.y * offsetUpY);

            m_builder.CreateSurfacePlane(midOffset + m_back * lengthHalf, m_up, m_right, widthMid, heighMid, widthSegs, heightSegs, generateMappingCoords, realWorldMapSize, UVOffsetMid, UVTilingMid, flipNormals);
            m_builder.CreateSurfacePlane(midOffset + m_forward * lengthHalf, m_up, m_left, widthMid, heighMid, widthSegs, heightSegs, generateMappingCoords, realWorldMapSize, UVOffsetMid, UVTilingMid, flipNormals);
            m_builder.CreateSurfacePlane(midOffset + m_left * widthHalf, m_up, m_back, lengthMid, heighMid, lengthSegs, heightSegs, generateMappingCoords, realWorldMapSize, UVOffsetMid, UVTilingMid, flipNormals);
            m_builder.CreateSurfacePlane(midOffset + m_right * widthHalf, m_up, m_forward, lengthMid, heighMid, lengthSegs, heightSegs, generateMappingCoords, realWorldMapSize, UVOffsetMid, UVTilingMid, flipNormals);

            m_builder.CreateSurfacePlane(midOffset + m_back * lengthHalf + m_forward * thickness, m_up, m_right, widthMid, heighMid, widthSegs, heightSegs, generateMappingCoords, realWorldMapSize, UVOffsetMid, UVTilingMid, !flipNormals);
            m_builder.CreateSurfacePlane(midOffset + m_forward * lengthHalf + m_back * thickness, m_up, m_left, widthMid, heighMid, widthSegs, heightSegs, generateMappingCoords, realWorldMapSize, UVOffsetMid, UVTilingMid, !flipNormals);
            m_builder.CreateSurfacePlane(midOffset + m_left * widthHalf + m_right * thickness, m_up, m_back, lengthMid, heighMid, lengthSegs, heightSegs, generateMappingCoords, realWorldMapSize, UVOffsetMid, UVTilingMid, !flipNormals);
            m_builder.CreateSurfacePlane(midOffset + m_right * widthHalf + m_left * thickness, m_up, m_forward, lengthMid, heighMid, lengthSegs, heightSegs, generateMappingCoords, realWorldMapSize, UVOffsetMid, UVTilingMid, !flipNormals);

            m_builder.CreateSurfaceCylinder(midOffset + TR, m_forward, m_right, heighMid, fillet, filletSegs, heightSegs, true, 0.0f, 90.0f, generateMappingCoords, realWorldMapSize, UVOffsetMid, UVTilingMid, flipNormals, smooth);
            m_builder.CreateSurfaceCylinder(midOffset + BR, m_forward, m_right, heighMid, fillet, filletSegs, heightSegs, true, 90.0f, 180.0f, generateMappingCoords, realWorldMapSize, UVOffsetMid, UVTilingMid, flipNormals, smooth);
            m_builder.CreateSurfaceCylinder(midOffset + BL, m_forward, m_right, heighMid, fillet, filletSegs, heightSegs, true, 180.0f, 270.0f, generateMappingCoords, realWorldMapSize, UVOffsetMid, UVTilingMid, flipNormals, smooth);
            m_builder.CreateSurfaceCylinder(midOffset + TL, m_forward, m_right, heighMid, fillet, filletSegs, heightSegs, true, 270.0f, 360.0f, generateMappingCoords, realWorldMapSize, UVOffsetMid, UVTilingMid, flipNormals, smooth);

            m_builder.CreateSurfaceCylinder(midOffset + TR, m_forward, m_right, heighMid, filletMid, filletSegs, heightSegs, true, 0.0f, 90.0f, generateMappingCoords, realWorldMapSize, UVOffsetMid, UVTilingMid, !flipNormals, smooth);
            m_builder.CreateSurfaceCylinder(midOffset + BR, m_forward, m_right, heighMid, filletMid, filletSegs, heightSegs, true, 90.0f, 180.0f, generateMappingCoords, realWorldMapSize, UVOffsetMid, UVTilingMid, !flipNormals, smooth);
            m_builder.CreateSurfaceCylinder(midOffset + BL, m_forward, m_right, heighMid, filletMid, filletSegs, heightSegs, true, 180.0f, 270.0f, generateMappingCoords, realWorldMapSize, UVOffsetMid, UVTilingMid, !flipNormals, smooth);
            m_builder.CreateSurfaceCylinder(midOffset + TL, m_forward, m_right, heighMid, filletMid, filletSegs, heightSegs, true, 270.0f, 360.0f, generateMappingCoords, realWorldMapSize, UVOffsetMid, UVTilingMid, !flipNormals, smooth);

            if (cap1)
            {
                m_builder.CreateTray(pivotOffset + m_down * (heightHalf - filletDown * 0.5f), m_forward, m_right, false, width, length, fillet, false, false, false, widthSegs, lengthSegs, filletSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTilingDown, flipNormals, smooth);
                m_builder.CreateSurfacePlane(pivotOffset + m_down * heightHalf, m_forward, m_left, widthMid, lengthMid, widthSegs, lengthSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
                m_builder.CreateTray(pivotOffset + m_down * (heightHalf - filletDown * 0.5f) + m_up * thickness * 0.5f, m_forward, m_right, false, width - thicknessDouble, length - thicknessDouble, fillet - thickness, false, false, false, widthSegs, lengthSegs, filletSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTilingDown, !flipNormals, smooth);
                m_builder.CreateSurfacePlane(pivotOffset + m_down * heightHalf + m_up * thickness, m_forward, m_left, widthMid, lengthMid, widthSegs, lengthSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, !flipNormals);
            }
            else
            {
                m_builder.CreateRectRing(pivotOffset + m_down * heightHalf, m_forward, m_left, width, length, fillet, thickness, widthSegs, lengthSegs, filletSegs, 1, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            }

            if (cap2)
            {
                m_builder.CreateTray(pivotOffset + m_up * (heightHalf - filletUp * 0.5f), m_forward, m_right, true, width, length, fillet, false, false, false, widthSegs, lengthSegs, filletSegs, generateMappingCoords, realWorldMapSize, UVOffsetUp, UVTilingUp, flipNormals, smooth);
                m_builder.CreateSurfacePlane(pivotOffset + m_up * heightHalf, m_forward, m_right, widthMid, lengthMid, widthSegs, lengthSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
                m_builder.CreateTray(pivotOffset + m_up * (heightHalf - filletUp * 0.5f) + m_down * thickness * 0.5f, m_forward, m_right, true, width - thicknessDouble, length - thicknessDouble, fillet - thickness, false, false, false, widthSegs, lengthSegs, filletSegs, generateMappingCoords, realWorldMapSize, UVOffsetUp, UVTilingUp, !flipNormals, smooth);
                m_builder.CreateSurfacePlane(pivotOffset + m_up * heightHalf + m_down * thickness, m_forward, m_right, widthMid, lengthMid, widthSegs, lengthSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, !flipNormals);
            }
            else
            {
                m_builder.CreateRectRing(pivotOffset + m_up * heightHalf, m_forward, m_right, width, length, fillet, thickness, widthSegs, lengthSegs, filletSegs, 1, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            }
        }
    }
}