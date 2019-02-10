using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LODInfo
{
    [Range(0,MeshSettings.NumSupportedLODs-1)]
    public int LOD;
    public float visibleDistThreshold;
    public float SqrVisibleDistThreshold { get { return visibleDistThreshold * visibleDistThreshold; } }
}
