using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil
{
    public class ChamferBox : PPBase
    {
        public float width = 1.0f;
        public float length = 1.0f;
        public float height = 1.0f;
        public float fillet = 0.1f;
        public bool chamfer1 = true;
        public bool chamfer2 = true;
        public bool flatChamfer = false;
        public int widthSegs = 2;
        public int lengthSegs = 2;
        public int heightSegs = 2;
        public int filletSegs = 3;

        protected override void CreateMesh()
        {
            m_mesh.name = "Chamfer Box";

            length = Mathf.Clamp(length, 0.00001f, 10000.0f);
            width = Mathf.Clamp(width, 0.00001f, 10000.0f);
            height = Mathf.Clamp(height, 0.00001f, 10000.0f);
            float min = length < width ? length : width;
            min = min < height ? min : height;
            fillet = Mathf.Clamp(fillet, 0.00001f, min / 2.0f);
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
            float widthMidHalf = widthMid * 0.5f;
            float lengthMidHalf = lengthMid * 0.5f;
            float filletDown = chamfer1 ? fillet : 0.0f;
            float filletUp = chamfer2 ? fillet : 0.0f;

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

            m_builder.CreateSurfacePlane(midOffset + m_back * lengthHalf,    m_up, m_right,   widthMid,  heighMid, widthSegs,  heightSegs, generateMappingCoords, realWorldMapSize, UVOffsetMid, UVTilingMid, flipNormals);
            m_builder.CreateSurfacePlane(midOffset + m_forward * lengthHalf, m_up, m_left,    widthMid,  heighMid, widthSegs,  heightSegs, generateMappingCoords, realWorldMapSize, UVOffsetMid, UVTilingMid, flipNormals);
            m_builder.CreateSurfacePlane(midOffset + m_left * widthHalf,     m_up, m_back,    lengthMid, heighMid, lengthSegs, heightSegs, generateMappingCoords, realWorldMapSize, UVOffsetMid, UVTilingMid, flipNormals);
            m_builder.CreateSurfacePlane(midOffset + m_right * widthHalf,    m_up, m_forward, lengthMid, heighMid, lengthSegs, heightSegs, generateMappingCoords, realWorldMapSize, UVOffsetMid, UVTilingMid, flipNormals);

            if (flatChamfer)
            {
                Vector3 outTR = (m_forward + m_right) * filletHalf;
                Vector3 outBR = (m_back + m_right) * filletHalf;
                Vector3 outBL = (m_back + m_left) * filletHalf;
                Vector3 outTL = (m_forward + m_left) * filletHalf;

                Vector3 rTR = (m_forward + m_left) * 0.5f;
                Vector3 rBR = (m_forward + m_right) * 0.5f;
                Vector3 rBL = (m_back + m_right) * 0.5f;
                Vector3 rTL = (m_back + m_left) * 0.5f;

                float flatWidth = Mathf.Sqrt(fillet * fillet * 2);
                m_builder.CreateSurfacePlane(midOffset + TR + outTR, m_up, rTR.normalized, flatWidth, heighMid, filletSegs, heightSegs, generateMappingCoords, realWorldMapSize, UVOffsetMid, UVTilingMid, flipNormals);
                m_builder.CreateSurfacePlane(midOffset + BR + outBR, m_up, rBR.normalized, flatWidth, heighMid, filletSegs, heightSegs, generateMappingCoords, realWorldMapSize, UVOffsetMid, UVTilingMid, flipNormals);
                m_builder.CreateSurfacePlane(midOffset + BL + outBL, m_up, rBL.normalized, flatWidth, heighMid, filletSegs, heightSegs, generateMappingCoords, realWorldMapSize, UVOffsetMid, UVTilingMid, flipNormals);
                m_builder.CreateSurfacePlane(midOffset + TL + outTL, m_up, rTL.normalized, flatWidth, heighMid, filletSegs, heightSegs, generateMappingCoords, realWorldMapSize, UVOffsetMid, UVTilingMid, flipNormals);
            }
            else
            {
                m_builder.CreateSurfaceCylinder(midOffset + TR, m_forward, m_right, heighMid, fillet, filletSegs, heightSegs, true, 0.0f, 90.0f,    generateMappingCoords, realWorldMapSize, UVOffsetMid, UVTilingMid, flipNormals, smooth);
                m_builder.CreateSurfaceCylinder(midOffset + BR, m_forward, m_right, heighMid, fillet, filletSegs, heightSegs, true, 90.0f, 180.0f,  generateMappingCoords, realWorldMapSize, UVOffsetMid, UVTilingMid, flipNormals, smooth);
                m_builder.CreateSurfaceCylinder(midOffset + BL, m_forward, m_right, heighMid, fillet, filletSegs, heightSegs, true, 180.0f, 270.0f, generateMappingCoords, realWorldMapSize, UVOffsetMid, UVTilingMid, flipNormals, smooth);
                m_builder.CreateSurfaceCylinder(midOffset + TL, m_forward, m_right, heighMid, fillet, filletSegs, heightSegs, true, 270.0f, 360.0f, generateMappingCoords, realWorldMapSize, UVOffsetMid, UVTilingMid, flipNormals, smooth);
            }

            if (chamfer1)
            {
                m_builder.CreateTray(pivotOffset + m_down * (heightHalf - filletDown * 0.5f), m_forward, m_right, false, width, length, fillet, false, false, flatChamfer, widthSegs, lengthSegs, filletSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTilingDown, flipNormals, smooth);
                m_builder.CreateSurfacePlane(pivotOffset + m_down * heightHalf, m_forward, m_left, widthMid, lengthMid, widthSegs, lengthSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            }
            else
            {
                int segF = flatChamfer ? 1 : filletSegs;
                m_builder.CreateChamferPlane(pivotOffset + m_down * heightHalf, m_forward, m_left, width, length, fillet, false, widthSegs, lengthSegs, segF, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            }

            if (chamfer2)
            {
                m_builder.CreateTray(pivotOffset + m_up * (heightHalf - filletUp * 0.5f), m_forward, m_right, true, width, length, fillet, false, false, flatChamfer, widthSegs, lengthSegs, filletSegs, generateMappingCoords, realWorldMapSize, UVOffsetUp, UVTilingUp, flipNormals, smooth);
                m_builder.CreateSurfacePlane(pivotOffset + m_up * heightHalf, m_forward, m_right, widthMid, lengthMid, widthSegs, lengthSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            }
            else
            {
                int segF = flatChamfer ? 1 : filletSegs;
                m_builder.CreateChamferPlane(pivotOffset + m_up * heightHalf, m_forward, m_right, width, length, fillet, false, widthSegs, lengthSegs, segF, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            }
        }
    }
}