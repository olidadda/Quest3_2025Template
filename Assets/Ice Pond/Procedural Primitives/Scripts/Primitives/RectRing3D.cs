using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil
{
    public class RectRing3D : PPBase
    {
        public float width = 1;
        public float length = 1;
        public float fillet = 0.3f;
        public float radius = 0.1f;
        public int widthSegs = 2;
        public int lengthSegs = 2;
        public int filletSegs = 5;
        public int ringSegs = 12;

        protected override void CreateMesh()
        {
            m_mesh.name = "RectRing";

            length = Mathf.Clamp(length, 0.00001f, 10000.0f);
            width = Mathf.Clamp(width, 0.00001f, 10000.0f);
            float min = length < width ? length : width;
            fillet = Mathf.Clamp(fillet, 0.00001f, min / 2.0f);
            radius = Mathf.Clamp(radius, 0.00001f, fillet);
            lengthSegs = Mathf.Clamp(lengthSegs, 1, 100);
            widthSegs = Mathf.Clamp(widthSegs, 1, 100);
            filletSegs = Mathf.Clamp(filletSegs, 1, 100);
            ringSegs = Mathf.Clamp(ringSegs, 3, 100);

            float lengthHalf = length * 0.5f;
            float widthHalf = width * 0.5f;
            float filletHalf = fillet * 0.5f;
            float widthMid = width - fillet * 2.0f;
            float lengthMid = length - fillet * 2.0f;
            float widthMidHalf = widthMid * 0.5f;
            float lengthMidHalf = lengthMid * 0.5f;

            Vector3 TR = m_forward * lengthMidHalf + m_right * widthMidHalf;
            Vector3 BR = m_back * lengthMidHalf + m_right * widthMidHalf;
            Vector3 BL = m_back * lengthMidHalf + m_left * widthMidHalf;
            Vector3 TL = m_forward * lengthMidHalf + m_left * widthMidHalf;

            m_builder.CreateSurfaceCylinder(pivotOffset + m_forward * lengthHalf, m_back,    m_down, widthMid,  radius, ringSegs, widthSegs,  false, 0.0f, 360.0f, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth, false, true);
            m_builder.CreateSurfaceCylinder(pivotOffset + m_back * lengthHalf,    m_forward, m_down, widthMid,  radius, ringSegs, widthSegs,  false, 0.0f, 360.0f, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth, false, true);
            m_builder.CreateSurfaceCylinder(pivotOffset + m_right * widthHalf,    m_left,    m_down, lengthMid, radius, ringSegs, lengthSegs, false, 0.0f, 360.0f, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth, false, true);
            m_builder.CreateSurfaceCylinder(pivotOffset + m_left * widthHalf,     m_right,   m_down, lengthMid, radius, ringSegs, lengthSegs, false, 0.0f, 360.0f, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth, false, true);

            m_builder.CreateTorus(pivotOffset + TR, m_forward, m_right, fillet, radius, filletSegs, ringSegs, 1, true, 0.0f,   90.0f,  false, false, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth);
            m_builder.CreateTorus(pivotOffset + BR, m_forward, m_right, fillet, radius, filletSegs, ringSegs, 1, true, 90.0f,  180.0f, false, false, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth);
            m_builder.CreateTorus(pivotOffset + BL, m_forward, m_right, fillet, radius, filletSegs, ringSegs, 1, true, 180.0f, 270.0f, false, false, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth);
            m_builder.CreateTorus(pivotOffset + TL, m_forward, m_right, fillet, radius, filletSegs, ringSegs, 1, true, 270.0f, 360.0f, false, false, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth);
        }
    }
}