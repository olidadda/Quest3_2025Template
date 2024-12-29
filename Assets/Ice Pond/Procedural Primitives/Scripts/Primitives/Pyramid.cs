using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil
{
    public class Pyramid : PPBase
    {
        public float width1 = 1.0f;
        public float length1 = 1.0f;
        public float width2 = 0.5f;
        public float length2 = 0.5f;
        public float height = 1.0f;
        public int widthSegs = 2;
        public int lengthSegs = 2;
        public int heightSegs = 4;

        protected override void CreateMesh()
        {
            m_mesh.name = "Pyramid";

            length1 = Mathf.Clamp(length1, 0.00001f, 10000.0f);
            width1 = Mathf.Clamp(width1, 0.00001f, 10000.0f);
            length2 = Mathf.Clamp(length2, 0.00001f, 10000.0f);
            width2 = Mathf.Clamp(width2, 0.00001f, 10000.0f);
            height = Mathf.Clamp(height, 0.00001f, 10000.0f);
            lengthSegs = Mathf.Clamp(lengthSegs, 1, 100);
            widthSegs = Mathf.Clamp(widthSegs, 1, 100);
            heightSegs = Mathf.Clamp(heightSegs, 1, 100);

            m_builder.CreatePyramid(pivotOffset, m_forward, m_right, width1, length1, width2, length2, height, true, true, widthSegs, lengthSegs, heightSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
        }
    }
}