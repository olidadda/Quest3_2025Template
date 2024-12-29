using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ProceduralPrimitivesUtil
{
    [ExecuteInEditMode]
    public abstract class PPBase : MonoBehaviour
    {
        public Vector3 pivotOffset;
        public Vector3 rotation;
        public bool generateMappingCoords = true;
        public bool realWorldMapSize = false;
        public Vector2 UVOffset = new Vector2(0.0f, 0.0f);
        public Vector2 UVTiling = new Vector2(1.0f, 1.0f);
        public bool flipNormals = false;
        public bool smooth = true;
        public bool autoUpdateCollider = true;

        private Collider m_collider;
        private MeshFilter m_meshFilter;
        private bool m_dirty = false;

        protected Mesh m_mesh;
        protected Quaternion m_rotation;
        protected PPMeshBuilder m_builder;

        protected Vector3 m_up;
        protected Vector3 m_down;
        protected Vector3 m_left;
        protected Vector3 m_right;
        protected Vector3 m_forward;
        protected Vector3 m_back;

        public Mesh mesh { get { return m_mesh; } }

        public bool isDirty { get { return m_dirty; } }

        private void Awake()
        {
            m_mesh = new Mesh();
            Apply();
        }

        private void OnDestroy()
        {
#if UNITY_EDITOR
            if (m_mesh != null && !AssetDatabase.Contains(mesh)) DestroyImmediate(mesh);
#else
            if (m_mesh != null) Destroy(m_mesh);
#endif
        }

        private void Reset()
        {
            Apply();
        }

        private void OnValidate()
        {
            m_dirty = true;
        }

        public GameObject CreateInstance()
        {
            return ProceduralPrimitives.CreateInstance(this);
        }

        protected abstract void CreateMesh();

        public void Apply()
        {
            if (m_mesh == null) m_mesh = new Mesh();
            m_builder = new PPMeshBuilder();
            m_rotation = Quaternion.Euler(rotation);
            m_up = m_rotation * Vector3.up;
            m_down = -m_up;
            m_right = m_rotation * Vector3.right;
            m_left = -m_right;
            m_forward = m_rotation * Vector3.forward;
            m_back = -m_forward;

            CreateMesh();

            //set mesh
            m_mesh.Clear();
            m_builder.Apply(m_mesh);

            RefreshMeshFilter();
            if (autoUpdateCollider) RefreshCollider();

            //clear
            m_builder.Clear();
            m_builder = null;
            m_dirty = false;
        }

#if PRIMITIVE_EDGES
        //use this function to get construnction edges
        public void Apply(ref List<PPMeshBuilder.Edge> horizontalEdges, ref List<PPMeshBuilder.Edge> verticalEdges)
        {
            if (m_mesh == null) m_mesh = new Mesh();
            m_rotation = Quaternion.Euler(rotation);

            CreateMesh();
            
            //set mesh
            m_mesh.Clear();
            m_builder.Apply(m_mesh);
            m_builder.GetEdges(ref horizontalEdges, ref verticalEdges);

            RefreshMeshFilter();
            if (autoUpdateCollider) RefreshCollider();

            //clear
            m_builder.Clear();
            m_builder = null;
            m_dirty = false;
        }
#endif

        public void RefreshMesh()
        {
            m_mesh = null;
            Apply();
        }

        public Collider GetCollider()
        {
            if (m_collider == null) m_collider = GetComponent<Collider>();
            return m_collider;
        }

        public bool HasCollider()
        {
            return GetCollider() != null;
        }

        public Collider AddBoxCollider()
        {
            if (HasCollider()) return m_collider;
            m_collider = gameObject.AddComponent<BoxCollider>();
            RefreshCollider();
            return m_collider;
        }

        public Collider AddSphereCollider()
        {
            if (HasCollider()) return m_collider;
            m_collider = gameObject.AddComponent<SphereCollider>();
            RefreshCollider();
            return m_collider;
        }

        public Collider AddCapsuleCollider()
        {
            if (HasCollider()) return m_collider;
            m_collider = gameObject.AddComponent<CapsuleCollider>();
            RefreshCollider();
            return m_collider;
        }

        public Collider AddMeshCollider()
        {
            if (HasCollider()) return m_collider;
            m_collider = gameObject.AddComponent<MeshCollider>();
            RefreshCollider();
            return m_collider;
        }

        private void RefreshMeshFilter()
        {
            if (m_meshFilter == null) m_meshFilter = GetComponent<MeshFilter>();
            if (m_meshFilter == null) m_meshFilter = gameObject.AddComponent<MeshFilter>();
            m_meshFilter.sharedMesh = m_mesh;
        }

        private void RefreshCollider()
        {
            if (!HasCollider()) return;
            if (m_collider is MeshCollider)
            {
                MeshCollider collider = (m_collider as MeshCollider);
                collider.sharedMesh = m_mesh;
            }
            else if (m_collider is BoxCollider)
            {
                BoxCollider collider = (m_collider as BoxCollider);
                collider.center = m_mesh.bounds.center;
                collider.size = m_mesh.bounds.size;
            }
            else if (m_collider is SphereCollider)
            {
                SphereCollider collider = (m_collider as SphereCollider);
                collider.center = m_mesh.bounds.center;
                float max = Mathf.Max(m_mesh.bounds.size.x, m_mesh.bounds.size.y, m_mesh.bounds.size.z);
                collider.radius = max * 0.5f;
            }
            else if (m_collider is CapsuleCollider)
            {
                CapsuleCollider collider = (m_collider as CapsuleCollider);
                collider.center = m_mesh.bounds.center;
                float max = Mathf.Max(m_mesh.bounds.size.x, m_mesh.bounds.size.z);
                collider.radius = max * 0.5f;
                collider.height = m_mesh.bounds.size.y;
            }
        }
    }
}