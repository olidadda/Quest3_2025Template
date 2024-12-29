using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil
{
    public class RectRing : PPBase
    {
        public float width = 2;
        public float length = 2;
        public float fillet = 0.5f;
        public float thickness = 0.3f;
        public int widthSegs = 5;
        public int lengthSegs = 5;
        public int filletSegs = 5;
        public int ringSegs = 3;

        protected override void CreateMesh()
        {
            m_mesh.name = "RectRing";

            length = Mathf.Clamp(length, 0.00001f, 10000.0f);
            width = Mathf.Clamp(width, 0.00001f, 10000.0f);
            float min = length < width ? length : width;
            fillet = Mathf.Clamp(fillet, 0.00001f, min / 2.0f);
            thickness = Mathf.Clamp(thickness, 0.00001f, fillet);
            lengthSegs = Mathf.Clamp(lengthSegs, 1, 100);
            widthSegs = Mathf.Clamp(widthSegs, 1, 100);
            filletSegs = Mathf.Clamp(filletSegs, 1, 100);
            ringSegs = Mathf.Clamp(ringSegs, 1, 100);

            m_builder.CreateRectRing(pivotOffset, m_forward, m_right, width, length, fillet, thickness, widthSegs, lengthSegs, filletSegs, ringSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
        }
    }
}