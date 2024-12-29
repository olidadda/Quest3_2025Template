using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProceduralPrimitivesUtil.PPEditor
{
    [CustomEditor(typeof(Prism))]
    [CanEditMultipleObjects]
    public class PrismEditor : PPBaseEditor
    {
        protected SerializedProperty width;
        protected SerializedProperty length;
        protected SerializedProperty height;
        protected SerializedProperty offset;
        protected SerializedProperty sideSegs;
        protected SerializedProperty heightSegs;

        protected override void Init()
        {
            base.Init();
            width = serializedObject.FindProperty("width");
            length = serializedObject.FindProperty("length");
            height = serializedObject.FindProperty("height");
            offset = serializedObject.FindProperty("offset");
            sideSegs = serializedObject.FindProperty("sideSegs");
            heightSegs = serializedObject.FindProperty("heightSegs");
        }

        protected override void DrawBasicParameters()
        {
            EditorGUILayout.PropertyField(width);
            EditorGUILayout.PropertyField(length);
            EditorGUILayout.PropertyField(height);
            EditorGUILayout.PropertyField(offset);
        }

        protected override void DrawSegments()
        {
            EditorGUILayout.PropertyField(sideSegs);
            EditorGUILayout.PropertyField(heightSegs);
        }
    }
}