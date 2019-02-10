using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Height Map Settings")]
public class HeightMapSettings : UpdatableData
{
    public Noise.NoiseSettings Settings;

    [Header("On Generated Noise")]
    public bool ApplyFalloff;
    public float HeightMultiplier = 2f;
    public AnimationCurve HeightCurve;

    public float MinHeight
    {
        get
        {
            return HeightMultiplier * HeightCurve.Evaluate(0);
        }
    }
    public float MaxHeight
    {
        get
        {
            return HeightMultiplier * HeightCurve.Evaluate(1f);
        }
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        Settings.ValidateValues();
        base.OnValidate();
    }
#endif
}
