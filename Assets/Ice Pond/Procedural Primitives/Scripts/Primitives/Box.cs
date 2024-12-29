using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil
{
    public class Box : PPBase
    {
        public float width = 1.0f;
        public float length = 1.0f;
        public float height = 1.0f;
        public int widthSegs = 2;
        public int lengthSegs = 2;
        public int heightSegs = 2;

        protected override void CreateMesh()
        {
            m_mesh.name = "Box";

            length = Mathf.Clamp(length, 0.00001f, 10000.0f);
            width = Mathf.Clamp(width, 0.00001f, 10000.0f);
            height = Mathf.Clamp(height, 0.00001f, 10000.0f);
            lengthSegs = Mathf.Clamp(lengthSegs, 1, 100);
            widthSegs = Mathf.Clamp(widthSegs, 1, 100);
            heightSegs = Mathf.Clamp(heightSegs, 1, 100);

            m_builder.CreateBox(pivotOffset, m_forward, m_right, width, length, height, widthSegs, lengthSegs, heightSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
        }
    }
}