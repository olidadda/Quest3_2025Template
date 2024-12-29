using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProceduralPrimitivesUtil.PPEditor
{
    [CustomEditor(typeof(TorusTube))]
    [CanEditMultipleObjects]
    public class TorusTubeEditor : PPCircularBaseEditor
    {
        protected SerializedProperty radius1;
        protected SerializedProperty radius2;
        protected SerializedProperty radius3;
        protected SerializedProperty segments;

        protected override void Init()
        {
            base.Init();
            radius1 = serializedObject.FindProperty("radius1");
            radius2 = serializedObject.FindProperty("radius2");
            radius3 = serializedObject.FindProperty("radius3");
            segments = serializedObject.FindProperty("segments");
        }

        protected override void DrawBasicParameters()
        {
            EditorGUILayout.PropertyField(radius1);
            EditorGUILayout.PropertyField(radius2);
            EditorGUILayout.PropertyField(radius3);
        }

        protected override void DrawSegments()
        {
            base.DrawSegments();
            EditorGUILayout.PropertyField(segments);
        }
    }
}