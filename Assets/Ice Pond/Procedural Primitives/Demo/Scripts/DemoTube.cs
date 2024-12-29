using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil.Demo
{
    public class DemoTube : DemoHelper
    {
        Tube obj;

        private void Awake()
        {
            obj = GetComponent<Tube>();
            baseObj = obj;
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

        public void SetSides(float x)
        {
            obj.sides = (int)x;
            Apply();
        }

        public void SetHeightSegs(float x)
        {
            obj.heightSegs = (int)x;
            Apply();
        }

        public void SetCaptSegs(float x)
        {
            obj.capSegs = (int)x;
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

        public void Smooth(bool isOn)
        {
            obj.smooth = isOn;
            Apply();
        }
    }
}