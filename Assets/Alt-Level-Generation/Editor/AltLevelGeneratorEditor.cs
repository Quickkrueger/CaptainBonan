#if UNITY_EDITOR
using RoomTools.Brushes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AltFloorGenerator))]
public class AltLevelGeneratorEditor : Editor
{

    /// <summary>
    /// Purpose: Sets up the GUI for the TileBrush3D interface
    /// </summary>
    public override void OnInspectorGUI()
    {
        AltFloorGenerator instance = (AltFloorGenerator)target;
        DrawDefaultInspector();

        if(GUILayout.Button("Generate Floor"))
        {
            instance.Clear();
            instance.Generate();
        }
        if (GUILayout.Button("Clear Floor"))
        {
            instance.Clear();
        }
    }
}

    #endif