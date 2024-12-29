using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProceduralPrimitivesUtil.PPEditor
{
    [CustomEditor(typeof(Cylinder))]
    [CanEditMultipleObjects]
    public class CylinderEditor : PPCircularBaseEditor
    {
        protected SerializedProperty radius;
        protected SerializedProperty height;
        protected SerializedProperty capSegs;
        protected SerializedProperty heightSegs;

        protected override void Init()
        {
            base.Init();
            radius = serializedObject.FindProperty("radius");
            height = serializedObject.FindProperty("height");
            capSegs = serializedObject.FindProperty("capSegs");
            heightSegs = serializedObject.FindProperty("heightSegs");
        }

        protected override void DrawBasicParameters()
        {
            EditorGUILayout.PropertyField(radius);
            EditorGUILayout.PropertyField(height);
        }

        protected override void DrawSegments()
        {
            base.DrawSegments();
            EditorGUILayout.PropertyField(capSegs);
            EditorGUILayout.PropertyField(heightSegs);
        }
    }
}