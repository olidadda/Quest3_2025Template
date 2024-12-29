using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProceduralPrimitivesUtil.PPEditor
{
    [CustomEditor(typeof(ChamferBox))]
    [CanEditMultipleObjects]
    public class ChamferBoxEditor : PPBaseEditor
    {
        protected SerializedProperty width;
        protected SerializedProperty length;
        protected SerializedProperty height;
        protected SerializedProperty fillet;
        protected SerializedProperty chamfer1;
        protected SerializedProperty chamfer2;
        protected SerializedProperty flatChamfer;
        protected SerializedProperty flatTop;
        protected SerializedProperty flatBottom;
        protected SerializedProperty widthSegs;
        protected SerializedProperty lengthSegs;
        protected SerializedProperty heightSegs;
        protected SerializedProperty filletSegs;

        protected override void Init()
        {
            base.Init();
            hasSmooth = true;
            width = serializedObject.FindProperty("width");
            length = serializedObject.FindProperty("length");
            height = serializedObject.FindProperty("height");
            fillet = serializedObject.FindProperty("fillet");
            chamfer1 = serializedObject.FindProperty("chamfer1");
            chamfer2 = serializedObject.FindProperty("chamfer2");
            flatChamfer = serializedObject.FindProperty("flatChamfer");
            widthSegs = serializedObject.FindProperty("widthSegs");
            lengthSegs = serializedObject.FindProperty("lengthSegs");
            heightSegs = serializedObject.FindProperty("heightSegs");
            filletSegs = serializedObject.FindProperty("filletSegs");
        }

        protected override void DrawBasicParameters()
        {
            EditorGUILayout.PropertyField(width);
            EditorGUILayout.PropertyField(length);
            EditorGUILayout.PropertyField(height);
            EditorGUILayout.PropertyField(fillet);
            EditorGUILayout.PropertyField(chamfer1);
            EditorGUILayout.PropertyField(chamfer2);
            EditorGUILayout.PropertyField(flatChamfer);
        }

        protected override void DrawSegments()
        {
            EditorGUILayout.PropertyField(widthSegs);
            EditorGUILayout.PropertyField(lengthSegs);
            EditorGUILayout.PropertyField(heightSegs);
            EditorGUILayout.PropertyField(filletSegs);
        }
    }
}