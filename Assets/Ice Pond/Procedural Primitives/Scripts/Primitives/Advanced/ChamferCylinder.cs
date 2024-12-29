using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil
{
    public class ChamferCylinder : PPCircularBase
    {
        public float radius = 0.5f;
        public float height = 1.0f;
        public float fillet = 0.1f;
        public bool chamfer1 = true;
        public bool chamfer2 = true;
        public bool flatChamfer = false;
        public int capSegs = 2;
        public int heightSegs = 2;
        public int filletSegs = 3;

        protected override void CreateMesh()
        {
            m_mesh.name = "ChamferCylinder";

            radius = Mathf.Clamp(radius, 0.00001f, 10000.0f);
            height = Mathf.Clamp(height, 0.00001f, 10000.0f);
            float min = radius < height / 2.0f ? radius : height / 2.0f;
            fillet = Mathf.Clamp(fillet, 0.00001f, min);
            sides = Mathf.Clamp(sides, 3, 100);
            capSegs = Mathf.Clamp(capSegs, 1, 100);
            heightSegs = Mathf.Clamp(heightSegs, 1, 100);
            filletSegs = Mathf.Clamp(filletSegs, 1, 100);
            sliceFrom = Mathf.Clamp(sliceFrom, 0.0f, 360.0f);
            sliceTo = Mathf.Clamp(sliceTo, sliceFrom, 360.0f);

            float heightHalf = height * 0.5f;
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

            Vector3 midOffset = m_up * (filletDown - filletUp) * 0.5f;
            Vector2 UVTilingMid = new Vector2(UVTiling.x, UVTiling.y * tilingMidY);
            Vector2 UVTilingDown = new Vector2(UVTiling.x, UVTiling.y * tilingDownY);
            Vector2 UVTilingUp = new Vector2(UVTiling.x, UVTiling.y * tilingUpY);
            Vector2 UVOffsetMid = UVOffset + new Vector2(0.0f, UVTiling.y * offsetMidY);
            Vector2 UVOffsetUp = UVOffset + new Vector2(0.0f, UVTiling.y * offsetUpY);

            m_builder.CreateCylinder(pivotOffset + midOffset, m_forward, m_right, heighMid, radius, false, false, sides, capSegs, heightSegs, sliceOn, sliceFrom, sliceTo, true, true, generateMappingCoords, realWorldMapSize, UVOffsetMid, UVTilingMid, flipNormals, smooth);
            
            if (chamfer1)
            {
                m_builder.CreatePlate(pivotOffset + m_down * (heightHalf - filletDown * 0.5f), m_forward, m_right, false, radius, fillet, false, false, flatChamfer, sides, filletSegs, capSegs, sliceOn, sliceFrom, sliceTo, true, true, generateMappingCoords, realWorldMapSize, UVOffset, UVTilingDown, flipNormals, smooth);
                m_builder.CreateSurfaceCircle(pivotOffset + m_down * heightHalf, m_forward, m_left, radius - fillet, sides, capSegs, sliceOn, -sliceTo, -sliceFrom, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            }
            else m_builder.CreateSurfaceCircle(pivotOffset + m_down * heightHalf, m_forward, m_left, radius, sides, capSegs, sliceOn, -sliceTo, -sliceFrom, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            
            if (chamfer2)
            {
                m_builder.CreatePlate(pivotOffset + m_up * (heightHalf - filletUp * 0.5f), m_forward, m_right, true, radius, fillet, false, false, flatChamfer, sides, filletSegs, capSegs, sliceOn, sliceFrom, sliceTo, true, true, generateMappingCoords, realWorldMapSize, UVOffsetUp, UVTilingUp, flipNormals, smooth);
                m_builder.CreateSurfaceCircle(pivotOffset + m_up * heightHalf, m_forward, m_right, radius - fillet, sides, capSegs, sliceOn, sliceFrom, sliceTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            }
            else m_builder.CreateSurfaceCircle(pivotOffset + m_up * heightHalf, m_forward, m_right, radius, sides, capSegs, sliceOn, sliceFrom, sliceTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
        }
    }
}