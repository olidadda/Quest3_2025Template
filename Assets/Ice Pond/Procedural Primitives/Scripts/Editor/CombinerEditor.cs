using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProceduralPrimitivesUtil.PPEditor
{
    [CustomEditor(typeof(PPCombiner))]
    [CanEditMultipleObjects]
    public class CombinerEditor : PPBaseEditor
    {
        protected SerializedProperty elements;
        protected SerializedProperty includeChildren;
        protected SerializedProperty applyElementsTransform;

        protected override void Init()
        {
            base.Init();
            hasSegment = false;
            elements = serializedObject.FindProperty("elements");
            includeChildren = serializedObject.FindProperty("includeChildren");
            applyElementsTransform = serializedObject.FindProperty("applyElementsTransform");
        }

        protected override void DrawBasicParameters()
        {
            EditorGUILayout.PropertyField(includeChildren);
            EditorGUILayout.PropertyField(applyElementsTransform);
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.PropertyField(elements);
        }
    }
}