using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProceduralPrimitivesUtil.PPEditor
{
    [CustomEditor(typeof(PolygonF))]
    [CanEditMultipleObjects]
    public class PolygonFEditor : PPBaseEditor
    {
        protected SerializedProperty pointOrder;
        protected SerializedProperty points;

        protected override void Init()
        {
            base.Init();
            hasSegment = false;
            pointOrder = serializedObject.FindProperty("pointOrder");
            points = serializedObject.FindProperty("points");
        }

        protected override void DrawBasicParameters()
        {
            EditorGUILayout.PropertyField(pointOrder);
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.PropertyField(points);
        }
    }
}