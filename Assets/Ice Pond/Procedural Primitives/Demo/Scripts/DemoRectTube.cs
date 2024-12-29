using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil.Demo
{
    public class DemoRectTube : DemoHelper
    {
        RectTube obj;

        private void Awake()
        {
            obj = GetComponent<RectTube>();
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

        public void SetLength1(float x)
        {
            obj.length1 = x;
            Apply();
        }

        public void SetLength2(float x)
        {
            obj.length2 = x;
            Apply();
        }

        public void SetHeight(float x)
        {
            obj.height = x;
            Apply();
        }

        public void Cap1(bool isOn)
        {
            obj.cap1 = isOn;
            Apply();
        }

        public void SetCapTickness1(float x)
        {
            obj.capThickness1 = x;
            Apply();
        }

        public void Cap2(bool isOn)
        {
            obj.cap2 = isOn;
            Apply();
        }

        public void SetCapTickness2(float x)
        {
            obj.capThickness2 = x;
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

        public void SetHeightSegs(float x)
        {
            obj.heightSegs = (int)x;
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