using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProceduralPrimitivesUtil.PPEditor
{
    [CustomEditor(typeof(Polygon))]
    [CanEditMultipleObjects]
    public class PolygonEditor : PPBaseEditor
    {
        protected SerializedProperty height;
        protected SerializedProperty pointOrder;
        protected SerializedProperty points;
        protected SerializedProperty heightSegs;

        protected override void Init()
        {
            base.Init();
            hasSegment = false;
            height = serializedObject.FindProperty("height");
            pointOrder = serializedObject.FindProperty("pointOrder");
            points = serializedObject.FindProperty("points");
            heightSegs = serializedObject.FindProperty("heightSegs");
        }

        protected override void DrawBasicParameters()
        {
            EditorGUILayout.PropertyField(height);
            EditorGUILayout.PropertyField(pointOrder);
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.PropertyField(points);
        }

        protected override void DrawSegments()
        {
            EditorGUILayout.PropertyField(heightSegs);
        }
    }
}