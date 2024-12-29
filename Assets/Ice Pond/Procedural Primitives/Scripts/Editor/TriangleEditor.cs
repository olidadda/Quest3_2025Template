using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProceduralPrimitivesUtil.PPEditor
{
    [CustomEditor(typeof(Triangle))]
    [CanEditMultipleObjects]
    public class TriangleEditor : PPBaseEditor
    {
        protected SerializedProperty width;
        protected SerializedProperty length;
        protected SerializedProperty offset;
        protected SerializedProperty widthSegs;
        protected SerializedProperty lengthSegs;
        protected SerializedProperty uniformTriangle;

        protected override void Init()
        {
            base.Init();
            width = serializedObject.FindProperty("width");
            length = serializedObject.FindProperty("length");
            offset = serializedObject.FindProperty("offset");
            widthSegs = serializedObject.FindProperty("widthSegs");
            lengthSegs = serializedObject.FindProperty("lengthSegs");
            uniformTriangle = serializedObject.FindProperty("uniformTriangle");
        }

        protected override void DrawBasicParameters()
        {
            EditorGUILayout.PropertyField(width);
            EditorGUILayout.PropertyField(length);
            EditorGUILayout.PropertyField(offset);
        }

        protected override void DrawSegments()
        {
            EditorGUILayout.PropertyField(widthSegs);
            EditorGUILayout.PropertyField(lengthSegs);
        }

        protected override void DrawOthers()
        {
            base.DrawOthers();
            EditorGUILayout.PropertyField(uniformTriangle);
        }
    }
}