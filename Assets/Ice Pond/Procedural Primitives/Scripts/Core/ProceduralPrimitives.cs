using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil
{
    public enum ProceduralPrimitiveType
    {
        Box, Capsule, Cone, Cylinder, Polygon, Prism, Pyramid, RectRing3D, Sphere, Torus, 
        ChamferPlane, Circle, Frame, Plane, PolygonF, RectRing, Ring, Trapezoid, Triangle, 
        Arrow, ChamferBox, ChamferCylinder, DoubleSphere, Plate, RectTube, RectTubeR, TorusTube, Tray, Tube,
        Stair, SpiralStair
    }

    public enum SphereCutOption
    { None, HemiSphere, SphericalSector }

    public enum PointOrder
    { Clockwise, CounterClockwise }

    public enum GeneralStyle
    { StyleA, StyleB, StyleC }

    public static class ProceduralPrimitives
    {
        static GameObject CreateBase()
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            go.GetComponent<MeshFilter>().mesh = null;
#if UNITY_EDITOR
            GameObject.DestroyImmediate(go.GetComponent<Collider>());
#else
            GameObject.Destroy(go.GetComponent<Collider>());
#endif
            return go;
        }

        public static GameObject CreateInstance(PPBase source = null)
        {
            GameObject go = CreateBase();
            go.name = "PP Instance";
            go.AddComponent<PPInstance>().ApplySource(source);
            return go;
        }

        public static GameObject CreateCombiner()
        {
            GameObject go = CreateBase();
            go.name = "PP Combiner";
            go.AddComponent<PPCombiner>();
            return go;
        }

        public static GameObject CreatePrimitive(ProceduralPrimitiveType type)
        {
            GameObject go = CreateBase();
            go.name = type.ToString();
            switch(type)
            {
                case ProceduralPrimitiveType.Plane:
                    go.AddComponent<Plane>();
                    break;
                case ProceduralPrimitiveType.Circle:
                    go.AddComponent<Circle>();
                    break;
                case ProceduralPrimitiveType.Ring:
                    go.AddComponent<Ring>();
                    break;
                case ProceduralPrimitiveType.Triangle:
                    go.AddComponent<Triangle>();
                    break;
                case ProceduralPrimitiveType.Trapezoid:
                    go.AddComponent<Trapezoid>();
                    break;
                case ProceduralPrimitiveType.ChamferPlane:
                    go.AddComponent<ChamferPlane>();
                    break;
                case ProceduralPrimitiveType.Capsule:
                    go.AddComponent<Capsule>();
                    break;
                case ProceduralPrimitiveType.Prism:
                    go.AddComponent<Prism>();
                    break;
                case ProceduralPrimitiveType.ChamferBox:
                    go.AddComponent<ChamferBox>();
                    break;
                case ProceduralPrimitiveType.ChamferCylinder:
                    go.AddComponent<ChamferCylinder>();
                    break;
                case ProceduralPrimitiveType.DoubleSphere:
                    go.AddComponent<DoubleSphere>();
                    break;
                case ProceduralPrimitiveType.Plate:
                    go.AddComponent<Plate>();
                    break;
                case ProceduralPrimitiveType.Arrow:
                    go.AddComponent<Arrow>();
                    break;
                case ProceduralPrimitiveType.Box:
                    go.AddComponent<Box>();
                    break;
                case ProceduralPrimitiveType.Sphere:
                    go.AddComponent<Sphere>();
                    break;
                case ProceduralPrimitiveType.Cylinder:
                    go.AddComponent<Cylinder>();
                    break;
                case ProceduralPrimitiveType.Cone:
                    go.AddComponent<Cone>();
                    break;
                case ProceduralPrimitiveType.Tube:
                    go.AddComponent<Tube>();
                    break;
                case ProceduralPrimitiveType.RectTube:
                    go.AddComponent<RectTube>();
                    break;
                case ProceduralPrimitiveType.RectTubeR:
                    go.AddComponent<RectTubeR>();
                    break;
                case ProceduralPrimitiveType.Torus:
                    go.AddComponent<Torus>();
                    break;
                case ProceduralPrimitiveType.TorusTube:
                    go.AddComponent<TorusTube>();
                    break;
                case ProceduralPrimitiveType.Pyramid:
                    go.AddComponent<Pyramid>();
                    break;
                case ProceduralPrimitiveType.RectRing3D:
                    go.AddComponent<RectRing3D>();
                    break;
                case ProceduralPrimitiveType.Frame:
                    go.AddComponent<Frame>();
                    break;
                case ProceduralPrimitiveType.Tray:
                    go.AddComponent<Tray>();
                    break;
                case ProceduralPrimitiveType.RectRing:
                    go.AddComponent<RectRing>();
                    break;
                case ProceduralPrimitiveType.Polygon:
                    go.AddComponent<Polygon>();
                    break;
                case ProceduralPrimitiveType.PolygonF:
                    go.AddComponent<PolygonF>();
                    break;
                case ProceduralPrimitiveType.Stair:
                    go.AddComponent<Stair>();
                    break;
                case ProceduralPrimitiveType.SpiralStair:
                    go.AddComponent<SpiralStair>();
                    break;
                default: break;
            }
            return go;
        }
    }
}