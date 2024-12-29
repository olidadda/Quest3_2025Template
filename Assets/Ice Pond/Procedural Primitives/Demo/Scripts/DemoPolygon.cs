using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil.Demo
{
    public class DemoPolygon : DemoHelper
    {
        Polygon obj;

        private void Awake()
        {
            obj = GetComponent<Polygon>();
            baseObj = obj;
        }

        public void SetHeight(float x)
        {
            obj.height = x;
            Apply();
        }

        public void SetP0X(float x)
        {
            Vector2 p = obj.points[0];
            p.x = x;
            obj.points[0] = p;
            Apply();
        }

        public void SetP0Y(float y)
        {
            Vector2 p = obj.points[0];
            p.y = y;
            obj.points[0] = p;
            Apply();
        }

        public void SetP1X(float x)
        {
            Vector2 p = obj.points[1];
            p.x = x;
            obj.points[1] = p;
            Apply();
        }

        public void SetP1Y(float y)
        {
            Vector2 p = obj.points[1];
            p.y = y;
            obj.points[1] = p;
            Apply();
        }

        public void SetP2X(float x)
        {
            Vector2 p = obj.points[2];
            p.x = x;
            obj.points[2] = p;
            Apply();
        }

        public void SetP2Y(float y)
        {
            Vector2 p = obj.points[2];
            p.y = y;
            obj.points[2] = p;
            Apply();
        }

        public void SetP3X(float x)
        {
            Vector2 p = obj.points[3];
            p.x = x;
            obj.points[3] = p;
            Apply();
        }

        public void SetP3Y(float y)
        {
            Vector2 p = obj.points[3];
            p.y = y;
            obj.points[3] = p;
            Apply();
        }

        public void AddAPoint()
        {
            Vector2 p = new Vector2(-0.5f, 0.5f);
            obj.points.Add(p);
            Apply();
        }

        public void RemoveAPoint()
        {
            obj.points.RemoveAt(obj.points.Count - 1);
            Apply();
        }

        public void GenerateCoords(bool isOn)
        {
            obj.generateMappingCoords = isOn;
            Apply();
        }

        public void RealWorldSize(bool isOn)
        {
            obj.realWorldMapSize = isOn;
            Apply();
        }

        public void SetUVOffsetX(float x)
        {
            obj.UVOffset.x = x;
            Apply();
        }

        public void SetUVOffsetY(float x)
        {
            obj.UVOffset.y = x;
            Apply();
        }

        public void SetUVTilingX(float x)
        {
            obj.UVTiling.x = x;
            Apply();
        }

        public void SetUVTilingY(float x)
        {
            obj.UVTiling.y = x;
            Apply();
        }

        public void FlipNormal(bool isOn)
        {
            obj.flipNormals = isOn;
            Apply();
        }
    }
}