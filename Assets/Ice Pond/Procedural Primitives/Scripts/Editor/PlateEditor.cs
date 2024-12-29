using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProceduralPrimitivesUtil.PPEditor
{
    [CustomEditor(typeof(Plate))]
    [CanEditMultipleObjects]
    public class PlateEditor : PPCircularBaseEditor
    {
        protected SerializedProperty radius;
        protected SerializedProperty fillet;
        protected SerializedProperty thickness;
        protected SerializedProperty cap;
        protected SerializedProperty flatChamfer;
        protected SerializedProperty capSegs;
        protected SerializedProperty filletSegs;

        protected override void Init()
        {
            base.Init();
            radius = serializedObject.FindProperty("radius");
            fillet = serializedObject.FindProperty("fillet");
            thickness = serializedObject.FindProperty("thickness");
            cap = serializedObject.FindProperty("cap");
            flatChamfer = serializedObject.FindProperty("flatChamfer");
            capSegs = serializedObject.FindProperty("capSegs");
            filletSegs = serializedObject.FindProperty("filletSegs");
        }

        protected override void DrawBasicParameters()
        {
            EditorGUILayout.PropertyField(radius);
            EditorGUILayout.PropertyField(fillet);
            EditorGUILayout.PropertyField(thickness);
            EditorGUILayout.PropertyField(cap);
            EditorGUILayout.PropertyField(flatChamfer);
        }

        protected override void DrawSegments()
        {
            base.DrawSegments();
            EditorGUILayout.PropertyField(capSegs);
            EditorGUILayout.PropertyField(filletSegs);
        }
    }
}