using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Terrain Settings")]
public class MeshSettings : UpdatableData
{
    public const int NumSupportedLODs = 5;
    public const int numSupportedChunkSizes = 9;
    public const int numSupportedFlatShadedChunkSizes = 3;

    public static readonly int[] SupportedMeshSizes = new int[]
    {
        48,72,96,120,144,168,192,216,240
    };

    public bool UseFlatShading;
    public float MeshScale = 1f;
     [Range(0, numSupportedChunkSizes - 1)] public int ChunkSizeIndex;
     [Range(0, numSupportedFlatShadedChunkSizes - 1)] public int FlatShadedChunkSizeIndex;

    //num vert per line mesh at highest resolution LOD = 0.
    //incluedes extra vertices that are excluded in final mesh only used for normals
    public int NumberOfVerticesPerLine
    {
        get
        {
            return SupportedMeshSizes[(UseFlatShading) ? FlatShadedChunkSizeIndex : ChunkSizeIndex] + 5;
        }
    }

    public float MeshWorldSize
    {
        get
        {
            return (NumberOfVerticesPerLine - 3) * MeshScale;
        }
    }

}
