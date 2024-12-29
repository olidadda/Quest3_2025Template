using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProceduralPrimitivesUtil.PPEditor
{
    [CustomEditor(typeof(SpiralStair))]
    [CanEditMultipleObjects]
    public class SpiralStairEditor : PPBaseEditor
    {
        protected SerializedProperty style;
        protected SerializedProperty radius1;
        protected SerializedProperty radius2;
        protected SerializedProperty height;
        protected SerializedProperty loops;
        protected SerializedProperty steps;
        protected SerializedProperty stepWidthSegs;
        protected SerializedProperty stepLengthSegs;
        protected SerializedProperty stepHeightSegs;

        protected override void Init()
        {
            base.Init();
            hasSmooth = true;
            style = serializedObject.FindProperty("style");
            radius1 = serializedObject.FindProperty("radius1");
            radius2 = serializedObject.FindProperty("radius2");
            height = serializedObject.FindProperty("height");
            loops = serializedObject.FindProperty("loops");
            steps = serializedObject.FindProperty("steps");
            stepWidthSegs = serializedObject.FindProperty("stepWidthSegs");
            stepLengthSegs = serializedObject.FindProperty("stepLengthSegs");
            stepHeightSegs = serializedObject.FindProperty("stepHeightSegs");
        }

        protected override void DrawBasicParameters()
        {
            EditorGUILayout.PropertyField(style);
            EditorGUILayout.PropertyField(radius1);
            EditorGUILayout.PropertyField(radius2);
            EditorGUILayout.PropertyField(height);
            EditorGUILayout.PropertyField(loops);
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