using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProceduralPrimitivesUtil.PPEditor
{
    [CustomEditor(typeof(DoubleSphere))]
    [CanEditMultipleObjects]
    public class DoubleSphereEditor : PPCircularBaseEditor
    {
        protected SerializedProperty radius1;
        protected SerializedProperty radius2;
        protected SerializedProperty cutType;
        protected SerializedProperty cutFrom;
        protected SerializedProperty cutTo;

        protected override void Init()
        {
            base.Init();
            radius1 = serializedObject.FindProperty("radius1");
            radius2 = serializedObject.FindProperty("radius2");
            cutType = serializedObject.FindProperty("cutType");
            cutFrom = serializedObject.FindProperty("cutFrom");
            cutTo = serializedObject.FindProperty("cutTo");
        }

        protected override void DrawBasicParameters()
        {
            EditorGUILayout.PropertyField(radius1);
            EditorGUILayout.PropertyField(radius2);
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