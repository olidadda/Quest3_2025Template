using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProceduralPrimitivesUtil.PPEditor
{
    [CustomEditor(typeof(Arrow))]
    [CanEditMultipleObjects]
    public class ArrowEditor : PPBaseEditor
    {
        protected SerializedProperty width1;
        protected SerializedProperty width2;
        protected SerializedProperty width3;
        protected SerializedProperty length1;
        protected SerializedProperty length2;
        protected SerializedProperty height;
        protected SerializedProperty widthSegs1;
        protected SerializedProperty lengthSegs1;
        protected SerializedProperty widthSegs2;
        protected SerializedProperty lengthSegs2;
        protected SerializedProperty heightSegs;

        protected override void Init()
        {
            base.Init();
            width1 = serializedObject.FindProperty("width1");
            width2 = serializedObject.FindProperty("width2");
            width3 = serializedObject.FindProperty("width3");
            length1 = serializedObject.FindProperty("length1");
            length2 = serializedObject.FindProperty("length2");
            height = serializedObject.FindProperty("height");
            widthSegs1 = serializedObject.FindProperty("widthSegs1");
            lengthSegs1 = serializedObject.FindProperty("lengthSegs1");
            widthSegs2 = serializedObject.FindProperty("widthSegs2");
            lengthSegs2 = serializedObject.FindProperty("lengthSegs2");
            heightSegs = serializedObject.FindProperty("heightSegs");
        }

        protected override void DrawBasicParameters()
        {
            EditorGUILayout.PropertyField(width1);
            EditorGUILayout.PropertyField(width2);
            EditorGUILayout.PropertyField(width3);
            EditorGUILayout.PropertyField(length1);
            EditorGUILayout.PropertyField(length2);
            EditorGUILayout.PropertyField(height);
        }

        protected override void DrawSegments()
        {
            EditorGUILayout.PropertyField(widthSegs1);
            EditorGUILayout.PropertyField(lengthSegs1);
            EditorGUILayout.PropertyField(widthSegs2);
            EditorGUILayout.PropertyField(lengthSegs2);
            EditorGUILayout.PropertyField(heightSegs);
        }
    }
}