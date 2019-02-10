using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ConnectionPoint
{
    public enum ConnectionPointType { In, Out };

    public Rect rect;

    public ConnectionPointType type;

    public EditorGraphNode node;

    public GUIStyle style;

    public Action<ConnectionPoint> OnClickConnectionPoint;

    public ConnectionPoint(ConnectionPointType type, EditorGraphNode node, GUIStyle style, Action<ConnectionPoint> onClickConnectionPoint)
    {
        this.type = type;
        this.node = node;
        this.style = style;
        OnClickConnectionPoint = onClickConnectionPoint;
        rect = new Rect(0, 0, node.editorBelongingTo.Settings.ConnectionPointWidth, node.editorBelongingTo.Settings.ConnectionPointHeight);
    }

    public void Resize()
    {
        rect = new Rect(0, 0, node.editorBelongingTo.Settings.ConnectionPointWidth, node.editorBelongingTo.Settings.ConnectionPointHeight);
    }

    public void Draw()
    {
        rect.y = node.rect.y + (node.rect.height * .5f) - rect.height * .5f;

        switch (type)
        {
            case ConnectionPointType.In:
                rect.x = node.rect.x - rect.width + node.editorBelongingTo.Settings.ConnectionPointXOffset;
                break;
            case ConnectionPointType.Out:
                rect.x = node.rect.x + node.rect.width - node.editorBelongingTo.Settings.ConnectionPointXOffset;
                break;
        }

        if (GUI.Button(rect, "", style))
        {
            OnClickConnectionPoint?.Invoke(this);
        }

    }

}

