using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProceduralPrimitivesUtil.PPEditor
{
    [CustomEditor(typeof(ChamferPlane))]
    [CanEditMultipleObjects]
    public class ChamferPlaneEditor : PPBaseEditor
    {
        protected SerializedProperty width;
        protected SerializedProperty length;
        protected SerializedProperty fillet;
        protected SerializedProperty widthSegs;
        protected SerializedProperty lengthSegs;
        protected SerializedProperty filletSegs;
        protected SerializedProperty hollow;

        protected override void Init()
        {
            base.Init();
            width = serializedObject.FindProperty("width");
            length = serializedObject.FindProperty("length");
            fillet = serializedObject.FindProperty("fillet");
            widthSegs = serializedObject.FindProperty("widthSegs");
            lengthSegs = serializedObject.FindProperty("lengthSegs");
            filletSegs = serializedObject.FindProperty("filletSegs");
            hollow = serializedObject.FindProperty("hollow");
        }

        protected override void DrawBasicParameters()
        {
            EditorGUILayout.PropertyField(width);
            EditorGUILayout.PropertyField(length);
            EditorGUILayout.PropertyField(fillet);
            EditorGUILayout.PropertyField(hollow);
        }

        protected override void DrawSegments()
        {
            EditorGUILayout.PropertyField(widthSegs);
            EditorGUILayout.PropertyField(lengthSegs);
            EditorGUILayout.PropertyField(filletSegs);
        }
    }
}