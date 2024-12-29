using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil
{
    public class SpiralStair : PPBase
    {
        public enum StairStyle
        { StyleA, StyleB, StyleC }

        public StairStyle style = StairStyle.StyleA;
        public float radius1 = 1.0f;
        public float radius2 = 0.5f;
        public float height = 1.0f;
        public float loops = 1.0f;
        public int steps = 10;
        public int stepWidthSegs = 2;
        public int stepLengthSegs = 5;
        public int stepHeightSegs = 2;

        protected override void CreateMesh()
        {
            m_mesh.name = "SpiralStair";

            radius1 = Mathf.Clamp(radius1, 0.00001f, 10000.0f);
            radius2 = Mathf.Clamp(radius2, 0.00001f, radius1);
            height = Mathf.Clamp(height, 0.00001f, 10000.0f);
            loops = Mathf.Clamp(loops, 0.00001f, 10000.0f);
            steps = Mathf.Clamp(steps, Mathf.CeilToInt(loops), 10000);
            stepWidthSegs = Mathf.Clamp(stepWidthSegs, 1, 100);
            stepLengthSegs = Mathf.Clamp(stepLengthSegs, 1, 100);
            stepHeightSegs = Mathf.Clamp(stepHeightSegs, 1, 100);

            float totalAngle = 360.0f * loops;
            float stepAngle = totalAngle / steps;
            float stepHeight = height / steps;

            if (style == StairStyle.StyleA)
            {
                for (int i = 0; i < steps; ++i)
                {
                    Vector3 posOffset = m_up * stepHeight * (i + 0.5f);
                    float startAngle = stepAngle * i;
                    float endAngle = startAngle + stepAngle;
                    m_builder.CreateTube(pivotOffset + posOffset, m_forward, m_right, stepHeight, radius1, radius2, true, true, stepLengthSegs, stepWidthSegs, stepHeightSegs, true, startAngle, endAngle, true, true, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth);
                }
            }
            else if (style == StairStyle.StyleB)
            {
                //start
                m_builder.CreateTube(pivotOffset + m_up * stepHeight * 0.5f, m_forward, m_right, stepHeight, radius1, radius2, true, true, stepLengthSegs, stepWidthSegs, stepHeightSegs, true, 0, stepAngle, true, false, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth);

                //steps
                for (int i = 1; i < steps; ++i)
                {
                    Vector3 posOffset = m_up * stepHeight * (i + 0.5f);
                    Vector3 posOffsetLow = m_up * stepHeight * (i - 0.5f);
                    Vector3 posOffsetBot = m_up * stepHeight * (i - 1.0f);
                    float startAngle = stepAngle * i;
                    float endAngle = startAngle + stepAngle;
                    m_builder.CreateTube(pivotOffset + posOffset, m_forward, m_right, stepHeight, radius1, radius2, true, false, stepLengthSegs, stepWidthSegs, stepHeightSegs, true, startAngle, endAngle, true, false, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth);
                    m_builder.CreateSurfaceCylinderSlanted(pivotOffset + posOffsetLow, m_forward, m_right, stepHeight, radius1, stepLengthSegs, stepHeightSegs, true, startAngle, endAngle, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth);
                    m_builder.CreateSurfaceCylinderSlanted(pivotOffset + posOffsetLow, m_forward, m_right, stepHeight, radius2, stepLengthSegs, stepHeightSegs, true, startAngle, endAngle, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, !flipNormals, smooth);
                    m_builder.CreateSurfaceRingSlanted(pivotOffset + posOffsetBot, m_forward, m_right, radius1, radius2, stepHeight, stepLengthSegs, stepWidthSegs, true, startAngle, endAngle, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, !flipNormals, true);
                }

                //last
                float radiusDif = radius1 - radius2;
                float radiusMid = (radius1 + radius2) * 0.5f;
                float startAngleL = stepAngle * (steps - 1);
                float endAngleL = startAngleL + stepAngle;
                Vector3 posOffsetL = m_up * stepHeight * (steps - 0.5f);
                Vector3 centerTo = m_right * Mathf.Sin(endAngleL * Mathf.Deg2Rad) + m_forward * Mathf.Cos(endAngleL * Mathf.Deg2Rad);
                m_builder.CreateSurfacePlane(pivotOffset + centerTo * radiusMid + posOffsetL, m_up, centerTo, radiusDif, stepHeight, stepWidthSegs, stepHeightSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            }
            else
            {
                Vector2 tilingSide = realWorldMapSize ? UVTiling : new Vector2(UVTiling.x * (steps - 1), UVTiling.y);
                int sSegs = stepLengthSegs * (steps - 1);
                float radiusDif = radius1 - radius2;
                float radiusMid = (radius1 + radius2) * 0.5f;

                //bottom
                float botAngle = Mathf.Clamp(totalAngle, 0.0f, 360.0f);
                m_builder.CreateSurfaceRing(pivotOffset, m_forward, m_right, radius1, radius2, sSegs, stepWidthSegs, true, 0.0f, botAngle, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, !flipNormals);

                //steps
                for (int i = 0; i < steps; ++i)
                {
                    float startAngle = stepAngle * i;
                    float endAngle = startAngle + stepAngle;
                    float loopAngle = Mathf.Clamp(totalAngle, endAngle, startAngle + 360.0f);
                    Vector3 posOffset = m_up * stepHeight * (i + 0.5f);
                    Vector3 centerTo = m_right * Mathf.Sin(loopAngle * Mathf.Deg2Rad) + m_forward * Mathf.Cos(loopAngle * Mathf.Deg2Rad);
                    m_builder.CreateTube(pivotOffset + posOffset, m_forward, m_right, stepHeight, radius1, radius2, true, false, stepLengthSegs, stepWidthSegs, stepHeightSegs, true, startAngle, endAngle, true, false, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth);
                    m_builder.CreateTube(pivotOffset + posOffset, m_forward, m_right, stepHeight, radius1, radius2, false, false, sSegs, stepWidthSegs, stepHeightSegs, true, endAngle, loopAngle, false, false, generateMappingCoords, realWorldMapSize, UVOffset, tilingSide, flipNormals, smooth);
                    m_builder.CreateSurfacePlane(pivotOffset + centerTo * radiusMid + posOffset, m_up, centerTo, radiusDif, stepHeight, stepWidthSegs, stepHeightSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
                    if (!realWorldMapSize) tilingSide.x -= UVTiling.x;
                    sSegs -= stepLengthSegs;
                }
            }
        }
    }
}