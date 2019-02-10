using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ThreadedDataRequester))]
public class ThreadedDataReuestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ThreadedDataRequester tar = (ThreadedDataRequester)target;

        if(GUILayout.Button("Make Useable In Editor"))
        {
            tar.MakeUseableInEditor();
        }
    }
}
