using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StatInfluenceGraphStorage : ScriptableObject
{

    [SerializeField] List<StatInfluenceConnectionInfo> connectionInfos;
    public IEnumerable<StatInfluenceConnectionInfo> ConnectionInfos { get { return connectionInfos; } }

    public static void CreateStatInfluenceGraphStorage(string path, string name, List<StatInfluenceConnectionInfo> connections)
    {
        StatInfluenceGraphStorage asset = ScriptableObject.CreateInstance<StatInfluenceGraphStorage>();

        asset.connectionInfos = connections;

        string assetPath = path + name + ".asset";

        UnityEngine.Object  obj = AssetDatabase.LoadAssetAtPath(assetPath,typeof(UnityEngine.Object));

        if (obj != null)
            AssetDatabase.DeleteAsset(assetPath);

        AssetDatabase.CreateAsset(asset, assetPath );
        AssetDatabase.SaveAssets();
    }
}

[Serializable]
public struct StatInfluenceConnectionInfo
{
    public int Influencer, Influenced;
    public float Value;
}