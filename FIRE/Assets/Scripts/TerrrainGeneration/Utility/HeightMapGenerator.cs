using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeightMapGenerator
{
    public static HeightMap GenerateHeigtMap(int width, int height, HeightMapSettings settings, Vector2 sampleCenter, ContinentTerrainSettings continentSettings, Vector2 continentSampleCenter)
    {
        float[,] values = Noise.GenerateNoiseMap(width, height, settings.Settings, sampleCenter);

        //make own heightcurve
        AnimationCurve heightCurve = new AnimationCurve(settings.HeightCurve.keys);

        MinMax minMaxTracker = new MinMax();

        ChunkFallOffMap fallOff = new ChunkFallOffMap();//none fall off
        if (settings.ApplyFalloff)
        {
             fallOff = ContinentGenerator.GetFallOffTypeForChunk(continentSampleCenter, continentSettings);
        }
        
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                values[i, j] *= heightCurve.Evaluate(values[i,j] - fallOff[i, j]) *settings.HeightMultiplier;
                //values[i, j] = heightCurve.Evaluate(values[i,j] - fallOff[i,j])*settings.HeightMultiplier;

                minMaxTracker.Update(values[i, j]);
            }
        }

        return new HeightMap(values, minMaxTracker);
    }

    public static HeightMap GenerateHeigtMap(int width, int height, HeightMapSettings settings, Vector2 sampleCenter)
    {
        float[,] values = Noise.GenerateNoiseMap(width, height, settings.Settings, sampleCenter);

        //make own heightcurve
        AnimationCurve heightCurve = new AnimationCurve(settings.HeightCurve.keys);

        MinMax minMaxTracker = new MinMax();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                values[i, j] = heightCurve.Evaluate(values[i,j])*settings.HeightMultiplier;

                minMaxTracker.Update(values[i, j]);
            }
        }

        return new HeightMap(values, minMaxTracker);
    }

}

public struct HeightMap
{
    public readonly float[,] values;
    public readonly MinMax minMaxValues;

    public float Min { get { return minMaxValues.Min; } }
    public float Max { get { return minMaxValues.Max; } }

    public int Width { get { return values.GetLength(0); } }
    public int Height { get { return values.GetLength(1); } }
    //public readonly Color[] colorMap;

    public HeightMap(float[,] values, float min, float max)//, Color[] colorMap)
    {
        this.values = values;
        //this.colorMap = colorMap;
        minMaxValues = new MinMax(min, max);
    }

    public HeightMap(float[,] values, MinMax minMaxValues)
    {
        this.values = values;
        this.minMaxValues = minMaxValues;
    }

    public float this[int x, int y]
    {
        get
        {
            return values[x, y];
        }
    }

    public float BidiractionalSample(Vector2 point)
    {
        int lowerX = Mathf.FloorToInt(point.x);
        int upperX = Mathf.CeilToInt(point.x);

        int lowerY = Mathf.FloorToInt(point.y);
        int upperY = Mathf.CeilToInt(point.y);

        //float xPercent = Mathf.InverseLerp(lowerX, upperX, point.x);
        //float yPercent = Mathf.InverseLerp(lowerY, upperY, point.y);
        float xPercent = point.x - lowerX;
        float yPercent = point.y - lowerY;

        float lowerXlowerYvalue = values[lowerX, lowerY];
        float upperXlowerYvalue = values[upperX, lowerY];

        float lowerXupperYvalue = values[lowerX, upperY];
        float upperXupperYvalue = values[upperX, upperY];

        float valueA = Mathf.Lerp(lowerXlowerYvalue, upperXlowerYvalue, xPercent);
        float valueB = Mathf.Lerp(lowerXupperYvalue, upperXupperYvalue, xPercent);

        return Mathf.Lerp(valueA, valueB, yPercent);
    }

}