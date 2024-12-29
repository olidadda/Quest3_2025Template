using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil.Demo
{
    public class DemoPlate : DemoHelper
    {
        Plate obj;

        private void Awake()
        {
            obj = GetComponent<Plate>();
            baseObj = obj;
        }

        public void SetRadius(float x)
        {
            obj.radius = x;
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

        public void Cap(bool isOn)
        {
            obj.cap = isOn;
            Apply();
        }

        public void FlatChamfer(bool isOn)
        {
            obj.flatChamfer = isOn;
            Apply();
        }

        public void SetSides(float x)
        {
            obj.sides = (int)x;
            Apply();
        }

        public void SetCapSegs(float x)
        {
            obj.capSegs = (int)x;
            Apply();
        }

        public void SetFilletSegs(float x)
        {
            obj.filletSegs = (int)x;
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