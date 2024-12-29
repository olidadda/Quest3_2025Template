using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ProceduralPrimitivesUtil.Math;

namespace ProceduralPrimitivesUtil
{
    public class PPMeshBuilder
    {
#if PRIMITIVE_EDGES
        public struct Edge
        {
            public Vector3 a;
            public Vector3 b;
            public Edge(Vector3 p1, Vector3 p2)
            {
                a = p1; b = p2;
            }
        }

        public struct EdgeIndex
        {
            public int a;
            public int b;
            public EdgeIndex(int p1, int p2)
            {
                a = p1; b = p2;
            }

            public override int GetHashCode()
            {
                return a << 2 ^ b;
            }
        }

        public HashSet<EdgeIndex> m_indexH = new HashSet<EdgeIndex>();
        public HashSet<EdgeIndex> m_indexV = new HashSet<EdgeIndex>();
#endif

        public List<Vector3> m_vertices = new List<Vector3>();
        public List<int> m_triangles = new List<int>();
        public List<Vector3> m_normals = new List<Vector3>();
        public List<Vector2> m_uv = new List<Vector2>();

        public void Apply(Mesh mesh)
        {
            mesh.SetVertices(m_vertices);
            mesh.SetNormals(m_normals);
            mesh.SetTriangles(m_triangles, 0);
            mesh.SetUVs(0, m_uv);
        }

        public void Clear()
        {
            m_vertices.Clear();
            m_triangles.Clear();
            m_normals.Clear();
            m_uv.Clear();
#if PRIMITIVE_EDGES
            m_indexH.Clear();
            m_indexV.Clear();
#endif
        }

#if PRIMITIVE_EDGES
        public void GetEdges(ref List<Edge> horizontalEdges, ref List<Edge> verticalEdges)
        {
            horizontalEdges = new List<Edge>();
            verticalEdges = new List<Edge>();
            foreach (EdgeIndex item in m_indexH)
            {
                horizontalEdges.Add(new Edge(m_vertices[item.a], m_vertices[item.b]));
            }
            foreach (EdgeIndex item in m_indexV)
            {
                verticalEdges.Add(new Edge(m_vertices[item.a], m_vertices[item.b]));
            }
        }
#endif

        // *****************
        // Create Object
        // *****************

        public void CreateBox(Vector3 pivotOffset, Vector3 m_forward, Vector3 m_right, float width, float length, float height, int widthSegs, int lengthSegs, int heightSegs, bool generateMappingCoords, bool realWorldMapSize, Vector2 UVOffset, Vector2 UVTiling, bool flipNormals)
        {
            Vector3 m_up = Vector3.Cross(m_forward, m_right).normalized;
            float lengthHalf = length * 0.5f;
            float widthHalf = width * 0.5f;
            float heightHalf = height * 0.5f;

            CreateSurfacePlane(pivotOffset + -m_forward * lengthHalf, m_up, m_right, width, height, widthSegs, heightSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            CreateSurfacePlane(pivotOffset + m_forward * lengthHalf, m_up, -m_right, width, height, widthSegs, heightSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            CreateSurfacePlane(pivotOffset + -m_right * widthHalf, m_up, -m_forward, length, height, lengthSegs, heightSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            CreateSurfacePlane(pivotOffset + m_right * widthHalf, m_up, m_forward, length, height, lengthSegs, heightSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            CreateSurfacePlane(pivotOffset + m_up * heightHalf, m_forward, m_right, width, length, widthSegs, lengthSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            CreateSurfacePlane(pivotOffset + -m_up * heightHalf, m_forward, -m_right, width, length, widthSegs, lengthSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
        }

        public void CreateBox(Vector3 pivotOffset, Vector3 m_forward, Vector3 m_right, float width, float length, float height, int widthSegs, int lengthSegs, int heightSegs, bool generateMappingCoords, bool realWorldMapSize, Vector2 UVOffset, Vector2 UVTiling, bool flipNormals, 
                              bool up, bool down, bool left, bool right, bool front, bool back)
        {
            Vector3 m_up = Vector3.Cross(m_forward, m_right).normalized;
            float lengthHalf = length * 0.5f;
            float widthHalf = width * 0.5f;
            float heightHalf = height * 0.5f;

            if (back)  CreateSurfacePlane(pivotOffset + -m_forward * lengthHalf, m_up, m_right, width, height, widthSegs, heightSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            if (front) CreateSurfacePlane(pivotOffset + m_forward * lengthHalf, m_up, -m_right, width, height, widthSegs, heightSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            if (left)  CreateSurfacePlane(pivotOffset + -m_right * widthHalf, m_up, -m_forward, length, height, lengthSegs, heightSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            if (right) CreateSurfacePlane(pivotOffset + m_right * widthHalf, m_up, m_forward, length, height, lengthSegs, heightSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            if (up)    CreateSurfacePlane(pivotOffset + m_up * heightHalf, m_forward, m_right, width, length, widthSegs, lengthSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            if (down)  CreateSurfacePlane(pivotOffset + -m_up * heightHalf, m_forward, -m_right, width, length, widthSegs, lengthSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
        }

        public void CreateCylinder(Vector3 pivotOffset, Vector3 m_forward, Vector3 m_right, float height, float radius, bool capUp, bool capDown, int sides, int segCap, int heightSegs, bool sliceOn, float sliceFrom, float sliceTo, bool capFrom, bool capTo, bool generateMappingCoords, bool realWorldMapSize, Vector2 UVOffset, Vector2 UVTiling, bool flipNormals, bool smooth)
        {
            Vector3 m_up = Vector3.Cross(m_forward, m_right).normalized;
            float heightHalf = height * 0.5f;

            CreateSurfaceCylinder(pivotOffset, m_forward, m_right, height, radius, sides, heightSegs, sliceOn, sliceFrom, sliceTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth);
            if (capUp)   CreateSurfaceCircle(pivotOffset + m_up * heightHalf, m_forward, m_right, radius, sides, segCap, sliceOn, sliceFrom, sliceTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            if (capDown) CreateSurfaceCircle(pivotOffset + -m_up * heightHalf, m_forward, -m_right, radius, sides, segCap, sliceOn, -sliceTo, -sliceFrom, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            if (sliceOn) CreateSliceFromToPlane(pivotOffset, m_forward, m_right, radius * 0.5f, radius, height, segCap, heightSegs, sliceFrom, sliceTo, capFrom, capTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
        }

        public void CreateTube(Vector3 pivotOffset, Vector3 m_forward, Vector3 m_right, float height, float radius1, float radius2, bool capUp, bool capDown, int sides, int segCap, int heightSegs, bool sliceOn, float sliceFrom, float sliceTo, bool capFrom, bool capTo, bool generateMappingCoords, bool realWorldMapSize, Vector2 UVOffset, Vector2 UVTiling, bool flipNormals, bool smooth)
        {
            Vector3 m_up = Vector3.Cross(m_forward, m_right).normalized;
            float heightHalf = height * 0.5f;
            float radiusDif = radius1 - radius2;

            CreateSurfaceCylinder(pivotOffset, m_forward, m_right, height, radius1, sides, heightSegs, sliceOn, sliceFrom, sliceTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth);
            CreateSurfaceCylinder(pivotOffset, m_forward, m_right, height, radius2, sides, heightSegs, sliceOn, sliceFrom, sliceTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, !flipNormals, smooth);
            if (sliceOn) CreateSliceFromToPlane(pivotOffset, m_forward, m_right, radius2 + radiusDif * 0.5f, radiusDif, height, 1, heightSegs, sliceFrom, sliceTo, capFrom, capTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, true);
            if (capUp) CreateSurfaceRing(pivotOffset + m_up * heightHalf, m_forward, m_right, radius1, radius2, sides, segCap, sliceOn, sliceFrom, sliceTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            if (capDown) CreateSurfaceRing(pivotOffset + -m_up * heightHalf, m_forward, -m_right, radius1, radius2, sides, segCap, sliceOn, -sliceTo, -sliceFrom, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
        }

        public void CreateCone(Vector3 pivotOffset, Vector3 m_forward, Vector3 m_right, float height, float radius1, float radius2, bool capUp, bool capDown, int sides, int segCap, int heightSegs, bool sliceOn, float sliceFrom, float sliceTo, bool capFrom, bool capTo, bool generateMappingCoords, bool realWorldMapSize, Vector2 UVOffset, Vector2 UVTiling, bool flipNormals, bool smooth)
        {
            Vector3 m_up = Vector3.Cross(m_forward, m_right).normalized;
            float heightHalf = height * 0.5f;

            CreateSurfaceCone(pivotOffset, m_forward, m_right, height, radius1, radius2, sides, heightSegs, sliceOn, sliceFrom, sliceTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth);
            if (capUp)   CreateSurfaceCircle(pivotOffset + m_up * heightHalf, m_forward, m_right, radius2, sides, segCap, sliceOn, sliceFrom, sliceTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            if (capDown) CreateSurfaceCircle(pivotOffset + -m_up * heightHalf, m_forward, -m_right, radius1, sides, segCap, sliceOn, -sliceTo, -sliceFrom, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);

            if (sliceOn)
            {
                Vector3 centerFrom = m_right * Mathf.Sin(sliceFrom * Mathf.Deg2Rad) + m_forward * Mathf.Cos(sliceFrom * Mathf.Deg2Rad);
                Vector3 centerTo = m_right * Mathf.Sin(sliceTo * Mathf.Deg2Rad) + m_forward * Mathf.Cos(sliceTo * Mathf.Deg2Rad);

                float offset = (radius1 - radius2) * 0.5f;
                float tilingSliceX = realWorldMapSize ? 1.0f : 0.5f;
                float offsetSliceX = realWorldMapSize ? (radius1 + radius2) * 0.5f : 0.5f;

                Vector2 UVTilingSlice = new Vector2(UVTiling.x * tilingSliceX, UVTiling.y);
                Vector2 UVOffsetSlice = UVOffset + new Vector2(UVTiling.x * offsetSliceX, 0.0f);

                if (capFrom) CreateSurfaceTrapezoid(pivotOffset + centerFrom * radius1 * 0.5f, m_up, -centerFrom, radius1, radius2, height, offset, segCap, heightSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTilingSlice, flipNormals);
                if (capTo)   CreateSurfaceTrapezoid(pivotOffset + centerTo * radius1 * 0.5f, m_up, centerTo, radius1, radius2, height, -offset, segCap, heightSegs, generateMappingCoords, realWorldMapSize, UVOffsetSlice, UVTilingSlice, flipNormals);
            }
        }

        public void CreateSphere(Vector3 pivotOffset, Vector3 m_forward, Vector3 m_right, float radius, SphereCutOption cutType, int sides, int seg, int segCap, bool sliceOn, float sliceFrom, float sliceTo, bool capFrom, bool capTo, bool cutOn, float cutFrom, float cutTo, bool generateMappingCoords, bool realWorldMapSize, Vector2 UVOffset, Vector2 UVTiling, bool flipNormals, bool smooth)
        {
            Vector3 m_up = Vector3.Cross(m_forward, m_right).normalized;

            CreateSurfaceSphere(pivotOffset, m_forward, m_right, radius, sides, seg, sliceOn, sliceFrom, sliceTo, cutOn, cutFrom, cutTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth);

            if (sliceOn)
            {
                Vector3 centerFrom = m_right * Mathf.Sin(sliceFrom * Mathf.Deg2Rad) + m_forward * Mathf.Cos(sliceFrom * Mathf.Deg2Rad);
                Vector3 centerTo = m_right * Mathf.Sin(sliceTo * Mathf.Deg2Rad) + m_forward * Mathf.Cos(sliceTo * Mathf.Deg2Rad);

                float tilingSliceY = realWorldMapSize ? Mathf.PI * 0.5f : 1.0f;
                Vector2 UVTilingSlice = new Vector2(UVTiling.x, UVTiling.y * tilingSliceY);

                if (capFrom) CreateSurfaceHemiCircle(pivotOffset, m_up, -centerFrom, true, radius, seg, segCap, cutType == SphereCutOption.SphericalSector, cutOn, cutFrom, cutTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTilingSlice, flipNormals);
                if (capTo)   CreateSurfaceHemiCircle(pivotOffset, m_up, centerTo, false, radius, seg, segCap, cutType == SphereCutOption.SphericalSector, cutOn, cutFrom, cutTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTilingSlice, flipNormals);
            }

            if (cutType != SphereCutOption.None)
            {
                float cf = (cutFrom - 0.5f) * 2.0f; //convert to -1 to 1
                float ct = (cutTo - 0.5f) * 2.0f; //convert to -1 to 1
                float hf = cf * radius;
                float ht = ct * radius;
                float angleCutFrom = Mathf.PI - Mathf.Acos(cf);
                float angleCutTo = Mathf.PI - Mathf.Acos(ct);
                Vector2 sincosFrom = new Vector2(Mathf.Sin(angleCutFrom), Mathf.Cos(angleCutFrom));
                Vector2 sincosTo = new Vector2(Mathf.Sin(angleCutTo), Mathf.Cos(angleCutTo));
                float rf = radius * sincosFrom.x;
                float rt = radius * sincosTo.x;

                if (cutType == SphereCutOption.HemiSphere)
                {
                    CreateSurfaceCircle(pivotOffset + m_up * hf, m_forward, -m_right, rf, sides, segCap, sliceOn, -sliceTo, -sliceFrom, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
                    CreateSurfaceCircle(pivotOffset + m_up * ht, m_forward, m_right, rt, sides, segCap, sliceOn, sliceFrom, sliceTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
                }

                if (cutType == SphereCutOption.SphericalSector)
                {
                    Vector3 offsetF = m_up * (hf * 0.5f);
                    Vector3 offsetT = m_up * (ht * 0.5f);
                    if (cf > 0.0f) CreateSurfaceCone(pivotOffset + offsetF, m_forward, -m_right, hf, rf, 0.0f, sides, segCap, sliceOn, -sliceTo, -sliceFrom, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth);
                    else CreateSurfaceCone(pivotOffset + offsetF, m_forward, -m_right, -hf, 0.0f, rf, sides, segCap, sliceOn, -sliceTo, -sliceFrom, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, !flipNormals, smooth);
                    if (ct > 0.0f) CreateSurfaceCone(pivotOffset + offsetT, m_forward, m_right, ht, 0.0f, rt, sides, segCap, sliceOn, sliceFrom, sliceTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, !flipNormals, smooth);
                    else CreateSurfaceCone(pivotOffset + offsetT, m_forward, m_right, -ht, rt, 0.0f, sides, segCap, sliceOn, sliceFrom, sliceTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth);
                }
            }
        }

        public void CreatePrism(Vector3 pivotOffset, Vector3 m_forward, Vector3 m_right, float width, float length, float height, float offset, int widthSegs, int lengthSegs, int heightSegs, bool generateMappingCoords, bool realWorldMapSize, Vector2 UVOffset, Vector2 UVTiling, bool flipNormals, bool uniform = true)
        {
            Vector3 m_up = Vector3.Cross(m_forward, m_right).normalized;
            float lengthHalf = length * 0.5f;
            float widthHalf = width * 0.5f;
            float heightHalf = height * 0.5f;

            if (uniform)
            {
                CreateSurfaceTriangle(pivotOffset + m_up * heightHalf, m_forward, m_right, width, length, offset, widthSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
                CreateSurfaceTriangle(pivotOffset + -m_up * heightHalf, m_forward, -m_right, width, length, -offset, widthSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            }
            else
            {
                CreateSurfaceTriangle(pivotOffset + m_up * heightHalf, m_forward, m_right, width, length, offset, widthSegs, lengthSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
                CreateSurfaceTriangle(pivotOffset + -m_up * heightHalf, m_forward, -m_right, width, length, -offset, widthSegs, lengthSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            }

            float c = 0.0f;
            if (offset < -widthHalf) c = (offset + widthHalf) * 0.5f;
            else if (offset > widthHalf) c = (offset - widthHalf) * 0.5f;
            Vector3 pc = -m_right * c + -m_forward * lengthHalf;
            Vector3 p0 = -m_right * (widthHalf + c) + -m_forward * lengthHalf;
            Vector3 p1 = m_right * (offset - c) + m_forward * lengthHalf;
            Vector3 p2 = m_right * (widthHalf - c) + -m_forward * lengthHalf;
            Vector3 vLeft = p0 - p1;
            Vector3 vRight = p1 - p2;
            CreateSurfacePlane(pivotOffset + pc, m_up, m_right, width, height, widthSegs, heightSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            CreateSurfacePlane(pivotOffset + (p0 + p1) * 0.5f, m_up, vLeft.normalized, vLeft.magnitude, height, widthSegs, heightSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            CreateSurfacePlane(pivotOffset + (p1 + p2) * 0.5f, m_up, vRight.normalized, vRight.magnitude, height, widthSegs, heightSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
        }

        public void CreatePyramid(Vector3 pivotOffset, Vector3 m_forward, Vector3 m_right, float width1, float length1, float width2, float length2, float height, bool capUp, bool capDown, int widthSegs, int lengthSegs, int heightSegs, bool generateMappingCoords, bool realWorldMapSize, Vector2 UVOffset, Vector2 UVTiling, bool flipNormals)
        {
            Vector3 m_up = Vector3.Cross(m_forward, m_right).normalized;
            float widthHalf1 = width1 * 0.5f;
            float lengthHalf1 = length1 * 0.5f;
            float widthHalf2 = width2 * 0.5f;
            float lengthHalf2 = length2 * 0.5f;
            float heightHalf = height * 0.5f;

            Vector3 upCenter = pivotOffset + m_up * heightHalf;
            Vector3 downCenter = pivotOffset + -m_up * heightHalf;

            Vector3 frontMid1 = downCenter + m_forward * lengthHalf1;
            Vector3 backMid1 = downCenter + -m_forward * lengthHalf1;
            Vector3 rightMid1 = downCenter + m_right * widthHalf1;
            Vector3 leftMid1 = downCenter + -m_right * widthHalf1;

            Vector3 frontMid2 = upCenter + m_forward * lengthHalf2;
            Vector3 backMid2 = upCenter + -m_forward * lengthHalf2;
            Vector3 rightMid2 = upCenter + m_right * widthHalf2;
            Vector3 leftMid2 = upCenter + -m_right * widthHalf2;

            Vector3 frontDir = frontMid2 - frontMid1;
            Vector3 backDir = backMid2 - backMid1;
            Vector3 rightDir = rightMid2 - rightMid1;
            Vector3 leftDir = leftMid2 - leftMid1;

            CreateSurfaceTrapezoid((frontMid1 + frontMid2) * 0.5f, frontDir.normalized, -m_right, width1, width2, frontDir.magnitude, 0.0f, widthSegs, heightSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            CreateSurfaceTrapezoid((backMid1 + backMid2) * 0.5f, backDir.normalized, m_right, width1, width2, backDir.magnitude, 0.0f, widthSegs, heightSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            CreateSurfaceTrapezoid((rightMid1 + rightMid2) * 0.5f, rightDir.normalized, m_forward, length1, length2, rightDir.magnitude, 0.0f, lengthSegs, heightSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            CreateSurfaceTrapezoid((leftMid1 + leftMid2) * 0.5f, leftDir.normalized, -m_forward, length1, length2, leftDir.magnitude, 0.0f, lengthSegs, heightSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);

            if (capUp) CreateSurfacePlane(pivotOffset + m_up * heightHalf, m_forward, m_right, width2, length2, widthSegs, lengthSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            if (capDown) CreateSurfacePlane(pivotOffset + -m_up * heightHalf, m_forward, -m_right, width1, length1, widthSegs, lengthSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
        }

        public void CreateTorus(Vector3 pivotOffset, Vector3 m_forward, Vector3 m_right, float radius1, float radius2, int sides, int seg, int segCap, bool sliceOn, float sliceFrom, float sliceTo, bool capFrom, bool capTo, bool generateMappingCoords, bool realWorldMapSize, Vector2 UVOffset, Vector2 UVTiling, bool flipNormals, bool smooth)
        {
            CreateSurfaceTorus(pivotOffset, m_forward, m_right, radius1, radius2, sides, seg, sliceOn, sliceFrom, sliceTo, 0.0f, 360.0f, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth);
            if (sliceOn) CreateSliceFromToCircle(pivotOffset, m_forward, m_right, radius1, radius2, seg, segCap, false, sliceFrom, sliceTo, capFrom, capTo, 0.0f, 360.0f, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, true);
        }

        public void CreateChamferPlane(Vector3 pivotOffset, Vector3 m_forward, Vector3 m_right, float width, float length, float fillet, bool hollow, int widthSegs, int lengthSegs, int filletSegs, bool generateMappingCoords, bool realWorldMapSize, Vector2 UVOffset, Vector2 UVTiling, bool flipNormals)
        {
            float lengthHalf = length * 0.5f;
            float widthHalf = width * 0.5f;
            float centerLengthHalf = lengthHalf - fillet;
            float centerWidthHalf = widthHalf - fillet;
            float centerLength = centerLengthHalf * 2.0f;
            float centerWidth = centerWidthHalf * 2.0f;

            CreateSurfaceCross(pivotOffset, m_forward, m_right, width, length, fillet, hollow, widthSegs, lengthSegs, filletSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);

            //corner
            float tilingCircleX = realWorldMapSize ? 1.0f : fillet * 2.0f / width;
            float tilingCircleY = realWorldMapSize ? 1.0f : fillet * 2.0f / length;
            float offsetCenterX = realWorldMapSize ? centerWidth : centerWidth / width;
            float offsetCenterY = realWorldMapSize ? centerLength : centerLength / length;

            Vector2 UVTilingCircle = new Vector2(UVTiling.x * tilingCircleX, UVTiling.y * tilingCircleY);
            Vector2 UVOffsetTL = UVOffset + new Vector2(0.0f, UVTiling.y * offsetCenterY);
            Vector2 UVOffsetBR = UVOffset + new Vector2(UVTiling.x * offsetCenterX, 0.0f);
            Vector2 UVOffsetTR = UVOffset + new Vector2(UVTiling.x * offsetCenterX, UVTiling.y * offsetCenterY);

            CreateSurfaceCircle(pivotOffset + m_right * centerWidthHalf + m_forward * centerLengthHalf, m_forward, m_right, fillet, filletSegs, filletSegs, true, 0.0f, 90.0f, generateMappingCoords, realWorldMapSize, UVOffsetTR, UVTilingCircle, flipNormals);
            CreateSurfaceCircle(pivotOffset + m_right * centerWidthHalf + -m_forward * centerLengthHalf, m_forward, m_right, fillet, filletSegs, filletSegs, true, 90.0f, 180.0f, generateMappingCoords, realWorldMapSize, UVOffsetBR, UVTilingCircle, flipNormals);
            CreateSurfaceCircle(pivotOffset + -m_right * centerWidthHalf + -m_forward * centerLengthHalf, m_forward, m_right, fillet, filletSegs, filletSegs, true, 180.0f, 270.0f, generateMappingCoords, realWorldMapSize, UVOffset, UVTilingCircle, flipNormals);
            CreateSurfaceCircle(pivotOffset + -m_right * centerWidthHalf + m_forward * centerLengthHalf, m_forward, m_right, fillet, filletSegs, filletSegs, true, 270.0f, 360.0f, generateMappingCoords, realWorldMapSize, UVOffsetTL, UVTilingCircle, flipNormals);
        }

        public void CreateRectRing(Vector3 pivotOffset, Vector3 m_forward, Vector3 m_right, float width, float length, float fillet, float thickness, int widthSegs, int lengthSegs, int filletSegs, int radiusSegs, bool generateMappingCoords, bool realWorldMapSize, Vector2 UVOffset, Vector2 UVTiling, bool flipNormals)
        {
            float lengthHalf = length * 0.5f;
            float widthHalf = width * 0.5f;
            float centerLengthHalf = lengthHalf - fillet;
            float centerWidthHalf = widthHalf - fillet;
            float centerLength = centerLengthHalf * 2.0f;
            float centerWidth = centerWidthHalf * 2.0f;

            CreateSurfaceCross(pivotOffset, m_forward, m_right, width, length, fillet, thickness, widthSegs, lengthSegs, radiusSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);

            //corner
            float tilingCircleX = realWorldMapSize ? 1.0f : fillet * 2.0f / width;
            float tilingCircleY = realWorldMapSize ? 1.0f : fillet * 2.0f / length;
            float offsetCenterX = realWorldMapSize ? centerWidth : centerWidth / width;
            float offsetCenterY = realWorldMapSize ? centerLength : centerLength / length;

            Vector2 UVTilingCircle = new Vector2(UVTiling.x * tilingCircleX, UVTiling.y * tilingCircleY);
            Vector2 UVOffsetTL = UVOffset + new Vector2(0.0f, UVTiling.y * offsetCenterY);
            Vector2 UVOffsetBR = UVOffset + new Vector2(UVTiling.x * offsetCenterX, 0.0f);
            Vector2 UVOffsetTR = UVOffset + new Vector2(UVTiling.x * offsetCenterX, UVTiling.y * offsetCenterY);

            float innerRadius = fillet - thickness;
            CreateSurfaceRing(pivotOffset + m_right * centerWidthHalf + m_forward * centerLengthHalf, m_forward, m_right, fillet, innerRadius, filletSegs, radiusSegs, true, 0.0f, 90.0f, generateMappingCoords, realWorldMapSize, UVOffsetTR, UVTilingCircle, flipNormals);
            CreateSurfaceRing(pivotOffset + m_right * centerWidthHalf + -m_forward * centerLengthHalf, m_forward, m_right, fillet, innerRadius, filletSegs, radiusSegs, true, 90.0f, 180.0f, generateMappingCoords, realWorldMapSize, UVOffsetBR, UVTilingCircle, flipNormals);
            CreateSurfaceRing(pivotOffset + -m_right * centerWidthHalf + -m_forward * centerLengthHalf, m_forward, m_right, fillet, innerRadius, filletSegs, radiusSegs, true, 180.0f, 270.0f, generateMappingCoords, realWorldMapSize, UVOffset, UVTilingCircle, flipNormals);
            CreateSurfaceRing(pivotOffset + -m_right * centerWidthHalf + m_forward * centerLengthHalf, m_forward, m_right, fillet, innerRadius, filletSegs, radiusSegs, true, 270.0f, 360.0f, generateMappingCoords, realWorldMapSize, UVOffsetTL, UVTilingCircle, flipNormals);
        }

        public void CreateSliceFromToPlane(Vector3 pivotOffset, Vector3 m_forward, Vector3 m_right, float radius, float width, float height, int radiusSegs, int heightSegs, float sliceFrom, float sliceTo, bool capFrom, bool capTo, bool generateMappingCoords, bool realWorldMapSize, Vector2 UVOffset, Vector2 UVTiling, bool flipNormals, bool separateUV = false)
        {
            Vector3 m_up = Vector3.Cross(m_forward, m_right).normalized;

            if (separateUV)
            {
                if (capFrom)
                {
                    Vector3 centerFrom = m_right * Mathf.Sin(sliceFrom * Mathf.Deg2Rad) + m_forward * Mathf.Cos(sliceFrom * Mathf.Deg2Rad);
                    CreateSurfacePlane(pivotOffset + centerFrom * radius, m_up, -centerFrom, width, height, radiusSegs, heightSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
                }
                if (capTo)
                {
                    Vector3 centerTo = m_right * Mathf.Sin(sliceTo * Mathf.Deg2Rad) + m_forward * Mathf.Cos(sliceTo * Mathf.Deg2Rad);
                    CreateSurfacePlane(pivotOffset + centerTo * radius, m_up, centerTo, width, height, radiusSegs, heightSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
                }
            }
            else
            {
                float tilingSliceX = realWorldMapSize ? 1.0f : width / (radius * 2 + width);
                Vector2 UVTilingSlice = new Vector2(UVTiling.x * tilingSliceX, UVTiling.y);
                if (capFrom)
                {
                    Vector3 centerFrom = m_right * Mathf.Sin(sliceFrom * Mathf.Deg2Rad) + m_forward * Mathf.Cos(sliceFrom * Mathf.Deg2Rad);
                    CreateSurfacePlane(pivotOffset + centerFrom * radius, m_up, -centerFrom, width, height, radiusSegs, heightSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTilingSlice, flipNormals);
                }
                if (capTo)
                {
                    Vector3 centerTo = m_right * Mathf.Sin(sliceTo * Mathf.Deg2Rad) + m_forward * Mathf.Cos(sliceTo * Mathf.Deg2Rad);
                    float offsetSliceToX = realWorldMapSize ? radius * 2.0f : (radius * 2.0f) / (radius * 2.0f + width);
                    Vector2 UVOffsetSliceTo = UVOffset + new Vector2(UVTiling.x * offsetSliceToX, 0.0f);
                    CreateSurfacePlane(pivotOffset + centerTo * radius, m_up, centerTo, width, height, radiusSegs, heightSegs, generateMappingCoords, realWorldMapSize, UVOffsetSliceTo, UVTilingSlice, flipNormals);
                }
            }
        }

        public void CreateSliceFromToCircle(Vector3 pivotOffset, Vector3 m_forward, Vector3 m_right, float radius1, float radius2, int sides, int segCap, bool sliceOn, float sliceFrom, float sliceTo, bool capFrom, bool capTo, float sectionSliceFrom, float sectionSliceTo, bool generateMappingCoords, bool realWorldMapSize, Vector2 UVOffset, Vector2 UVTiling, bool flipNormals, bool towardCenter = false)
        {
            Vector3 m_up = Vector3.Cross(m_forward, m_right).normalized;

            if (towardCenter)
            {
                if (capFrom)
                {
                    Vector3 centerFrom = m_right * Mathf.Sin(sliceFrom * Mathf.Deg2Rad) + m_forward * Mathf.Cos(sliceFrom * Mathf.Deg2Rad);
                    CreateSurfaceCircle(pivotOffset + centerFrom * radius1, -centerFrom, -m_up, radius2, sides, segCap, sliceOn, sectionSliceFrom, sectionSliceTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, false, true);
                }
                if (capTo)
                {
                    Vector3 centerTo = m_right * Mathf.Sin(sliceTo * Mathf.Deg2Rad) + m_forward * Mathf.Cos(sliceTo * Mathf.Deg2Rad);
                    CreateSurfaceCircle(pivotOffset + centerTo * radius1, -centerTo, m_up, radius2, sides, segCap, sliceOn, sectionSliceFrom, sectionSliceTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, false, true);
                }
            }
            else
            {
                if (capFrom)
                {
                    Vector3 centerFrom = m_right * Mathf.Sin(sliceFrom * Mathf.Deg2Rad) + m_forward * Mathf.Cos(sliceFrom * Mathf.Deg2Rad);
                    CreateSurfaceCircle(pivotOffset + centerFrom * radius1, m_up, -centerFrom, radius2, sides, segCap, sliceOn, sectionSliceFrom, sectionSliceTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
                }
                if (capTo)
                {
                    Vector3 centerTo = m_right * Mathf.Sin(sliceTo * Mathf.Deg2Rad) + m_forward * Mathf.Cos(sliceTo * Mathf.Deg2Rad);

                    float offsetCircleToX = realWorldMapSize ? radius1 * 2.0f : radius1 / radius2;
                    Vector2 UVOffsetCircleTo = UVOffset + new Vector2(UVTiling.x * offsetCircleToX, 0.0f);

                    CreateSurfaceCircle(pivotOffset + centerTo * radius1, m_up, centerTo, radius2, sides, segCap, sliceOn, -sectionSliceTo, -sectionSliceFrom, generateMappingCoords, realWorldMapSize, UVOffsetCircleTo, UVTiling, flipNormals);
                }
            }
        }

        public void CreateSliceFromToRing(Vector3 pivotOffset, Vector3 m_forward, Vector3 m_right, float radius1, float radius2, float innerRatio, int sides, int segCap, bool sliceOn, float sliceFrom, float sliceTo, bool capFrom, bool capTo, float sectionSliceFrom, float sectionSliceTo, bool generateMappingCoords, bool realWorldMapSize, Vector2 UVOffset, Vector2 UVTiling, bool flipNormals, bool towardCenter = false)
        {
            Vector3 m_up = Vector3.Cross(m_forward, m_right).normalized;
            float innerRadius = radius2 * innerRatio;

            if (towardCenter)
            {
                if (capFrom)
                {
                    Vector3 centerFrom = m_right * Mathf.Sin(sliceFrom * Mathf.Deg2Rad) + m_forward * Mathf.Cos(sliceFrom * Mathf.Deg2Rad);
                    CreateSurfaceRing(pivotOffset + centerFrom * radius1, -centerFrom, -m_up, radius2, innerRadius, sides, segCap, sliceOn, sectionSliceFrom, sectionSliceTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, false, true);
                }
                if (capTo)
                {
                    Vector3 centerTo = m_right * Mathf.Sin(sliceTo * Mathf.Deg2Rad) + m_forward * Mathf.Cos(sliceTo * Mathf.Deg2Rad);
                    CreateSurfaceRing(pivotOffset + centerTo * radius1, -centerTo, m_up, radius2, innerRadius, sides, segCap, sliceOn, sectionSliceFrom, sectionSliceTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, false, true);
                }
            }
            else
            {
                if (capFrom)
                {
                    Vector3 centerFrom = m_right * Mathf.Sin(sliceFrom * Mathf.Deg2Rad) + m_forward * Mathf.Cos(sliceFrom * Mathf.Deg2Rad);
                    CreateSurfaceRing(pivotOffset + centerFrom * radius1, m_up, -centerFrom, radius2, innerRadius, sides, segCap, sliceOn, sectionSliceFrom, sectionSliceTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
                }
                if (capTo)
                {
                    Vector3 centerTo = m_right * Mathf.Sin(sliceTo * Mathf.Deg2Rad) + m_forward * Mathf.Cos(sliceTo * Mathf.Deg2Rad);

                    float offsetCircleToX = realWorldMapSize ? radius1 * 2.0f : radius1 / radius2;
                    Vector2 UVOffsetCircleTo = UVOffset + new Vector2(UVTiling.x * offsetCircleToX, 0.0f);

                    CreateSurfaceRing(pivotOffset + centerTo * radius1, m_up, centerTo, radius2, innerRadius, sides, segCap, sliceOn, -sectionSliceTo, -sectionSliceFrom, generateMappingCoords, realWorldMapSize, UVOffsetCircleTo, UVTiling, flipNormals);
                }
            }
        }

        public void CreatePlate(Vector3 pivotOffset, Vector3 m_forward, Vector3 m_right, bool isUp, float radius, float fillet, bool cap, bool capBottom, bool flatChamfer, int sides, int seg, int segCap, bool sliceOn, float sliceFrom, float sliceTo, bool capFrom, bool capTo, bool generateMappingCoords, bool realWorldMapSize, Vector2 UVOffset, Vector2 UVTiling, bool flipNormals, bool smooth)
        {
            Vector3 m_up = Vector3.Cross(m_forward, m_right).normalized;
            float filletHalf = fillet * 0.5f;

            if (isUp)
            {
                if (flatChamfer) CreateSurfaceCone(pivotOffset, m_forward, m_right, fillet, radius, radius - fillet, sides, seg, sliceOn, sliceFrom, sliceTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth);
                else CreateSurfaceTorus(pivotOffset + -m_up * filletHalf, m_forward, m_right, radius - fillet, fillet, sides, seg, sliceOn, sliceFrom, sliceTo, 180.0f, 270.0f, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth);
                if (cap) CreateSurfaceCircle(pivotOffset + -m_up * filletHalf, m_forward, -m_right, radius, sides, segCap, sliceOn, -sliceTo, -sliceFrom, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
                if (capBottom) CreateSurfaceCircle(pivotOffset + m_up * filletHalf, m_forward, m_right, radius - fillet, sides, segCap, sliceOn, sliceFrom, sliceTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            }
            else
            {
                if (flatChamfer) CreateSurfaceCone(pivotOffset, m_forward, m_right, fillet, radius - fillet, radius, sides, seg, sliceOn, sliceFrom, sliceTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth);
                else CreateSurfaceTorus(pivotOffset + m_up * filletHalf, m_forward, m_right, radius - fillet, fillet, sides, seg, sliceOn, sliceFrom, sliceTo, 90.0f, 180.0f, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth);
                if (cap) CreateSurfaceCircle(pivotOffset + m_up * filletHalf, m_forward, m_right, radius, sides, segCap, sliceOn, sliceFrom, sliceTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
                if (capBottom) CreateSurfaceCircle(pivotOffset + -m_up * filletHalf, m_forward, -m_right, radius - fillet, sides, segCap, sliceOn, -sliceTo, -sliceFrom, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            }

            if (sliceOn && (capFrom || capTo))
            {
                int segSlice = flatChamfer ? 1 : seg;
                float planeWidth = radius - fillet;
                float tilingSliceX = realWorldMapSize ? 1.0f : planeWidth / radius;
                float tilingSliceY = realWorldMapSize ? Mathf.PI * 0.5f : 1.0f;
                float tilingCircleX = realWorldMapSize ? 1.0f : fillet / radius;
                float tilingCircleY = realWorldMapSize ? Mathf.PI * 0.5f : 2.0f;
                float offsetSliceX = realWorldMapSize ? fillet : fillet / (radius * 2);

                Vector2 UVTilingSlice = new Vector2(UVTiling.x * tilingSliceX, UVTiling.y * tilingSliceY);
                Vector2 UVTilingCircle = new Vector2(UVTiling.x * tilingCircleX, UVTiling.y * tilingCircleY);
                Vector2 UVOffsetSlice = UVOffset + new Vector2(UVTiling.x * offsetSliceX, 0.0f);

                CreateSliceFromToPlane(pivotOffset, m_forward, m_right, planeWidth * 0.5f, planeWidth, fillet, segCap, 1, sliceFrom, sliceTo, capFrom, capTo, generateMappingCoords, realWorldMapSize, UVOffsetSlice, UVTilingSlice, flipNormals);
                if (isUp) 
                {
                    float offsetCircleY = realWorldMapSize ? -fillet * Mathf.PI * 0.5f : -1.0f;
                    Vector2 UVOffsetCircle = UVOffset + new Vector2(0.0f, UVTiling.y * offsetCircleY);
                    CreateSliceFromToCircle(pivotOffset + -m_up * filletHalf, m_forward, m_right, radius - fillet, fillet, segSlice, 1, true, sliceFrom, sliceTo, capFrom, capTo, 270.0f, 360.0f, generateMappingCoords, realWorldMapSize, UVOffsetCircle, UVTilingCircle, flipNormals); 
                }
                else CreateSliceFromToCircle(pivotOffset + m_up * filletHalf, m_forward, m_right, radius - fillet, fillet, segSlice, 1, true, sliceFrom, sliceTo, capFrom, capTo, 180.0f, 270.0f, generateMappingCoords, realWorldMapSize, UVOffset, UVTilingCircle, flipNormals);
            }
        }

        public void CreateTray(Vector3 pivotOffset, Vector3 m_forward, Vector3 m_right, bool isUp, float width, float length, float fillet, bool cap, bool capBottom, bool flatChamfer, int widthSegs, int lengthSegs, int filletSegs, bool generateMappingCoords, bool realWorldMapSize, Vector2 UVOffset, Vector2 UVTiling, bool flipNormals, bool smooth)
        {
            Vector3 m_up = Vector3.Cross(m_forward, m_right).normalized;
            float filletHalf = fillet * 0.5f;
            Vector3 downCenter = pivotOffset + -m_up * filletHalf;
            Vector3 upCenter = pivotOffset + m_up * filletHalf;

            float width1 = isUp ? width : width - fillet * 2;
            float width2 = isUp ? width - fillet * 2 : width;
            float length1 = isUp ? length : length - fillet * 2;
            float length2 = isUp ? length - fillet * 2 : length;
            float widthHalf1 = width1 * 0.5f;
            float lengthHalf1 = length1 * 0.5f;
            float widthHalf2 = width2 * 0.5f;
            float lengthHalf2 = length2 * 0.5f;

            Vector3 frontMid1 = downCenter + m_forward * lengthHalf1;
            Vector3 backMid1 = downCenter + -m_forward * lengthHalf1;
            Vector3 rightMid1 = downCenter + m_right * widthHalf1;
            Vector3 leftMid1 = downCenter + -m_right * widthHalf1;

            Vector3 frontMid2 = upCenter + m_forward * lengthHalf2;
            Vector3 backMid2 = upCenter + -m_forward * lengthHalf2;
            Vector3 rightMid2 = upCenter + m_right * widthHalf2;
            Vector3 leftMid2 = upCenter + -m_right * widthHalf2;

            if (isUp)
            {
                Vector3 frontMid3 = downCenter + m_forward * lengthHalf2;
                Vector3 backMid3 = downCenter + -m_forward * lengthHalf2;
                Vector3 rightMid3 = downCenter + m_right * widthHalf2;
                Vector3 leftMid3 = downCenter + -m_right * widthHalf2;

                Vector3 TR = frontMid3 + m_right * widthHalf2;
                Vector3 BR = backMid3 + m_right * widthHalf2;
                Vector3 BL = backMid3 + -m_right * widthHalf2;
                Vector3 TL = frontMid3 + -m_right * widthHalf2;

                if (capBottom) CreateSurfacePlane(upCenter, m_forward, m_right, width2, length2, widthSegs, lengthSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);

                if (flatChamfer)
                {
                    if (cap) CreateChamferPlane(downCenter, m_forward, -m_right, width1, length1, fillet, false, widthSegs, lengthSegs, 1, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);

                    Vector3 frontDir = frontMid2 - frontMid1;
                    Vector3 backDir = backMid2 - backMid1;
                    Vector3 rightDir = rightMid2 - rightMid1;
                    Vector3 leftDir = leftMid2 - leftMid1;

                    CreateSurfaceTrapezoid((frontMid1 + frontMid2) * 0.5f, frontDir.normalized, -m_right, width2, width2, frontDir.magnitude, 0.0f, widthSegs, filletSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
                    CreateSurfaceTrapezoid((backMid1 + backMid2) * 0.5f, backDir.normalized, m_right, width2, width2, backDir.magnitude, 0.0f, widthSegs, filletSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
                    CreateSurfaceTrapezoid((rightMid1 + rightMid2) * 0.5f, rightDir.normalized, m_forward, length2, length2, rightDir.magnitude, 0.0f, lengthSegs, filletSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
                    CreateSurfaceTrapezoid((leftMid1 + leftMid2) * 0.5f, leftDir.normalized, -m_forward, length2, length2, leftDir.magnitude, 0.0f, lengthSegs, filletSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);

                    Vector3 downTR = TR + (m_forward + m_right) * filletHalf;
                    Vector3 downBR = BR + (-m_forward + m_right) * filletHalf;
                    Vector3 downBL = BL + (-m_forward + -m_right) * filletHalf;
                    Vector3 downTL = TL + (m_forward + -m_right) * filletHalf;

                    Vector3 rTR = (m_forward + -m_right) * 0.5f;
                    Vector3 rBR = (m_forward + m_right) * 0.5f;
                    Vector3 rBL = (-m_forward + m_right) * 0.5f;
                    Vector3 rTL = (-m_forward + -m_right) * 0.5f;

                    Vector3 upTR = TR + m_up * fillet;
                    Vector3 upBR = BR + m_up * fillet;
                    Vector3 upBL = BL + m_up * fillet;
                    Vector3 upTL = TL + m_up * fillet;

                    float triWidth = Mathf.Sqrt(fillet * fillet * 2);
                    float triLength = (upTR - downTR).magnitude;

                    CreateSurfaceTriangle((upTR + downTR) * 0.5f, (upTR - downTR).normalized, rTR.normalized, triWidth, triLength, 0.0f, filletSegs, filletSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, false, false);
                    CreateSurfaceTriangle((upBR + downBR) * 0.5f, (upBR - downBR).normalized, rBR.normalized, triWidth, triLength, 0.0f, filletSegs, filletSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, false, false);
                    CreateSurfaceTriangle((upBL + downBL) * 0.5f, (upBL - downBL).normalized, rBL.normalized, triWidth, triLength, 0.0f, filletSegs, filletSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, false, false);
                    CreateSurfaceTriangle((upTL + downTL) * 0.5f, (upTL - downTL).normalized, rTL.normalized, triWidth, triLength, 0.0f, filletSegs, filletSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, false, false);
                }
                else
                {
                    if (cap) CreateChamferPlane(downCenter, m_forward, -m_right, width1, length1, fillet, false, widthSegs, lengthSegs, filletSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);

                    CreateSurfaceCylinder(frontMid3, -m_forward, -m_up, width2, fillet, filletSegs, widthSegs, true, 180.0f, 270.0f, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth, false, true);
                    CreateSurfaceCylinder(backMid3, m_forward, -m_up, width2, fillet, filletSegs, widthSegs, true, 180.0f, 270.0f, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth, false, true);
                    CreateSurfaceCylinder(rightMid3, -m_right, -m_up, length2, fillet, filletSegs, lengthSegs, true, 180.0f, 270.0f, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth, false, true);
                    CreateSurfaceCylinder(leftMid3, m_right, -m_up, length2, fillet, filletSegs, lengthSegs, true, 180.0f, 270.0f, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth, false, true);

                    int segF2 = filletSegs * 2;
                    float tilingSphereY = realWorldMapSize ? 2.0f : 2.0f;
                    float offsetSphereY = realWorldMapSize ? -fillet * Mathf.PI : -1.0f;
                    Vector2 UVTilingSphere = new Vector2(UVTiling.x, UVTiling.y * tilingSphereY);
                    Vector2 UVOffsetSphere = UVOffset + new Vector2(0.0f, UVTiling.y * offsetSphereY);
                    CreateSurfaceSphere(TR, m_forward, m_right, fillet, filletSegs, segF2, true, 0.0f, 90.0f, true, 0.5f, 1.0f, generateMappingCoords, realWorldMapSize, UVOffsetSphere, UVTilingSphere, flipNormals, smooth);
                    CreateSurfaceSphere(BR, m_forward, m_right, fillet, filletSegs, segF2, true, 90.0f, 180.0f, true, 0.5f, 1.0f, generateMappingCoords, realWorldMapSize, UVOffsetSphere, UVTilingSphere, flipNormals, smooth);
                    CreateSurfaceSphere(BL, m_forward, m_right, fillet, filletSegs, segF2, true, 180.0f, 270.0f, true, 0.5f, 1.0f, generateMappingCoords, realWorldMapSize, UVOffsetSphere, UVTilingSphere, flipNormals, smooth);
                    CreateSurfaceSphere(TL, m_forward, m_right, fillet, filletSegs, segF2, true, 270.0f, 360.0f, true, 0.5f, 1.0f, generateMappingCoords, realWorldMapSize, UVOffsetSphere, UVTilingSphere, flipNormals, smooth);
                }
            }
            else
            {
                Vector3 frontMid3 = upCenter + m_forward * lengthHalf1;
                Vector3 backMid3 = upCenter + -m_forward * lengthHalf1;
                Vector3 rightMid3 = upCenter + m_right * widthHalf1;
                Vector3 leftMid3 = upCenter + -m_right * widthHalf1;

                Vector3 TR = frontMid3 + m_right * widthHalf1;
                Vector3 BR = backMid3 + m_right * widthHalf1;
                Vector3 BL = backMid3 + -m_right * widthHalf1;
                Vector3 TL = frontMid3 + -m_right * widthHalf1;

                if (capBottom) CreateSurfacePlane(downCenter, m_forward, -m_right, width1, length1, widthSegs, lengthSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);

                if (flatChamfer)
                {
                    if (cap) CreateChamferPlane(upCenter, m_forward, m_right, width2, length2, fillet, false, widthSegs, lengthSegs, 1, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);

                    Vector3 frontDir = frontMid2 - frontMid1;
                    Vector3 backDir = backMid2 - backMid1;
                    Vector3 rightDir = rightMid2 - rightMid1;
                    Vector3 leftDir = leftMid2 - leftMid1;

                    CreateSurfaceTrapezoid((frontMid1 + frontMid2) * 0.5f, frontDir.normalized, -m_right, width1, width1, frontDir.magnitude, 0.0f, widthSegs, filletSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
                    CreateSurfaceTrapezoid((backMid1 + backMid2) * 0.5f, backDir.normalized, m_right, width1, width1, backDir.magnitude, 0.0f, widthSegs, filletSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
                    CreateSurfaceTrapezoid((rightMid1 + rightMid2) * 0.5f, rightDir.normalized, m_forward, length1, length1, rightDir.magnitude, 0.0f, lengthSegs, filletSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
                    CreateSurfaceTrapezoid((leftMid1 + leftMid2) * 0.5f, leftDir.normalized, -m_forward, length1, length1, leftDir.magnitude, 0.0f, lengthSegs, filletSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);

                    Vector3 downTR = TR + (m_forward + m_right) * filletHalf;
                    Vector3 downBR = BR + (-m_forward + m_right) * filletHalf;
                    Vector3 downBL = BL + (-m_forward + -m_right) * filletHalf;
                    Vector3 downTL = TL + (m_forward + -m_right) * filletHalf;

                    Vector3 rTR = (m_forward + -m_right) * -0.5f;
                    Vector3 rBR = (m_forward + m_right) * -0.5f;
                    Vector3 rBL = (-m_forward + m_right) * -0.5f;
                    Vector3 rTL = (-m_forward + -m_right) * -0.5f;

                    Vector3 upTR = TR + -m_up * fillet;
                    Vector3 upBR = BR + -m_up * fillet;
                    Vector3 upBL = BL + -m_up * fillet;
                    Vector3 upTL = TL + -m_up * fillet;

                    float triWidth = Mathf.Sqrt(fillet * fillet * 2);
                    float triLength = (upTR - downTR).magnitude;

                    CreateSurfaceTriangle((upTR + downTR) * 0.5f, (upTR - downTR).normalized, rTR.normalized, triWidth, triLength, 0.0f, filletSegs, filletSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, false, true);
                    CreateSurfaceTriangle((upBR + downBR) * 0.5f, (upBR - downBR).normalized, rBR.normalized, triWidth, triLength, 0.0f, filletSegs, filletSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, false, true);
                    CreateSurfaceTriangle((upBL + downBL) * 0.5f, (upBL - downBL).normalized, rBL.normalized, triWidth, triLength, 0.0f, filletSegs, filletSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, false, true);
                    CreateSurfaceTriangle((upTL + downTL) * 0.5f, (upTL - downTL).normalized, rTL.normalized, triWidth, triLength, 0.0f, filletSegs, filletSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, false, true);
                }
                else
                {
                    if (cap) CreateChamferPlane(upCenter, m_forward, m_right, width2, length2, fillet, false, widthSegs, lengthSegs, filletSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);

                    CreateSurfaceCylinder(frontMid3, -m_forward, -m_up, width1, fillet, filletSegs, widthSegs, true, 90.0f, 180.0f, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth, false, true);
                    CreateSurfaceCylinder(backMid3, m_forward, -m_up, width1, fillet, filletSegs, widthSegs, true, 90.0f, 180.0f, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth, false, true);
                    CreateSurfaceCylinder(rightMid3, -m_right, -m_up, length1, fillet, filletSegs, lengthSegs, true, 90.0f, 180.0f, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth, false, true);
                    CreateSurfaceCylinder(leftMid3, m_right, -m_up, length1, fillet, filletSegs, lengthSegs, true, 90.0f, 180.0f, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth, false, true);

                    int segF2 = filletSegs * 2;
                    float tilingSphereY = realWorldMapSize ? 2.0f : 2.0f;
                    Vector2 UVTilingSphere = new Vector2(UVTiling.x, UVTiling.y * tilingSphereY);
                    CreateSurfaceSphere(TR, m_forward, m_right, fillet, filletSegs, segF2, true, 0.0f, 90.0f, true, 0.0f, 0.5f, generateMappingCoords, realWorldMapSize, UVOffset, UVTilingSphere, flipNormals, smooth);
                    CreateSurfaceSphere(BR, m_forward, m_right, fillet, filletSegs, segF2, true, 90.0f, 180.0f, true, 0.0f, 0.5f, generateMappingCoords, realWorldMapSize, UVOffset, UVTilingSphere, flipNormals, smooth);
                    CreateSurfaceSphere(BL, m_forward, m_right, fillet, filletSegs, segF2, true, 180.0f, 270.0f, true, 0.0f, 0.5f, generateMappingCoords, realWorldMapSize, UVOffset, UVTilingSphere, flipNormals, smooth);
                    CreateSurfaceSphere(TL, m_forward, m_right, fillet, filletSegs, segF2, true, 270.0f, 360.0f, true, 0.0f, 0.5f, generateMappingCoords, realWorldMapSize, UVOffset, UVTilingSphere, flipNormals, smooth);
                }
            }
        }

        // *****************
        // Create Surface
        // *****************

        public void CreateSurfaceTriangle(Vector3 pivotOffset, Vector3 m_forward, Vector3 m_right, float width, float length, float offset, int seg, bool generateMappingCoords, bool realWorldMapSize, Vector2 UVOffset, Vector2 UVTiling, bool flipNormals, bool mirrorUV = false, bool flipUV = false)
        {
            Vector3 m_up = Vector3.Cross(m_forward, m_right).normalized;
            float lengthHalf = length * 0.5f;
            float widthHalf = width * 0.5f;
            float c = 0.0f;
            if (offset < -widthHalf) c = (offset + widthHalf) * 0.5f;
            else if (offset > widthHalf) c = (offset - widthHalf) * 0.5f;

            //points
            //   1  
            //  / \ 
            // 0---2
            Vector3[] points = new Vector3[3];
            points[0] = pivotOffset - m_right * (widthHalf + c) - m_forward * lengthHalf;
            points[1] = pivotOffset + m_right * (offset - c) + m_forward * lengthHalf;
            points[2] = pivotOffset + m_right * (widthHalf - c) - m_forward * lengthHalf;

            //uvs    * flip
            //   1   * 0---2
            //  / \  *  \ / 
            // 0---2 *   1  
            Vector2[] uvs = null;
            if (generateMappingCoords)
            {
                if (realWorldMapSize)
                {
                    UVTiling.x *= width;
                    UVTiling.y *= length;
                }
                Vector3 v01 = points[1] - points[0];
                Vector3 v02 = points[2] - points[0];
                float vx = PPMathLib.PointLineProjection(v01, v02);
                if (mirrorUV) vx = v02.magnitude - vx;
                vx /= v02.magnitude;
                uvs = new Vector2[3];
                if (flipUV)
                {
                    uvs[0] = UVOffset + new Vector2(0.0f, UVTiling.y);
                    uvs[1] = new Vector2(vx * UVTiling.x, 0.0f);
                    uvs[2] = uvs[0] + new Vector2(UVTiling.x, 0.0f);
                }
                else
                {
                    uvs[0] = UVOffset;
                    uvs[1] = uvs[0] + new Vector2(vx * UVTiling.x, UVTiling.y);
                    uvs[2] = uvs[0] + new Vector2(UVTiling.x, 0.0f);
                }
                if (mirrorUV ^ flipNormals) uvs = uvs.Reverse().ToArray();
            }

            if (flipNormals) 
            {
                points = points.Reverse().ToArray();
                m_up *= -1;
            }

            BuildSurfaceTriangle(points, uvs, m_up, seg);
        }

        public void CreateSurfaceTriangle(Vector3 pivotOffset, Vector3 m_forward, Vector3 m_right, float width, float length, float offset, int widthSegs, int lengthSegs, bool generateMappingCoords, bool realWorldMapSize, Vector2 UVOffset, Vector2 UVTiling, bool flipNormals, bool mirrorUV = false, bool flipUV = false)
        {
            Vector3 m_up = Vector3.Cross(m_forward, m_right).normalized;
            float lengthHalf = length * 0.5f;
            float widthHalf = width * 0.5f;
            float c = 0.0f;
            if (offset < -widthHalf) c = (offset + widthHalf) * 0.5f;
            else if (offset > widthHalf) c = (offset - widthHalf) * 0.5f;

            //points
            //   12  
            //  / \  
            // 0---3 
            Vector3[] points = new Vector3[4];
            points[0] = pivotOffset - m_right * (widthHalf + c) - m_forward * lengthHalf;
            points[1] = pivotOffset + m_right * (offset - c) + m_forward * lengthHalf;
            points[2] = points[1];
            points[3] = pivotOffset + m_right * (widthHalf - c) - m_forward * lengthHalf;

            //uvs    * flip
            // 1-2   * 0-3
            // | |   * | |
            // 0-3   * 1-2
            Vector2[] uvs = null;
            if (generateMappingCoords)
            {
                if (realWorldMapSize)
                {
                    UVTiling.x *= width;
                    UVTiling.y *= length;
                }
                uvs = new Vector2[4];
                if (flipUV)
                {
                    uvs[1] = UVOffset;
                    uvs[0] = uvs[1] + new Vector2(0.0f, UVTiling.y);
                    uvs[3] = uvs[0] + new Vector2(UVTiling.x, 0.0f);
                    uvs[2] = uvs[1] + new Vector2(UVTiling.x, 0.0f);
                }
                else
                {
                    uvs[0] = UVOffset;
                    uvs[1] = uvs[0] + new Vector2(0.0f, UVTiling.y);
                    uvs[2] = uvs[1] + new Vector2(UVTiling.x, 0.0f);
                    uvs[3] = uvs[0] + new Vector2(UVTiling.x, 0.0f);
                }
                if (mirrorUV ^ flipNormals) uvs = uvs.Reverse().ToArray();
            }

            if (flipNormals)
            {
                points = points.Reverse().ToArray();
                m_up *= -1;
            }

            BuildSurfaceRectangle(points, uvs, m_up, widthSegs, lengthSegs, false);
        }

        public void CreateSurfacePlane(Vector3 pivotOffset, Vector3 m_forward, Vector3 m_right, float width, float length, int widthSegs, int lengthSegs, bool generateMappingCoords, bool realWorldMapSize, Vector2 UVOffset, Vector2 UVTiling, bool flipNormals, bool mirrorUV = false)
        {
            Vector3 m_up = Vector3.Cross(m_forward, m_right).normalized;
            float lengthHalf = length * 0.5f;
            float widthHalf = width * 0.5f;

            //points
            // 1-2 
            // | | 
            // 0-3 
            Vector3[] points = new Vector3[4];
            points[0] = pivotOffset - m_right * widthHalf - m_forward * lengthHalf;
            points[1] = pivotOffset - m_right * widthHalf + m_forward * lengthHalf;
            points[2] = pivotOffset + m_right * widthHalf + m_forward * lengthHalf;
            points[3] = pivotOffset + m_right * widthHalf - m_forward * lengthHalf;

            //uvs
            // 1-2 
            // | | 
            // 0-3 
            Vector2[] uvs = null;
            if (generateMappingCoords)
            {
                if (realWorldMapSize)
                {
                    UVTiling.x *= width;
                    UVTiling.y *= length;
                }
                uvs = new Vector2[4];
                uvs[0] = UVOffset;
                uvs[1] = uvs[0] + new Vector2(0.0f, UVTiling.y);
                uvs[2] = uvs[1] + new Vector2(UVTiling.x, 0.0f);
                uvs[3] = uvs[0] + new Vector2(UVTiling.x, 0.0f);
                if (mirrorUV ^ flipNormals) uvs = uvs.Reverse().ToArray();
            }

            if (flipNormals)
            {
                points = points.Reverse().ToArray();
                m_up *= -1; 
            }

            BuildSurfaceRectangle(points, uvs, m_up, widthSegs, lengthSegs);
        }

        public void CreateSurfacePolygon(Vector3 pivotOffset, Vector3 m_forward, Vector3 m_right, List<Vector2> m_points, List<int> triangles, bool generateMappingCoords, bool realWorldMapSize, Vector2 UVOffset, Vector2 UVTiling, bool flipNormals, bool mirrorUV = false)
        {
            if (m_points.Count < 3) return;

            Vector3 m_up = Vector3.Cross(m_forward, m_right).normalized;

            //points
            Vector3[] points = new Vector3[m_points.Count];
            Vector2 minP = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 maxP = new Vector2(float.MinValue, float.MinValue);
            for (int i = 0; i < m_points.Count; ++i)
            {
                points[i] = pivotOffset + m_right * m_points[i].x + m_forward * m_points[i].y;
                if (m_points[i].x < minP.x) minP.x = m_points[i].x;
                if (m_points[i].x > maxP.x) maxP.x = m_points[i].x;
                if (m_points[i].y < minP.y) minP.y = m_points[i].y;
                if (m_points[i].y > maxP.y) maxP.y = m_points[i].y;
            }

            //uvs
            Vector2[] uvs = null;
            if (generateMappingCoords)
            {
                if (mirrorUV)
                {
                    float temp = minP.x;
                    minP.x = maxP.x;
                    maxP.x = temp;
                }
                if (realWorldMapSize)
                {
                    UVTiling.x *= maxP.x - minP.x;
                    UVTiling.y *= maxP.y - minP.y;
                }
                uvs = new Vector2[m_points.Count];
                for (int i = 0; i < m_points.Count; ++i)
                {
                    float x = Mathf.InverseLerp(minP.x, maxP.x, m_points[i].x);
                    float y = Mathf.InverseLerp(minP.y, maxP.y, m_points[i].y);
                    uvs[i] = UVOffset + new Vector2(UVTiling.x * x, UVTiling.y * y);
                }
            }

            if (flipNormals)
            {
                if (triangles != null) triangles.Reverse();
                m_up *= -1;
            }

            BuildSurfacePolygon(points, uvs, triangles, m_up);
        }

        public void CreateSurfaceTrapezoid(Vector3 pivotOffset, Vector3 m_forward, Vector3 m_right, float width1, float width2, float length, float offset, int widthSegs, int lengthSegs, bool generateMappingCoords, bool realWorldMapSize, Vector2 UVOffset, Vector2 UVTiling, bool flipNormals, bool mirrorUV = false)
        {
            Vector3 m_up = Vector3.Cross(m_forward, m_right).normalized;
            float lengthHalf = length * 0.5f;
            float widthHalf1 = width1 * 0.5f;
            float widthHalf2 = width2 * 0.5f;
            float c = 0.0f;
            if (width1 > width2)
            {
                if (offset + widthHalf2 > widthHalf1) c = (offset + widthHalf2 - widthHalf1) * 0.5f;
                else if (offset - widthHalf2 < -widthHalf1) c = (offset - widthHalf2 + widthHalf1) * 0.5f;
            }
            else
            {
                if (offset + widthHalf1 > widthHalf2) c = (offset + widthHalf1 - widthHalf2) * 0.5f;
                else if (offset - widthHalf1 < -widthHalf2) c = (offset - widthHalf1 + widthHalf2) * 0.5f;
            }

            //points
            // 1-2 
            // | | 
            // 0-3 
            Vector3[] points = new Vector3[4];
            points[0] = pivotOffset - m_right * (widthHalf1 + c) - m_forward * lengthHalf;
            points[1] = pivotOffset - m_right * (widthHalf2 - offset + c) + m_forward * lengthHalf;
            points[2] = pivotOffset + m_right * (widthHalf2 + offset - c) + m_forward * lengthHalf;
            points[3] = pivotOffset + m_right * (widthHalf1 - c) - m_forward * lengthHalf;

            //uvs
            // 1-2 
            // | | 
            // 0-3 
            Vector2[] uvs = null;
            if (generateMappingCoords)
            {
                if (realWorldMapSize)
                {
                    UVTiling.x *= (width1 + width2) * 0.5f;
                    UVTiling.y *= length;
                }
                uvs = new Vector2[4];
                uvs[0] = UVOffset;
                uvs[1] = uvs[0] + new Vector2(0.0f, UVTiling.y);
                uvs[2] = uvs[1] + new Vector2(UVTiling.x, 0.0f);
                uvs[3] = uvs[0] + new Vector2(UVTiling.x, 0.0f);
                if (mirrorUV ^ flipNormals) uvs = uvs.Reverse().ToArray();
            }

            if (flipNormals)
            {
                points = points.Reverse().ToArray();
                m_up *= -1; 
            }

            BuildSurfaceRectangle(points, uvs, m_up, widthSegs, lengthSegs, false);
        }

        public void CreateSurfaceCircle(Vector3 pivotOffset, Vector3 m_forward, Vector3 m_right, float radius, int sides, int seg, bool sliceOn, float sliceFrom, float sliceTo, bool generateMappingCoords, bool realWorldMapSize, Vector2 UVOffset, Vector2 UVTiling, bool flipNormals, bool mirrorUV = false, bool flipUV = false)
        {
            if (radius < 0.0001f) return;

            if (!sliceOn) { sliceFrom = 0.0f; sliceTo = 360.0f; }
            float angleStart = sliceFrom * Mathf.Deg2Rad;
            float angle = (sliceTo - sliceFrom) / sides * Mathf.Deg2Rad;
            Vector3 m_up = Vector3.Cross(m_forward, m_right).normalized;

            //points
            //  n,0  
            //  / \  
            // 3 c 1 
            //  \ /  
            //   2   
            Vector3[] points = new Vector3[sides + 1];
            Vector2[] sinCos = new Vector2[sides + 1];
            for (int i = 0; i <= sides; ++i)
            {
                float rad = angleStart + angle * i;
                sinCos[i].x = Mathf.Sin(rad);
                sinCos[i].y = Mathf.Cos(rad);
                points[i] = pivotOffset + (m_forward * sinCos[i].y + m_right * sinCos[i].x) * radius;
            }

            //uvs
            //  n,0  
            //  / \  
            // 3 c 1 
            //  \ /  
            //   2   
            Vector2[] uvs = null;
            Vector2 uvCenter = new Vector2();
            if (generateMappingCoords)
            {
                if (realWorldMapSize)
                {
                    UVTiling *= radius * 2.0f;
                }
                float xDir = mirrorUV ? -1 : 1;
                float yDir = flipUV ? -1 : 1;
                uvCenter = new Vector2(UVOffset.x + UVTiling.x * 0.5f, UVOffset.y + UVTiling.y * 0.5f);
                uvs = new Vector2[sides + 1];
                for (int i = 0; i <= sides; ++i)
                {
                    uvs[i] = uvCenter + new Vector2(sinCos[i].x * xDir * UVTiling.x, sinCos[i].y * yDir * UVTiling.y) * 0.5f;
                }
                if (flipNormals) uvs = uvs.Reverse().ToArray();
            }

            if (flipNormals)
            {
                points = points.Reverse().ToArray();
                m_up *= -1;
            }

            BuildSurfaceCircle(points, pivotOffset, uvs, uvCenter, m_up, seg);
        }

        public void CreateSurfaceHemiCircle(Vector3 pivotOffset, Vector3 m_forward, Vector3 m_right, bool isLeft, float radius, int sides, int seg, bool isSector, bool cutOn, float cutFrom, float cutTo, bool generateMappingCoords, bool realWorldMapSize, Vector2 UVOffset, Vector2 UVTiling, bool flipNormals, bool mirrorUV = false)
        {
            if (radius < 0.0001f) return;

            float PIhalf = Mathf.PI * 0.5f;
            float angleStart = isLeft ? Mathf.PI : 0.0f;
            float angle = Mathf.PI / sides;
            Vector3 m_up = Vector3.Cross(m_forward, m_right).normalized;
            if (!isLeft)
            {
                float temp = cutFrom;
                cutFrom = 1.0f - cutTo;
                cutTo = 1.0f - temp;
            }

            //points
            //  n,0  
            //  / \  
            // 3 c 1 
            //  \ /  
            //   2   
            float centerOffsetY = 0.0f;
            List<Vector2> sinCos = new List<Vector2>();
            if (cutOn)
            {
                cutFrom = (cutFrom - 0.5f) * 2.0f; //convert to -1 to 1
                cutTo = (cutTo - 0.5f) * 2.0f; //convert to -1 to 1
                float angleFrom = Mathf.PI - Mathf.Acos(cutFrom);
                float angleTo = Mathf.PI - Mathf.Acos(cutTo);
                float angleCutFrom = angleStart + angleFrom;
                float angleCutTo = angleStart + angleTo;

                int indexFrom = Mathf.FloorToInt(angleFrom / angle) + 1;
                int indexTo = Mathf.CeilToInt(angleTo / angle) - 1;

                if (isSector)
                {
                    sinCos.Add(new Vector2(Mathf.Sin(angleCutFrom), Mathf.Cos(angleCutFrom)));
                    for (int i = indexFrom; i <= indexTo; ++i)
                    {
                        float rad = angleStart + angle * i;
                        sinCos.Add(new Vector2(Mathf.Sin(rad), Mathf.Cos(rad)));
                    }
                    sinCos.Add(new Vector2(Mathf.Sin(angleCutTo), Mathf.Cos(angleCutTo)));
                }
                else
                {
                    if (!isLeft) { cutFrom = -cutFrom; cutTo = -cutTo; } //reverse in right side
                    if (angleFrom > PIhalf) centerOffsetY = cutFrom;
                    if (angleTo < PIhalf) centerOffsetY = cutTo;

                    for (int i = 0; i <= sides; ++i)
                    {
                        float rad = angleStart + angle * i;
                        //start
                        if (rad < angleCutFrom)
                        {
                            if (angleFrom < PIhalf) sinCos.Add(new Vector2(Mathf.Sin(rad) / Mathf.Cos(rad) * cutFrom, cutFrom));
                            if (rad + angle > angleCutFrom) sinCos.Add(new Vector2(Mathf.Sin(angleCutFrom), Mathf.Cos(angleCutFrom)));
                        }
                        //end
                        else if (rad > angleCutTo)
                        {
                            if (rad - angle < angleCutTo) sinCos.Add(new Vector2(Mathf.Sin(angleCutTo), Mathf.Cos(angleCutTo)));
                            if (angleTo > PIhalf) sinCos.Add(new Vector2(Mathf.Sin(rad) / Mathf.Cos(rad) * cutTo, cutTo));
                        }
                        //mid
                        else
                        {
                            sinCos.Add(new Vector2(Mathf.Sin(rad), Mathf.Cos(rad)));
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i <= sides; ++i)
                {
                    float rad = angleStart + angle * i;
                    sinCos.Add(new Vector2(Mathf.Sin(rad), Mathf.Cos(rad)));
                }
            }

            Vector3[] points = new Vector3[sinCos.Count];
            for (int i = 0; i < sinCos.Count; ++i)
            {
                points[i] = pivotOffset + (m_forward * sinCos[i].y + m_right * sinCos[i].x) * radius;
            }
            pivotOffset += m_forward * centerOffsetY * radius;

            //uvs
            //  n,0  
            //  / \  
            // 3 c 1 
            //  \ /  
            //   2   
            Vector2[] uvs = null;
            Vector2 uvCenter = new Vector2();
            if (generateMappingCoords)
            {
                if (realWorldMapSize)
                {
                    UVTiling *= radius * 2.0f;
                }
                float xDir = mirrorUV ? -1 : 1;
                uvCenter = new Vector2(UVOffset.x + UVTiling.x * 0.5f, UVOffset.y + UVTiling.y * 0.5f);
                uvs = new Vector2[sinCos.Count];
                for (int i = 0; i < sinCos.Count; ++i)
                {
                    uvs[i] = uvCenter + new Vector2(sinCos[i].x * xDir * UVTiling.x, sinCos[i].y * UVTiling.y) * 0.5f;
                }
                uvCenter += new Vector2(0.0f, centerOffsetY * 0.5f * UVTiling.y);
                if (flipNormals) uvs = uvs.Reverse().ToArray();
            }

            if (flipNormals) 
            {
                points = points.Reverse().ToArray();
                m_up *= -1;
            }

            BuildSurfaceCircle(points, pivotOffset, uvs, uvCenter, m_up, seg);
        }

        public void CreateSurfaceRing(Vector3 pivotOffset, Vector3 m_forward, Vector3 m_right, float radius1, float radius2, int sides, int seg, bool sliceOn, float sliceFrom, float sliceTo, bool generateMappingCoords, bool realWorldMapSize, Vector2 UVOffset, Vector2 UVTiling, bool flipNormals, bool mirrorUV = false, bool flipUV = false)
        {
            if (radius1 + radius2 < 0.0001f) return;

            if (!sliceOn) { sliceFrom = 0.0f; sliceTo = 360.0f; }
            float angleStart = sliceFrom * Mathf.Deg2Rad;
            float angle = (sliceTo - sliceFrom) / sides * Mathf.Deg2Rad;
            Vector3 m_up = Vector3.Cross(m_forward, m_right).normalized;

            //points
            //  n,0  
            //  / \  
            // 3 c 1 
            //  \ /  
            //   2   
            Vector3[] points = new Vector3[sides + 1];
            Vector2[] sinCos = new Vector2[sides + 1];
            for (int i = 0; i <= sides; ++i)
            {
                float rad = angleStart + angle * i;
                sinCos[i].x = Mathf.Sin(rad);
                sinCos[i].y = Mathf.Cos(rad);
                points[i] = pivotOffset + (m_forward * sinCos[i].y + m_right * sinCos[i].x) * radius1;
            }

            //uvs
            //  n,0  
            //  / \  
            // 3 c 1 
            //  \ /  
            //   2   
            Vector2[] uvs = null;
            Vector2 uvCenter = new Vector2();
            if (generateMappingCoords)
            {
                if (realWorldMapSize)
                {
                    UVTiling *= radius1 * 2.0f;
                }
                float xDir = mirrorUV ? -1 : 1;
                float yDir = flipUV ? -1 : 1;
                uvCenter = new Vector2(UVOffset.x + UVTiling.x * 0.5f, UVOffset.y + UVTiling.y * 0.5f);
                uvs = new Vector2[sides + 1];
                for (int i = 0; i <= sides; ++i)
                {
                    uvs[i] = uvCenter + new Vector2(sinCos[i].x * xDir * UVTiling.x, sinCos[i].y * yDir * UVTiling.y) * 0.5f;
                }
                if (flipNormals) uvs = uvs.Reverse().ToArray();
            }

            if (flipNormals)
            {
                points = points.Reverse().ToArray();
                m_up *= -1;
            }

            BuildSurfaceRing(points, pivotOffset, uvs, uvCenter, m_up, seg, radius2 / radius1);
        }

        public void CreateSurfaceRingSlanted(Vector3 pivotOffset, Vector3 m_forward, Vector3 m_right, float radius1, float radius2, float height, int sides, int seg, bool sliceOn, float sliceFrom, float sliceTo, bool generateMappingCoords, bool realWorldMapSize, Vector2 UVOffset, Vector2 UVTiling, bool flipNormals, bool mirrorUV = false, bool flipUV = false)
        {
            if (radius1 + radius2 < 0.0001f) return;

            if (!sliceOn) { sliceFrom = 0.0f; sliceTo = 360.0f; }
            float angleStart = sliceFrom * Mathf.Deg2Rad;
            float angle = (sliceTo - sliceFrom) / sides * Mathf.Deg2Rad;
            float heightSide = height / sides;
            Vector3 m_up = Vector3.Cross(m_forward, m_right).normalized;

            //points
            //  n,0  
            //  / \  
            // 3 c 1 
            //  \ /  
            //   2   
            Vector3[] points = new Vector3[sides + 1];
            Vector3[] pointsInner = new Vector3[sides + 1];
            Vector2[] sinCos = new Vector2[sides + 1];
            for (int i = 0; i <= sides; ++i)
            {
                float rad = angleStart + angle * i;
                sinCos[i].x = Mathf.Sin(rad);
                sinCos[i].y = Mathf.Cos(rad);
                Vector3 center = pivotOffset + m_up * heightSide * i;
                Vector3 v = m_forward * sinCos[i].y + m_right * sinCos[i].x;
                points[i] = center + v * radius1;
                pointsInner[i] = center + v * radius2;
            }

            //uvs
            //  n,0  
            //  / \  
            // 3 c 1 
            //  \ /  
            //   2   
            Vector2[] uvs = null;
            Vector2[] uvsInner = null;
            if (generateMappingCoords)
            {
                if (realWorldMapSize)
                {
                    UVTiling *= radius1 * 2.0f;
                }
                float xDir = mirrorUV ? -1 : 1;
                float yDir = flipUV ? -1 : 1;
                Vector2 uvCenter = new Vector2(UVOffset.x + UVTiling.x * 0.5f, UVOffset.y + UVTiling.y * 0.5f);
                uvs = new Vector2[sides + 1];
                uvsInner = new Vector2[sides + 1];
                for (int i = 0; i <= sides; ++i)
                {
                    Vector2 v = new Vector2(sinCos[i].x * xDir * UVTiling.x, sinCos[i].y * yDir * UVTiling.y) * 0.5f;
                    uvs[i] = uvCenter + v;
                    uvsInner[i] = uvCenter + v * (radius2 / radius1);
                }
                if (flipNormals)
                {
                    uvs = uvs.Reverse().ToArray();
                    uvsInner = uvsInner.Reverse().ToArray();
                }
            }

            if (flipNormals)
            {
                points = points.Reverse().ToArray();
                pointsInner = pointsInner.Reverse().ToArray();
                m_up *= -1;
            }

            BuildSurfaceRing(points, pointsInner, uvs, uvsInner, m_up, seg);
        }

        public void CreateSurfaceRectRing(Vector3 pivotOffset, Vector3 m_forward, Vector3 m_right, float width1, float length1, float width2, float length2, int seg, int widthSegs, int lengthSegs, bool generateMappingCoords, bool realWorldMapSize, Vector2 UVOffset, Vector2 UVTiling, bool flipNormals, bool mirrorUV = false)
        {
            Vector3 m_up = Vector3.Cross(m_forward, m_right).normalized;
            float lengthHalf1 = length1 * 0.5f;
            float widthHalf1 = width1 * 0.5f;
            float lengthHalf2 = length2 * 0.5f;
            float widthHalf2 = width2 * 0.5f;

            //points 
            // 1---2 
            // |5-6| 
            // || || 
            // |4-7| 
            // 0---3 
            Vector3[] points = new Vector3[8];
            points[0] = pivotOffset - m_right * widthHalf1 - m_forward * lengthHalf1;
            points[1] = points[0] + m_forward * length1;
            points[2] = points[1] + m_right * width1;
            points[3] = points[0] + m_right * width1;
            points[4] = pivotOffset - m_right * widthHalf2 - m_forward * lengthHalf2;
            points[5] = points[4] + m_forward * length2;
            points[6] = points[5] + m_right * width2;
            points[7] = points[4] + m_right * width2;

            Vector3[] pointsBottom = new Vector3[] { points[0], points[4], points[7], points[3] };
            Vector3[] pointsLeft = new Vector3[] { points[0], points[1], points[5], points[4] };
            Vector3[] pointsTop = new Vector3[] { points[5], points[1], points[2], points[6] };
            Vector3[] pointsRight = new Vector3[] { points[7], points[6], points[2], points[3] };

            //uvs
            // 1---2 
            // |5-6| 
            // || || 
            // |4-7| 
            // 0---3 
            Vector2[] uvs = null;
            float widthHalfDif = (widthHalf1 - widthHalf2) / width1;
            float lengthHalfDif = (lengthHalf1 - lengthHalf2) / length1;
            if (generateMappingCoords)
            {
                if (realWorldMapSize)
                {
                    UVTiling.x *= width1;
                    UVTiling.y *= length1;
                }
                uvs = new Vector2[8];
                uvs[0] = UVOffset;
                uvs[1] = uvs[0] + new Vector2(0.0f, UVTiling.y);
                uvs[2] = uvs[1] + new Vector2(UVTiling.x, 0.0f);
                uvs[3] = uvs[0] + new Vector2(UVTiling.x, 0.0f);
                uvs[4] = uvs[0] + new Vector2(UVTiling.x * widthHalfDif, UVTiling.y * lengthHalfDif);
                uvs[5] = uvs[1] + new Vector2(UVTiling.x * widthHalfDif, UVTiling.y * -lengthHalfDif);
                uvs[6] = uvs[2] + new Vector2(UVTiling.x * -widthHalfDif, UVTiling.y * -lengthHalfDif);
                uvs[7] = uvs[3] + new Vector2(UVTiling.x * -widthHalfDif, UVTiling.y * lengthHalfDif);
            }

            Vector2[] uvsBottom = uvs == null ? null : new Vector2[] { uvs[0], uvs[4], uvs[7], uvs[3] };
            Vector2[] uvsLeft = uvs == null ? null : new Vector2[] { uvs[0], uvs[1], uvs[5], uvs[4] };
            Vector2[] uvsTop = uvs == null ? null : new Vector2[] { uvs[5], uvs[1], uvs[2], uvs[6] };
            Vector2[] uvsRight = uvs == null ? null : new Vector2[] { uvs[7], uvs[6], uvs[2], uvs[3] };
            if (uvs != null && (mirrorUV ^ flipNormals))
            {
                uvsBottom = uvsBottom.Reverse().ToArray();
                uvsLeft = uvsLeft.Reverse().ToArray();
                uvsTop = uvsTop.Reverse().ToArray();
                uvsRight = uvsRight.Reverse().ToArray();
            }

            if (flipNormals)
            {
                pointsBottom = pointsBottom.Reverse().ToArray();
                pointsLeft = pointsLeft.Reverse().ToArray();
                pointsTop = pointsTop.Reverse().ToArray();
                pointsRight = pointsRight.Reverse().ToArray();
                m_up *= -1;
            }

            BuildSurfaceRectangle(pointsBottom, uvsBottom, m_up, widthSegs, seg, false);
            BuildSurfaceRectangle(pointsLeft, uvsLeft, m_up, seg, lengthSegs, false);
            BuildSurfaceRectangle(pointsTop, uvsTop, m_up, widthSegs, seg, false);
            BuildSurfaceRectangle(pointsRight, uvsRight, m_up, seg, lengthSegs, false);
        }

        public void CreateSurfaceCross(Vector3 pivotOffset, Vector3 m_forward, Vector3 m_right, float width, float length, float fillet, bool hollow, int widthSegs, int lengthSegs, int filletSegs, bool generateMappingCoords, bool realWorldMapSize, Vector2 UVOffset, Vector2 UVTiling, bool flipNormals, bool mirrorUV = false)
        {
            Vector3 m_up = Vector3.Cross(m_forward, m_right).normalized;
            float lengthHalf = length * 0.5f;
            float widthHalf = width * 0.5f;
            float centerLengthHalf = lengthHalf - fillet;
            float centerWidthHalf = widthHalf - fillet;
            float centerLength = centerLengthHalf * 2.0f;
            float centerWidth = centerWidthHalf * 2.0f;

            //points 
            //   7-8   
            //  /| |\  
            // 6-1-2-9 
            // | | | | 
            // 5-0-3-A 
            //  \| |/  
            //   4-B   
            Vector3[] points = new Vector3[12];
            points[0] = pivotOffset - m_right * centerWidthHalf - m_forward * centerLengthHalf;
            points[1] = points[0] + m_forward * centerLength;
            points[2] = points[1] + m_right * centerWidth;
            points[3] = points[0] + m_right * centerWidth;
            points[4] = points[0] - m_forward * fillet;
            points[5] = points[0] - m_right * fillet;
            points[6] = points[1] - m_right * fillet;
            points[7] = points[1] + m_forward * fillet;
            points[8] = points[2] + m_forward * fillet;
            points[9] = points[2] + m_right * fillet;
            points[10] = points[3] + m_right * fillet;
            points[11] = points[3] - m_forward * fillet;

            Vector3[] pointsCenter = new Vector3[] { points[0], points[1], points[2], points[3] };
            Vector3[] pointsBottom = new Vector3[] { points[4], points[0], points[3], points[11] };
            Vector3[] pointsLeft = new Vector3[] { points[5], points[6], points[1], points[0] };
            Vector3[] pointsTop = new Vector3[] { points[1], points[7], points[8], points[2] };
            Vector3[] pointsRight = new Vector3[] { points[3], points[2], points[9], points[10] };

            //uvs
            //   7-8   
            //  /| |\  
            // 6-1-2-9 
            // | | | | 
            // 5-0-3-A 
            //  \| |/  
            //   4-B   
            Vector2[] uvs = null;
            float filletDifX = fillet / width;
            float filletDifY = fillet / length;
            float centerDifX = centerWidth / width;
            float centerDifY = centerLength / length;
            if (generateMappingCoords)
            {
                if (realWorldMapSize)
                {
                    UVTiling.x *= width;
                    UVTiling.y *= length;
                }
                uvs = new Vector2[12];
                uvs[0] = UVOffset + new Vector2(UVTiling.x * filletDifX, UVTiling.y * filletDifY);
                uvs[1] = uvs[0] + new Vector2(0.0f, UVTiling.y * centerDifY);
                uvs[2] = uvs[1] + new Vector2(UVTiling.x * centerDifX, 0.0f);
                uvs[3] = uvs[0] + new Vector2(UVTiling.x * centerDifX, 0.0f);
                uvs[4] = uvs[0] + new Vector2(0.0f, UVTiling.y * -filletDifY);
                uvs[5] = uvs[0] + new Vector2(UVTiling.x * -filletDifX, 0.0f);
                uvs[6] = uvs[1] + new Vector2(UVTiling.x * -filletDifX, 0.0f);
                uvs[7] = uvs[1] + new Vector2(0.0f, UVTiling.y * filletDifY);
                uvs[8] = uvs[2] + new Vector2(0.0f, UVTiling.y * filletDifY);
                uvs[9] = uvs[2] + new Vector2(UVTiling.x * filletDifX, 0.0f);
                uvs[10] = uvs[3] + new Vector2(UVTiling.x * filletDifX, 0.0f);
                uvs[11] = uvs[3] + new Vector2(0.0f, UVTiling.y * -filletDifY);
            }

            Vector2[] uvsCenter = new Vector2[] { uvs[0], uvs[1], uvs[2], uvs[3] };
            Vector2[] uvsBottom = new Vector2[] { uvs[4], uvs[0], uvs[3], uvs[11] };
            Vector2[] uvsLeft =   new Vector2[] { uvs[5], uvs[6], uvs[1], uvs[0] };
            Vector2[] uvsTop =    new Vector2[] { uvs[1], uvs[7], uvs[8], uvs[2] };
            Vector2[] uvsRight =  new Vector2[] { uvs[3], uvs[2], uvs[9], uvs[10] };
            if (uvs != null && (mirrorUV ^ flipNormals))
            {
                uvsCenter = uvsCenter.Reverse().ToArray();
                uvsBottom = uvsBottom.Reverse().ToArray();
                uvsLeft = uvsLeft.Reverse().ToArray();
                uvsTop = uvsTop.Reverse().ToArray();
                uvsRight = uvsRight.Reverse().ToArray();
            }

            if (flipNormals)
            {
                pointsCenter = pointsCenter.Reverse().ToArray();
                pointsBottom = pointsBottom.Reverse().ToArray();
                pointsLeft = pointsLeft.Reverse().ToArray();
                pointsTop = pointsTop.Reverse().ToArray();
                pointsRight = pointsRight.Reverse().ToArray();
                m_up *= -1;
            }

            if (!hollow) BuildSurfaceRectangle(pointsCenter, uvsCenter, m_up, widthSegs, lengthSegs);
            BuildSurfaceRectangle(pointsBottom, uvsBottom, m_up, widthSegs, filletSegs);
            BuildSurfaceRectangle(pointsLeft, uvsLeft, m_up, filletSegs, lengthSegs);
            BuildSurfaceRectangle(pointsTop, uvsTop, m_up, widthSegs, filletSegs);
            BuildSurfaceRectangle(pointsRight, uvsRight, m_up, filletSegs, lengthSegs);
        }

        public void CreateSurfaceCross(Vector3 pivotOffset, Vector3 m_forward, Vector3 m_right, float width, float length, float fillet, float thickness, int widthSegs, int lengthSegs, int filletSegs, bool generateMappingCoords, bool realWorldMapSize, Vector2 UVOffset, Vector2 UVTiling, bool flipNormals, bool mirrorUV = false)
        {
            Vector3 m_up = Vector3.Cross(m_forward, m_right).normalized;
            float lengthHalf = length * 0.5f;
            float widthHalf = width * 0.5f;
            float centerLengthHalf = lengthHalf - fillet;
            float centerWidthHalf = widthHalf - fillet;
            float centerLength = centerLengthHalf * 2.0f;
            float centerWidth = centerWidthHalf * 2.0f;

            //points 
            //   7-8   
            //  /| |\  
            // 6-1-2-9 
            // | | | | 
            // 5-0-3-A 
            //  \| |/  
            //   4-B   
            Vector3[] points = new Vector3[12];
            points[0] = pivotOffset - m_right * centerWidthHalf - m_forward * centerLengthHalf;
            points[1] = points[0] + m_forward * centerLength;
            points[2] = points[1] + m_right * centerWidth;
            points[3] = points[0] + m_right * centerWidth;
            points[4] = points[0] - m_forward * fillet;
            points[5] = points[0] - m_right * fillet;
            points[6] = points[1] - m_right * fillet;
            points[7] = points[1] + m_forward * fillet;
            points[8] = points[2] + m_forward * fillet;
            points[9] = points[2] + m_right * fillet;
            points[10] = points[3] + m_right * fillet;
            points[11] = points[3] - m_forward * fillet;

            Vector3 offsetUp = m_forward * (fillet - thickness);
            Vector3 offsetRight = m_right * (fillet - thickness);
            Vector3[] pointsBottom = new Vector3[] { points[4], points[0] - offsetUp, points[3] - offsetUp, points[11] };
            Vector3[] pointsLeft = new Vector3[] { points[5], points[6], points[1] - offsetRight, points[0] - offsetRight };
            Vector3[] pointsTop = new Vector3[] { points[1] + offsetUp, points[7], points[8], points[2] + offsetUp };
            Vector3[] pointsRight = new Vector3[] { points[3] + offsetRight, points[2] + offsetRight, points[9], points[10] };

            //uvs
            //   7-8   
            //  /| |\  
            // 6-1-2-9 
            // | | | | 
            // 5-0-3-A 
            //  \| |/  
            //   4-B   
            Vector2[] uvs = null;
            Vector2[] uvsBottom = null;
            Vector2[] uvsLeft   = null;
            Vector2[] uvsTop    = null;
            Vector2[] uvsRight  = null;
            if (generateMappingCoords)
            {
                if (realWorldMapSize)
                {
                    UVTiling.x *= width;
                    UVTiling.y *= length;
                }

                float filletDifX = fillet / width;
                float filletDifY = fillet / length;
                float centerDifX = centerWidth / width;
                float centerDifY = centerLength / length;
                float offsetDifX = (fillet - thickness) / width;
                float offsetDifY = (fillet - thickness) / length;

                uvs = new Vector2[12];
                uvs[0] = UVOffset + new Vector2(UVTiling.x * filletDifX, UVTiling.y * filletDifY);
                uvs[1] = uvs[0] + new Vector2(0.0f, UVTiling.y * centerDifY);
                uvs[2] = uvs[1] + new Vector2(UVTiling.x * centerDifX, 0.0f);
                uvs[3] = uvs[0] + new Vector2(UVTiling.x * centerDifX, 0.0f);
                uvs[4] = uvs[0] + new Vector2(0.0f, UVTiling.y * -filletDifY);
                uvs[5] = uvs[0] + new Vector2(UVTiling.x * -filletDifX, 0.0f);
                uvs[6] = uvs[1] + new Vector2(UVTiling.x * -filletDifX, 0.0f);
                uvs[7] = uvs[1] + new Vector2(0.0f, UVTiling.y * filletDifY);
                uvs[8] = uvs[2] + new Vector2(0.0f, UVTiling.y * filletDifY);
                uvs[9] = uvs[2] + new Vector2(UVTiling.x * filletDifX, 0.0f);
                uvs[10] = uvs[3] + new Vector2(UVTiling.x * filletDifX, 0.0f);
                uvs[11] = uvs[3] + new Vector2(0.0f, UVTiling.y * -filletDifY);

                Vector2 uvOffsetRight = new Vector2(UVTiling.x * offsetDifX, 0.0f);
                Vector2 uvOffsetUp = new Vector2(0.0f, UVTiling.y * offsetDifY);
                uvsBottom = new Vector2[] { uvs[4], uvs[0] - uvOffsetUp, uvs[3] - uvOffsetUp, uvs[11] };
                uvsLeft   = new Vector2[] { uvs[5], uvs[6], uvs[1] - uvOffsetRight, uvs[0] - uvOffsetRight };
                uvsTop    = new Vector2[] { uvs[1] + uvOffsetUp, uvs[7], uvs[8], uvs[2] + uvOffsetUp };
                uvsRight  = new Vector2[] { uvs[3] + uvOffsetRight, uvs[2] + uvOffsetRight, uvs[9], uvs[10] };
            }

            if (uvs != null && (mirrorUV ^ flipNormals))
            {
                uvsBottom = uvsBottom.Reverse().ToArray();
                uvsLeft = uvsLeft.Reverse().ToArray();
                uvsTop = uvsTop.Reverse().ToArray();
                uvsRight = uvsRight.Reverse().ToArray();
            }

            if (flipNormals)
            {
                pointsBottom = pointsBottom.Reverse().ToArray();
                pointsLeft = pointsLeft.Reverse().ToArray();
                pointsTop = pointsTop.Reverse().ToArray();
                pointsRight = pointsRight.Reverse().ToArray();
                m_up *= -1;
            }

            BuildSurfaceRectangle(pointsBottom, uvsBottom, m_up, widthSegs, filletSegs);
            BuildSurfaceRectangle(pointsLeft, uvsLeft, m_up, filletSegs, lengthSegs);
            BuildSurfaceRectangle(pointsTop, uvsTop, m_up, widthSegs, filletSegs);
            BuildSurfaceRectangle(pointsRight, uvsRight, m_up, filletSegs, lengthSegs);
        }

        public void CreateSurfaceHemiRing(Vector3 pivotOffset, Vector3 m_forward, Vector3 m_right, bool isLeft, float radius1, float radius2, int sides, int seg, bool isSector, bool cutOn, float cutFrom, float cutTo, bool generateMappingCoords, bool realWorldMapSize, Vector2 UVOffset, Vector2 UVTiling, bool flipNormals, bool mirrorUV = false)
        {
            if (radius1 + radius2 < 0.0001f) return;

            Vector3 m_up = Vector3.Cross(m_forward, m_right).normalized;
            float radiusT = radius2 / radius1;
            float angleStart = Mathf.PI;
            float angle = Mathf.PI / sides;
            float min = (radius1 - radius2) / (radius1 * 2);
            float max = (radius1 + radius2) / (radius1 * 2);
            float cutFrom2 = Mathf.Clamp01((cutFrom - min) / (max - min));
            float cutTo2 = Mathf.Clamp01((cutTo - min) / (max - min));
            if (!isLeft)
            {
                angleStart = 0.0f;
                float temp = cutFrom;
                cutFrom = 1.0f - cutTo;
                cutTo = 1.0f - temp;
                temp = cutFrom2;
                cutFrom2 = 1.0f - cutTo2;
                cutTo2 = 1.0f - temp;
            }

            //points
            //  n,0  
            //  / \  
            // 3 c 1 
            //  \ /  
            //   2   
            List<Vector2> sinCos = new List<Vector2>();
            List<Vector2> sinCos2 = new List<Vector2>();
            if (cutOn)
            {
                cutFrom = (cutFrom - 0.5f) * 2.0f; //convert to -1 to 1
                cutTo = (cutTo - 0.5f) * 2.0f; //convert to -1 to 1
                cutFrom2 = (cutFrom2 - 0.5f) * 2.0f; //convert to -1 to 1
                cutTo2 = (cutTo2 - 0.5f) * 2.0f; //convert to -1 to 1
                float angleFrom = Mathf.PI - Mathf.Acos(cutFrom);
                float angleTo = Mathf.PI - Mathf.Acos(cutTo);
                float angleFrom2 = Mathf.PI - Mathf.Acos(cutFrom2);
                float angleTo2 = Mathf.PI - Mathf.Acos(cutTo2);
                float angleCutFrom = angleStart + angleFrom;
                float angleCutTo = angleStart + angleTo;
                float angleCutFrom2 = angleStart + angleFrom2;
                float angleCutTo2 = angleStart + angleTo2;

                if (isSector)
                {
                    int indexFrom = Mathf.FloorToInt(angleFrom / angle) + 1;
                    int indexTo = Mathf.CeilToInt(angleTo / angle) - 1;

                    Vector2 sc = new Vector2(Mathf.Sin(angleCutFrom), Mathf.Cos(angleCutFrom));
                    sinCos.Add(sc);
                    sinCos2.Add(sc * radiusT);
                    for (int i = indexFrom; i <= indexTo; ++i)
                    {
                        float rad = angleStart + angle * i;
                        sc = new Vector2(Mathf.Sin(rad), Mathf.Cos(rad));
                        sinCos.Add(sc);
                        sinCos2.Add(sc * radiusT);
                    }
                    sc = new Vector2(Mathf.Sin(angleCutTo), Mathf.Cos(angleCutTo));
                    sinCos.Add(sc);
                    sinCos2.Add(sc * radiusT);
                }
                else
                {
                    if (!isLeft) { cutFrom = -cutFrom; cutTo = -cutTo; cutFrom2 = -cutFrom2; cutTo2 = -cutTo2; } //reverse in right side

                    Vector2 sc;
                    for (int i = 0; i <= sides; ++i)
                    {
                        float rad = angleStart + angle * i;

                        //start
                        if (angleCutFrom2 <= angleCutFrom)
                        {
                            //rad < cutfrom
                            if (rad <= angleCutFrom)
                            {
                                //rad < cutfrom2 < cutfrom
                                if (rad < angleCutFrom2)
                                {
                                    //angleCutFrom2
                                    if (rad + angle > angleCutFrom2)
                                    {
                                        sc = new Vector2(Mathf.Sin(angleCutFrom2), Mathf.Cos(angleCutFrom2));
                                        sinCos.Add(new Vector2(sc.x / sc.y * cutFrom, cutFrom));
                                        sinCos2.Add(sc * radiusT);
                                    }
                                }
                                //cutfrom2 < rad < cutfrom
                                else
                                {
                                    //cutfrom2 < rad < cutto2 < cutfrom
                                    if (rad < angleCutTo2)
                                    {
                                        sc = new Vector2(Mathf.Sin(rad), Mathf.Cos(rad));
                                        sinCos.Add(new Vector2(sc.x / sc.y * cutFrom, cutFrom));
                                        sinCos2.Add(sc * radiusT);
                                        //angleCutTo2
                                        if (rad + angle > angleCutTo2)
                                        {
                                            if (angleCutTo2 < angleCutFrom)
                                            {
                                                sc = new Vector2(Mathf.Sin(angleCutTo2), Mathf.Cos(angleCutTo2));
                                                sinCos.Add(new Vector2(sc.x / sc.y * cutFrom, cutFrom));
                                                sinCos2.Add(sc * radiusT);
                                            }
                                        }
                                    }
                                    //cutfrom2 < cutto2 < rad < cutfrom
                                    else
                                    {
                                        sc = new Vector2(Mathf.Sin(rad), Mathf.Cos(rad));
                                        sinCos.Add(new Vector2(sc.x / sc.y * cutFrom, cutFrom));
                                        sinCos2.Add(new Vector2(sc.x / sc.y * cutTo, cutTo));
                                    }
                                }
                                //angleCutFrom
                                if (rad + angle > angleCutFrom)
                                {
                                    if (angleCutTo2 > angleCutFrom)
                                    {
                                        sc = new Vector2(Mathf.Sin(angleCutFrom), Mathf.Cos(angleCutFrom));
                                        sinCos.Add(sc);
                                        sinCos2.Add(sc * radiusT);
                                    }
                                    else
                                    {
                                        sc = new Vector2(Mathf.Sin(angleCutFrom), Mathf.Cos(angleCutFrom));
                                        sinCos.Add(sc);
                                        sinCos2.Add(new Vector2(sc.x / sc.y * cutTo, cutTo));
                                    }
                                }
                            }
                        }
                        else
                        {
                            //rad < cutfrom2
                            if (rad <= angleCutFrom2)
                            {
                                //rad < cutfrom < cutfrom2
                                if (rad < angleCutFrom)
                                {
                                    //angleCutFrom
                                    if (rad + angle > angleCutFrom)
                                    {
                                        sc = new Vector2(Mathf.Sin(angleCutFrom), Mathf.Cos(angleCutFrom));
                                        sinCos.Add(sc);
                                        sinCos2.Add(new Vector2(sc.x / sc.y * cutFrom, cutFrom));
                                    }
                                }
                                //cutfrom < rad < cutfrom2
                                else
                                {
                                    //cutfrom < rad < cutto < cutfrom2
                                    if (rad < angleCutTo)
                                    {
                                        sc = new Vector2(Mathf.Sin(rad), Mathf.Cos(rad));
                                        sinCos.Add(sc);
                                        sinCos2.Add(new Vector2(sc.x / sc.y * cutFrom, cutFrom));
                                        if (rad + angle > angleCutTo)
                                        {
                                            sc = new Vector2(Mathf.Sin(angleCutTo), Mathf.Cos(angleCutTo));
                                            sinCos.Add(sc);
                                            sinCos2.Add(new Vector2(sc.x / sc.y * cutFrom, cutFrom));
                                        }
                                    }
                                    //cutfrom < cutto < rad < cutfrom2
                                    else
                                    {
                                        sc = new Vector2(Mathf.Sin(rad), Mathf.Cos(rad));
                                        sinCos.Add(new Vector2(sc.x / sc.y * cutTo, cutTo));
                                        sinCos2.Add(new Vector2(sc.x / sc.y * cutFrom, cutFrom));
                                    }
                                }
                                //angleCutFrom2
                                if (rad + angle > angleCutFrom2)
                                {
                                    if (angleCutTo > angleCutFrom2)
                                    {
                                        sc = new Vector2(Mathf.Sin(angleCutFrom2), Mathf.Cos(angleCutFrom2));
                                        sinCos.Add(sc);
                                        sinCos2.Add(sc * radiusT);
                                    }
                                    else
                                    {
                                        sc = new Vector2(Mathf.Sin(angleCutFrom2), Mathf.Cos(angleCutFrom2));
                                        sinCos.Add(new Vector2(sc.x / sc.y * cutTo, cutTo));
                                        sinCos2.Add(sc * radiusT);
                                    }
                                }
                            }
                        }

                        //end
                        if (angleCutTo2 >= angleCutTo)
                        {
                            //rad > cutto
                            if (rad >= angleCutTo)
                            {
                                //angleCutTo
                                if (rad - angle < angleCutTo)
                                {
                                    if (angleCutTo > angleCutFrom2)
                                    {
                                        sc = new Vector2(Mathf.Sin(angleCutTo), Mathf.Cos(angleCutTo));
                                        sinCos.Add(sc);
                                        sinCos2.Add(sc * radiusT);
                                    }
                                }
                                //rad > cutto2 > cutto
                                if (rad > angleCutTo2)
                                {
                                    //angleCutTo2
                                    if (rad - angle < angleCutTo2)
                                    {
                                        sc = new Vector2(Mathf.Sin(angleCutTo2), Mathf.Cos(angleCutTo2));
                                        sinCos.Add(new Vector2(sc.x / sc.y * cutTo, cutTo));
                                        sinCos2.Add(sc * radiusT);
                                    }
                                }
                                //cutto2 > rad > cutto
                                else
                                {
                                    //cutto2 > rad > cutfrom2 > cutto
                                    if (rad > angleCutFrom2)
                                    {
                                        sc = new Vector2(Mathf.Sin(rad), Mathf.Cos(rad));
                                        sinCos.Add(new Vector2(sc.x / sc.y * cutTo, cutTo));
                                        sinCos2.Add(sc * radiusT);
                                    }
                                }
                            }
                        }
                        else
                        {
                            //rad > cutto2
                            if (rad >= angleCutTo2)
                            {
                                //angleCutTo2
                                if (rad - angle < angleCutTo2)
                                {
                                    if (angleCutTo2 > angleCutFrom)
                                    {
                                        sc = new Vector2(Mathf.Sin(angleCutTo2), Mathf.Cos(angleCutTo2));
                                        sinCos.Add(sc);
                                        sinCos2.Add(sc * radiusT);
                                    }
                                }
                                //rad > cutto > cutto2
                                if (rad > angleCutTo)
                                {
                                    //angleCutTo
                                    if (rad - angle < angleCutTo)
                                    {
                                        sc = new Vector2(Mathf.Sin(angleCutTo), Mathf.Cos(angleCutTo));
                                        sinCos.Add(sc);
                                        sinCos2.Add(new Vector2(sc.x / sc.y * cutTo, cutTo));
                                    }
                                }
                                //cutto > rad > cutto2
                                else
                                {
                                    //cutto > rad > cutfrom > cutto2
                                    if (rad > angleCutFrom)
                                    {
                                        sc = new Vector2(Mathf.Sin(rad), Mathf.Cos(rad));
                                        sinCos.Add(sc);
                                        sinCos2.Add(new Vector2(sc.x / sc.y * cutTo, cutTo));
                                    }
                                }
                            }
                        }

                        //mid
                        if (rad > angleCutFrom && rad < angleCutTo &&
                            rad > angleCutFrom2 && rad < angleCutTo2)
                        {
                            sc = new Vector2(Mathf.Sin(rad), Mathf.Cos(rad));
                            sinCos.Add(sc);
                            sinCos2.Add(sc * radiusT);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i <= sides; ++i)
                {
                    float rad = angleStart + angle * i;
                    Vector2 sc = new Vector2(Mathf.Sin(rad), Mathf.Cos(rad));
                    sinCos.Add(sc);
                    sinCos2.Add(sc * radiusT);
                }
            }

            Vector3[] points = new Vector3[sinCos.Count];
            Vector3[] points2 = new Vector3[sinCos2.Count];
            for (int i = 0; i < sinCos.Count; ++i)
            {
                points[i] = pivotOffset + (m_forward * sinCos[i].y + m_right * sinCos[i].x) * radius1;
                points2[i] = pivotOffset + (m_forward * sinCos2[i].y + m_right * sinCos2[i].x) * radius1;
            }

            //uvs
            //  n,0  
            //  / \  
            // 3 c 1 
            //  \ /  
            //   2   
            Vector2[] uvs = null;
            Vector2[] uvs2 = null;
            if (generateMappingCoords)
            {
                if (realWorldMapSize)
                {
                    UVTiling *= radius1 * 2.0f;
                }
                float xDir = mirrorUV ? -1 : 1;
                Vector2 uvCenter = new Vector2(UVOffset.x + UVTiling.x * 0.5f, UVOffset.y + UVTiling.y * 0.5f);
                uvs = new Vector2[sinCos.Count];
                uvs2 = new Vector2[sinCos.Count];
                for (int i = 0; i < sinCos.Count; ++i)
                {
                    uvs[i] = uvCenter + new Vector2(sinCos[i].x * xDir * UVTiling.x, sinCos[i].y * UVTiling.y) * 0.5f;
                    uvs2[i] = uvCenter + new Vector2(sinCos2[i].x * xDir * UVTiling.x, sinCos2[i].y * UVTiling.y) * 0.5f;
                }
                if (flipNormals) { uvs = uvs.Reverse().ToArray(); uvs2 = uvs2.Reverse().ToArray(); }
            }

            if (flipNormals)
            {
                points = points.Reverse().ToArray();
                points2 = points2.Reverse().ToArray();
                m_up *= -1;
            }

            BuildSurfaceUpDownPoints(points, points2, uvs, uvs2, m_up, seg);
        }

        public void CreateSurfaceCylinder(Vector3 pivotOffset, Vector3 m_forward, Vector3 m_right, float height, float radius, int sides, int heightSegs, bool sliceOn, float sliceFrom, float sliceTo, bool generateMappingCoords, bool realWorldMapSize, Vector2 UVOffset, Vector2 UVTiling, bool flipNormals, bool smooth, bool mirrorUV = true, bool horizontalUV = false)
        {
            if (radius < 0.0001f) return;

            if (!sliceOn) { sliceFrom = 0.0f; sliceTo = 360.0f; }
            float angleStart = sliceFrom * Mathf.Deg2Rad;
            float totalRad = (sliceTo - sliceFrom) * Mathf.Deg2Rad;
            float angle = totalRad / sides;
            float heightHalf = height * 0.5f;
            Vector3 m_up = Vector3.Cross(m_forward, m_right).normalized;
            Vector3 downCenter = pivotOffset - m_up * heightHalf;
            Vector3 upCenter = pivotOffset + m_up * heightHalf;

            //points
            //  n,0  
            //  / \  
            // 3 c 1 
            //  \ /  
            //   2   
            float normalUp = flipNormals ? -1.0f : 1.0f;
            Vector3[] upPoints = new Vector3[sides + 1];
            Vector3[] downPoints = new Vector3[sides + 1];
            Vector3[] normals = new Vector3[sides + 1];
            for (int i = 0; i <= sides; ++i)
            {
                float rad = angleStart + angle * i;
                float sin = Mathf.Sin(rad);
                float cos = Mathf.Cos(rad);

                Vector3 v = m_forward * cos + m_right * sin;
                upPoints[i] = upCenter + v * radius;
                downPoints[i] = downCenter + v * radius;
                normals[i] = v * normalUp;
            }

            //uvs   | horizontal
            // 1-2  | 3-2 
            // | |  | | | 
            // 0-3  | 0-1 
            Vector2[] uvs = null;
            if (generateMappingCoords)
            {
                uvs = new Vector2[4];
                if (horizontalUV)
                {
                    if (realWorldMapSize)
                    {
                        UVTiling.y *= totalRad * radius;
                        UVTiling.x *= height;
                    }
                    uvs[0] = UVOffset;
                    uvs[3] = uvs[0] + new Vector2(0.0f, UVTiling.y);
                    uvs[2] = uvs[3] + new Vector2(UVTiling.x, 0.0f);
                    uvs[1] = uvs[0] + new Vector2(UVTiling.x, 0.0f);
                }
                else
                {
                    if (realWorldMapSize)
                    {
                        UVTiling.x *= totalRad * radius;
                        UVTiling.y *= height;
                    }
                    uvs[0] = UVOffset;
                    uvs[1] = uvs[0] + new Vector2(0.0f, UVTiling.y);
                    uvs[2] = uvs[1] + new Vector2(UVTiling.x, 0.0f);
                    uvs[3] = uvs[0] + new Vector2(UVTiling.x, 0.0f);
                }
                if (mirrorUV ^ flipNormals) uvs = uvs.Reverse().ToArray();
            }

            if (flipNormals)
            {
                downPoints = downPoints.Reverse().ToArray();
                upPoints = upPoints.Reverse().ToArray();
                normals = normals.Reverse().ToArray();
            }

            if (smooth) BuildSurfaceUpDownPointsSmooth(upPoints, downPoints, uvs, normals, heightSegs);
            else BuildSurfaceUpDownPointsHard(upPoints, downPoints, uvs, heightSegs);
        }

        public void CreateSurfaceCylinderSlanted(Vector3 pivotOffset, Vector3 m_forward, Vector3 m_right, float height, float radius, int sides, int heightSegs, bool sliceOn, float sliceFrom, float sliceTo, bool generateMappingCoords, bool realWorldMapSize, Vector2 UVOffset, Vector2 UVTiling, bool flipNormals, bool smooth, bool mirrorUV = true)
        {
            if (radius < 0.0001f) return;

            if (!sliceOn) { sliceFrom = 0.0f; sliceTo = 360.0f; }
            float angleStart = sliceFrom * Mathf.Deg2Rad;
            float totalRad = (sliceTo - sliceFrom) * Mathf.Deg2Rad;
            float angle = totalRad / sides;
            float heightHalf = height * 0.5f;
            float heightSide = height / sides;
            Vector3 m_up = Vector3.Cross(m_forward, m_right).normalized;
            Vector3 downCenter = pivotOffset - m_up * heightHalf;
            Vector3 upCenter = pivotOffset + m_up * heightHalf;

            //points
            //  n,0  
            //  / \  
            // 3 c 1 
            //  \ /  
            //   2   
            float normalUp = flipNormals ? -1.0f : 1.0f;
            Vector3[] upPoints = new Vector3[sides + 1];
            Vector3[] downPoints = new Vector3[sides + 1];
            Vector3[] normals = new Vector3[sides + 1];
            for (int i = 0; i <= sides; ++i)
            {
                float rad = angleStart + angle * i;
                float sin = Mathf.Sin(rad);
                float cos = Mathf.Cos(rad);

                Vector3 v = m_forward * cos + m_right * sin;
                upPoints[i] = upCenter + v * radius;
                downPoints[i] = downCenter + v * radius + m_up * heightSide * i;
                normals[i] = v * normalUp;
            }

            //uvs 
            // 1-2
            // | |
            // 0-3
            Vector2[] uvsUp = new Vector2[sides + 1];
            Vector2[] uvsDown = new Vector2[sides + 1];
            if (generateMappingCoords)
            {
                if (realWorldMapSize)
                {
                    UVTiling.x *= totalRad * radius;
                    UVTiling.y *= height;
                }

                float uvx = UVTiling.x / sides;
                float uvy = UVTiling.y / sides;
                for (int i = 0; i <= sides; ++i)
                {
                    uvsUp[i] = new Vector2(uvx * i, UVTiling.y);
                    uvsDown[i] = new Vector2(uvx * i, UVTiling.y - uvy * i);
                }

                if (mirrorUV ^ flipNormals)
                {
                    uvsUp = uvsUp.Reverse().ToArray();
                    uvsDown = uvsDown.Reverse().ToArray();
                }
            }

            if (flipNormals)
            {
                downPoints = downPoints.Reverse().ToArray();
                upPoints = upPoints.Reverse().ToArray();
                normals = normals.Reverse().ToArray();
            }

            if (smooth) BuildSurfaceUpDownPointsSmooth(upPoints, downPoints, uvsUp, uvsDown, normals, heightSegs);
            else BuildSurfaceUpDownPointsHard(upPoints, downPoints, uvsUp, uvsDown, heightSegs, false);
        }

        public void CreateSurfaceCone(Vector3 pivotOffset, Vector3 m_forward, Vector3 m_right, float height, float radius1, float radius2, int sides, int heightSegs, bool sliceOn, float sliceFrom, float sliceTo, bool generateMappingCoords, bool realWorldMapSize, Vector2 UVOffset, Vector2 UVTiling, bool flipNormals, bool smooth, bool mirrorUV = true)
        {
            Vector3 m_up = Vector3.Cross(m_forward, m_right).normalized;
            Vector3 downCenter = pivotOffset - m_up * height * 0.5f;
            Vector3 upCenter = pivotOffset + m_up * height * 0.5f;
            CreateSurfaceCone(downCenter, upCenter, m_forward, m_right, height, radius1, radius2, sides, heightSegs, sliceOn, sliceFrom, sliceTo, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals, smooth, mirrorUV);
        }

        public void CreateSurfaceCone(Vector3 downCenter, Vector3 upCenter, Vector3 m_forward, Vector3 m_right, float height, float radius1, float radius2, int sides, int heightSegs, bool sliceOn, float sliceFrom, float sliceTo, bool generateMappingCoords, bool realWorldMapSize, Vector2 UVOffset, Vector2 UVTiling, bool flipNormals, bool smooth, bool mirrorUV = true)
        {
            if (radius1 + radius2 < 0.0001f) return;

            if (!sliceOn) { sliceFrom = 0.0f; sliceTo = 360.0f; }
            float angleStart = sliceFrom * Mathf.Deg2Rad;
            float totalRad = (sliceTo - sliceFrom) * Mathf.Deg2Rad;
            float angle = totalRad / sides;
            Vector3 m_up = Vector3.Cross(m_forward, m_right).normalized;

            //points
            //  n,0  
            //  / \  
            // 3 c 1 
            //  \ /  
            //   2   
            float normalUp = flipNormals ? -1.0f : 1.0f;
            Vector3[] downPoints = new Vector3[sides + 1];
            Vector3[] upPoints = new Vector3[sides + 1];
            Vector3[] normals = new Vector3[sides + 1];
            for (int i = 0; i <= sides; ++i)
            {
                float rad = angleStart + angle * i;
                float sin = Mathf.Sin(rad);
                float cos = Mathf.Cos(rad);

                Vector3 v = m_forward * cos + m_right * sin;
                upPoints[i] = upCenter + v * radius2;
                downPoints[i] = downCenter + v * radius1;
                Vector3 ve = upPoints[i] - downPoints[i];
                Vector3 vr = v * (radius1 + radius2);
                Vector3 vup = m_up * (radius1 + radius2) * PPMathLib.PointLineProjection(ve, -vr) / PPMathLib.PointLineProjection(ve, m_up);
                normals[i] = (vr + vup).normalized * normalUp;
            }

            //uvs
            // 1-2 
            // | | 
            // 0-3 
            Vector2[] uvs = null;
            if (generateMappingCoords)
            {
                if (realWorldMapSize)
                {
                    UVTiling.x *= totalRad * (radius1 + radius2) * 0.5f; 
                    UVTiling.y *= height;
                }
                uvs = new Vector2[4];
                uvs[0] = UVOffset;
                uvs[1] = uvs[0] + new Vector2(0.0f, UVTiling.y);
                uvs[2] = uvs[1] + new Vector2(UVTiling.x, 0.0f);
                uvs[3] = uvs[0] + new Vector2(UVTiling.x, 0.0f);
                if (mirrorUV ^ flipNormals) uvs = uvs.Reverse().ToArray();
            }

            if (flipNormals)
            {
                downPoints = downPoints.Reverse().ToArray();
                upPoints = upPoints.Reverse().ToArray();
                normals = normals.Reverse().ToArray();
            }

            if (smooth) BuildSurfaceUpDownPointsSmooth(upPoints, downPoints, uvs, normals, heightSegs);
            else BuildSurfaceUpDownPointsHard(upPoints, downPoints, uvs, heightSegs, false);
        }

        public void CreateSurfaceSphere(Vector3 pivotOffset, Vector3 m_forward, Vector3 m_right, float radius, int sides, int seg, bool sliceOn, float sliceFrom, float sliceTo, bool cutOn, float cutFrom, float cutTo, bool generateMappingCoords, bool realWorldMapSize, Vector2 UVOffset, Vector2 UVTiling, bool flipNormals, bool smooth, bool mirrorUV = true)
        {
            if (radius < 0.0001f) return;

            if (!sliceOn) { sliceFrom = 0.0f; sliceTo = 360.0f; }
            float angleStart = sliceFrom * Mathf.Deg2Rad;
            float totalRad = (sliceTo - sliceFrom) * Mathf.Deg2Rad;
            float angle = totalRad / sides;
            float secAngleStart = Mathf.PI;
            float secTotalRad = Mathf.PI;
            float secAngle = secTotalRad / seg;
            Vector3 m_up = Vector3.Cross(m_forward, m_right).normalized;

            //section
            //  n,0  
            //  / \  
            // 3 c 1 
            //  \ /  
            //   2   
            float[] uvv = null;
            List<Vector2> secSinCos = new List<Vector2>();
            if (cutOn)
            {
                //float angleCutFrom = Mathf.PPMathLib.Lerp(180.0f, 360.0f, cutFrom) * Mathf.Deg2Rad;
                //float angleCutTo = Mathf.PPMathLib.Lerp(180.0f, 360.0f, cutTo) * Mathf.Deg2Rad;
                cutFrom = (cutFrom - 0.5f) * 2.0f; //convert to -1 to 1
                cutTo = (cutTo - 0.5f) * 2.0f; //convert to -1 to 1
                float angleCutFrom = secAngleStart + (Mathf.PI - Mathf.Acos(cutFrom));
                float angleCutTo = secAngleStart + (Mathf.PI - Mathf.Acos(cutTo));
                float cutTotalRad = angleCutTo - angleCutFrom;
                cutFrom = (angleCutFrom - secAngleStart) / secTotalRad; //uv cutfrom
                cutTo = (angleCutTo - secAngleStart) / secTotalRad; //uv cutto
                int indexFrom = Mathf.FloorToInt((angleCutFrom - secAngleStart) / secAngle) + 1;
                int indexTo = Mathf.CeilToInt((angleCutTo - secAngleStart) / secAngle) - 1;
                List<float> uvvList = new List<float>();

                secSinCos.Add(new Vector2(Mathf.Sin(angleCutFrom), Mathf.Cos(angleCutFrom)));
                uvvList.Add(0.0f);
                float counter = (secAngle * indexFrom - (angleCutFrom - secAngleStart)) / cutTotalRad;
                for (int i = indexFrom; i <= indexTo; ++i)
                {
                    float rad = secAngleStart + secAngle * i;
                    secSinCos.Add(new Vector2(Mathf.Sin(rad), Mathf.Cos(rad)));
                    uvvList.Add(counter);
                    counter += secAngle / cutTotalRad;
                }
                secSinCos.Add(new Vector2(Mathf.Sin(angleCutTo), Mathf.Cos(angleCutTo)));
                uvvList.Add(1.0f);
                uvv = uvvList.ToArray();
            }
            else
            {
                cutFrom = 0.0f;
                cutTo = 1.0f;
                for (int i = 0; i <= seg; ++i)
                {
                    float rad = secAngleStart + secAngle * i;
                    secSinCos.Add(new Vector2(Mathf.Sin(rad), Mathf.Cos(rad)));
                }
            }

            //pivotOffset
            //  n,0  
            //  / \  
            // 3 c 1 
            //  \ /  
            //   2   
            Vector3[] centers = new Vector3[sides + 1];
            for (int i = 0; i <= sides; ++i)
            {
                float rad = angleStart + angle * i;
                float sin = Mathf.Sin(rad);
                float cos = Mathf.Cos(rad);
                centers[i] = pivotOffset + (m_forward * cos + m_right * sin) * radius;
            }

            //uvs
            // 1-2 
            // | | 
            // 0-3 
            Vector2[] uvs = null;
            if (generateMappingCoords)
            {
                if (realWorldMapSize)
                {
                    UVTiling.x *= totalRad * radius; 
                    UVTiling.y *= Mathf.PI * radius;
                }
                uvs = new Vector2[4];
                uvs[0] = new Vector2(UVOffset.x, UVOffset.y + cutFrom * UVTiling.y);
                uvs[1] = uvs[0] + new Vector2(0.0f, (cutTo - cutFrom) * UVTiling.y);
                uvs[2] = uvs[1] + new Vector2(UVTiling.x, 0.0f);
                uvs[3] = uvs[0] + new Vector2(UVTiling.x, 0.0f);
                if (mirrorUV ^ flipNormals) uvs = uvs.Reverse().ToArray();
            }

            float normalUp = 1.0f;
            if (flipNormals)
            {
                centers = centers.Reverse().ToArray();
                normalUp = -1;
            }

            //points
            Vector3[,] points = new Vector3[sides + 1, secSinCos.Count];
            Vector3[,] normals = new Vector3[sides + 1, secSinCos.Count];
            for (int i = 0; i <= sides; ++i)
            {
                Vector3 secRight = (pivotOffset - centers[i]) / radius;
                for (int j = 0; j < secSinCos.Count; ++j)
                {
                    points[i, j] = pivotOffset + (m_up * secSinCos[j].y + secRight * secSinCos[j].x) * radius;
                    normals[i, j] = (points[i, j] - pivotOffset).normalized * normalUp;
                }
            }

            if (smooth) BuildSurfaceFullPointsSmooth(points, uvs, normals, uvv);
            else BuildSurfaceFullPointsHard(points, uvs, uvv);
        }

        public void CreateSurfaceTorus(Vector3 pivotOffset, Vector3 m_forward, Vector3 m_right, float radius1, float radius2, int sides, int seg, bool sliceOn, float sliceFrom, float sliceTo, float sectionSliceFrom, float sectionSliceTo, bool generateMappingCoords, bool realWorldMapSize, Vector2 UVOffset, Vector2 UVTiling, bool flipNormals, bool smooth, bool mirrorUV = true)
        {
            if (radius2 < 0.0001f) return;
            if (!sliceOn) { sliceFrom = 0.0f; sliceTo = 360.0f; }
            radius1 += 0.00001f;
            sectionSliceFrom += 90.0f;
            sectionSliceTo += 90.0f;

            float angleStart = sliceFrom * Mathf.Deg2Rad;
            float totalRad = (sliceTo - sliceFrom) * Mathf.Deg2Rad;
            float angle = totalRad / sides;
            float secAngleStart = sectionSliceFrom * Mathf.Deg2Rad;
            float secTotalRad = (sectionSliceTo - sectionSliceFrom) * Mathf.Deg2Rad;
            float secAngle = secTotalRad / seg;
            Vector3 m_up = Vector3.Cross(m_forward, m_right).normalized;

            //section
            //  n,0  
            //  / \  
            // 3 c 1 
            //  \ /  
            //   2   
            Vector2[] secSinCos = new Vector2[seg + 1];
            for (int i = 0; i <= seg; ++i)
            {
                float rad = secAngleStart + secAngle * i;
                secSinCos[i].x = Mathf.Sin(rad);
                secSinCos[i].y = Mathf.Cos(rad);
            }

            //pivotOffset
            //  n,0  
            //  / \  
            // 3 c 1 
            //  \ /  
            //   2   
            Vector3[] centers = new Vector3[sides + 1];
            for (int i = 0; i <= sides; ++i)
            {
                float rad = angleStart + angle * i;
                float sin = Mathf.Sin(rad);
                float cos = Mathf.Cos(rad);
                centers[i] = pivotOffset + (m_forward * cos + m_right * sin) * radius1;
            }

            //uvs
            // 1-2 
            // | | 
            // 0-3 
            Vector2[] uvs = null;
            if (generateMappingCoords)
            {
                if (realWorldMapSize)
                {
                    UVTiling.x *= totalRad * (radius1 + radius2);
                    UVTiling.y *= secTotalRad * radius2;
                }
                uvs = new Vector2[4];
                uvs[0] = UVOffset;
                uvs[1] = uvs[0] + new Vector2(0.0f, UVTiling.y);
                uvs[2] = uvs[1] + new Vector2(UVTiling.x, 0.0f);
                uvs[3] = uvs[0] + new Vector2(UVTiling.x, 0.0f);
                if (mirrorUV ^ flipNormals) uvs = uvs.Reverse().ToArray();
            }

            float normalUp = 1.0f;
            if (flipNormals)
            {
                centers = centers.Reverse().ToArray();
                normalUp = -1;
            }

            //points
            Vector3[,] points = new Vector3[sides + 1, seg + 1];
            Vector3[,] normals = new Vector3[sides + 1, seg + 1];
            for (int i = 0; i <= sides; ++i)
            {
                Vector3 secRight = (pivotOffset - centers[i]) / radius1;
                for (int j = 0; j <= seg; ++j)
                {
                    points[i, j] = centers[i] + (m_up * secSinCos[j].y + secRight * secSinCos[j].x) * radius2;
                    normals[i, j] = (points[i, j] - centers[i]).normalized * normalUp;
                }
            }

            if (smooth) BuildSurfaceFullPointsSmooth(points, uvs, normals);
            else BuildSurfaceFullPointsHard(points, uvs);
        }

        public void CreateSurfaceExtrusion(Vector3 pivotOffset, Vector3 m_forward, Vector3 m_right, List<Vector2> m_points, bool connected, float height, int heightSegs, bool generateMappingCoords, bool realWorldMapSize, Vector2 UVOffset, Vector2 UVTiling, bool flipNormals, bool reverse)
        {
            Vector3 m_up = Vector3.Cross(m_forward, m_right).normalized;

            //points
            Vector3[] points = new Vector3[connected ? m_points.Count + 1 : m_points.Count];
            Vector3[] extrudePoints = new Vector3[points.Length];
            Vector2 minP = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 maxP = new Vector2(float.MinValue, float.MinValue);
            for (int i = 0; i < m_points.Count; ++i)
            {
                points[i] = pivotOffset + m_forward * m_points[i].y + m_right * m_points[i].x;
                extrudePoints[i] = points[i] + m_up * height;
                if (m_points[i].x < minP.x) minP.x = m_points[i].x;
                if (m_points[i].x > maxP.x) maxP.x = m_points[i].x;
                if (m_points[i].y < minP.y) minP.y = m_points[i].y;
                if (m_points[i].y > maxP.y) maxP.y = m_points[i].y;
            }
            if (connected)
            {
                points[m_points.Count] = points[0];
                extrudePoints[m_points.Count] = extrudePoints[0];
            }

            //uvs
            Vector2[] uvs = null;
            if (generateMappingCoords)
            {
                if (realWorldMapSize)
                {
                    UVTiling.x *= maxP.x - minP.x;
                    UVTiling.y *= maxP.y - minP.y;
                }
                uvs = new Vector2[points.Length];
                for (int i = 0; i < m_points.Count; ++i)
                {
                    float x = Mathf.InverseLerp(minP.x, maxP.x, m_points[i].x);
                    float y = Mathf.InverseLerp(minP.y, maxP.y, m_points[i].y);
                    uvs[i] = UVOffset + new Vector2(UVTiling.x * x, UVTiling.y * y);
                }
                if (connected)
                {
                    uvs[m_points.Count] = uvs[0];
                }
                if (reverse ^ flipNormals) uvs = uvs.Reverse().ToArray();
            }

            if (reverse ^ flipNormals)
            {
                points = points.Reverse().ToArray();
                extrudePoints = extrudePoints.Reverse().ToArray();
            }

            BuildSurfaceUpDownPointsHard(extrudePoints, points, uvs, uvs, heightSegs);
        }

        // *****************
        // Build Surface
        // *****************

        // points * uvs
        // ***************
        //   1    *   1  
        //  / \   *  / \ 
        // 0---2  * 0---2
        private void BuildSurfaceTriangle(Vector3[] points, Vector2[] uvs)
        {
            Vector3 normal = Vector3.Cross(points[1] - points[0], points[2] - points[0]).normalized;
            BuildSurfaceTriangle(points, uvs, normal);
        }

        // points * uvs
        // ***************
        //   1    *   1  
        //  / \   *  / \ 
        // 0---2  * 0---2
        private void BuildSurfaceTriangle(Vector3[] points, Vector2[] uvs, Vector3 normal)
        {
            int index = m_vertices.Count;

            m_vertices.Add(points[0]);
            m_vertices.Add(points[1]);
            m_vertices.Add(points[2]);

            if (uvs != null)
            {
                m_uv.Add(uvs[0]);
                m_uv.Add(uvs[1]);
                m_uv.Add(uvs[2]);
            }

            m_normals.Add(normal);
            m_normals.Add(normal);
            m_normals.Add(normal);

            m_triangles.Add(index);
            m_triangles.Add(index + 1);
            m_triangles.Add(index + 2);
        }

        // points * uvs
        // ***************
        //   1    *   1  
        //  / \   *  / \ 
        // 0---2  * 0---2
        private void BuildSurfaceTriangle(Vector3[] points, Vector2[] uvs, Vector3 normal, int seg)
        {
            int index = m_vertices.Count;

            // .....
            // C 1...
            // B 12...
            // A 123...
            Vector3 v01 = (points[1] - points[0]) / seg;
            Vector3 v02 = (points[2] - points[0]) / seg;
            if (uvs == null)
            {
                for (int i = 0; i <= seg; ++i)
                {
                    int row = seg - i;
                    for (int j = 0; j <= row; ++j)
                    {
                        m_vertices.Add(points[0] + v01 * i + v02 * j);
                        m_normals.Add(normal);
                    }
                }
            }
            else
            {
                Vector2 uv01 = (uvs[1] - uvs[0]) / seg;
                Vector2 uv02 = (uvs[2] - uvs[0]) / seg;
                for (int i = 0; i <= seg; ++i)
                {
                    int row = seg - i;
                    for (int j = 0; j <= row; ++j)
                    {
                        m_vertices.Add(points[0] + v01 * i + v02 * j);
                        m_normals.Add(normal);
                        m_uv.Add(uvs[0] + uv01 * i + uv02 * j);
                    }
                }
            }

            // t1    * t2
            // *************
            //   1   * 1---3
            //  / \  *  \ /
            // 0---2 *   2
            int[] indice = new int[4];
            int sIndex = index;
            for (int i = 0; i < seg; ++i)
            {
                int row = seg - i;
                for (int j = 0; j < row - 1; ++j)
                {
                    indice[0] = sIndex + j;
                    indice[1] = indice[0] + row + 1;
                    indice[2] = indice[0] + 1;
                    indice[3] = indice[1] + 1;

                    m_triangles.Add(indice[0]);
                    m_triangles.Add(indice[1]);
                    m_triangles.Add(indice[2]);
                    m_triangles.Add(indice[2]);
                    m_triangles.Add(indice[1]);
                    m_triangles.Add(indice[3]);
                }

                indice[0] = sIndex + row - 1;
                indice[1] = indice[0] + row + 1;
                indice[2] = indice[0] + 1;
                m_triangles.Add(indice[0]);
                m_triangles.Add(indice[1]);
                m_triangles.Add(indice[2]);

                sIndex += row + 1;
            }
        }

        // points * uvs
        // *************
        // 1-2    * 1-2
        // | |    * | |
        // 0-3    * 0-3
        private void BuildSurfaceRectangle(Vector3[] points, Vector2[] uvs)
        {
            Vector3 normal = Vector3.Cross(points[2] - points[0], points[3] - points[1]).normalized;
            BuildSurfaceRectangle(points, uvs, normal);
        }

        // points * uvs
        // *************
        // 1-2    * 1-2
        // | |    * | |
        // 0-3    * 0-3
        private void BuildSurfaceRectangle(Vector3[] points, Vector2[] uvs, Vector3 normal)
        {
            int index = m_vertices.Count;

            m_vertices.Add(points[0]);
            m_vertices.Add(points[1]);
            m_vertices.Add(points[2]);
            m_vertices.Add(points[3]);

            if (uvs != null)
            {
                m_uv.Add(uvs[0]);
                m_uv.Add(uvs[1]);
                m_uv.Add(uvs[2]);
                m_uv.Add(uvs[3]);
            }

            m_normals.Add(normal);
            m_normals.Add(normal);
            m_normals.Add(normal);
            m_normals.Add(normal);

            // t1  * t2
            // **********
            // 1-2 *   2
            // |/  *  /|
            // 0   * 0-3
            int[] indice = new int[4];
            indice[0] = index;
            indice[1] = index + 1;
            indice[2] = index + 2;
            indice[3] = index + 3;

            m_triangles.Add(indice[0]);
            m_triangles.Add(indice[1]);
            m_triangles.Add(indice[2]);
            m_triangles.Add(indice[2]);
            m_triangles.Add(indice[3]);
            m_triangles.Add(indice[0]);

#if PRIMITIVE_EDGES
            m_indexH.Add(new EdgeIndex(indice[1], indice[2]));
            m_indexH.Add(new EdgeIndex(indice[3], indice[0]));
            m_indexV.Add(new EdgeIndex(indice[0], indice[1]));
            m_indexV.Add(new EdgeIndex(indice[2], indice[3]));
#endif
        }

        // points * uvs
        // *************
        // 1-2    * 1-2
        // | |    * | |
        // 0-3    * 0-3
        private void BuildSurfaceRectangle(Vector3[] points, Vector2[] uvs, int widthSegs, int lengthSegs, bool isUniform = true)
        {
            Vector3 normal = Vector3.Cross(points[2] - points[0], points[3] - points[1]).normalized;
            BuildSurfaceRectangle(points, uvs, normal, widthSegs, lengthSegs, isUniform);
        }

        // points * uvs
        // *************
        // 1-2    * 1-2
        // | |    * | |
        // 0-3    * 0-3
        private void BuildSurfaceRectangle(Vector3[] points, Vector2[] uvs, Vector3 normal, int widthSegs, int lengthSegs, bool isUniform = true)
        {
            int index = m_vertices.Count;

            // ........
            // C 123...
            // B 123...
            // A 123...
            if (isUniform)
            {
                Vector3 v03 = (points[3] - points[0]) / widthSegs;
                Vector3 v01 = (points[1] - points[0]) / lengthSegs;
                if (uvs == null)
                {
                    for (int i = 0; i <= lengthSegs; ++i)
                    {
                        for (int j = 0; j <= widthSegs; ++j)
                        {
                            m_vertices.Add(points[0] + v01 * i + v03 * j);
                            m_normals.Add(normal);
                        }
                    }
                }
                else
                {
                    Vector2 uv03 = (uvs[3] - uvs[0]) / widthSegs;
                    Vector2 uv01 = (uvs[1] - uvs[0]) / lengthSegs;
                    for (int i = 0; i <= lengthSegs; ++i)
                    {
                        for (int j = 0; j <= widthSegs; ++j)
                        {
                            m_vertices.Add(points[0] + v01 * i + v03 * j);
                            m_normals.Add(normal);
                            m_uv.Add(uvs[0] + uv01 * i + uv03 * j);
                        }
                    }
                }
            }
            else
            {
                Vector3 v01 = (points[1] - points[0]) / lengthSegs;
                Vector3 v32 = (points[2] - points[3]) / lengthSegs;
                if (uvs == null)
                {
                    for (int i = 0; i <= lengthSegs; ++i)
                    {
                        Vector3 p01 = points[0] + v01 * i;
                        Vector3 p32 = points[3] + v32 * i;
                        for (int j = 0; j <= widthSegs; ++j)
                        {
                            float t = j / (float)widthSegs;
                            m_vertices.Add(PPMathLib.Lerp(p01, p32, t));
                            m_normals.Add(normal);
                        }
                    }
                }
                else
                {
                    Vector2 uv01 = (uvs[1] - uvs[0]) / lengthSegs;
                    Vector2 uv32 = (uvs[2] - uvs[3]) / lengthSegs;
                    for (int i = 0; i <= lengthSegs; ++i)
                    {
                        Vector3 p01 = points[0] + v01 * i;
                        Vector3 p32 = points[3] + v32 * i;
                        Vector2 puv01 = uvs[0] + uv01 * i;
                        Vector2 puv32 = uvs[3] + uv32 * i;
                        for (int j = 0; j <= widthSegs; ++j)
                        {
                            float t = j / (float)widthSegs;
                            m_vertices.Add(PPMathLib.Lerp(p01, p32, t));
                            m_normals.Add(normal);
                            m_uv.Add(PPMathLib.Lerp(puv01, puv32, t));
                        }
                    }
                }
            }

            // t1  * t2
            // **********
            // 1-2 *   2
            // |/  *  /|
            // 0   * 0-3
            int[] indice = new int[4];
            for (int i = 0; i < lengthSegs; ++i)
            {
                int sIndex = index + (widthSegs + 1) * i;
                for (int j = 0; j < widthSegs; ++j)
                {
                    indice[0] = sIndex + j;
                    indice[1] = indice[0] + widthSegs + 1;
                    indice[2] = indice[1] + 1;
                    indice[3] = indice[0] + 1;

                    m_triangles.Add(indice[0]);
                    m_triangles.Add(indice[1]);
                    m_triangles.Add(indice[2]);
                    m_triangles.Add(indice[2]);
                    m_triangles.Add(indice[3]);
                    m_triangles.Add(indice[0]);

#if PRIMITIVE_EDGES
                    m_indexH.Add(new EdgeIndex(indice[1], indice[2]));
                    m_indexH.Add(new EdgeIndex(indice[3], indice[0]));
                    m_indexV.Add(new EdgeIndex(indice[0], indice[1]));
                    m_indexV.Add(new EdgeIndex(indice[2], indice[3]));
#endif
                }
            }
        }

        // points * uvs
        // *************
        // 1-2    * 1-2
        // | |    * | |
        // 0-n    * 0-n
        private void BuildSurfacePolygon(Vector3[] points, Vector2[] uvs, List<int> triangles, Vector3 normal)
        {
            int index = m_vertices.Count;

            if (uvs == null)
            {
                for (int i = 0; i < points.Length; ++i)
                {
                    m_vertices.Add(points[i]);
                    m_normals.Add(normal);
                }
            }
            else
            {
                for (int i = 0; i < points.Length; ++i)
                {
                    m_vertices.Add(points[i]);
                    m_normals.Add(normal);
                    m_uv.Add(uvs[i]);
                }
            }

            if (triangles == null)
            {
                for (int i = 1; i < points.Length - 1; ++i)
                {
                    m_triangles.Add(index);
                    m_triangles.Add(index + i);
                    m_triangles.Add(index + i + 1);
                }
            }
            else
            {
                for (int i = 0; i < triangles.Count; ++i)
                {
                    m_triangles.Add(index + triangles[i]);
                }
            }
        }

        // points * uvs
        // ***************
        //  n,0   *  n,0 
        //  / \   *  / \ 
        // 3   1  * 3   1
        //  \ /   *  \ / 
        //   2    *   2  
        private void BuildSurfaceCircle(Vector3[] points, Vector3 center, Vector2[] uvs, Vector2 uvCenter, Vector3 normal, int lengthSegs)
        {
            int index = m_vertices.Count;
            int widthSegs = points.Length - 1;

            // ...   ...
            // 3    3
            // 2   2
            // 1  1
            // A B
            //   C 1 2 3...
            m_vertices.Add(center);
            m_normals.Add(normal);
            if (uvs == null)
            {
                for (int i = 0; i <= widthSegs; ++i)
                {
                    for (int j = 1; j <= lengthSegs; ++j)
                    {
                        float t = j / (float)lengthSegs;
                        m_vertices.Add(PPMathLib.Lerp(center, points[i], t));
                        m_normals.Add(normal);
                    }
                }
            }
            else
            {
                m_uv.Add(uvCenter);
                for (int i = 0; i <= widthSegs; ++i)
                {
                    for (int j = 1; j <= lengthSegs; ++j)
                    {
                        float t = j / (float)lengthSegs;
                        m_vertices.Add(PPMathLib.Lerp(center, points[i], t));
                        m_normals.Add(normal);
                        m_uv.Add(PPMathLib.Lerp(uvCenter, uvs[i], t));
                    }
                }
            }

            // t1  * t2
            // **********
            // 1-2 *   2
            // |/  *  /|
            // 0   * 0-3
            int[] indice = new int[4];
            for (int i = 0; i < widthSegs; ++i)
            {
                int sIndex = (index + 1) + lengthSegs * i;

                m_triangles.Add(index);
                m_triangles.Add(sIndex);
                m_triangles.Add(sIndex + lengthSegs);

                for (int j = 1; j < lengthSegs; ++j)
                {
                    indice[0] = sIndex + j - 1;
                    indice[1] = indice[0] + 1;
                    indice[2] = indice[1] + lengthSegs;
                    indice[3] = indice[2] - 1;

                    m_triangles.Add(indice[0]);
                    m_triangles.Add(indice[1]);
                    m_triangles.Add(indice[2]);
                    m_triangles.Add(indice[2]);
                    m_triangles.Add(indice[3]);
                    m_triangles.Add(indice[0]);

#if PRIMITIVE_EDGES
                    m_indexH.Add(new EdgeIndex(indice[1], indice[2]));
                    m_indexH.Add(new EdgeIndex(indice[3], indice[0]));
                    m_indexV.Add(new EdgeIndex(indice[0], indice[1]));
                    m_indexV.Add(new EdgeIndex(indice[2], indice[3]));
#endif
                }
            }
        }

        // points * uvs
        // ***************
        //  n,0   *  n,0 
        //  / \   *  / \ 
        // 3   1  * 3   1
        //  \ /   *  \ / 
        //   2    *   2  
        private void BuildSurfaceRing(Vector3[] points, Vector3 center, Vector2[] uvs, Vector2 uvCenter, Vector3 normal, int lengthSegs, float innerRatio)
        {
            int index = m_vertices.Count;
            int widthSegs = points.Length - 1;

            // ...   ...
            // 3    3
            // 2   2
            // 1  1
            // A B
            //   C 1 2 3...
            if (uvs == null)
            {
                for (int i = 0; i <= widthSegs; ++i)
                {
                    Vector3 innerPoint = center + (points[i] - center) * innerRatio;
                    for (int j = 0; j <= lengthSegs; ++j)
                    {
                        float t = j / (float)lengthSegs;
                        m_vertices.Add(PPMathLib.Lerp(innerPoint, points[i], t));
                        m_normals.Add(normal);
                    }
                }
            }
            else
            {
                for (int i = 0; i <= widthSegs; ++i)
                {
                    Vector3 innerPoint = center + (points[i] - center) * innerRatio;
                    Vector2 innerUvs = uvCenter + (uvs[i] - uvCenter) * innerRatio;
                    for (int j = 0; j <= lengthSegs; ++j)
                    {
                        float t = j / (float)lengthSegs;
                        m_vertices.Add(PPMathLib.Lerp(innerPoint, points[i], t));
                        m_normals.Add(normal);
                        m_uv.Add(PPMathLib.Lerp(innerUvs, uvs[i], t));
                    }
                }
            }

            // t1  * t2
            // **********
            // 1-2 *   2
            // |/  *  /|
            // 0   * 0-3
            int[] indice = new int[4];
            for (int i = 0; i < widthSegs; ++i)
            {
                int sIndex = index + (lengthSegs + 1) * i;
                for (int j = 0; j < lengthSegs; ++j)
                {
                    indice[0] = sIndex + j;
                    indice[1] = indice[0] + 1;
                    indice[2] = indice[1] + lengthSegs + 1;
                    indice[3] = indice[2] - 1;

                    m_triangles.Add(indice[0]);
                    m_triangles.Add(indice[1]);
                    m_triangles.Add(indice[2]);
                    m_triangles.Add(indice[2]);
                    m_triangles.Add(indice[3]);
                    m_triangles.Add(indice[0]);

#if PRIMITIVE_EDGES
                    m_indexH.Add(new EdgeIndex(indice[1], indice[2]));
                    m_indexH.Add(new EdgeIndex(indice[3], indice[0]));
                    m_indexV.Add(new EdgeIndex(indice[0], indice[1]));
                    m_indexV.Add(new EdgeIndex(indice[2], indice[3]));
#endif
                }
            }
        }

        private void BuildSurfaceRing(Vector3[] points, Vector3[] pointsInner, Vector2[] uvs, Vector2[] uvInner, Vector3 up, int lengthSegs)
        {
            int index = m_vertices.Count;
            int widthSegs = points.Length - 1;

            // ...   ...
            // 3    3
            // 2   2
            // 1  1
            // A B
            //   C 1 2 3...
            if (uvs == null)
            {
                for (int i = 0; i <= widthSegs; ++i)
                {
                    Vector3 normal = 
                        i == 0 ? Vector3.Cross(points[i] - pointsInner[i], points[i + 1] - points[i]) : 
                        i == widthSegs ? Vector3.Cross(points[i - 1] - pointsInner[i - 1], points[i] - points[i - 1]) : 
                        Vector3.Cross(points[i] - pointsInner[i], points[i + 1] - points[i - 1]);
                    for (int j = 0; j <= lengthSegs; ++j)
                    {
                        float t = j / (float)lengthSegs;
                        m_vertices.Add(PPMathLib.Lerp(pointsInner[i], points[i], t));
                        m_normals.Add(normal);
                    }
                }
            }
            else
            {
                for (int i = 0; i <= widthSegs; ++i)
                {
                    int l = i - 1;
                    Vector3 normal =
                        i == 0 ? Vector3.Cross(points[i] - pointsInner[i], points[i + 1] - points[i]) :
                        i == widthSegs ? Vector3.Cross(points[i - 1] - pointsInner[i - 1], points[i] - points[i - 1]) :
                        Vector3.Cross(points[i] - pointsInner[i], points[i + 1] - points[i - 1]);
                    for (int j = 0; j <= lengthSegs; ++j)
                    {
                        float t = j / (float)lengthSegs;
                        m_vertices.Add(PPMathLib.Lerp(pointsInner[i], points[i], t));
                        m_normals.Add(normal);
                        m_uv.Add(PPMathLib.Lerp(uvInner[i], uvs[i], t));
                    }
                }
            }

            // t1  * t2
            // **********
            // 1-2 *   2
            // |/  *  /|
            // 0   * 0-3
            int[] indice = new int[4];
            for (int i = 0; i < widthSegs; ++i)
            {
                int sIndex = index + (lengthSegs + 1) * i;
                for (int j = 0; j < lengthSegs; ++j)
                {
                    indice[0] = sIndex + j;
                    indice[1] = indice[0] + 1;
                    indice[2] = indice[1] + lengthSegs + 1;
                    indice[3] = indice[2] - 1;

                    m_triangles.Add(indice[0]);
                    m_triangles.Add(indice[1]);
                    m_triangles.Add(indice[2]);
                    m_triangles.Add(indice[2]);
                    m_triangles.Add(indice[3]);
                    m_triangles.Add(indice[0]);

#if PRIMITIVE_EDGES
                    m_indexH.Add(new EdgeIndex(indice[1], indice[2]));
                    m_indexH.Add(new EdgeIndex(indice[3], indice[0]));
                    m_indexV.Add(new EdgeIndex(indice[0], indice[1]));
                    m_indexV.Add(new EdgeIndex(indice[2], indice[3]));
#endif
                }
            }
        }

        //      points  * uvs
        // **********************
        // up   1-2-3-n * 1-2-3-n
        //      | | | | * | | | |
        // down 1-2-3-n * 1-2-3-n
        private void BuildSurfaceUpDownPoints(Vector3[] upPoints, Vector3[] downPoints, Vector2[] upUvs, Vector2[] downUvs, Vector3 normal, int lengthSegs)
        {
            int index = m_vertices.Count;
            int widthSegs = downPoints.Length - 1;

            // ........
            // C 123...
            // B 123...
            // A 123...
            if (upUvs == null)
            {
                for (int i = 0; i <= widthSegs; ++i)
                {
                    for (int j = 0; j <= lengthSegs; ++j)
                    {
                        float t = j / (float)lengthSegs;
                        m_vertices.Add(PPMathLib.Lerp(downPoints[i], upPoints[i], t));
                        m_normals.Add(normal);
                    }
                }
            }
            else
            {
                for (int i = 0; i <= widthSegs; ++i)
                {
                    for (int j = 0; j <= lengthSegs; ++j)
                    {
                        float t = j / (float)lengthSegs;
                        m_vertices.Add(PPMathLib.Lerp(downPoints[i], upPoints[i], t));
                        m_normals.Add(normal);
                        m_uv.Add(PPMathLib.Lerp(downUvs[i], upUvs[i], t));
                    }
                }
            }

            // t1  * t2
            // **********
            // 1-2 *   2
            // |/  *  /|
            // 0   * 0-3
            int[] indice = new int[4];
            for (int i = 0; i < widthSegs; ++i)
            {
                int sIndex = index + (lengthSegs + 1) * i;
                for (int j = 0; j < lengthSegs; ++j)
                {
                    indice[0] = sIndex + j;
                    indice[1] = indice[0] + 1;
                    indice[2] = indice[1] + lengthSegs + 1;
                    indice[3] = indice[2] - 1;

                    m_triangles.Add(indice[0]);
                    m_triangles.Add(indice[1]);
                    m_triangles.Add(indice[2]);
                    m_triangles.Add(indice[2]);
                    m_triangles.Add(indice[3]);
                    m_triangles.Add(indice[0]);

#if PRIMITIVE_EDGES
                    m_indexH.Add(new EdgeIndex(indice[1], indice[2]));
                    m_indexH.Add(new EdgeIndex(indice[3], indice[0]));
                    m_indexV.Add(new EdgeIndex(indice[0], indice[1]));
                    m_indexV.Add(new EdgeIndex(indice[2], indice[3]));
#endif
                }
            }
        }

        //      points  * uvs
        // *******************
        // up   1-2-3-n * 1-2
        //      | | | | * | |
        // down 1-2-3-n * 0-3
        private void BuildSurfaceUpDownPointsHard(Vector3[] upPoints, Vector3[] downPoints, Vector2[] uvs, int lengthSegs, bool isUniform = true)
        {
            int widthSegs = downPoints.Length - 1;

            // ... 3 3 3 
            // ... 2 2 2 
            // ... 1 1 1 
            // ... C B A 

            // t1  * t2
            // **********
            // 2-1 *   1
            // |/  *  /|
            // 3   * 3-0
            if (uvs == null)
            {
                for (int i = 0; i < widthSegs; ++i)
                {
                    int inext = i + 1;
                    BuildSurfaceRectangle(new Vector3[] {
                        downPoints[inext],
                        upPoints[inext],
                        upPoints[i],
                        downPoints[i]
                    }, null, 1, lengthSegs, isUniform);
                }
            }
            else
            {
                Vector2 uv03 = (uvs[3] - uvs[0]) / widthSegs;
                Vector2 uv12 = (uvs[2] - uvs[1]) / widthSegs;
                for (int i = 0; i < widthSegs; ++i)
                {
                    int inext = i + 1;
                    BuildSurfaceRectangle(new Vector3[] {
                        downPoints[inext],
                        upPoints[inext],
                        upPoints[i],
                        downPoints[i]
                    }, new Vector2[] {
                        uvs[0] + uv03 * inext,
                        uvs[1] + uv12 * inext,
                        uvs[1] + uv12 * i,
                        uvs[0] + uv03 * i
                    }, 1, lengthSegs, isUniform);
                }
            }
        }

        //      points  * uvs
        // **********************
        // up   1-2-3-n * 1-2-3-n
        //      | | | | * | | | |
        // down 1-2-3-n * 1-2-3-n
        private void BuildSurfaceUpDownPointsHard(Vector3[] upPoints, Vector3[] downPoints, Vector2[] upUvs, Vector2[] downUvs, int lengthSegs, bool isUniform = true)
        {
            int widthSegs = downPoints.Length - 1;

            // ... 3 3 3 
            // ... 2 2 2 
            // ... 1 1 1 
            // ... C B A 

            // t1  * t2
            // **********
            // 2-1 *   1
            // |/  *  /|
            // 3   * 3-0
            if (upUvs == null)
            {
                for (int i = 0; i < widthSegs; ++i)
                {
                    int inext = i + 1;
                    BuildSurfaceRectangle(new Vector3[] {
                        downPoints[inext],
                        upPoints[inext],
                        upPoints[i],
                        downPoints[i]
                    }, null, 1, lengthSegs, isUniform);
                }
            }
            else
            {
                for (int i = 0; i < widthSegs; ++i)
                {
                    int inext = i + 1;
                    BuildSurfaceRectangle(new Vector3[] {
                        downPoints[inext],
                        upPoints[inext],
                        upPoints[i],
                        downPoints[i]
                    }, new Vector2[] {
                        downUvs[inext],
                        upUvs[inext],
                        upUvs[i],
                        downUvs[i]
                    }, 1, lengthSegs, isUniform);
                }
            }
        }

        //      points  * uvs
        // **********************
        // up   1-2-3-n * 1-2-3-n
        //      | | | | * | | | |
        // down 1-2-3-n * 1-2-3-n
        private void BuildSurfaceUpDownPointsSmooth(Vector3[] upPoints, Vector3[] downPoints, Vector2[] upUvs, Vector2[] downUvs, Vector3[] normals, int lengthSegs)
        {
            int index = m_vertices.Count;
            int widthSegs = downPoints.Length - 1;

            // ... 3 3 3 
            // ... 2 2 2 
            // ... 1 1 1 
            // ... C B A 
            if (upUvs == null)
            {
                for (int i = 0; i <= widthSegs; ++i)
                {
                    for (int j = 0; j <= lengthSegs; ++j)
                    {
                        float t = j / (float)lengthSegs;
                        m_vertices.Add(PPMathLib.Lerp(downPoints[i], upPoints[i], t));
                        m_normals.Add(normals[i]);
                    }
                }
            }
            else
            {
                for (int i = 0; i <= widthSegs; ++i)
                {
                    for (int j = 0; j <= lengthSegs; ++j)
                    {
                        float t = j / (float)lengthSegs;
                        m_vertices.Add(PPMathLib.Lerp(downPoints[i], upPoints[i], t));
                        m_normals.Add(normals[i]);
                        m_uv.Add(PPMathLib.Lerp(downUvs[i], upUvs[i], t));
                    }
                }
            }

            // t1  * t2
            // **********
            // 2-1 *   1
            // |/  *  /|
            // 3   * 3-0
            int[] indice = new int[4];
            for (int i = 0; i < widthSegs; ++i)
            {
                int sIndex = index + (lengthSegs + 1) * i;
                for (int j = 0; j < lengthSegs; ++j)
                {
                    indice[0] = sIndex + j;
                    indice[1] = indice[0] + 1;
                    indice[2] = indice[1] + lengthSegs + 1;
                    indice[3] = indice[2] - 1;

                    m_triangles.Add(indice[3]);
                    m_triangles.Add(indice[2]);
                    m_triangles.Add(indice[1]);
                    m_triangles.Add(indice[1]);
                    m_triangles.Add(indice[0]);
                    m_triangles.Add(indice[3]);

#if PRIMITIVE_EDGES
                    m_indexH.Add(new EdgeIndex(indice[2], indice[1]));
                    m_indexH.Add(new EdgeIndex(indice[0], indice[3]));
                    m_indexV.Add(new EdgeIndex(indice[1], indice[0]));
                    m_indexV.Add(new EdgeIndex(indice[3], indice[2]));
#endif
                }
            }
        }

        //      points  * uvs
        // *******************
        // up   1-2-3-n * 1-2
        //      | | | | * | |
        // down 1-2-3-n * 0-3
        private void BuildSurfaceUpDownPointsSmooth(Vector3[] upPoints, Vector3[] downPoints, Vector2[] uvs, Vector3[] normals, int lengthSegs)
        {
            int index = m_vertices.Count;
            int widthSegs = downPoints.Length - 1;

            // ... 3 3 3 
            // ... 2 2 2 
            // ... 1 1 1 
            // ... C B A 
            if (uvs == null)
            {
                for (int i = 0; i <= widthSegs; ++i)
                {
                    for (int j = 0; j <= lengthSegs; ++j)
                    {
                        float t = j / (float)lengthSegs;
                        m_vertices.Add(PPMathLib.Lerp(downPoints[i], upPoints[i], t));
                        m_normals.Add(normals[i]);
                    }
                }
            }
            else
            {
                Vector2 uv03 = (uvs[3] - uvs[0]) / widthSegs;
                Vector2 uv01 = (uvs[1] - uvs[0]) / lengthSegs;
                for (int i = 0; i <= widthSegs; ++i)
                {
                    for (int j = 0; j <= lengthSegs; ++j)
                    {
                        float t = j / (float)lengthSegs;
                        m_vertices.Add(PPMathLib.Lerp(downPoints[i], upPoints[i], t));
                        m_normals.Add(normals[i]);
                        m_uv.Add(uvs[0] + uv03 * i + uv01 * j);
                    }
                }
            }
            
            // t1  * t2
            // **********
            // 2-1 *   1
            // |/  *  /|
            // 3   * 3-0
            int[] indice = new int[4];
            for (int i = 0; i < widthSegs; ++i)
            {
                int sIndex = index + (lengthSegs + 1) * i;
                for (int j = 0; j < lengthSegs; ++j)
                {
                    indice[0] = sIndex + j;
                    indice[1] = indice[0] + 1;
                    indice[2] = indice[1] + lengthSegs + 1;
                    indice[3] = indice[2] - 1;

                    m_triangles.Add(indice[3]);
                    m_triangles.Add(indice[2]);
                    m_triangles.Add(indice[1]);
                    m_triangles.Add(indice[1]);
                    m_triangles.Add(indice[0]);
                    m_triangles.Add(indice[3]);

#if PRIMITIVE_EDGES
                    m_indexH.Add(new EdgeIndex(indice[2], indice[1]));
                    m_indexH.Add(new EdgeIndex(indice[0], indice[3]));
                    m_indexV.Add(new EdgeIndex(indice[1], indice[0]));
                    m_indexV.Add(new EdgeIndex(indice[3], indice[2]));
#endif
                }
            }
        }

        //  points  * uvs
        // ***************
        //  n-|-|-n * 1-2
        //  2-|-|-| * | |
        //  1-|-|-| * 0-3
        //  0-1-2-n
        private void BuildSurfaceFullPointsHard(Vector3[,] points, Vector2[] uvs, float[] uvv = null)
        {
            int widthSegs = points.GetLength(0) - 1;
            int lengthSegs = points.GetLength(1) - 1;

            // ... 3 3 3 
            // ... 2 2 2 
            // ... 1 1 1 
            // ... C B A 

            // t1  * t2
            // **********
            // 2-1 *   1
            // |/  *  /|
            // 3   * 3-0
            if (uvs == null)
            {
                for (int i = 0; i < widthSegs; ++i)
                {
                    for (int j = 0; j < lengthSegs; ++j)
                    {
                        int inext = i + 1;
                        int jnext = j + 1;
                        BuildSurfaceRectangle(
                            new Vector3[] {
                                points[inext, j],
                                points[inext, jnext],
                                points[i, jnext],
                                points[i, j] },
                            null);
                    }
                }
            }
            else
            {
                Vector2[,] uvss = new Vector2[points.GetLength(0), points.GetLength(1)];
                if (uvv == null)
                {
                    Vector2 uv03 = (uvs[3] - uvs[0]) / widthSegs;
                    Vector2 uv01 = (uvs[1] - uvs[0]) / lengthSegs;
                    for (int i = 0; i < points.GetLength(0); ++i)
                    {
                        for (int j = 0; j < points.GetLength(1); ++j)
                        {
                            uvss[i, j] = uvs[0] + uv03 * i + uv01 * j;
                        }
                    }
                }
                else
                {
                    Vector2 uv03 = (uvs[3] - uvs[0]) / widthSegs;
                    Vector2 uv01 = (uvs[1] - uvs[0]);
                    for (int i = 0; i < points.GetLength(0); ++i)
                    {
                        for (int j = 0; j < points.GetLength(1); ++j)
                        {
                            uvss[i, j] = uvs[0] + uv03 * i + uv01 * uvv[j];
                        }
                    }
                }

                for (int i = 0; i < widthSegs; ++i)
                {
                    for (int j = 0; j < lengthSegs; ++j)
                    {
                        int inext = i + 1;
                        int jnext = j + 1;
                        BuildSurfaceRectangle(
                            new Vector3[] {
                                points[inext, j],
                                points[inext, jnext],
                                points[i, jnext],
                                points[i, j] },
                            new Vector2[] {
                                uvss[inext, j],
                                uvss[inext, jnext],
                                uvss[i, jnext],
                                uvss[i, j] }
                        );
                    }
                }
            }
        }

        //  points  * uvs
        // ***************
        //  n-|-|-n * 1-2
        //  2-|-|-| * | |
        //  1-|-|-| * 0-3
        //  0-1-2-n
        private void BuildSurfaceFullPointsSmooth(Vector3[,] points, Vector2[] uvs, Vector3[,] normals, float[] uvv = null)
        {
            int index = m_vertices.Count;
            int widthSegs = points.GetLength(0) - 1;
            int lengthSegs = points.GetLength(1) - 1;

            // ... 3 3 3 
            // ... 2 2 2 
            // ... 1 1 1 
            // ... C B A 
            if (uvs == null)
            {
                for (int i = 0; i < points.GetLength(0); ++i)
                {
                    for (int j = 0; j < points.GetLength(1); ++j)
                    {
                        m_vertices.Add(points[i, j]);
                        m_normals.Add(normals[i, j]);
                    }
                }
            }
            else
            {
                if (uvv == null)
                {
                    Vector2 uv03 = (uvs[3] - uvs[0]) / widthSegs;
                    Vector2 uv01 = (uvs[1] - uvs[0]) / lengthSegs;
                    for (int i = 0; i < points.GetLength(0); ++i)
                    {
                        for (int j = 0; j < points.GetLength(1); ++j)
                        {
                            m_vertices.Add(points[i, j]);
                            m_normals.Add(normals[i, j]);
                            m_uv.Add(uvs[0] + uv03 * i + uv01 * j);
                        }
                    }
                }
                else
                {
                    Vector2 uv03 = (uvs[3] - uvs[0]) / widthSegs;
                    Vector2 uv01 = (uvs[1] - uvs[0]);
                    for (int i = 0; i < points.GetLength(0); ++i)
                    {
                        for (int j = 0; j < points.GetLength(1); ++j)
                        {
                            m_vertices.Add(points[i, j]);
                            m_normals.Add(normals[i, j]);
                            m_uv.Add(uvs[0] + uv03 * i + uv01 * uvv[j]);
                        }
                    }
                }
            }

            // t1  * t2
            // **********
            // 2-1 *   1
            // |/  *  /|
            // 3   * 3-0
            int[] indice = new int[4];
            for (int i = 0; i < widthSegs; ++i)
            {
                int sIndex = index + (lengthSegs + 1) * i;
                for (int j = 0; j < lengthSegs; ++j)
                {
                    indice[0] = sIndex + j;
                    indice[1] = indice[0] + 1;
                    indice[2] = indice[1] + lengthSegs + 1;
                    indice[3] = indice[2] - 1;

                    m_triangles.Add(indice[3]);
                    m_triangles.Add(indice[2]);
                    m_triangles.Add(indice[1]);
                    m_triangles.Add(indice[1]);
                    m_triangles.Add(indice[0]);
                    m_triangles.Add(indice[3]);

#if PRIMITIVE_EDGES
                    m_indexH.Add(new EdgeIndex(indice[2], indice[1]));
                    m_indexH.Add(new EdgeIndex(indice[0], indice[3]));
                    m_indexV.Add(new EdgeIndex(indice[1], indice[0]));
                    m_indexV.Add(new EdgeIndex(indice[3], indice[2]));
#endif
                }
            }
        }
    }
}