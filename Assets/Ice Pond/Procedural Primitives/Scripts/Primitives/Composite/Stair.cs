using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil
{
    public class Stair : PPBase
    {
        public enum StairStyle
        { StyleA, StyleB, StyleC }

        public StairStyle style = StairStyle.StyleA;
        public float width = 1.0f;
        public float length = 2.0f;
        public float height = 1.0f;
        public int steps = 5;
        public int stepWidthSegs = 2;
        public int stepLengthSegs = 2;
        public int stepHeightSegs = 2;

        protected override void CreateMesh()
        {
            m_mesh.name = "Stair";

            length = Mathf.Clamp(length, 0.00001f, 10000.0f);
            width = Mathf.Clamp(width, 0.00001f, 10000.0f);
            height = Mathf.Clamp(height, 0.00001f, 10000.0f);
            steps = Mathf.Clamp(steps, 1, 10000);
            stepWidthSegs = Mathf.Clamp(stepWidthSegs, 1, 100);
            stepLengthSegs = Mathf.Clamp(stepLengthSegs, 1, 100);
            stepHeightSegs = Mathf.Clamp(stepHeightSegs, 1, 100);

            float widthHalf = width * 0.5f;
            float lengthHalf = length * 0.5f;
            Vector3 stepUnit = new Vector3(0.0f, height / steps, length / steps);
            Vector3 stepUnitHalf = stepUnit * 0.5f;

            if (style == StairStyle.StyleA)
            {
                for (int i = 0; i < steps; ++i)
                {
                    Vector3 posOffset = m_up * stepUnit.y * (i + 0.5f) + m_forward * stepUnit.z * (i + 0.5f);
                    m_builder.CreateBox(pivotOffset + posOffset, m_forward, m_right, width, stepUnit.z, stepUnit.y, stepWidthSegs, stepLengthSegs, stepHeightSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
                }
            }
            else if (style == StairStyle.StyleB)
            {
                Vector3 tOffset = m_down * stepUnit.y;
                Vector3 tOffsetL = m_left * widthHalf + m_down * stepUnit.y;
                Vector3 tOffsetR = m_right * widthHalf + m_down * stepUnit.y;
                Vector3 botDir = (m_forward * length + m_up * height) / steps;
                float botLength = botDir.magnitude;
                botDir.Normalize();

                //first
                Vector3 tempOffset = m_up * stepUnit.y * 0.5f + m_forward * stepUnit.z * 0.5f;
                m_builder.CreateBox(pivotOffset + tempOffset, m_forward, m_right, width, stepUnit.z, stepUnit.y, stepWidthSegs, stepLengthSegs, stepHeightSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, true, true, true, true, false, true);

                //steps
                for (int i = 1; i < steps; ++i)
                {
                    Vector3 posOffset = m_up * stepUnit.y * (i + 0.5f) + m_forward * stepUnit.z * (i + 0.5f);
                    m_builder.CreateBox(pivotOffset + posOffset, m_forward, m_right, width, stepUnit.z, stepUnit.y, stepWidthSegs, stepLengthSegs, stepHeightSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, true, false, true, true, false, true);
                    m_builder.CreateSurfaceTriangle(pivotOffset + posOffset + tOffsetL, m_down, m_forward, stepUnit.z, stepUnit.y, -stepUnitHalf.z, stepHeightSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, true, true);
                    m_builder.CreateSurfaceTriangle(pivotOffset + posOffset + tOffsetR, m_down, m_back, stepUnit.z, stepUnit.y, stepUnitHalf.z, stepHeightSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, true, true);
                    m_builder.CreateSurfacePlane(pivotOffset + posOffset + tOffset, botDir, m_left, width, botLength, stepWidthSegs, stepLengthSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
                }

                //last
                Vector3 frontFacePivot = m_forward * length + m_up * (height - stepUnit.y * 0.5f);
                m_builder.CreateSurfacePlane(pivotOffset + frontFacePivot, m_up, m_left, width, stepUnit.y, stepWidthSegs, stepLengthSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            }
            else
            {
                Vector3 tOffsetL = m_left * widthHalf;
                Vector3 tOffsetR = m_right * widthHalf;
                Vector2 tilingSide = realWorldMapSize ? UVTiling : new Vector2(UVTiling.x * steps, UVTiling.y);
                Vector2 tilingBot = realWorldMapSize ? UVTiling : new Vector2(UVTiling.x, UVTiling.y * steps);
                int sSegs = stepLengthSegs * steps;

                //bottom
                m_builder.CreateSurfacePlane(pivotOffset + m_forward * lengthHalf, m_forward, m_left, width, length, stepWidthSegs, sSegs, generateMappingCoords, realWorldMapSize, UVOffset, tilingBot, flipNormals);

                //steps
                for (int i = 0; i < steps; ++i)
                {
                    float l = length - stepUnit.z * i;
                    Vector3 posOffset = m_up * stepUnit.y * (i + 0.5f) + m_forward * stepUnit.z * (i + 0.5f);
                    Vector3 posSide = m_up * stepUnit.y * (i + 0.5f) + m_forward * (length - l * 0.5f);
                    Vector3 posFront = m_up * stepUnit.y * (i + 0.5f) + m_forward * length;
                    m_builder.CreateBox(pivotOffset + posOffset, m_forward, m_right, width, stepUnit.z, stepUnit.y, stepWidthSegs, stepLengthSegs, stepHeightSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, true, false, false, false, false, true);
                    m_builder.CreateSurfacePlane(pivotOffset + posSide + tOffsetL, m_up, m_back, l, stepUnit.y, sSegs, stepHeightSegs, generateMappingCoords, realWorldMapSize, UVOffset, tilingSide, flipNormals);
                    m_builder.CreateSurfacePlane(pivotOffset + posSide + tOffsetR, m_up, m_forward, l, stepUnit.y, sSegs, stepHeightSegs, generateMappingCoords, realWorldMapSize, UVOffset, tilingSide, flipNormals);
                    m_builder.CreateSurfacePlane(pivotOffset + posFront, m_up, m_left, width, stepUnit.y, stepWidthSegs, stepLengthSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
                    if (!realWorldMapSize) tilingSide.x -= UVTiling.x;
                    sSegs -= stepLengthSegs;
                }
            }
        }
    }
}