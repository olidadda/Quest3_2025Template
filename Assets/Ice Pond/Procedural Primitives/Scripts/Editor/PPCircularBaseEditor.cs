using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProceduralPrimitivesUtil.PPEditor
{
    [CustomEditor(typeof(PPCircularBase), true)]
    [CanEditMultipleObjects]
    public class PPCircularBaseEditor : PPBaseEditor
    {
        protected SerializedProperty sides;
        protected SerializedProperty sliceOn;
        protected SerializedProperty sliceFrom;
        protected SerializedProperty sliceTo;

        protected override void Init()
        {
            base.Init();
            hasSmooth = true;
            sides = serializedObject.FindProperty("sides");
            sliceOn = serializedObject.FindProperty("sliceOn");
            sliceFrom = serializedObject.FindProperty("sliceFrom");
            sliceTo = serializedObject.FindProperty("sliceTo");
        }

        protected override void DrawSegments()
        {
            EditorGUILayout.PropertyField(sides);
        }

        protected override void DrawSlice()
        {
            EditorGUILayout.PropertyField(sliceOn);
            EditorGUILayout.PropertyField(sliceFrom);
            EditorGUILayout.PropertyField(sliceTo);
        }
    }
}