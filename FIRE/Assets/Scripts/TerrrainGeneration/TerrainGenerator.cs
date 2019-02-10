using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public static TerrainGenerator Instance { get; protected set; }

    [Header("Random Prefabs and Materials")]
    [SerializeField] GameObject[] trees = null;
    public static GameObject RandomTree { get { return Instance.trees[Random.Range(0,Instance.trees.Length)]; } }

    [SerializeField] Material[] treeMaterials = null;
    public static Material RandomTreeMaterial { get { return Instance.treeMaterials[Random.Range(0, Instance.treeMaterials.Length)]; } }

    const float viewerMoveThresholdForChunkUpdate = 10f;
    const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate* viewerMoveThresholdForChunkUpdate;

    [Header("View Distance Related")]
    [SerializeField] Transform viewer = null;
    Vector2 viewerPosition;
    Vector2 viewrPositionOld;

    int chunksVisibleInViewDistance;

    [Header("LOD")]
    [SerializeField] int colliderLOD = 0;
    public int ColliderLOD { get { return colliderLOD; } }
    [SerializeField] LODInfo[] detailLevels = null;

    #region settings
    [Header("Noise Settings")]

    [SerializeField] HeightMapSettings heightSettings = null;
    public HeightMapSettings HeightSettings { get { return heightSettings; } }

    float[,] falloffMap;
    public bool ApplyFalloff { get { return heightSettings.ApplyFalloff; } }

    public float MeshHeightMultiplier { get { return heightSettings.HeightMultiplier; } }

    public AnimationCurve MeshHeightCurve { get { return heightSettings.HeightCurve; } }

    public Noise.NoiseSettings NoiseSettings { get { return heightSettings.Settings; } }

    [Header("Mesh Generation Values")]
    [SerializeField] MeshSettings meshSettings = null;
    public MeshSettings MeshSettings { get { return meshSettings; } }

    public float MeshScale { get { return meshSettings.MeshScale; } }

    public bool UseFlatShading { get { return meshSettings.UseFlatShading; } }

    [Header("Texturing")]
    [SerializeField] TextureData textureSettings = null;
    public TextureData TextureData { get { return textureSettings; } }
    [SerializeField] Material terrainMaterial = null;

    [Header("Continent")]
    [SerializeField] ContinentTerrainSettings continentSettings = null;
    #endregion

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    public  List<TerrainChunk> visibleChunks = new List<TerrainChunk>();

    private void Awake()
    {
        Instance = FindObjectOfType<TerrainGenerator>();
        FallOffMapContainer.Size = meshSettings.NumberOfVerticesPerLine;
    }

    private void Start()
    {
        textureSettings.UpdateMeshHeights(terrainMaterial, heightSettings.MinHeight, heightSettings.MaxHeight);
        textureSettings.ApplyToMaterial(terrainMaterial);

        chunksVisibleInViewDistance = Mathf.RoundToInt(detailLevels[detailLevels.Length - 1].visibleDistThreshold / MeshSettings.MeshWorldSize);
        UpdateVisibleChunks();
    }

    private void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);

        if(viewerPosition != viewrPositionOld)
        {
            foreach(TerrainChunk chunk in visibleChunks)
            {
                chunk.UpdateCollisionMesh();
            }
        }

        if((viewrPositionOld-viewerPosition).sqrMagnitude >= sqrViewerMoveThresholdForChunkUpdate)
        {
            UpdateVisibleChunks();
            viewrPositionOld = viewerPosition;
        }
    }

    void UpdateVisibleChunks()
    {
        HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2>();
        for (int i = visibleChunks.Count-1; i >= 0; i--)
        {
            alreadyUpdatedChunkCoords.Add(visibleChunks[i].Coord);
            visibleChunks[i].UpdateTerrainChunk();
        }

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / MeshSettings.MeshWorldSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / MeshSettings.MeshWorldSize);

        for (int yOffset = -chunksVisibleInViewDistance; yOffset <= chunksVisibleInViewDistance; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDistance; xOffset <= chunksVisibleInViewDistance; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX+xOffset, currentChunkCoordY+yOffset);

                if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord))
                {
                    if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                    {
                        terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                    }//end if dick contains chunk
                    else
                    {
                        TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord,
                                    HeightSettings,
                                    MeshSettings,
                                    TextureData,
                                    continentSettings,
                                    detailLevels,
                                    colliderLOD,
                                    transform,
                                    viewer,
                                    terrainMaterial);

                        newChunk.OnVisiblityChanged += OnChunkVisibilityChanged;

                        terrainChunkDictionary.Add(viewedChunkCoord, newChunk);
                        newChunk.Load();
                    }
                }
            }
        }
    }

    void OnChunkVisibilityChanged(TerrainChunk chunk, bool visibility)
    {
        if (visibility)
            visibleChunks.Add(chunk);
        else
            visibleChunks.Remove(chunk);
    }
}
