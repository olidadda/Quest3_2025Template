using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProceduralPrimitivesUtil.PPEditor
{
    [CustomEditor(typeof(Tray))]
    [CanEditMultipleObjects]
    public class TrayEditor : PPBaseEditor
    {
        protected SerializedProperty width;
        protected SerializedProperty length;
        protected SerializedProperty fillet;
        protected SerializedProperty thickness;
        protected SerializedProperty cap;
        protected SerializedProperty flatChamfer;
        protected SerializedProperty widthSegs;
        protected SerializedProperty lengthSegs;
        protected SerializedProperty filletSegs;

        protected override void Init()
        {
            base.Init();
            hasSmooth = true;
            width = serializedObject.FindProperty("width");
            length = serializedObject.FindProperty("length");
            fillet = serializedObject.FindProperty("fillet");
            thickness = serializedObject.FindProperty("thickness");
            cap = serializedObject.FindProperty("cap");
            flatChamfer = serializedObject.FindProperty("flatChamfer");
            widthSegs = serializedObject.FindProperty("widthSegs");
            lengthSegs = serializedObject.FindProperty("lengthSegs");
            filletSegs = serializedObject.FindProperty("filletSegs");
        }

        protected override void DrawBasicParameters()
        {
            EditorGUILayout.PropertyField(width);
            EditorGUILayout.PropertyField(length);
            EditorGUILayout.PropertyField(fillet);
            EditorGUILayout.PropertyField(thickness);
            EditorGUILayout.PropertyField(cap);
            EditorGUILayout.PropertyField(flatChamfer);
        }

        protected override void DrawSegments()
        {
            base.DrawSegments();
            EditorGUILayout.PropertyField(widthSegs);
            EditorGUILayout.PropertyField(lengthSegs);
            EditorGUILayout.PropertyField(filletSegs);
        }
    }
}