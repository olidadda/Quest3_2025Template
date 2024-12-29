using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProceduralPrimitivesUtil.PPEditor
{
    [CustomEditor(typeof(Stair))]
    [CanEditMultipleObjects]
    public class StairEditor : PPBaseEditor
    {
        protected SerializedProperty style;
        protected SerializedProperty width;
        protected SerializedProperty length;
        protected SerializedProperty height;
        protected SerializedProperty steps;
        protected SerializedProperty stepWidthSegs;
        protected SerializedProperty stepLengthSegs;
        protected SerializedProperty stepHeightSegs;

        protected override void Init()
        {
            base.Init();
            style = serializedObject.FindProperty("style");
            width = serializedObject.FindProperty("width");
            length = serializedObject.FindProperty("length");
            height = serializedObject.FindProperty("height");
            steps = serializedObject.FindProperty("steps");
            stepWidthSegs = serializedObject.FindProperty("stepWidthSegs");
            stepLengthSegs = serializedObject.FindProperty("stepLengthSegs");
            stepHeightSegs = serializedObject.FindProperty("stepHeightSegs");
        }

        protected override void DrawBasicParameters()
        {
            EditorGUILayout.PropertyField(style);
            EditorGUILayout.PropertyField(width);
            EditorGUILayout.PropertyField(length);
            EditorGUILayout.PropertyField(height);
            EditorGUILayout.PropertyField(steps);
        }

        protected override void DrawSegments()
        {
            EditorGUILayout.PropertyField(stepWidthSegs);
            EditorGUILayout.PropertyField(stepLengthSegs);
            EditorGUILayout.PropertyField(stepHeightSegs);
        }
    }
}