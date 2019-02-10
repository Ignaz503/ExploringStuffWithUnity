using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Node Based Editor Settings")]
public class NodeBasedEditorSetttings : UpdatableData
{

    [Header("Node")]
    public float NodeWidth = 200f;
    public float NodeHeight = 50f;

    [Header("Node Styles")]
    public GUIStyle NodeStyle;
    public GUIStyle SelectedNodeStyle;

    [Header("Connection Point")]
    public float ConnectionPointWidth = 10f;
    public float ConnectionPointHeight = 20f;
    public float ConnectionPointXOffset = 4f;

    [Header("Connection Point Style")]
    public GUIStyle InPointStyle;
    public GUIStyle OutPointStyle;

    [Header("Connection Settings")]
    public Color ConnectionColor = Color.white;
    public float ConnectionBezierTangentMultiplier = 50f;
    public float ConnectionBezierWidth = 2f;
    [Tooltip("Can be left null")]
    public Texture2D ConnectionBezierTexture = null;
    public float ConnectionButtonSize = 4f;

    [Header("BackGround")]
    public Color WindowBackgroundColor = Color.gray;

    public float GridMinorLineSpacing = 20f;
    public float GridMinorLineOpacity = .2f;
    public Color GridMinorLineColor = Color.gray;

    public float GridMajorLineSpacing = 100f;
    public float GridMajorLineOpacity = .4f;
    public Color GridMajorLineColor = Color.gray;

}
