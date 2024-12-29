using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProceduralPrimitivesUtil.PPEditor
{
    [CustomEditor(typeof(Circle))]
    [CanEditMultipleObjects]
    public class CircleEditor : PPCircularBaseEditor
    {
        protected SerializedProperty radius;
        protected SerializedProperty segments;

        protected override void Init()
        {
            base.Init();
            radius = serializedObject.FindProperty("radius");
            segments = serializedObject.FindProperty("segments");
        }

        protected override void DrawBasicParameters()
        {
            EditorGUILayout.PropertyField(radius);
        }

        protected override void DrawSegments()
        {
            base.DrawSegments();
            EditorGUILayout.PropertyField(segments);
        }
    }
}