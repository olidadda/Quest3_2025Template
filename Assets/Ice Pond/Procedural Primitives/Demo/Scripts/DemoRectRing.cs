using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil.Demo
{
    public class DemoRectRing : DemoHelper
    {
        RectRing obj;

        private void Awake()
        {
            obj = GetComponent<RectRing>();
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

        public void SetFilletSegs(float x)
        {
            obj.filletSegs = (int)x;
            Apply();
        }

        public void SetRingSegs(float x)
        {
            obj.ringSegs = (int)x;
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