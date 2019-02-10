using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapDisplay))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapDisplay mapGen = (MapDisplay)target;

        if (DrawDefaultInspector())
        {
            if (mapGen.AutoUpdate)
            {
                mapGen.DrawInEditor();
            }
        }// end if draw default

        if (GUILayout.Button("Generat"))
        {
            mapGen.DrawInEditor();
        }
    }
}
