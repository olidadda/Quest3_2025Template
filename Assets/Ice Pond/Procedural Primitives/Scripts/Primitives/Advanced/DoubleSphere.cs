using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil
{
    public class DoubleSphere : PPCircularBase
    {
        public float radius1 = 0.5f;
        public float radius2 = 0.3f;
        public SphereCutOption cutType = SphereCutOption.None;
        public float cutFrom = 0.0f;
        public float cutTo = 1.0f;

        protected override void CreateMesh()
        {
            m_mesh.name = "DoubleSphere";

            radius1 = Mathf.Clamp(radius1, 0.00001f, 10000.0f);
            radius2 = Mathf.Clamp(radius2, 0.00001f, radius1);
            sides = Mathf.Clamp(sides, 4, 100);
            sliceFrom = Mathf.Clamp(sliceFrom, 0.0f, 360.0f);
            sliceTo = Mathf.Clamp(sliceTo, sliceFrom, 360.0f);
            cutFrom = Mathf.Clamp01(cutFrom);
            cutTo = Mathf.Clamp(cutTo, cutFrom, 1.0f);
            int seg = Mathf.Clamp(sides / 2, 1, sides);
            int segCap = Mathf.Clamp(sides / 6, 1, sides);

            float min = (radius1 - radius2) / (radius1 * 2);
            float max = (radius1 + radius2) / (radius1 * 2);
            float cutFrom2 = Mathf.Clamp01((cutFrom - min) / (max - min));
            float cutTo2 = Mathf.Clamp01((cutTo - min) / (max - min));
            bool hemiSphere = cutType == SphereCutOption.HemiSphere;
            bool sphereSector = cutType == SphereCutOption.SphericalSector;

            m_builder.CreateSurfaceSphere(pivotOffset + Vector3.zero, m_rotation * Vector3.forward, m_rotation * Vector3.right, radius1, sides, seg, sliceOn, sliceFrom, sliceTo, hemiSphere || sphereSector, cutFrom, cutTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth);
            if (sphereSector) m_builder.CreateSurfaceSphere(pivotOffset + Vector3.zero, m_rotation * Vector3.forward, m_rotation * Vector3.right, radius2, sides, seg, sliceOn, sliceFrom, sliceTo, sphereSector, cutFrom, cutTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, !flipNormals, smooth);
            else m_builder.CreateSurfaceSphere(pivotOffset + Vector3.zero, m_rotation * Vector3.forward, m_rotation * Vector3.right, radius2, sides, seg, sliceOn, sliceFrom, sliceTo, hemiSphere, cutFrom2, cutTo2, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, !flipNormals, smooth);

            if (sliceOn)
            {
                Vector3 centerFrom = m_right * Mathf.Sin(sliceFrom * Mathf.Deg2Rad) + m_forward * Mathf.Cos(sliceFrom * Mathf.Deg2Rad);
                Vector3 centerTo = m_right * Mathf.Sin(sliceTo * Mathf.Deg2Rad) + m_forward * Mathf.Cos(sliceTo * Mathf.Deg2Rad);

                float tilingSliceY = realWorldMapSize ? Mathf.PI * 0.5f : 1.0f;
                Vector2 UVTilingSlice = new Vector2(UVTiling.x, UVTiling.y * tilingSliceY);

                m_builder.CreateSurfaceHemiRing(pivotOffset, m_up, -centerFrom, true, radius1, radius2, seg, segCap, sphereSector, hemiSphere || sphereSector, cutFrom, cutTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTilingSlice, flipNormals);
                m_builder.CreateSurfaceHemiRing(pivotOffset, m_up, centerTo, false, radius1, radius2, seg, segCap, sphereSector, hemiSphere || sphereSector, cutFrom, cutTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTilingSlice, flipNormals);
            }

            if (cutType != SphereCutOption.None)
            {
                float cf = (cutFrom - 0.5f) * 2.0f; //convert to -1 to 1
                float ct = (cutTo - 0.5f) * 2.0f; //convert to -1 to 1
                float cf2 = (cutFrom2 - 0.5f) * 2.0f; //convert to -1 to 1
                float ct2 = (cutTo2 - 0.5f) * 2.0f; //convert to -1 to 1
                float hf = cf * radius1;
                float ht = ct * radius1;
                float angleCutFrom = Mathf.PI - Mathf.Acos(cf);
                float angleCutTo = Mathf.PI - Mathf.Acos(ct);
                float angleCutFrom2 = Mathf.PI - Mathf.Acos(cf2);
                float angleCutTo2 = Mathf.PI - Mathf.Acos(ct2);
                Vector2 sincosFrom = new Vector2(Mathf.Sin(angleCutFrom), Mathf.Cos(angleCutFrom));
                Vector2 sincosTo = new Vector2(Mathf.Sin(angleCutTo), Mathf.Cos(angleCutTo));
                Vector2 sincosFrom2 = new Vector2(Mathf.Sin(angleCutFrom2), Mathf.Cos(angleCutFrom2));
                Vector2 sincosTo2 = new Vector2(Mathf.Sin(angleCutTo2), Mathf.Cos(angleCutTo2));
                float rf1 = radius1 * sincosFrom.x;
                float rt1 = radius1 * sincosTo.x;
                float rf2 = radius2 * sincosFrom2.x;
                float rt2 = radius2 * sincosTo2.x;

                if (hemiSphere)
                {
                    m_builder.CreateSurfaceRing(pivotOffset + m_up * hf, m_forward, -m_right, rf1, rf2, sides, segCap, sliceOn, -sliceTo, -sliceFrom, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
                    m_builder.CreateSurfaceRing(pivotOffset + m_up * ht, m_forward, m_right, rt1, rt2, sides, segCap, sliceOn, sliceFrom, sliceTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
                }

                if (sphereSector)
                {
                    float t = radius2 / radius1;
                    float tr = 1.0f - t;
                    Vector3 offsetF = m_up * (hf * (1.0f - tr * 0.5f));
                    Vector3 offsetT = m_up * (ht * (1.0f - tr * 0.5f));
                    if (cf > 0.0f) m_builder.CreateSurfaceCone(pivotOffset + offsetF, m_forward, m_left, hf * tr, rf1, rf1 * t, sides, segCap, sliceOn, -sliceTo, -sliceFrom, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth);
                    else m_builder.CreateSurfaceCone(pivotOffset + offsetF, m_forward, m_left, -hf * tr, rf1 * t, rf1, sides, segCap, sliceOn, -sliceTo, -sliceFrom, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, !flipNormals, smooth);
                    if (ct > 0.0f) m_builder.CreateSurfaceCone(pivotOffset + offsetT, m_forward, m_right, ht * tr, rt1 * t, rt1, sides, segCap, sliceOn, sliceFrom, sliceTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, !flipNormals, smooth);
                    else m_builder.CreateSurfaceCone(pivotOffset + offsetT, m_forward, m_right, -ht * tr, rt1, rt1 * t, sides, segCap, sliceOn, sliceFrom, sliceTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth);
                }
            }
        }
    }
}