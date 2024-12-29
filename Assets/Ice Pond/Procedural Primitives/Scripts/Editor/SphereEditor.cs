using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProceduralPrimitivesUtil.PPEditor
{
    [CustomEditor(typeof(Sphere))]
    [CanEditMultipleObjects]
    public class SphereEditor : PPCircularBaseEditor
    {
        protected SerializedProperty radius;
        protected SerializedProperty cutType;
        protected SerializedProperty cutFrom;
        protected SerializedProperty cutTo;

        protected override void Init()
        {
            base.Init();
            radius = serializedObject.FindProperty("radius");
            cutType = serializedObject.FindProperty("cutType");
            cutFrom = serializedObject.FindProperty("cutFrom");
            cutTo = serializedObject.FindProperty("cutTo");
        }

        protected override void DrawBasicParameters()
        {
            EditorGUILayout.PropertyField(radius);
        }

        protected override void DrawSlice()
        {
            base.DrawSlice();
            EditorGUILayout.PropertyField(cutType);
            EditorGUILayout.PropertyField(cutFrom);
            EditorGUILayout.PropertyField(cutTo);
        }
    }
}