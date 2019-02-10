using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LODMesh
{
    public event System.Action<MeshData> OnMeshDataReceived;
    public Mesh Mesh { get; protected set; }
    public bool HasRequestedMesh { get; protected set; }
    public bool HasMesh { get; protected set; }
    public int LoD { get; protected set; }
    public event System.Action updateCallback;

    public LODMesh(int lod)
    {
        this.LoD = lod;
        
    }

    public void RequestMesh(HeightMap heightMap,MeshSettings meshSettings)
    {
        HasRequestedMesh = true;
        ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, LoD), MeshDataReceived);
    }

    void MeshDataReceived(object meshData)
    {
        MeshData mesh = (MeshData)meshData;

        OnMeshDataReceived?.Invoke(mesh);

        Mesh = mesh.CreateMesh();
        HasMesh = true;
        updateCallback();
    }

}
