using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EditorTerrainChunk),true)]
public class EditorTerrainChunkEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorTerrainChunk chunk = (EditorTerrainChunk)target;

        if(GUILayout.Button("Generate Mesh"))
        {
            chunk.GenerateMesh();
        }

        if (GUILayout.Button("Split"))
        {
            chunk.Split();
        }
    }


}
