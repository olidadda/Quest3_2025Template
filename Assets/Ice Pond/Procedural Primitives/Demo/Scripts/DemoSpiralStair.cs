using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil.Demo
{
    public class DemoSpiralStair : DemoHelper
    {
        SpiralStair obj;

        private void Awake()
        {
            obj = GetComponent<SpiralStair>();
            baseObj = obj;
        }

        public void SetStyle(int x)
        {
            obj.style = (SpiralStair.StairStyle)x;
            Apply();
        }

        public void SetRadius1(float x)
        {
            obj.radius1 = x;
            Apply();
        }

        public void SetRadius2(float x)
        {
            obj.radius2 = x;
            Apply();
        }

        public void SetHeight(float x)
        {
            obj.height = x;
            Apply();
        }

        public void SetLoops(float x)
        {
            obj.loops = x;
            Apply();
        }

        public void SetSteps(float x)
        {
            obj.steps = (int)x;
            Apply();
        }

        public void SetWidthSegs(float x)
        {
            obj.stepWidthSegs = (int)x;
            Apply();
        }

        public void SetLengthSegs(float x)
        {
            obj.stepLengthSegs = (int)x;
            Apply();
        }

        public void SetHeightSegs(float x)
        {
            obj.stepHeightSegs = (int)x;
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