using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil
{
    public class Prism : PPBase
    {
        public float width = 1;
        public float length = 1;
        public float height = 1.0f;
        public float offset = 0;
        public int sideSegs = 2;
        public int heightSegs = 2;

        protected override void CreateMesh()
        {
            m_mesh.name = "Prism";

            width = Mathf.Clamp(width, 0.00001f, 10000.0f);
            length = Mathf.Clamp(length, 0.00001f, 10000.0f);
            height = Mathf.Clamp(height, 0.00001f, 10000.0f);
            offset = Mathf.Clamp(offset, -10000.0f, 10000.0f);
            sideSegs = Mathf.Clamp(sideSegs, 1, 100);
            heightSegs = Mathf.Clamp(heightSegs, 1, 100);

            m_builder.CreatePrism(pivotOffset, m_forward, m_right, width, length, height, offset, sideSegs, 1, heightSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
        }
    }
}