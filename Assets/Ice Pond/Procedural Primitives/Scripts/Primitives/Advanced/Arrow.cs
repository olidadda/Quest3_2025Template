using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil
{
    public class Arrow : PPBase
    {
        public float width1 = 0.5f;
        public float width2 = 0.3f;
        public float width3 = 1.0f;
        public float length1 = 1.0f;
        public float length2 = 0.5f;
        public float height = 0.5f;
        public int widthSegs1 = 2;
        public int lengthSegs1 = 2;
        public int widthSegs2 = 4;
        public int lengthSegs2 = 2;
        public int heightSegs = 2;

        protected override void CreateMesh()
        {
            m_mesh.name = "Arrow";

            width1 = Mathf.Clamp(width1, 0.00001f, 10000.0f);
            width2 = Mathf.Clamp(width2, 0.00001f, 10000.0f);
            width3 = Mathf.Clamp(width3, width2, 10000.0f);
            length1 = Mathf.Clamp(length1, 0.00001f, 10000.0f);
            length2 = Mathf.Clamp(length2, 0.00001f, 10000.0f);
            height = Mathf.Clamp(height, 0.00001f, 10000.0f);
            lengthSegs1 = Mathf.Clamp(lengthSegs1, 1, 100);
            widthSegs1 = Mathf.Clamp(widthSegs1, 1, 100);
            lengthSegs2 = Mathf.Clamp(lengthSegs2, 1, 100);
            widthSegs2 = Mathf.Clamp(widthSegs2, 1, 100);
            heightSegs = Mathf.Clamp(heightSegs, 1, 100);

            m_builder.CreatePyramid(pivotOffset + m_forward * length1 * 0.5f, m_up, m_left, width3, height, 0.0f,   height, length2, false, true, widthSegs2, heightSegs, lengthSegs2, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            m_builder.CreatePyramid(pivotOffset + m_back * length2 * 0.5f,    m_up, m_left, width1, height, width2, height, length1, false, true, widthSegs1, heightSegs, lengthSegs1, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
        }
    }
}