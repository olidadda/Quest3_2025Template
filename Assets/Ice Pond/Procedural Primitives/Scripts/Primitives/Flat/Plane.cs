using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil
{
    public class Plane : PPBase
    {
        public float width = 2;
        public float length = 2;
        public int widthSegs = 10;
        public int lengthSegs = 10;

        protected override void CreateMesh()
        {
            m_mesh.name = "Plane";

            length = Mathf.Clamp(length, 0.00001f, 10000.0f);
            width = Mathf.Clamp(width, 0.00001f, 10000.0f);
            lengthSegs = Mathf.Clamp(lengthSegs, 1, 100);
            widthSegs = Mathf.Clamp(widthSegs, 1, 100);

            m_builder.CreateSurfacePlane(pivotOffset, m_forward, m_right, width, length, widthSegs, lengthSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
        }
    }
}