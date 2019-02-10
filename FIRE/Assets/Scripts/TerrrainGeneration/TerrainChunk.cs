using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChunk
{
    public const float ColliderGenerationDistThreshold = 5f;
    public const float SqrColliderGenerationDistThreshold = ColliderGenerationDistThreshold * ColliderGenerationDistThreshold;

    public event Action<TerrainChunk,bool> OnVisiblityChanged;

    public Vector2 Coord { get; protected set; }
    Vector2 sampleCenter;
    GameObject meshObject;
    Bounds bounds;

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    LODInfo[] detailLevels;
    float maxViewDistance;
    LODMesh[] lodMeshes;
    int colliderLODIndex;
    bool hasColliderMesh;

    int previsouLODIndex = -1;

    HeightMap heightData;
    bool heightMapReceived;

    HeightMapSettings heightSettings;
    MeshSettings meshSettings;
    TextureData textureData;
    ContinentTerrainSettings continentSettings;

    Transform viewer;
    Vector2 viewerPosition
    {
        get
        {
            return new Vector2(viewer.position.x, viewer.position.z);
        }
    }

    Vector2 viewerCoord
    {
        get
        {
            return new Vector2(Mathf.RoundToInt(viewerPosition.x / meshSettings.MeshWorldSize), Mathf.RoundToInt(viewerPosition.y / meshSettings.MeshWorldSize));
        }
    }

    public bool ViewerInChunk
    {
        get
        {
            return bounds.Contains(viewerPosition);
        }
    }

    TerrainChunkObjectScatter objectScatter;
    PointsAsGizmo pointsDisplay;

    public bool IsVisible { get { return meshObject.activeSelf; } }

    public TerrainChunk(Vector2 coord,HeightMapSettings heightSettings ,MeshSettings meshSettings, TextureData textureData, ContinentTerrainSettings continentSettings, LODInfo[] detailLevels,int colliderLOD, Transform parent,Transform viewer, Material mat)
    {
        this.detailLevels = detailLevels;
        maxViewDistance = detailLevels[detailLevels.Length - 1].SqrVisibleDistThreshold;

        colliderLODIndex = colliderLOD;

        this.heightSettings = heightSettings;
        this.meshSettings = meshSettings;
        this.textureData = textureData;
        this.continentSettings = continentSettings;
        this.viewer = viewer;

        this.Coord = coord;
        sampleCenter = coord * meshSettings.MeshWorldSize / meshSettings.MeshScale;
        Vector2 position = coord * meshSettings.MeshWorldSize;

        bounds = new Bounds(position, Vector2.one * meshSettings.MeshWorldSize);

        meshObject = new GameObject();
        meshObject.name = $"Terrrain Chunk:{ coord }";
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshRenderer.material = mat;

        meshFilter = meshObject.AddComponent<MeshFilter>();

        meshCollider = meshObject.AddComponent<MeshCollider>();

        meshObject.transform.position = new Vector3(position.x,0, position.y);

        meshObject.transform.SetParent(parent);
        SetVisible(false);

        //TEMP
        pointsDisplay = meshObject.AddComponent<PointsAsGizmo>();
        pointsDisplay.Chunk = this;
        pointsDisplay.SetViewer(viewer);
        //END TEMP

        lodMeshes = new LODMesh[detailLevels.Length];

        for (int i = 0; i < lodMeshes.Length; i++)
        {
            lodMeshes[i] = new LODMesh(detailLevels[i].LOD);
            lodMeshes[i].updateCallback += UpdateTerrainChunk;
            if(i == colliderLODIndex)
                lodMeshes[i].updateCallback += UpdateCollisionMesh;
        }
    }

    public void Load()
    {
        ThreadedDataRequester.RequestData(() => HeightMapGenerator.GenerateHeigtMap(meshSettings.NumberOfVerticesPerLine, meshSettings.NumberOfVerticesPerLine, heightSettings, sampleCenter, continentSettings, Coord), OnHeihgtMapReceived);
    }

    void OnHeihgtMapReceived(object heightMapData)
    {
        heightData = (HeightMap)heightMapData;
        heightMapReceived = true;

        //Texture2D tex = TextureGenerator.CreateTexture(mapData.colorMap, mapGen.Settings.MapChunkSize, mapGen.Settings.MapChunkSize);
        //meshRenderer.material.mainTexture = tex;
        //float minheight = meshSettings.MeshScale * heightSettings.HeightMultiplier * heightSettings.HeightCurve.Evaluate(textureData.layers[1].startHeight + textureData.layers[1].blendStrength);
        float minHeight = meshSettings.MeshScale * heightSettings.HeightMultiplier * heightSettings.HeightCurve.Evaluate(0.235f);

        float maxHeight = meshSettings.MeshScale * heightSettings.HeightMultiplier * heightSettings.HeightCurve.Evaluate(.39f);

        objectScatter = new TerrainChunkObjectScatter(this,heightData,minHeight,maxHeight ,bounds, pointsDisplay);

        UpdateTerrainChunk();
        UpdateCollisionMesh();
    }

    public void UpdateTerrainChunk()
    {
        if (!heightMapReceived)//don't do anything
            return;

        float viewerDistanceFromEdge = bounds.SqrDistance(viewerPosition);

        bool wasVisible = IsVisible;
        bool visible = viewerDistanceFromEdge <= maxViewDistance;

        if (visible)
        {
            int lodIdx = 0;
            for (int i = 0; i < detailLevels.Length - 1; i++)
            {
                if (viewerDistanceFromEdge > detailLevels[i].SqrVisibleDistThreshold)
                {
                    lodIdx = i + 1;
                }
                else
                {
                    break;
                }
            }//end for detail levels

            if(Coord == viewerCoord)
            {
                pointsDisplay.SetVisible(true);
            }
            else
            {
                pointsDisplay.SetVisible(false);
            }
            //end temp

            if (lodIdx != previsouLODIndex)
            {
                LODMesh lodMesh = lodMeshes[lodIdx];

                if (lodMesh.HasMesh)
                {
                    previsouLODIndex = lodIdx;
                    meshFilter.mesh = lodMesh.Mesh;
                    
                }
                else if (!lodMesh.HasRequestedMesh)
                {
                    lodMesh.RequestMesh(heightData,meshSettings);
                }

            }// end if not equal previous
        }
        if(wasVisible != visible)
        {
            SetVisible(visible);
            OnVisiblityChanged?.Invoke(this, visible);
        }
    }

    public void UpdateCollisionMesh()
    {
        if (hasColliderMesh)
            return;//nothing todo

        float sqrDistViewerToEdge = bounds.SqrDistance(viewerPosition);

        if(sqrDistViewerToEdge < detailLevels[colliderLODIndex].SqrVisibleDistThreshold)
        {
            if(!lodMeshes[colliderLODIndex].HasRequestedMesh)
            {
                lodMeshes[colliderLODIndex].RequestMesh(heightData,meshSettings);
            }
        }

        if(sqrDistViewerToEdge < SqrColliderGenerationDistThreshold)
        {
            if (lodMeshes[colliderLODIndex].HasMesh)
            {
                meshCollider.sharedMesh = lodMeshes[colliderLODIndex].Mesh;
                hasColliderMesh = true;
            }//
        }
    }

    public void SetVisible(bool value)
    {
        meshObject.SetActive(value);
    }
}