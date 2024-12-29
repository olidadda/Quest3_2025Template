using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProceduralPrimitivesUtil.PPEditor
{
    [CustomEditor(typeof(Pyramid))]
    [CanEditMultipleObjects]
    public class PyramidEditor : PPBaseEditor
    {
        protected SerializedProperty width1;
        protected SerializedProperty length1;
        protected SerializedProperty width2;
        protected SerializedProperty length2;
        protected SerializedProperty height;
        protected SerializedProperty widthSegs;
        protected SerializedProperty lengthSegs;
        protected SerializedProperty heightSegs;

        protected override void Init()
        {
            base.Init();
            width1 = serializedObject.FindProperty("width1");
            length1 = serializedObject.FindProperty("length1");
            width2 = serializedObject.FindProperty("width2");
            length2 = serializedObject.FindProperty("length2");
            height = serializedObject.FindProperty("height");
            widthSegs = serializedObject.FindProperty("widthSegs");
            lengthSegs = serializedObject.FindProperty("lengthSegs");
            heightSegs = serializedObject.FindProperty("heightSegs");
        }

        protected override void DrawBasicParameters()
        {
            EditorGUILayout.PropertyField(width1);
            EditorGUILayout.PropertyField(length1);
            EditorGUILayout.PropertyField(width2);
            EditorGUILayout.PropertyField(length2);
            EditorGUILayout.PropertyField(height);
        }

        protected override void DrawSegments()
        {
            EditorGUILayout.PropertyField(widthSegs);
            EditorGUILayout.PropertyField(lengthSegs);
            EditorGUILayout.PropertyField(heightSegs);
        }
    }
}