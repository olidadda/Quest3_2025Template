using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProceduralPrimitivesUtil.PPEditor
{
    [CustomEditor(typeof(RectRing))]
    [CanEditMultipleObjects]
    public class RectRingEditorEditor : PPBaseEditor
    {
        protected SerializedProperty width;
        protected SerializedProperty length;
        protected SerializedProperty fillet;
        protected SerializedProperty thickness;
        protected SerializedProperty widthSegs;
        protected SerializedProperty lengthSegs;
        protected SerializedProperty filletSegs;
        protected SerializedProperty ringSegs;

        protected override void Init()
        {
            base.Init();
            width = serializedObject.FindProperty("width");
            length = serializedObject.FindProperty("length");
            fillet = serializedObject.FindProperty("fillet");
            thickness = serializedObject.FindProperty("thickness");
            widthSegs = serializedObject.FindProperty("widthSegs");
            lengthSegs = serializedObject.FindProperty("lengthSegs");
            filletSegs = serializedObject.FindProperty("filletSegs");
            ringSegs = serializedObject.FindProperty("ringSegs");
        }

        protected override void DrawBasicParameters()
        {
            EditorGUILayout.PropertyField(width);
            EditorGUILayout.PropertyField(length);
            EditorGUILayout.PropertyField(fillet);
            EditorGUILayout.PropertyField(thickness);
        }

        protected override void DrawSegments()
        {
            base.DrawSegments();
            EditorGUILayout.PropertyField(widthSegs);
            EditorGUILayout.PropertyField(lengthSegs);
            EditorGUILayout.PropertyField(filletSegs);
            EditorGUILayout.PropertyField(ringSegs);
        }
    }
}