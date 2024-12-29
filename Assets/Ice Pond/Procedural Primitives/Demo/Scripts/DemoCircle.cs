using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil.Demo
{
    public class DemoCircle : DemoHelper
    {
        Circle obj;

        private void Awake()
        {
            obj = GetComponent<Circle>();
            baseObj = obj;
        }

        public void SetRadius(float x)
        {
            obj.radius = x;
            Apply();
        }

        public void SetSides(float x)
        {
            obj.sides = (int)x;
            Apply();
        }

        public void SetSegs(float x)
        {
            obj.segments = (int)x;
            Apply();
        }

        public void Slice(bool isOn)
        {
            obj.sliceOn = isOn;
            Apply();
        }

        public void SetSliceFrom(float x)
        {
            obj.sliceFrom = x;
            Apply();
        }

        public void SetSliceTo(float x)
        {
            obj.sliceTo = x;
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