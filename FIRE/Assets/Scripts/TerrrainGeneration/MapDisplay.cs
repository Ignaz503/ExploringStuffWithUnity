using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour {

    public enum DrawMode
    {
        Noise,
        Mesh,
        FallOff,
        ContinentMap
    }

    public enum FallOffSubSettings
    {
        None,
        Completely,
        UpperSide,
        LowerSide,
        RightSide,
        LeftSide,
        CornerUpperLeftSmall,
        CornerLowerLeftSmall,
        CornerUpperRightSmall,
        CornerLowerRightSmall
    }
    [Header("UpdateMode")]
    [SerializeField] bool autoUpdate = true;
    public bool AutoUpdate { get { return autoUpdate; } }
    [Header("Texture Display")]
    [SerializeField] DrawMode drawMode = DrawMode.Noise;
    [SerializeField] FallOffSubSettings fallOffSubSettings = FallOffSubSettings.None;
    [SerializeField] MeshRenderer textureRenderer = null;
    [Header("Mesh Display")]
    [SerializeField] MeshFilter meshFilter= null;
    [SerializeField] MeshRenderer meshRenderer = null;

    [Header("Noise Settings")]

    [SerializeField] HeightMapSettings heightSettings = null;
    public HeightMapSettings HeightSettings { get { return heightSettings; } }

    [SerializeField] Vector2 editorOffset = Vector2.zero;

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

    [SerializeField] [Range(0, MeshSettings.NumSupportedLODs - 1)] int editorPreviewLevelOfDetail = 0;
    public int EditorPreviewLOD { get { return editorPreviewLevelOfDetail; } }

    [Header("Texturing")]
    [SerializeField] TextureData textureData = null;
    public TextureData TextureData { get { return textureData; } }
    [SerializeField] Material terrainMaterial = null;

    [Header("Continent")]
    [SerializeField] ContinentTerrainSettings continentSettings = null;

    public void DrawHeightMap(HeightMap heightMap)
    {
        switch (drawMode)
        {
            case DrawMode.Noise:
                ActivateTextureRendering();
                DrawNoise(heightMap);
                break;
            case DrawMode.Mesh:
                ActivateMeshRendering();
                DrawMesh(heightMap);
                break;
            case DrawMode.FallOff:
                ActivateTextureRendering();
                DrawFallOff();
                break;
            case DrawMode.ContinentMap:
                ActivateTextureRendering();
                DrawNoise(ContinentGenerator.TempDisplayContinent(meshSettings.NumberOfVerticesPerLine, continentSettings));
                break;
            default:
                ActivateTextureRendering();
                DrawNoise(heightMap);
                break;
        }
    }

    /// <summary>
    /// Don't   Ugly
    /// Open    Inside
    /// </summary>
    void DrawFallOff()
    {
        switch (fallOffSubSettings)
        {
            case FallOffSubSettings.UpperSide:
                DrawNoise(FalloffGenerator.UpperSideFallOffMap(MeshSettings.NumberOfVerticesPerLine));
                break;
            case FallOffSubSettings.LowerSide:
                DrawNoise(FalloffGenerator.LowerSideFallOffMap(MeshSettings.NumberOfVerticesPerLine));
                break;
            case FallOffSubSettings.LeftSide:
                DrawNoise(FalloffGenerator.LeftSideFallOffMap(MeshSettings.NumberOfVerticesPerLine));
                break;
            case FallOffSubSettings.RightSide:
                DrawNoise(FalloffGenerator.RightSideFallOffMap(MeshSettings.NumberOfVerticesPerLine));
                break;
         
            case FallOffSubSettings.CornerUpperLeftSmall:
                DrawNoise(FalloffGenerator.CornerUpperLeftSmallFallOffMap(meshSettings.NumberOfVerticesPerLine));
                break;
            case FallOffSubSettings.CornerLowerLeftSmall:
                DrawNoise(FalloffGenerator.CornerLowerLeftSmallFallOff(meshSettings.NumberOfVerticesPerLine));
                break;
            case FallOffSubSettings.CornerUpperRightSmall:
                DrawNoise(FalloffGenerator.CornerUpperRightSmallFallOff(meshSettings.NumberOfVerticesPerLine));
                break;
            case FallOffSubSettings.CornerLowerRightSmall:
                DrawNoise(FalloffGenerator.CornerLowerRightSmallFallOffMap(meshSettings.NumberOfVerticesPerLine));
                break;
            
            case FallOffSubSettings.None:
                DrawNoise(new float[meshSettings.NumberOfVerticesPerLine,meshSettings.NumberOfVerticesPerLine]);
                break;
            case FallOffSubSettings.Completely:
                float[,] one = new float[meshSettings.NumberOfVerticesPerLine, meshSettings.NumberOfVerticesPerLine];
                for (int i = 0; i < meshSettings.NumberOfVerticesPerLine; i++)
                {
                    for (int j = 0; j < meshSettings.NumberOfVerticesPerLine; j++)
                    {
                        one[i, j] = 1.0f;
                    }
                }
                DrawNoise(one);
                break;
            default:
                DrawNoise(FalloffGenerator.UpperSideFallOffMap(MeshSettings.NumberOfVerticesPerLine));
                break;
        }
    }

    void ActivateTextureRendering()
    {
        meshRenderer.gameObject.SetActive(false);
        textureRenderer.gameObject.SetActive(true);
    }

    void ActivateMeshRendering()
    {
        meshRenderer.gameObject.SetActive(true);
        textureRenderer.gameObject.SetActive(false);
    }

    void DrawNoise(float[,] noise)
    {
        int width = noise.GetLength(0);
        int height = noise.GetLength(1);

        Color[] colorMap = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, noise[x, y]);
            }
        }
        CreateAndApplyTextue(width, height, colorMap);
    }

    void DrawNoise(HeightMap heightMap)
    {
        int width = heightMap.values.GetLength(0);
        int height = heightMap.values.GetLength(1);

        Color[] colorMap = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, Mathf.InverseLerp(heightMap.Min,heightMap.Max,heightMap.values[x, y]));
            }
        }
        CreateAndApplyTextue(width, height, colorMap);
    }

    void CreateAndApplyTextue( int width, int height, Color[] c)
    {
        textureRenderer.sharedMaterial.mainTexture = TextureGenerator.CreateTexture(c,width,height);
        textureRenderer.transform.localScale = new Vector3(width, 1, height);
    }

    void DrawMesh(HeightMap heightMap)
    {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(heightMap.values, MeshSettings, EditorPreviewLOD);

        meshFilter.sharedMesh = meshData.CreateMesh();
    }

    public void DrawInEditor()
    {
        //update somewhere else
        textureData.UpdateMeshHeights(terrainMaterial, heightSettings.MinHeight, heightSettings.MaxHeight);
        textureData.ApplyToMaterial(terrainMaterial);
        HeightMap mData = HeightMapGenerator.GenerateHeigtMap(meshSettings.NumberOfVerticesPerLine, meshSettings.NumberOfVerticesPerLine, heightSettings, editorOffset, continentSettings,Vector2.zero);
        DrawHeightMap(mData);
    }

    private void OnValidate()
    {
        if (heightSettings == null || meshSettings == null || textureData == null || continentSettings == null)
            return;

        if (meshSettings != null)
        {
            meshSettings.OnValuesUpdated -= OnValuesUpdated;
            meshSettings.OnValuesUpdated += OnValuesUpdated;
        }
        if (heightSettings != null)
        {
            heightSettings.OnValuesUpdated -= OnValuesUpdated;
            heightSettings.OnValuesUpdated += OnValuesUpdated;
        }
        if (textureData != null)
        {
            TextureData.OnValuesUpdated -= OnTextureValuesUpdated;
            TextureData.OnValuesUpdated += OnTextureValuesUpdated;
        }
        if(continentSettings != null)
        {
            continentSettings.OnValuesUpdated -= OnValuesUpdated;
            continentSettings.OnValuesUpdated += OnValuesUpdated;
        }
    }

    void OnValuesUpdated()
    {
        FallOffMapContainer.Size = meshSettings.NumberOfVerticesPerLine;
        if (!Application.isPlaying)
        {
            DrawInEditor();
        }
    }

    void OnTextureValuesUpdated()
    {
        textureData.ApplyToMaterial(terrainMaterial);
    }

}
