using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class NodeBasedEditor : EditorWindow 
{
    protected static Texture2D tex;

    public event Action<Connection> OnConnectionCreated;
    [SerializeField]protected NodeBasedEditorSetttings settings;
    public NodeBasedEditorSetttings Settings { get { return settings; } }

    protected List<EditorGraphNode> nodes;
    protected List<Connection> connections;

    protected EditorGraphNode[] selectedNodes = new EditorGraphNode[2];
    int selectIdx = 0;

    Vector2 dragThisOnGUICall;
    Vector2 offset;

    protected virtual Func<Connection> ConnectionGenerator
    {
        get
        {
            EditorGraphNode firstNode = selectedNodes[0];//local for closure
            EditorGraphNode secondNode = selectedNodes[1];//local for closure
            return () => new Connection(() => firstNode.rect, () => secondNode.rect, OnClickRemoveConnection, this);
        }
    }

    protected virtual void OnEnable()
    {
        if (settings == null)
            Debug.Log("Oh no");
        settings.OnValuesUpdated += UpdateBackgroundColorAndResizeNodes;
    }

    protected virtual void OnDestroy()
    {
        if(settings != null)
        {
            settings.OnValuesUpdated -= UpdateBackgroundColorAndResizeNodes;
        }
    }

    void UpdateBackgroundColorAndResizeNodes()
    {
        tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        tex.SetPixel(0, 0, Settings.WindowBackgroundColor);
        tex.Apply();
        if(nodes != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Resize();
            }
        }
    }

    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), tex, ScaleMode.StretchToFill);

        DrawGrid(Settings.GridMinorLineSpacing, Settings.GridMinorLineOpacity, Settings.GridMinorLineColor);
        DrawGrid(Settings.GridMajorLineSpacing, Settings.GridMajorLineOpacity, Settings.GridMajorLineColor);

        DrawNodes();
        DrawConnections();

        DrawConnectionLine(Event.current);

        ProcessNodeEvents(Event.current);
        ProcessEvents(Event.current);

        if (GUI.changed) Repaint();
    }

    protected void DrawGrid(float spacing, float opacity, Color col)
    {
        int widthDivs = Mathf.CeilToInt(position.width / spacing);
        int heightDivs = Mathf.CeilToInt(position.height / spacing);

        Handles.BeginGUI();
        Handles.color = new Color(col.r, col.g, col.b, opacity);

        offset += dragThisOnGUICall * .5f;
        Vector3 newOffset = new Vector3(offset.x % spacing, offset.y % spacing, 0);

        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(spacing * i, -spacing, 0) + newOffset, new Vector3(spacing * i, position.height, 0f) + newOffset);
        }

        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-spacing, spacing * j, 0) + newOffset, new Vector3(position.width, spacing * j, 0) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();

    }

    protected void DrawConnectionLine(Event e)
    {
        if(selectedNodes[0] != null)
        {
            Vector2 inPoBez = GetClosestPointRect(selectedNodes[0].rect, e.mousePosition);
            Vector2 tangentMod = Vector2.zero;

            Vector2 dir = inPoBez - selectedNodes[0].rect.center;

            if (dir.x > 0)
                tangentMod += Vector2.right;
            else if (dir.x < 0)
                tangentMod -= Vector2.right;

            Handles.DrawBezier(
                inPoBez,
                e.mousePosition,
                inPoBez + tangentMod * Settings.ConnectionBezierTangentMultiplier,
                e.mousePosition- tangentMod * Settings.ConnectionBezierTangentMultiplier,
                Settings.ConnectionColor,
                Settings.ConnectionBezierTexture,
                Settings.ConnectionBezierWidth
                );
            GUI.changed = true;
        }
    }

    protected void DrawConnections()
    {
        if(connections != null)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                connections[i].Draw();
            }
        }
    }

    protected void ProcessNodeEvents(Event e)
    {
        if(nodes!= null)
        {
            for (int i = nodes.Count-1; i >= 0; i--)
            {//reversed for cause last node drawn on top and needs events processed first
                bool guiChanged = nodes[i].ProcessEvents(e);
                if (guiChanged)//don't set directly to guiChanged cause we don't wanna set it flase
                    GUI.changed = true;
            }
        }
    }

    protected virtual void ProcessEvents(Event e)
    {
        dragThisOnGUICall = Vector2.zero;
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                    ClearConnectionSelection();
                if (e.button == 1)
                    ProcessRightClickContextMenu(e);
                break;
            case EventType.MouseDrag:
                if(e.button == 0)
                {
                    OnDrag(e.delta);
                }
                break;
        }
    }

    private void OnDrag(Vector2 delta)
    {

        dragThisOnGUICall = delta;
        if (nodes != null)
            for (int i = 0; i < nodes.Count; i++)
                nodes[i].Drag(delta);
        GUI.changed = true;
    }

    protected virtual void ProcessRightClickContextMenu(Event e)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add Node"), false, () => OnClickAddNode(e.mousePosition));
        genericMenu.ShowAsContext();
    }

    protected  void OnClickAddNode(Vector2 mousePosition)
    {
        AddNode(()=> new EditorGraphNode(mousePosition, OnNodeClickWithCtrl, OnClickRemoveNode, this));
    }

    protected EditorGraphNode AddNode(Func<EditorGraphNode> nodeCreator)
    {
        if (nodeCreator == null)
            return null;
        if (nodes == null)
            nodes = new List<EditorGraphNode>();

        nodes.Add(nodeCreator());
        return nodes[nodes.Count - 1];
    }

    protected void OnClickRemoveNode(EditorGraphNode node)
    {
        if(connections != null)
        {
            List<Connection> connectionsToRemove = new List<Connection>();

            for (int i = connections.Count-1; i >= 0; i--)
            {
                if(connections[i].InPoint == node.rect || connections[i].OutPoint == node.rect)
                {
                    connections.RemoveAt(i);
                }
            }
        }

        nodes.Remove(node);
    }

    protected void OnNodeClickWithCtrl(EditorGraphNode node)
    {
        ProcessNodeClickedWithCtrl(node,ConnectionGenerator);
    }

    void ProcessNodeClickedWithCtrl(EditorGraphNode node,Func<Connection> ConectionGenerator)
    {
        if (selectIdx > 0)
        {
            //Debug.Log("Bigger 0 selct idx");
            //we already clicked one node
            if (selectedNodes[0] != node)
            {
                selectedNodes[selectIdx] = node;

                //create connection
                CreateConnection(ConnectionGenerator);
                ClearConnectionSelection();
            }
            else
            {

                ClearConnectionSelection();
            }
        }
        else
        {
            //first node we select
            selectedNodes[selectIdx] = node;
            selectIdx+=1;

        }
    }

    protected void OnClickRemoveConnection(Connection con)
    {
        connections.Remove(con);
    }

    private void ClearConnectionSelection()
    {
        selectIdx = 0;
        selectedNodes[0] = null;
        selectedNodes[1] = null;
    }

    protected void CreateConnection(Func<Connection> ConnectionCreator)
    {
        if (ConnectionCreator == null)
            return;
        if (connections == null)
            connections = new List<Connection>();
        connections.Add(ConnectionCreator());
        OnConnectionCreated?.Invoke(connections[connections.Count - 1]);
    }

    protected void AddConnection(Connection con)
    {
        if (con == null)
            return;
        if (connections == null)
            connections = new List<Connection>();
        connections.Add(con);
        OnConnectionCreated?.Invoke(connections[connections.Count - 1]);
    }

    private void DrawNodes()
    {
        if(nodes != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Draw();
            }
        }
    }

    Vector2 GetClosestPointRect(Rect r, Vector2 pos)
    {
        Vector2 upperLeft = new Vector2(r.xMin, r.yMin);
        Vector2 upperRight = new Vector2(r.xMax, r.yMin);
        Vector2 lowerRight = new Vector2(r.xMax, r.yMax);
        Vector2 lowerLeft = new Vector2(r.xMin, r.yMax);

        Vector2 uRuLPoint = Vector2.Lerp(upperRight, upperLeft, .5f);
        Vector2 uRlRPoint = Vector2.Lerp(upperRight, lowerRight, .5f);
        Vector2 uLlLPoint = Vector2.Lerp(upperLeft, lowerLeft, .5f);
        Vector2 lRlLPoint = Vector2.Lerp(lowerRight, lowerLeft, .5f);

        Vector2 closest = r.center;
        float minSqr = float.MaxValue;

        if ((pos - uRuLPoint).sqrMagnitude < minSqr)
        {
            minSqr = (pos - uRuLPoint).sqrMagnitude;
            closest = uRuLPoint;
        }

        if ((pos - uRlRPoint).sqrMagnitude < minSqr)
        {
            minSqr = (pos - uRlRPoint).sqrMagnitude;
            closest = uRlRPoint;
        }
        if ((pos - uLlLPoint).sqrMagnitude < minSqr)
        {
            minSqr = (pos - uLlLPoint).sqrMagnitude;
            closest = uLlLPoint;
        }
        if ((pos - lRlLPoint).sqrMagnitude < minSqr)
        {
            minSqr = (pos - lRlLPoint).sqrMagnitude;
            closest = lRlLPoint;
        }
        return closest;
    }

    public abstract void Save();

    private void OnDisable()
    {
        Save();
    }

}