using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorGraphNode
{
    public Rect rect;
    public string title;
    public GUIStyle style;
    public GUIStyle defaultStyle, selectedStyle;
    public bool isDragged;
    public bool isSelected;

    public NodeBasedEditor editorBelongingTo;

    public Action<EditorGraphNode> OnRemoveNode;
    public Action<EditorGraphNode> OnClickWithCtrl;

    public EditorGraphNode(Vector2 position, Action<EditorGraphNode> OnClickWithCtrl, Action<EditorGraphNode> OnClickRemoveNode,NodeBasedEditor editorBelongingTo)
    {
        this.editorBelongingTo = editorBelongingTo;

        rect = new Rect(position.x, position.y, editorBelongingTo.Settings.NodeWidth, editorBelongingTo.Settings.NodeHeight);

        style = editorBelongingTo.Settings.NodeStyle;
        defaultStyle = editorBelongingTo.Settings.NodeStyle;
        selectedStyle = editorBelongingTo.Settings.SelectedNodeStyle;

        this.OnClickWithCtrl = OnClickWithCtrl;

        OnRemoveNode = OnClickRemoveNode;

        editorBelongingTo.OnConnectionCreated += OnConnectionCreated;
    }

    void OnConnectionCreated(Connection createdConnection)
    {
        if ((createdConnection.InPoint == rect || createdConnection.OutPoint == rect) && isSelected)
        {
            isSelected = false;
            style = defaultStyle;
        }
    }

    public void Resize()
    {
        Rect newRect = new Rect(rect.position.x,rect.position.y,editorBelongingTo.Settings.NodeWidth, editorBelongingTo.Settings.NodeHeight);
        rect = newRect;
    }

    public virtual void Drag(Vector2 delta)
    {
        rect.position += delta;
    }

    public virtual void Draw()
    {
        GUI.Box(rect, title, style);
    }

    public virtual bool ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if (rect.Contains(e.mousePosition))
                    {
                        isDragged = true;
                        GUI.changed = true;
                        isSelected = true;
                        style = selectedStyle;
                        if (e.control)
                        {
                            e.Use();
                            OnClickWithCtrl?.Invoke(this);
                        }
                    }
                    else
                    {
                        GUI.changed = true;
                        isSelected = false;
                        style = defaultStyle;
                    }
                }
                //right click
                if (e.button == 1 && isSelected && rect.Contains(e.mousePosition))
                {
                    ProcessContextMenu();
                    e.Use();
                }
                break;
            case EventType.MouseUp:
                isDragged = false;
                break;
            case EventType.MouseDrag:
                if (e.button == 0 && isDragged)
                {
                    Drag(e.delta);
                    e.Use();
                    return true;
                }
                break;
        }
        return false;
    }

    protected virtual void ProcessContextMenu()
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
        genericMenu.ShowAsContext();
    }

    private void OnClickRemoveNode()
    {
        editorBelongingTo.OnConnectionCreated -= OnConnectionCreated;
        OnRemoveNode?.Invoke(this);
    }
}

