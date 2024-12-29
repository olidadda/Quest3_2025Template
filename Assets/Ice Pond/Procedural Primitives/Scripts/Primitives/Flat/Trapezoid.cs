using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil
{
    public class Trapezoid : PPBase
    {
        public float width1 = 2;
        public float width2 = 1;
        public float length = 2;
        public float offset = 0;
        public int widthSegs = 10;
        public int lengthSegs = 10;

        protected override void CreateMesh()
        {
            m_mesh.name = "Trapezoid";

            length = Mathf.Clamp(length, 0.00001f, 10000.0f);
            width1 = Mathf.Clamp(width1, 0.00001f, 10000.0f);
            width2 = Mathf.Clamp(width2, 0.00001f, 10000.0f);
            offset = Mathf.Clamp(offset, -10000.0f, 10000.0f);
            lengthSegs = Mathf.Clamp(lengthSegs, 1, 100);
            widthSegs = Mathf.Clamp(widthSegs, 1, 100);

            m_builder.CreateSurfaceTrapezoid(pivotOffset, m_forward, m_right, width1, width2, length, offset, widthSegs, lengthSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
        }
    }
}