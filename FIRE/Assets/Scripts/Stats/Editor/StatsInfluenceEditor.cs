using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class StatsInfluenceEditor : NodeBasedEditor
{
    [SerializeField] float radius = 2f;

    [SerializeField] StatsInfluenceEditorGraphStorage storage = null;

    [MenuItem("Window/Stats Influence Graph Editor")]
    private static void OpenWindow()
    {
        StatsInfluenceEditor window = GetWindow<StatsInfluenceEditor>();

        tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        tex.SetPixel(0, 0, window.Settings.WindowBackgroundColor);
        tex.Apply();

        window.titleContent = new GUIContent("Stats Influence Editor");
        window.Initialize();
    }

    protected override Func<Connection> ConnectionGenerator
    {
        get
        {
            EditorGraphNode firstNode = selectedNodes[0];//local for closure
            EditorGraphNode secondNode = selectedNodes[1];//local for closure
            return () => new StatInfluenceConnection(() => firstNode.rect, () => secondNode.rect, OnClickRemoveConnection, this,(firstNode as StatNode).PropertiesInfoArrayIndex,(secondNode as StatNode).PropertiesInfoArrayIndex);
        }
    }

    public void Initialize()
    {
        if (storage.HasDataStored)
            InitializeFromStorage();
        else
            InitializeNew();
    }
       
    public void InitializeFromStorage()
    {
        //TODO everything
        Dictionary<int, StatNode> tempNodeDic = new Dictionary<int, StatNode>();
        foreach(StatsInfluenceEditorGraphStorage.StatNodeInfo info in storage.NodeInfos)
        {
            StatNode node =
            AddNode(()=>CreateNode(info.Position, info.Name, info.Idx)) as StatNode;
            tempNodeDic.Add(info.Idx, node);
        }

        foreach(StatInfluenceConnectionInfo info in storage.ConnectionInfos)
        {
            StatNode influencer = tempNodeDic[info.Influencer];
            StatNode influenced = tempNodeDic[info.Influenced];

            StatInfluenceConnection con = new StatInfluenceConnection(() => influencer.rect,() => influenced.rect, OnClickRemoveConnection, this, influencer.PropertiesInfoArrayIndex, influenced.PropertiesInfoArrayIndex,info.Value);

            AddConnection(con);
        }

        //InitializeNew();
    }

    public void InitializeNew()
    {
        PropertyInfo[] stats = typeof(StatSheet).GetProperties();

        float spacing = 1.0f / stats.Length;

        for (int i = 0; i < stats.Length; i++)
        {
            float circleRad = (i * spacing) * Mathf.PI * 2f;

            Vector2 posUnit = new Vector2(Mathf.Cos(circleRad), Mathf.Sin(circleRad));

            float mult = Mathf.Sqrt((Settings.NodeHeight*Settings.NodeHeight)+ (Settings.NodeWidth * Settings.NodeWidth));

            Vector2 pos = ((posUnit * radius * mult) + new Vector2(position.width,position.height)*.5f) - new Vector2(Settings.NodeWidth,Settings.NodeHeight)*.5f;

            AddNode(()=> CreateNode(pos, stats[i].PropertyType.ToString(),i));
        }
    }

    protected StatNode CreateNode(Vector2 pos, string name , int index)
    {
        return new StatNode(pos, OnNodeClickWithCtrl, OnClickRemoveNode, this, name, index);
    }

    protected override void ProcessRightClickContextMenu(Event e)
    {}

    public override void Save()
    {
        if(connections != null)
        {
            storage.StoreGraph(nodes.ConvertAll((e)=> e as StatNode), connections.ConvertAll((c)=> c as StatInfluenceConnection));
        }
    }
}