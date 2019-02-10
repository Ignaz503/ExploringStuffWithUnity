using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinMax
{
    public float Min { get; protected set; }
    public float Max { get; protected set; }

    public MinMax()
    {
        Min = float.MaxValue;
        Max = float.MinValue;
    }

    public MinMax(float min, float max)
    {
        Min = min;
        Max = max;
    }

    public void Update(float value)
    {
        if (value < Min)
            Min = value;

        if (value > Max)
            Max = value;
    }
}

public class RectMinMaxTracker
{
    public MinMax X { get; protected set; }
    public MinMax Y { get; protected set; }

    public RectMinMaxTracker()
    {
        X = new MinMax();
        Y = new MinMax();
    }

    public void UpdateX(float val)
    {
        X.Update(val);
    }

    public void UpdateY(float y)
    {
        Y.Update(y);
    }
}

