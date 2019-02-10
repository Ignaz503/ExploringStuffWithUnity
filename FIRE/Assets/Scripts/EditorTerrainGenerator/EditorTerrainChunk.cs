using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorTerrainChunk : MonoBehaviour
{
    [SerializeField] MeshSettings meshSettings = null;
    [SerializeField] HeightMapSettings heightMapSettings = null;

    [Range(0, MeshSettings.NumSupportedLODs - 1)]
    public int LOD;

    Mesh chunk;
    [SerializeField]MeshFilter mFilter = null;
    [SerializeField]MeshRenderer meshRenderer = null;

    Vector2 Coord
    {
        get
        {
            return new Vector2(transform.position.x, transform.position.z);
        }
    }
    Vector2 coord;
    Vector2 sampleCenter
    {
        get
        {
            return coord * meshSettings.MeshWorldSize / meshSettings.MeshScale;
        }
    }

    Vector3[] vertices;

    public void GenerateMesh()
    {
        coord = Coord;

        if (gameObject.GetComponent<MeshFilter>() == null)
        {
            mFilter = gameObject.AddComponent<MeshFilter>();
        }
        if (gameObject.GetComponent<MeshRenderer>() == null)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat");
        }
            ThreadedDataRequester.RequestData(() => HeightMapGenerator.GenerateHeigtMap(meshSettings.NumberOfVerticesPerLine, meshSettings.NumberOfVerticesPerLine, heightMapSettings, sampleCenter), OnHeihgtMapReceived);
    }

    private void OnHeihgtMapReceived(object obj)
    {
        HeightMap heightData = (HeightMap)obj;
        RequestMesh(heightData, meshSettings);
    }

    public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings)
    {
        ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, LOD), MeshDataReceived);
    }

    void MeshDataReceived(object meshData)
    {
        MeshData mesh = (MeshData)meshData;

        vertices = mesh.Vertices;
        chunk = mesh.CreateMesh();
        mFilter.sharedMesh = chunk;
    }

    Vector3[] scaler = new Vector3[4]
    {
        new Vector3(-.5f,1,-.5f),
        new Vector3(.5f,1,-.5f),
        new Vector3(-.5f,1,.5f),
        new Vector3(.5f,1,.5f)
    };


    public void Split()
    {
        int vNum = (int)Mathf.Sqrt(chunk.vertices.Length);
        Mesh[] meshes = ChunkMeshSplitter.SplitMesh(chunk, vNum, vNum);
        Vector3 samCent = sampleCenter;
        for (int i = 0; i < meshes.Length; i++)
        {
            GameObject newObj = new GameObject();

            Vector3 newPos = samCent;
            newPos.Scale(scaler[i]);
            newObj.transform.position = newPos;

            MeshFilter mf = newObj.AddComponent<MeshFilter>();
            mf.sharedMesh = meshes[i];

            MeshRenderer re = newObj.AddComponent<MeshRenderer>();
            re.sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat");
                                 
        }
        gameObject.SetActive(false);
    }

}
