using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SaveableOutsideOfEditorFolder),true)]
public class SaveableOutsideOfEditorFolderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SaveableOutsideOfEditorFolder tar = (SaveableOutsideOfEditorFolder)target;

        if (GUILayout.Button("Create Useable Data Outside Editor"))
        {
            tar.CreateUseableDataOutsideOfEditor();
        }

        if (GUILayout.Button("Clear Data"))
        {
            tar.ClearData();
        }

    }
}
