using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName ="Stats Influence Storage")]
public class StatsInfluenceEditorGraphStorage : EditorGraphStorage<StatNode, StatInfluenceConnection>
{
    [SerializeField] public List<StatNodeInfo> NodeInfos;
    [SerializeField] public List<StatInfluenceConnectionInfo> ConnectionInfos;
    [SerializeField] bool hasDataStored;
    public bool HasDataStored { get { return hasDataStored; }}

    public override void ClearData()
    {
        hasDataStored = false;
        NodeInfos = null;
        ConnectionInfos = null;
        AssetDatabase.SaveAssets();
    }

    public override void CreateUseableDataOutsideOfEditor()
    {
        if(ConnectionInfos != null)
        {
            StatInfluenceGraphStorage.CreateStatInfluenceGraphStorage(CreateUseableAssetPath, name, ConnectionInfos);
        }
    }

    public override void StoreGraph(List<StatNode> nodes, List<StatInfluenceConnection> connections)
    {
        hasDataStored = true;

        NodeInfos = new List<StatNodeInfo>();
        ConnectionInfos = new List<StatInfluenceConnectionInfo>();

        foreach(StatNode n in nodes)
        {
            NodeInfos.Add(new StatNodeInfo() { Name = n.Name, Position = n.rect.position, Idx = n.PropertiesInfoArrayIndex });
        }

        foreach(StatInfluenceConnection c in connections)
        {
            ConnectionInfos.Add(new StatInfluenceConnectionInfo()
            {
                Influencer = c.Influencer,
                Influenced = c.Influenced,
                Value = c.Influence

            });
        }

        AssetDatabase.SaveAssets();
    }


    [Serializable]
    public struct StatNodeInfo
    {
        public string Name;
        public int Idx;
        public Vector2 Position;
    }
}
