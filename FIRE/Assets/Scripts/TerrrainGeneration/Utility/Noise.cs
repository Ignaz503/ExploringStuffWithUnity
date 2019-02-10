using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public enum NormalizeMode
    {
        Local,
        Global
    };

    public static float[,] GenerateNoiseMap(int width, int height, NoiseSettings settings, Vector2 sampleCenter)
    {
        System.Random prng = new System.Random(settings.Seed);
        Vector2[] octaveOffsets = new Vector2[settings.Octaves];

        float maxPossibleNoiseValue = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < settings.Octaves; i++)
        {
            float offsetX = prng.Next(-100000, 10000) + (settings.Offset.x + sampleCenter.x);
            float offsetY = prng.Next(-100000, 10000) - (settings.Offset.y + sampleCenter.y);
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleNoiseValue += amplitude;

            amplitude *= settings.Persistence;
        }

        float halfWidth = width / 2f;
        float halfHeight = height / 2f;

        float[,] noiseMap = new float[width, height];

        MinMax heightMinMaxLocal = new MinMax();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for (int octave = 0; octave < settings.Octaves; octave++)
                {
                    float sampleX = (x - halfWidth + octaveOffsets[octave].x) / settings.Scale * frequency;
                    float sampleY = (y-halfHeight + octaveOffsets[octave].y) / settings.Scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY)*2f -1f;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= settings.Persistence;
                    frequency *= settings.Lacrunarity;
                }
                noiseMap[x, y] = noiseHeight;

                if(settings.NormalizeMode == NormalizeMode.Global)
                {
                    float normalizedValue = (noiseMap[x, y] + 1)/(2f * maxPossibleNoiseValue /  settings.GlobalEstimationCorrection);
                    noiseMap[x, y] = Mathf.Clamp(normalizedValue,0,float.MaxValue);
                }

                heightMinMaxLocal.Update(noiseHeight);
            }
        }
        //normalize
        if (settings.NormalizeMode == NormalizeMode.Local)
        {
            for (int y = 0; y < width; y++)
            {
                for (int x = 0; x < height; x++)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(heightMinMaxLocal.Min, heightMinMaxLocal.Max, noiseMap[x, y]);
                }
            }
        }// end if local normalization
        return noiseMap;
    }

    public static float GenerateSingleNoiseValue(NoiseSettings settings, float x, float y, Vector2 sampleCenter)
    {
        System.Random prng = new System.Random(settings.Seed);
        Vector2[] octaveOffsets = new Vector2[settings.Octaves];

        float maxPossibleNoiseValue = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < settings.Octaves; i++)
        {
            float offsetX = prng.Next(-100000, 10000) + (settings.Offset.x + sampleCenter.x);
            float offsetY = prng.Next(-100000, 10000) - (settings.Offset.y +  sampleCenter.y);
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleNoiseValue += amplitude;

            amplitude *= settings.Persistence;
        }

        amplitude = 1;
        frequency = 1;
        float noiseHeight = 0;

        for (int octave = 0; octave < settings.Octaves; octave++)
        {
            float sampleX = (x  + octaveOffsets[octave].x) / settings.Scale * frequency;
            float sampleY = (y  + octaveOffsets[octave].y) / settings.Scale * frequency;

            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2f - 1f;
            noiseHeight += perlinValue * amplitude;

            amplitude *= settings.Persistence;
            frequency *= settings.Lacrunarity;
        }

        float normalizedValue = (noiseHeight + 1) / (2f * maxPossibleNoiseValue / settings.GlobalEstimationCorrection);

        return Mathf.Clamp(normalizedValue, 0, float.MaxValue); ;
    }


    [Serializable]
    public class NoiseSettings
    {
        [Header("Random Controll")]
        public int Seed;
        [Header("Noise Controll")]
        public NormalizeMode NormalizeMode;
        public float Scale;
        public int Octaves;
        [Range(0f,1f)]public float Persistence;
        public float Lacrunarity;
        public Vector2 Offset;
        public float GlobalEstimationCorrection;

        public void ValidateValues()
        {
            Scale = Mathf.Max(Scale, 0.01f);
            Octaves = Mathf.Max(Octaves, 1);
            Lacrunarity = Mathf.Max(Lacrunarity, 1f);
            Persistence = Mathf.Clamp01(Persistence);
        }
    }

}
