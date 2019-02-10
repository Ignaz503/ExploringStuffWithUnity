using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FireWood
{
    [SerializeField] float value;
    public float Value { get { return value; } }

    public FireWood(float value)
    {
        this.value = value;
    }

    public FireWood()
    { }
}