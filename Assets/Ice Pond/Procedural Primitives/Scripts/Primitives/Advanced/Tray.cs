using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil
{
    public class Tray : PPBase
    {
        public float width = 1.0f;
        public float length = 1.0f;
        public float fillet = 0.2f;
        public float thickness = 0.02f;
        public bool cap = false;
        public bool flatChamfer = false;
        public int widthSegs = 2;
        public int lengthSegs = 2;
        public int filletSegs = 3;

        protected override void CreateMesh()
        {
            m_mesh.name = "Tray";

            length = Mathf.Clamp(length, 0.00001f, 10000.0f);
            width = Mathf.Clamp(width, 0.00001f, 10000.0f);
            float min = length < width ? length : width;
            fillet = Mathf.Clamp(fillet, 0.00001f, min * 0.5f);
            thickness = Mathf.Clamp(thickness, 0.00001f, fillet);
            lengthSegs = Mathf.Clamp(lengthSegs, 1, 100);
            widthSegs = Mathf.Clamp(widthSegs, 1, 100);
            filletSegs = Mathf.Clamp(filletSegs, 1, 100);

            m_builder.CreateTray(pivotOffset, m_forward, m_right, false, width, length, fillet, cap, true, flatChamfer, widthSegs, lengthSegs, filletSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth);

            if (!cap)
            {
                int segF = flatChamfer ? 1 : filletSegs; 
                float thicknessDouble = thickness * 2.0f;
                m_builder.CreateTray(pivotOffset + m_up * thickness * 0.5f, m_forward, m_right, false, width - thicknessDouble, length - thicknessDouble, fillet - thickness, cap, true, flatChamfer, widthSegs, lengthSegs, filletSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, !flipNormals, smooth);
                m_builder.CreateRectRing(pivotOffset + m_up * fillet * 0.5f, m_forward, m_right, width, length, fillet, thickness, widthSegs, lengthSegs, segF, 1, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            }
        }
    }
}