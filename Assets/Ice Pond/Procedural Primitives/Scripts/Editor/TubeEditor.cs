using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProceduralPrimitivesUtil.PPEditor
{
    [CustomEditor(typeof(Tube))]
    [CanEditMultipleObjects]
    public class TubeEditor : PPCircularBaseEditor
    {
        protected SerializedProperty radius1;
        protected SerializedProperty radius2;
        protected SerializedProperty height;
        protected SerializedProperty cap1;
        protected SerializedProperty capThickness1;
        protected SerializedProperty cap2;
        protected SerializedProperty capThickness2;
        protected SerializedProperty capSegs;
        protected SerializedProperty heightSegs;

        protected override void Init()
        {
            base.Init();
            radius1 = serializedObject.FindProperty("radius1");
            radius2 = serializedObject.FindProperty("radius2");
            height = serializedObject.FindProperty("height");
            cap1 = serializedObject.FindProperty("cap1");
            capThickness1 = serializedObject.FindProperty("capThickness1");
            cap2 = serializedObject.FindProperty("cap2");
            capThickness2 = serializedObject.FindProperty("capThickness2");
            capSegs = serializedObject.FindProperty("capSegs");
            heightSegs = serializedObject.FindProperty("heightSegs");
        }

        protected override void DrawBasicParameters()
        {
            EditorGUILayout.PropertyField(radius1);
            EditorGUILayout.PropertyField(radius2);
            EditorGUILayout.PropertyField(height);
            EditorGUILayout.PropertyField(cap1);
            EditorGUILayout.PropertyField(capThickness1);
            EditorGUILayout.PropertyField(cap2);
            EditorGUILayout.PropertyField(capThickness2);
        }

        protected override void DrawSegments()
        {
            base.DrawSegments();
            EditorGUILayout.PropertyField(capSegs);
            EditorGUILayout.PropertyField(heightSegs);
        }
    }
}