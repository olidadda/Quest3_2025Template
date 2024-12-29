using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil.Math
{
    public static class EarClipping
    {
        //The points on the hull (vertices) should be ordered counter-clockwise
        public static List<int> Triangulate(List<Vector2> vertices, bool clockwise)
        {
            //if (vertices == null || vertices.Count <= 2)
            //{
            //    Debug.LogWarning("Can't triangulate with Ear Clipping because too few vertices on the hull");
            //    return null;
            //}

            //Create a linked list connecting all vertices with each other which will make the calculations easier and faster
            List<LinkedVertex> verticesLinked = new List<LinkedVertex>();
            if (clockwise)
            {
                for (int i = 0; i < vertices.Count; i++)
                {
                    int j = vertices.Count - i - 1;
                    LinkedVertex v = new LinkedVertex(vertices[j], j);
                    verticesLinked.Add(v);
                }
            }
            else
            {
                for (int i = 0; i < vertices.Count; i++)
                {
                    LinkedVertex v = new LinkedVertex(vertices[i], i);
                    verticesLinked.Add(v);
                }
            }
            for (int i = 0; i < verticesLinked.Count; i++)
            {
                LinkedVertex v = verticesLinked[i];
                v.prevLinkedVertex = verticesLinked[PPMathLib.ClampListIndex(i - 1, verticesLinked.Count)];
                v.nextLinkedVertex = verticesLinked[PPMathLib.ClampListIndex(i + 1, verticesLinked.Count)];
            }

            //- Convex vertices (interior angle smaller than 180 degrees)
            //- Reflect/concave vertices (interior angle greater than 180 degrees)
            //Interior angle is the angle between two vectors inside the polygon if we move around the polygon counter-clockwise
            //If they are neither we assume they are reflect (or we will end up with odd triangulations)
            HashSet<LinkedVertex> convexVerts = new HashSet<LinkedVertex>();
            HashSet<LinkedVertex> reflectVerts = new HashSet<LinkedVertex>();
            foreach (LinkedVertex v in verticesLinked)
            {
                bool isConvex = IsVertexConvex(v);
                if (isConvex) convexVerts.Add(v);
                else reflectVerts.Add(v);
            }

            //Find the initial ears
            HashSet<LinkedVertex> earVerts = new HashSet<LinkedVertex>();
            //An ear is always a convex vertex
            foreach (LinkedVertex v in convexVerts)
            {
                //And we only need to test the reflect vertices
                if (IsVertexEar(v, reflectVerts)) earVerts.Add(v);
            }

            //Build the triangles
            List<int> triangulation = new List<int>();
            //We know how many triangles we will get (number of vertices - 2) which is true for all simple polygons
            //This can be used to stop the algorithm
            int maxTriangles = verticesLinked.Count - 2;
            int safety = 0;
            while (true && safety < 30000)
            {
                //Pick an ear vertex and form a triangle
                LinkedVertex ear = GetEarVertex(earVerts);
                if (ear == null) { Debug.Log("Cant find ear"); break; }

                LinkedVertex v_prev = ear.prevLinkedVertex;
                LinkedVertex v_next = ear.nextLinkedVertex;
                triangulation.Add(ear.index);
                triangulation.Add(v_prev.index);
                triangulation.Add(v_next.index);

                //Check if we have found all triangles
                //This should also prevent us from getting stuck in an infinite loop
                if (triangulation.Count / 3 >= maxTriangles) break;

                //If we havent found all triangles we have to reconfigure the data structure
                //Remove the ear we used to build a triangle
                convexVerts.Remove(ear);
                earVerts.Remove(ear);

                //Reconnect the vertices because one vertex has now been removed
                v_prev.nextLinkedVertex = v_next;
                v_next.prevLinkedVertex = v_prev;

                //Reconfigure the adjacent vertices
                ReconfigureAdjacentVertex(v_prev, convexVerts, reflectVerts, earVerts);
                ReconfigureAdjacentVertex(v_next, convexVerts, reflectVerts, earVerts);

                safety += 1;
            }
            if (safety >= 30000) Debug.Log("Ear Clipping is stuck in an infinite loop!");

            return triangulation;
        }

        //Reconfigure an adjacent vertex that was used to build a triangle
        private static void ReconfigureAdjacentVertex(LinkedVertex v, HashSet<LinkedVertex> convexVerts, HashSet<LinkedVertex> reflectVerts, HashSet<LinkedVertex> earVerts)
        {
            //If the adjacent vertex was reflect...
            if (reflectVerts.Contains(v))
            {
                //it may now be convex...
                if (IsVertexConvex(v))
                {
                    reflectVerts.Remove(v);
                    convexVerts.Add(v);

                    //and possible a new ear
                    if (IsVertexEar(v, reflectVerts))
                    {
                        earVerts.Add(v);
                    }
                }
            }
            //If an adjacent vertex was convex, it will always still be convex
            else
            {
                bool isEar = IsVertexEar(v, reflectVerts);

                //This vertex was an ear but is no longer an ear
                if (earVerts.Contains(v) && !isEar)
                {
                    earVerts.Remove(v);
                }
                //This vertex wasn't an ear but has now become an ear
                else if (isEar)
                {
                    earVerts.Add(v);
                }
            }
        }

        private static LinkedVertex GetEarVertex(HashSet<LinkedVertex> earVertices)
        {
            LinkedVertex bestEarVertex = null;
            //just get the first ear
            foreach (LinkedVertex v in earVertices)
            {
                bestEarVertex = v;
                break;
            }
            return bestEarVertex;
        }

        private static bool IsVertexEar(LinkedVertex vertex, HashSet<LinkedVertex> reflectVertices)
        {
            //Consider the triangle
            Vector2 p_prev = vertex.prevLinkedVertex.pos;
            Vector2 p = vertex.pos;
            Vector2 p_next = vertex.nextLinkedVertex.pos;
            Triangle2 t = new Triangle2(p_prev, p, p_next);

            //If any of the other vertices is within this triangle, then this vertex is not an ear
            //We only need to check the reflect vertices
            //if no vertex is intersecting with the triangle, this vertex must be an ear
            bool isEar = true;
            foreach (LinkedVertex otherVertex in reflectVertices)
            {
                Vector2 test_p = otherVertex.pos;

                //Dont compare with any of the vertices the triangle consist of
                if (test_p.Equals(p_prev) || test_p.Equals(p) || test_p.Equals(p_next)) continue;

                //If a relect vertex intersects with the triangle, then this vertex is not an ear
                if (PPMathLib.IsPointWithinTriangle(t.p1, t.p2, t.p3, test_p, true))
                {
                    isEar = false;
                    break;
                }
            }
            return isEar;
        }

        //Is a vertex convex? (if not its concave or neither if its a straight line)
        private static bool IsVertexConvex(LinkedVertex v)
        {
            Vector2 p_prev = v.prevLinkedVertex.pos;
            Vector2 p = v.pos;
            Vector2 p_next = v.nextLinkedVertex.pos;
            return IsVertexConvex(p_prev, p, p_next);
        }

        public static bool IsVertexConvex(Vector2 p_prev, Vector2 p, Vector2 p_next)
        {
            float interiorAngle = CalculateInteriorAngle(p_prev, p, p_next);
            return interiorAngle < Mathf.PI;
        }

        private static float CalculateInteriorAngle(Vector2 p_prev, Vector2 p, Vector2 p_next)
        {
            //Two vectors going from the vertex
            Vector2 p_to_p_prev = p_prev - p;
            Vector2 p_to_p_next = p_next - p;

            //The angle between the two vectors [rad]
            //This will calculate the outside angle
            float angle = PPMathLib.AngleFromToCCW(p_to_p_prev.normalized, p_to_p_next.normalized);

            //The interior angle is the opposite of the outside angle
            float interiorAngle = (Mathf.PI * 2f) - angle;

            return interiorAngle;
        }
    }
}