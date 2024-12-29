using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil.Demo
{
    public class DemoRectTubeR : DemoHelper
    {
        RectTubeR obj;

        private void Awake()
        {
            obj = GetComponent<RectTubeR>();
            baseObj = obj;
        }

        public void SetWidth(float x)
        {
            obj.width = x;
            Apply();
        }

        public void SetLength(float x)
        {
            obj.length = x;
            Apply();
        }

        public void SetHeight(float x)
        {
            obj.height = x;
            Apply();
        }

        public void SetFillet(float x)
        {
            obj.fillet = x;
            Apply();
        }

        public void SetThickness(float x)
        {
            obj.thickness = x;
            Apply();
        }

        public void SetCap1(bool isOn)
        {
            obj.cap1 = isOn;
            Apply();
        }

        public void SetCap2(bool isOn)
        {
            obj.cap2 = isOn;
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

        public void SetFilletSegs(float x)
        {
            obj.filletSegs = (int)x;
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

        public void Smooth(bool isOn)
        {
            obj.smooth = isOn;
            Apply();
        }
    }
}