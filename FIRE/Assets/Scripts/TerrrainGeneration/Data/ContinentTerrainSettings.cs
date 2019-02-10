using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Continent Settings")]
public class ContinentTerrainSettings : UpdatableData
{
    public Noise.NoiseSettings NoiseSettings;
    [Range(0f,1f)]public float LandOceanThreshold;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        NoiseSettings.ValidateValues();
        base.OnValidate();
    }
#endif
}
