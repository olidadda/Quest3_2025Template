using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProceduralPrimitivesUtil.PPEditor
{
    [CustomEditor(typeof(ChamferCylinder))]
    [CanEditMultipleObjects]
    public class ChamferCylinderEditor : PPCircularBaseEditor
    {
        protected SerializedProperty radius;
        protected SerializedProperty height;
        protected SerializedProperty fillet;
        protected SerializedProperty chamfer1;
        protected SerializedProperty chamfer2;
        protected SerializedProperty flatChamfer;
        protected SerializedProperty capSegs;
        protected SerializedProperty heightSegs;
        protected SerializedProperty filletSegs;

        protected override void Init()
        {
            base.Init();
            radius = serializedObject.FindProperty("radius");
            height = serializedObject.FindProperty("height");
            fillet = serializedObject.FindProperty("fillet");
            chamfer1 = serializedObject.FindProperty("chamfer1");
            chamfer2 = serializedObject.FindProperty("chamfer2");
            flatChamfer = serializedObject.FindProperty("flatChamfer");
            capSegs = serializedObject.FindProperty("capSegs");
            heightSegs = serializedObject.FindProperty("heightSegs");
            filletSegs = serializedObject.FindProperty("filletSegs");
        }

        protected override void DrawBasicParameters()
        {
            EditorGUILayout.PropertyField(radius);
            EditorGUILayout.PropertyField(height);
            EditorGUILayout.PropertyField(fillet);
            EditorGUILayout.PropertyField(chamfer1);
            EditorGUILayout.PropertyField(chamfer2);
            EditorGUILayout.PropertyField(flatChamfer);
        }

        protected override void DrawSegments()
        {
            base.DrawSegments();
            EditorGUILayout.PropertyField(capSegs);
            EditorGUILayout.PropertyField(heightSegs);
            EditorGUILayout.PropertyField(filletSegs);
        }
    }
}