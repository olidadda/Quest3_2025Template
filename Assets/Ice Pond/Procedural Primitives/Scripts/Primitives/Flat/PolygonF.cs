using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProceduralPrimitivesUtil.Math;

namespace ProceduralPrimitivesUtil
{
    public class PolygonF : PPBase
    {
        public PointOrder pointOrder = PointOrder.CounterClockwise;
        public List<Vector2> points = new List<Vector2>();

        protected override void CreateMesh()
        {
            m_mesh.name = "PolygonF";

            if (points == null || points.Count < 3) return;

            List<int> triangles = EarClipping.Triangulate(points, pointOrder == PointOrder.Clockwise);
            m_builder.CreateSurfacePolygon(pivotOffset, m_forward, m_right, points, triangles, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
        }
    }
}