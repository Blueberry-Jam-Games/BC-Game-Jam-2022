using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RuntimeSlide))]
public class RuntimeSlideInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RuntimeSlide slide = (RuntimeSlide)target;

        if (GUILayout.Button("Add staff"))
        {
            slide.addStaff();
        }

        if (GUILayout.Button("Remove staff"))
        {
            slide.removeStaff();
        }

        if (GUILayout.Button("Close Slide"))
        {
            slide.closeRide();
        }

        if (GUILayout.Button("Open Slide"))
        {
            slide.openRide();
        }
    }
}