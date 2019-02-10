using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public static class ContinentGenerator 
{
    public static float[,] TempDisplayContinent(int size, ContinentTerrainSettings settings)
    {
        float[,] displaymap = new float[size, size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float val = Noise.GenerateSingleNoiseValue(settings.NoiseSettings, x, y, Vector2.zero);
                val = val >= settings.LandOceanThreshold ? 1 : 0;
                displaymap[x, y] = val;
            }
        }
        return displaymap;
    }

    public static ChunkFallOffMap GetFallOffTypeForChunk(Vector2 coords,ContinentTerrainSettings settings)
    {
        return new ChunkFallOffMap(GetContinentTerrainSurroundingChunk(coords, settings));
    }

    //returns souroundings 0 means land -1 means sea
    static int[,] GetContinentTerrainSurroundingChunk(Vector2 coords, ContinentTerrainSettings settings)
    {
        int[,] sourroundings = new int[3, 3];

        for (int i = -1; i < 1; i++)
        {
            for (int j = -1; j < 1; j++)
            {
                sourroundings[i + 1, j + 1] = GetValueForChunk(coords - new Vector2(i, j), settings); 
            }
        }

        return sourroundings;
    }

    //returns 0 for sea tile and 1 for land
    static int GetValueForChunk(Vector2 coords, ContinentTerrainSettings settings)
    {
        float val = Noise.GenerateSingleNoiseValue(settings.NoiseSettings, coords.x, coords.y, Vector2.zero);
        return val >= settings.LandOceanThreshold ? 1 : 0;
    }

}


public class ChunkFallOffMap
{
    public enum FallOffType
    {
        None,
        Completely,
        Other
    }

    FallOffType type;
    float[,] chunkFallOffMap;

    /// <summary>
    /// generate one with no fall off
    /// </summary>
    public ChunkFallOffMap()
    {
        type = FallOffType.None;
    }

    public ChunkFallOffMap(int[,] souroundingContinentMap)
    {
        GetCorrectFallOffMaps(souroundingContinentMap);
    }

    private void GetCorrectFallOffMaps(int[,] souroundingContinentMap)
    {
        if (souroundingContinentMap[1, 1] == 0)//check if ocean just all sides or completly
        {
            //System.Random rng = new System.Random((int)DateTime.Now.Ticks);
            ////TODO Random chance completly or all sides falloff map added
            ////don't use unity engine random cause different threads are here
            type = FallOffType.Completely;
            //if(rng.NextDouble() > .5)
            //{
            //    type = FallOffType.Completely;
            //}
            //else
            //{
            //    type = FallOffType.Other;
            //    float[,] left = FallOffMapContainer.Left;
            //    float[,] right = FallOffMapContainer.Right;
            //    float[,] up = FallOffMapContainer.Upper;
            //    float[,] low = FallOffMapContainer.Lower;
            //    chunkFallOffMap = new float[left.GetLength(0), left.GetLength(1)];
            //    for (int x = 0; x < chunkFallOffMap.GetLength(0); x++)
            //    {
            //        for (int y = 0; y < chunkFallOffMap.GetLength(1); y++)
            //        {
            //            float max = left[x, y];
            //            if (right[x, y] > max)
            //                max = right[x, y];
            //            if (up[x, y] > max)
            //                max = up[x, y];
            //            if (low[x, y] > max)
            //                max = low[x, y];
            //            chunkFallOffMap[x, y] = max;
            //        }
            //    }
            //}// end else rng > .5
        }// end if ocean in middle tile

        List<float[,]> relevantFallOffMaps = new List<float[,]>();
        for (int x = 0; x < souroundingContinentMap.GetLength(0); x++)
        {
            for (int y = 0; y < souroundingContinentMap.GetLength(1); y++)
            {
                if (x == 1 && y == 1)
                    continue;//middle of map actual chunk, not intersted here handled with upper if
                int continentValue = souroundingContinentMap[x, y];
                if (x == 1 && continentValue == 0)
                {
                    if (y == 0)
                        relevantFallOffMaps.Add(FallOffMapContainer.Upper);
                    else
                        relevantFallOffMaps.Add(FallOffMapContainer.Lower);
                }
                if (y == 1 && continentValue == 0)
                {
                    if (x == 0)
                        relevantFallOffMaps.Add(FallOffMapContainer.Left);
                    else
                        relevantFallOffMaps.Add(FallOffMapContainer.Right);
                }
                if (x == 0 && y == 0 && continentValue == 0)
                {
                    //upper left corner is ocean
                    if (souroundingContinentMap[x + 1, y] != 0 && souroundingContinentMap[x, y + 1] != 0)
                    {
                        //add corner upper left
                        relevantFallOffMaps.Add(FallOffMapContainer.CornerUpperLeft);
                    }
                }
                if (x == 2 && y == 0 && continentValue == 0)
                {
                    //upper right corner
                    if (souroundingContinentMap[x - 1, y] != 0 && souroundingContinentMap[x, y + 1] != 0)
                    {
                        //add corner upper right
                        relevantFallOffMaps.Add(FallOffMapContainer.CornerUpperRight);
                    }
                }
                if (x == 0 && y == 2 && continentValue == 0)
                {
                    //corner lower left
                    if (souroundingContinentMap[x + 1, y] != 0 && souroundingContinentMap[x, y - 1] != 0)
                    {
                        relevantFallOffMaps.Add(FallOffMapContainer.CornerLowerLeft);

                    }
                }
                if (x == 2 && y == 2 && continentValue == 0)
                {
                    //corner lower right
                    if (souroundingContinentMap[x -1 , y] != 0 && souroundingContinentMap[x, y - 1] != 0)
                    {
                        relevantFallOffMaps.Add(FallOffMapContainer.CornerLowerRight);

                    }
                }
            }// end for y
        }// end for x

        if (relevantFallOffMaps.Count == 0)
        {
            //chunkFallOffMap = FallOffMapContainer.None;
            type = FallOffType.None;
        }
        else
        {
            type = FallOffType.Other;
            chunkFallOffMap = new float[relevantFallOffMaps[0].GetLength(0), relevantFallOffMaps[0].GetLength(1)];

            for (int x = 0; x < chunkFallOffMap.GetLength(0); x++)
            {
                for (int y = 0; y < chunkFallOffMap.GetLength(1); y++)
                {
                    float max = float.MinValue;

                    for (int i = 0; i < relevantFallOffMaps.Count; i++)
                    {
                        //get max value from relevant falloff map
                        if (relevantFallOffMaps[i][x, y] > max)
                            max = relevantFallOffMaps[i][x, y];
                    }
                    chunkFallOffMap[x,y] = max;
                }
            }
        }
    }

    public float this[int x, int y]
    {
        get
        {
            return GetFallOffValue(x, y);
        }
    }

    public float GetFallOffValue(int x, int y)
    {
        if (type == FallOffType.Completely)
            return 1f;
        else if (type == FallOffType.None)
            return 0;
        else
            return chunkFallOffMap[x, y];
    }
}

