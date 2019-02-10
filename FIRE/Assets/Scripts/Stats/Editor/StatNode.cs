using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StatNode : EditorGraphNode
{
    public string Name { get; protected set; }
    public int PropertiesInfoArrayIndex { get; protected set; }
    Rect fontRect;
    public StatNode(Vector2 position, Action<EditorGraphNode> OnClickWithCtrl, Action<EditorGraphNode> OnClickRemoveNode, NodeBasedEditor editorBelongingTo, string name, int idx) : base(position, OnClickWithCtrl, OnClickRemoveNode, editorBelongingTo)
    {
        Name = name;
        PropertiesInfoArrayIndex = idx;
        fontRect = rect;
        fontRect.center = rect.center;
    }

    public override void Drag(Vector2 delta)
    {
        base.Drag(delta);
        fontRect.position += delta;
    }

    public override void Draw()
    {
        base.Draw();

        GUI.Label(fontRect, Name, new GUIStyle() { alignment = TextAnchor.MiddleCenter });
    }

    protected override void ProcessContextMenu()
    { }
}