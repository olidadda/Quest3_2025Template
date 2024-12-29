using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProceduralPrimitivesUtil.PPEditor
{
    [CustomEditor(typeof(RectTubeR))]
    [CanEditMultipleObjects]
    public class RectTubeREditor : PPBaseEditor
    {
        protected SerializedProperty width;
        protected SerializedProperty length;
        protected SerializedProperty height;
        protected SerializedProperty fillet;
        protected SerializedProperty thickness;
        protected SerializedProperty cap1;
        protected SerializedProperty cap2;
        protected SerializedProperty widthSegs;
        protected SerializedProperty lengthSegs;
        protected SerializedProperty heightSegs;
        protected SerializedProperty filletSegs;

        protected override void Init()
        {
            base.Init();
            hasSmooth = true;
            width = serializedObject.FindProperty("width");
            length = serializedObject.FindProperty("length");
            height = serializedObject.FindProperty("height");
            fillet = serializedObject.FindProperty("fillet");
            thickness = serializedObject.FindProperty("thickness");
            cap1 = serializedObject.FindProperty("cap1");
            cap2 = serializedObject.FindProperty("cap2");
            widthSegs = serializedObject.FindProperty("widthSegs");
            lengthSegs = serializedObject.FindProperty("lengthSegs");
            heightSegs = serializedObject.FindProperty("heightSegs");
            filletSegs = serializedObject.FindProperty("filletSegs");
        }

        protected override void DrawBasicParameters()
        {
            EditorGUILayout.PropertyField(width);
            EditorGUILayout.PropertyField(length);
            EditorGUILayout.PropertyField(height);
            EditorGUILayout.PropertyField(fillet);
            EditorGUILayout.PropertyField(thickness);
            EditorGUILayout.PropertyField(cap1);
            EditorGUILayout.PropertyField(cap2);
        }

        protected override void DrawSegments()
        {
            EditorGUILayout.PropertyField(widthSegs);
            EditorGUILayout.PropertyField(lengthSegs);
            EditorGUILayout.PropertyField(heightSegs);
            EditorGUILayout.PropertyField(filletSegs);
        }
    }
}