using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProceduralPrimitivesUtil.PPEditor
{
    [CustomEditor(typeof(RectTube))]
    [CanEditMultipleObjects]
    public class RectTubeEditor : PPBaseEditor
    {
        protected SerializedProperty width1;
        protected SerializedProperty length1;
        protected SerializedProperty width2;
        protected SerializedProperty length2;
        protected SerializedProperty height;
        protected SerializedProperty cap1;
        protected SerializedProperty capThickness1;
        protected SerializedProperty cap2;
        protected SerializedProperty capThickness2;
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
            cap1 = serializedObject.FindProperty("cap1");
            capThickness1 = serializedObject.FindProperty("capThickness1");
            cap2 = serializedObject.FindProperty("cap2");
            capThickness2 = serializedObject.FindProperty("capThickness2");
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
            EditorGUILayout.PropertyField(cap1);
            EditorGUILayout.PropertyField(capThickness1);
            EditorGUILayout.PropertyField(cap2);
            EditorGUILayout.PropertyField(capThickness2);
        }

        protected override void DrawSegments()
        {
            EditorGUILayout.PropertyField(widthSegs);
            EditorGUILayout.PropertyField(lengthSegs);
            EditorGUILayout.PropertyField(heightSegs);
        }
    }
}