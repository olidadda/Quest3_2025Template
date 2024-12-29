using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProceduralPrimitivesUtil.PPEditor
{
    [CustomEditor(typeof(Trapezoid))]
    [CanEditMultipleObjects]
    public class TrapezoidEditor : PPBaseEditor
    {
        protected SerializedProperty width1;
        protected SerializedProperty width2;
        protected SerializedProperty length;
        protected SerializedProperty offset;
        protected SerializedProperty widthSegs;
        protected SerializedProperty lengthSegs;

        protected override void Init()
        {
            base.Init();
            width1 = serializedObject.FindProperty("width1");
            width2 = serializedObject.FindProperty("width2");
            length = serializedObject.FindProperty("length");
            offset = serializedObject.FindProperty("offset");
            widthSegs = serializedObject.FindProperty("widthSegs");
            lengthSegs = serializedObject.FindProperty("lengthSegs");
        }

        protected override void DrawBasicParameters()
        {
            EditorGUILayout.PropertyField(width1);
            EditorGUILayout.PropertyField(width2);
            EditorGUILayout.PropertyField(length);
            EditorGUILayout.PropertyField(offset);
        }

        protected override void DrawSegments()
        {
            EditorGUILayout.PropertyField(widthSegs);
            EditorGUILayout.PropertyField(lengthSegs);
        }
    }
}