using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil.Demo
{
    public class DemoDoubleSphere : DemoHelper
    {
        DoubleSphere obj;

        private void Awake()
        {
            obj = GetComponent<DoubleSphere>();
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

        public void SetSegs(float x)
        {
            obj.sides = (int)x;
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

        public void SetCutType(int x)
        {
            obj.cutType = (SphereCutOption)x;
            Apply();
        }

        public void SetCutFrom(float x)
        {
            obj.cutFrom = x;
            Apply();
        }

        public void SetCutTo(float x)
        {
            obj.cutTo = x;
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