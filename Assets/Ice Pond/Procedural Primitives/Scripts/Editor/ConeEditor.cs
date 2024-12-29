using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProceduralPrimitivesUtil.PPEditor
{
    [CustomEditor(typeof(Cone))]
    [CanEditMultipleObjects]
    public class ConeEditor : PPCircularBaseEditor
    {
        protected SerializedProperty radius1;
        protected SerializedProperty radius2;
        protected SerializedProperty height;
        protected SerializedProperty capSegs;
        protected SerializedProperty heightSegs;

        protected override void Init()
        {
            base.Init();
            radius1 = serializedObject.FindProperty("radius1");
            radius2 = serializedObject.FindProperty("radius2");
            height = serializedObject.FindProperty("height");
            capSegs = serializedObject.FindProperty("capSegs");
            heightSegs = serializedObject.FindProperty("heightSegs");
        }

        protected override void DrawBasicParameters()
        {
            EditorGUILayout.PropertyField(radius1);
            EditorGUILayout.PropertyField(radius2);
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