using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil
{
    public class ChamferPlane : PPBase
    {
        public float width = 2;
        public float length = 2;
        public float fillet = 0.4f;
        public int widthSegs = 10;
        public int lengthSegs = 10;
        public int filletSegs = 5;
        public bool hollow = false;

        protected override void CreateMesh()
        {
            m_mesh.name = "ChamferPlane";

            length = Mathf.Clamp(length, 0.00001f, 10000.0f);
            width = Mathf.Clamp(width, 0.00001f, 10000.0f);
            float min = length < width ? length : width;
            fillet = Mathf.Clamp(fillet, 0.00001f, min / 2.0f);
            lengthSegs = Mathf.Clamp(lengthSegs, 1, 100);
            widthSegs = Mathf.Clamp(widthSegs, 1, 100);
            filletSegs = Mathf.Clamp(filletSegs, 1, 100);

            m_builder.CreateChamferPlane(pivotOffset, m_forward, m_right, width, length, fillet, hollow, widthSegs, lengthSegs, filletSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
        }
    }
}