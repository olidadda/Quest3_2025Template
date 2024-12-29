using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil.Math
{

    public class Edge2
    {
        public Vector2 p1;
        public Vector2 p2;
        public bool isIntersecting = false;

        public Edge2(Vector2 p1, Vector2 p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }
    }

    public struct Triangle2
    {
        public Vector2 p1;
        public Vector2 p2;
        public Vector2 p3;

        public Triangle2(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
        }

        public void ChangeOrientation()
        {
            //Swap two vertices
            (p1, p2) = (p2, p1);
        }

        public float MinX()
        {
            return Mathf.Min(p1.x, Mathf.Min(p2.x, p3.x));
        }

        public float MaxX()
        {
            return Mathf.Max(p1.x, Mathf.Max(p2.x, p3.x));
        }

        public float MinY()
        {
            return Mathf.Min(p1.y, Mathf.Min(p2.y, p3.y));
        }

        public float MaxY()
        {
            return Mathf.Max(p1.y, Mathf.Max(p2.y, p3.y));
        }

        public Edge2 FindOppositeEdgeToVertex(Vector2 p)
        {
            if (p.Equals(p1)) return new Edge2(p2, p3);
            else if (p.Equals(p2)) return new Edge2(p3, p1);
            else return new Edge2(p1, p2);
        }

        public bool IsEdgePartOfTriangle(Edge2 e)
        {
            if ((e.p1.Equals(p1) && e.p2.Equals(p2)) || (e.p1.Equals(p2) && e.p2.Equals(p1))) return true;
            if ((e.p1.Equals(p2) && e.p2.Equals(p3)) || (e.p1.Equals(p3) && e.p2.Equals(p2))) return true;
            if ((e.p1.Equals(p3) && e.p2.Equals(p1)) || (e.p1.Equals(p1) && e.p2.Equals(p3))) return true;
            return false;
        }

        public Vector2 GetVertexWhichIsNotPartOfEdge(Edge2 e)
        {
            if (!p1.Equals(e.p1) && !p1.Equals(e.p2)) return p1;
            if (!p2.Equals(e.p1) && !p2.Equals(e.p2)) return p2;
            return p3;
        }
    }

    public class LinkedVertex
    {
        public int index;
        public Vector2 pos;
        public LinkedVertex prevLinkedVertex;
        public LinkedVertex nextLinkedVertex;

        public LinkedVertex(Vector2 pos)
        {
            this.pos = pos;
        }

        public LinkedVertex(Vector2 pos, int index)
        {
            this.pos = pos;
            this.index = index;
        }
    }

    public static class PPMathLib
    {
        static float EPSILON = 0.00001f;

        public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
        {
            return new Vector2(
                Mathf.Lerp(a.x, b.x, t),
                Mathf.Lerp(a.y, b.y, t));
        }

        public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            return new Vector3(
                Mathf.Lerp(a.x, b.x, t),
                Mathf.Lerp(a.y, b.y, t),
                Mathf.Lerp(a.z, b.z, t));
        }

        public static float PointLineDistance(Vector3 vp, Vector3 v)
        {
            return Vector3.Cross(vp, v).magnitude / v.magnitude;
        }

        public static float PointLineProjection(Vector3 vp, Vector3 v)
        {
            return Vector3.Dot(vp, v) / v.magnitude;
        }

        public static int ClampListIndex(int index, int listSize)
        {
            return ((index % listSize) + listSize) % listSize;
        }

        public static bool IsTriangleOrientedClockwise(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            float determinant = p1.x * p2.y + p3.x * p1.y + p2.x * p3.y - p1.x * p3.y - p3.x * p2.y - p2.x * p1.y;
            return determinant <= 0.0f;
        }

        public static bool IsPointWithinTriangle(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p, bool includeBorder)
        {
            float denominator = ((p2.y - p3.y) * (p1.x - p3.x) + (p3.x - p2.x) * (p1.y - p3.y));
            float a = ((p2.y - p3.y) * (p.x - p3.x) + (p3.x - p2.x) * (p.y - p3.y)) / denominator;
            float b = ((p3.y - p1.y) * (p.x - p3.x) + (p1.x - p3.x) * (p.y - p3.y)) / denominator;
            float c = 1 - a - b;

            bool isWithinTriangle = false;
            float zero = 0f + EPSILON;
            float one = 1f - EPSILON;

            if (includeBorder)
            {
                //The point is within the triangle or on the border
                if (a >= zero && a <= one && b >= zero && b <= one && c >= zero && c <= one) isWithinTriangle = true;
            }
            else
            {
                //The point is within the triangle
                if (a > zero && a < one && b > zero && b < one && c > zero && c < one) isWithinTriangle = true;
            }

            return isWithinTriangle;
        }

        //normalize from/to first
        public static float AngleFromToCCW(Vector2 from, Vector2 to)
        {
            float angleRad = AngleBetween(from, to);
            if (Det2(from, to) <= 0f) angleRad = (Mathf.PI * 2f) - angleRad;
            return angleRad;
        }

        //normalize from/to first
        public static float AngleBetween(Vector2 from, Vector2 to)
        {
            float dot = Vector2.Dot(from, to);
            float angleRad = Mathf.Acos(dot);
            return angleRad;
        }

        // Returns the determinant of the 2x2 matrix defined as
        // | x1 x2 |
        // | y1 y2 |
        //det(a_normalized, b_normalized) = sin(alpha) so it's similar to the dot product
        //Vector alignment dot det
        //Same:            1   0
        //Perpendicular:   0  -1
        //Opposite:       -1   0
        //Perpendicular:   0   1
        public static float Det2(float x1, float x2, float y1, float y2)
        {
            return x1 * y2 - y1 * x2;
        }

        public static float Det2(Vector2 a, Vector2 b)
        {
            return a.x * b.y - a.y * b.x;
        }
    }
}