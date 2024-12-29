using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil.Demo
{
    public class DemoTrapezoid : DemoHelper
    {
        Trapezoid obj;

        private void Awake()
        {
            obj = GetComponent<Trapezoid>();
            baseObj = obj;
        }

        public void SetWidth1(float x)
        {
            obj.width1 = x;
            Apply();
        }

        public void SetWidth2(float x)
        {
            obj.width2 = x;
            Apply();
        }

        public void SetLength(float x)
        {
            obj.length = x;
            Apply();
        }

        public void SetOffset(float x)
        {
            obj.offset = x;
            Apply();
        }

        public void SetWidthSegs(float x)
        {
            obj.widthSegs = (int)x;
            Apply();
        }

        public void SetLengthSegs(float x)
        {
            obj.lengthSegs = (int)x;
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