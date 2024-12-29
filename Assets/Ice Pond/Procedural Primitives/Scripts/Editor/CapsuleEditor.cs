using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProceduralPrimitivesUtil.PPEditor
{
    [CustomEditor(typeof(Capsule))]
    [CanEditMultipleObjects]
    public class CapsuleEditor : PPCircularBaseEditor
    {
        protected SerializedProperty radius;
        protected SerializedProperty height;
        protected SerializedProperty heightSegs;

        protected override void Init()
        {
            base.Init();
            radius = serializedObject.FindProperty("radius");
            height = serializedObject.FindProperty("height");
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
            EditorGUILayout.PropertyField(heightSegs);
        }
    }
}