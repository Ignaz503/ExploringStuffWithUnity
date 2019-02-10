using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FallOffMapType
{
    None,
    Completely,
    UpperSide,
    LowerSide,
    RightSide,
    LeftSide,
    CornerUpperLeftSmall,
    CornerUpperRightSmall,
    CornerLowerLeftSmall,
    CornerLowerRightSmall
}

public static class FalloffGenerator
{

    public static float[,] RightSideFallOffMap(int width, int height)
    {
        float[,] falloffMap = new float[width, height];

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                float x =   1- (i / (float)width);
                //float y =  (j / (float)height);

                float val = Mathf.Abs(x);
                val = square(val);

                falloffMap[i, j] = Evaluate(val);

            }
        }
        return falloffMap;
    }

    public static float[,] RightSideFallOffMap(int size)
    {
        return RightSideFallOffMap(size, size);
    }

    public static float[,] LeftSideFalloffMap(int width, int height)
    {
        float[,] falloffMap = new float[width, height];

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                float x =  (i / (float)width);
                //float y =  (j / (float)height);

                float val = Mathf.Abs(x);
                val = square(val);

                falloffMap[i, j] = Evaluate(val);

            }
        }
        return falloffMap;
    }

    public static float[,] LeftSideFallOffMap(int size)
    {
        return LeftSideFalloffMap(size, size);
    }

    public static float[,] LowerSideFallOffMap(int width, int height)
    {
        float[,] falloffMap = new float[width, height];

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                //float x = 1 - (i / (float)width);
                float y =  (j / (float)height);

                float val = Mathf.Abs(y);
                val = square(val);

                falloffMap[i, j] = Evaluate(val);

            }
        }
        return falloffMap;
    }

    public static float[,] LowerSideFallOffMap(int size)
    {
        return LowerSideFallOffMap(size, size);
    }

    public static float[,] UpperSideFallOffMap(int width, int height)
    {
        float[,] falloffMap = new float[width, height];

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                //float x = 1 - (i / (float)width);
                float y = 1- (j / (float)height);

                float val = Mathf.Abs(y);
                val = square(val);

                falloffMap[i, j] = Evaluate(val);

            }
        }
        return falloffMap;
    }

    public static float[,] UpperSideFallOffMap(int size)
    {
        return UpperSideFallOffMap(size, size);
    }

    public static float[,] CornerUpperLeftSmallFallOffMap(int width, int height)
    {
        float[,] falloffMap = new float[width, height];

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                float x = (i / (float)width);

                float y = 1 - (j / (float)height);

                float leftVal = Mathf.Abs(x);
                leftVal = square(leftVal);
                float leftEval = Evaluate(leftVal);

                float upperVal = Mathf.Abs(y);
                upperVal = square(upperVal);
                float upperEval = Evaluate(upperVal);

                float val = Mathf.Min(upperEval, leftEval);

                falloffMap[i, j] = val;
            }
        }
        return falloffMap;
    }

    public static float[,] CornerUpperLeftSmallFallOffMap(int size)
    {
        return CornerUpperLeftSmallFallOffMap(size, size);
    }

    public static float[,] CornerLowerLeftSmallFallOff(int width, int height)
    {
        float[,] falloffMap = new float[width, height];

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                float x = (i / (float)width);

                float y = (j / (float)height);

                float leftVal = Mathf.Abs(x);
                leftVal = square(leftVal);
                float leftEval = Evaluate(leftVal);

                float lowerVal = Mathf.Abs(y);
                lowerVal = square(lowerVal);
                float lowerEval = Evaluate(lowerVal);

                float val = Mathf.Min(lowerEval, leftEval);

                falloffMap[i, j] = val;
            }
        }

        return falloffMap;
    }

    public static float[,] CornerLowerLeftSmallFallOff(int size)
    {
        return CornerLowerLeftSmallFallOff(size, size);
    }

    public static float[,] CornerUpperRightSmallFallOff(int width, int height)
    {
        float[,] falloffMap = new float[width, height];

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                float inverseX = 1 - (i / (float)width);
                float y = 1 - (j / (float)height);

                float leftVal = Mathf.Abs(inverseX);
                leftVal = square(leftVal);
                float leftEval = Evaluate(leftVal);

                float lowerVal = Mathf.Abs(y);
                lowerVal = square(lowerVal);
                float lowerEval = Evaluate(lowerVal);

                float val = Mathf.Min(lowerEval, leftEval);

                falloffMap[i, j] = val;
            }
        }

        return falloffMap;
    }

    public static float[,] CornerUpperRightSmallFallOff(int size)
    {
        return CornerUpperRightSmallFallOff(size, size);
    }

    public static float[,] CornerLowerRightSmallFallOffMap(int width, int height)
    {
        float[,] falloffMap = new float[width, height];

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                float x = 1 - (i / (float)width);

                float y = (j / (float)height);

                float leftVal = Mathf.Abs(x);
                leftVal = square(leftVal);
                float leftEval = Evaluate(leftVal);

                float lowerVal = Mathf.Abs(y);
                lowerVal = square(lowerVal);
                float lowerEval = Evaluate(lowerVal);

                float val = Mathf.Min(lowerEval, leftEval);

                falloffMap[i, j] = val;
            }
        }

        return falloffMap;
    }

    public static float[,] CornerLowerRightSmallFallOffMap(int size)
    {
        return CornerLowerRightSmallFallOffMap(size, size);
    }

    static float Evaluate(float value)
    {
         const float a = 3;
         const float b = 2.2f;

        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow((b - (b * value)), a));
    }

    static float square(float t)
    {
        return t * t;
    }

    static float inversesquare(float t)
    {
        return 1 - square(t);
    }
}


public static class FallOffMapContainer 
{
    //set in terrain generator awake method
    public static int Size;

    static float[,] left;
    public static float[,] Left
    {
        get
        {
            if (left == null)
                left = FalloffGenerator.LeftSideFallOffMap(Size);
            return left;
        }
    }

    static float[,] right;
    public static float[,] Right
    {
        get
        {
            if (right == null)
                right = FalloffGenerator.RightSideFallOffMap(Size);
            return right;
        }
    }

    static float[,] upper;
    public static float[,] Upper
    {
        get
        {
            if (upper == null)
                upper = FalloffGenerator.UpperSideFallOffMap(Size);
            return upper;
        }
    }

    static float[,] lower;
    public static float[,] Lower
    {
        get
        {
            if (lower == null)
                lower = FalloffGenerator.LowerSideFallOffMap(Size);
            return lower;
        }
    }

    static float[,] cornerUpperLeft;
    public static float[,] CornerUpperLeft
    {
        get
        {
            if (cornerUpperLeft == null)
                cornerUpperLeft = FalloffGenerator.CornerUpperLeftSmallFallOffMap(Size);
            return cornerUpperLeft;
        }
    }

    static float[,] cornerUpperRight;
    public static float[,] CornerUpperRight
    {
        get
        {
            if (cornerUpperRight == null)
                cornerUpperRight = FalloffGenerator.CornerUpperRightSmallFallOff(Size);
            return cornerUpperRight;

        }
    }

    static float[,] cornerLowerLeft;
    public static float[,] CornerLowerLeft
    {
        get
        {
            if (cornerLowerLeft == null)
                cornerLowerLeft = FalloffGenerator.CornerLowerLeftSmallFallOff(Size);
            return cornerLowerLeft;
        }
    }

    static float[,] cornerLowerRight;
    public static float[,] CornerLowerRight
    {
        get
        {
            if (cornerLowerRight == null)
                cornerLowerRight = FalloffGenerator.CornerLowerRightSmallFallOffMap(Size);
            return cornerLowerRight;
        }
    }

}