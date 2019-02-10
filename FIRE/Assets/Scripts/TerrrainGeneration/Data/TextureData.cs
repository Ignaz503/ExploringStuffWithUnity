using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu()]
public class TextureData : UpdatableData
{
    const int TextureSize = 1024;
    const TextureFormat textureFormat = TextureFormat.RGB565;

    public Layer[] layers;

    //only in editor?
    float savedMinHeight;
    float savedMaxHeight;

    public void ApplyToMaterial(Material material)
    {
        material.SetInt("layerCount", layers.Length);

        material.SetColorArray("baseColors", layers.Select(x=>x.tint).ToArray());

        material.SetFloatArray("baseStartHeights", layers.Select(x => x.startHeight).ToArray());

        material.SetFloatArray("baseBlends", layers.Select(x => x.blendStrength).ToArray());

        material.SetFloatArray("baseColorStrengths", layers.Select(x => x.tintStrength).ToArray());


        material.SetFloatArray("baseTextureScales", layers.Select(x => x.textureScale).ToArray());

        Texture2DArray texArray = GenerateTextureArray(layers.Select(x => x.tex).ToArray());
        material.SetTexture("baseTextures", texArray);

        UpdateMeshHeights(material, savedMinHeight, savedMaxHeight);
    }

    Texture2DArray GenerateTextureArray(Texture2D[] textures)
    {
        Texture2DArray arr = new Texture2DArray(TextureSize, TextureSize, textures.Length,textureFormat, true);

        for (int i = 0; i < textures.Length; i++)
        {
            arr.SetPixels(textures[i].GetPixels(), i);
        }
        arr.Apply();
        return arr;
    }

    public void UpdateMeshHeights(Material mat, float minHeight, float maxHeight)
    {
        //TODO only in editor?
        savedMaxHeight = maxHeight;
        savedMinHeight = minHeight;

        mat.SetFloat("minHeight", minHeight);
        mat.SetFloat("maxHeight", maxHeight);
    }

    [System.Serializable]
    public class Layer
    {
        public Texture2D tex;
        public Color tint;
        [Range(0f,1f)] public float tintStrength;
        [Range(0f, 1f)] public float startHeight;
        [Range(0f, 1f)] public float blendStrength;
        public float textureScale;

    }

}
