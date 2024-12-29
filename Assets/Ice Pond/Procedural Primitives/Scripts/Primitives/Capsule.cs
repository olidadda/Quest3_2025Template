using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil
{
    public class Capsule : PPCircularBase
    {
        public float radius = 0.5f;
        public float height = 1.0f;
        public int heightSegs = 1;

        protected override void CreateMesh()
        {
            m_mesh.name = "Capsule";

            radius = Mathf.Clamp(radius, 0.00001f, 10000.0f);
            height = Mathf.Clamp(height, 0.00001f, 10000.0f);
            sides = Mathf.Clamp(sides, 4, 100);
            heightSegs = Mathf.Clamp(heightSegs, 1, 100);
            sliceFrom = Mathf.Clamp(sliceFrom, 0.0f, 360.0f);
            sliceTo = Mathf.Clamp(sliceTo, sliceFrom, 360.0f);

            float heightHalf = height * 0.5f;
            float pir = radius * Mathf.PI;
            float pirHalf = pir * 0.5f;
            float heightTotal = height + pir;
            float tilingShpereY = realWorldMapSize ? 1.0f : pir / heightTotal;
            float tilingCylinderY = realWorldMapSize ? 1.0f : height / heightTotal;
            float offsetCylinderY = realWorldMapSize ? pirHalf : pirHalf / heightTotal;
            float offsetShpereY = realWorldMapSize ? height : height / heightTotal;

            Vector2 UVTilingSphere = new Vector2(UVTiling.x, UVTiling.y * tilingShpereY);
            Vector2 UVTilingCylinder = new Vector2(UVTiling.x, UVTiling.y * tilingCylinderY);
            Vector2 UVOffsetSphere = UVOffset + new Vector2(0.0f, UVTiling.y * offsetShpereY);
            Vector2 UVOffsetCylinder = UVOffset + new Vector2(0.0f, UVTiling.y * offsetCylinderY);

            m_builder.CreateCylinder(pivotOffset, m_forward, m_right, height, radius, false, false, sides, 1, heightSegs, sliceOn, sliceFrom, sliceTo, true, true, generateMappingCoords, realWorldMapSize, UVOffsetCylinder, UVTilingCylinder, flipNormals, smooth);
            m_builder.CreateSphere(pivotOffset + m_down * heightHalf, m_forward, m_right, radius, SphereCutOption.None, sides, sides / 2, 1, sliceOn, sliceFrom, sliceTo, true, true, true, 0.0f, 0.5f, generateMappingCoords, realWorldMapSize, UVOffset, UVTilingSphere, flipNormals, smooth);
            m_builder.CreateSphere(pivotOffset + m_up * heightHalf, m_forward, m_right, radius, SphereCutOption.None, sides, sides / 2, 1, sliceOn, sliceFrom, sliceTo, true, true, true, 0.5f, 1.0f, generateMappingCoords, realWorldMapSize, UVOffsetSphere, UVTilingSphere, flipNormals, smooth);
        }
    }
}