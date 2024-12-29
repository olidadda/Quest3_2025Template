using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProceduralPrimitivesUtil.Math;

namespace ProceduralPrimitivesUtil
{
    public class Polygon : PPBase
    {
        public float height = 0.1f;
        public PointOrder pointOrder = PointOrder.CounterClockwise;
        public List<Vector2> points = new List<Vector2>();

        protected override void CreateMesh()
        {
            m_mesh.name = "Polygon";

            if (points == null || points.Count < 3) return;
            height = Mathf.Clamp(height, 0.00001f, 10000.0f);
            float heightHalf = height * 0.5f;

            List<int> triangles = EarClipping.Triangulate(points, pointOrder == PointOrder.Clockwise);
            m_builder.CreateSurfacePolygon(pivotOffset + m_up * heightHalf, m_forward, m_right, points, triangles, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            m_builder.CreateSurfacePolygon(pivotOffset + m_down * heightHalf, m_forward, m_right, points, triangles, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, !flipNormals);
            m_builder.CreateSurfaceExtrusion(pivotOffset + m_down * heightHalf, m_forward, m_right, points, true, height, 1, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, pointOrder == PointOrder.CounterClockwise);
        }
    }
}