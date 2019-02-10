using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable 
{
    void Damage(HitInfo hitInfo);
}

public struct HitInfo
{
    /// <summary>
    /// Hit position
    /// </summary>
    public Vector3 Position { get; }

    /// <summary>
    /// hit normal
    /// </summary>
    public Vector3 Normal { get; }

    /// <summary>
    /// dmg dealt
    /// </summary>
    public float Dmg { get; }

    /// <summary>
    /// collider hit
    /// </summary>
    public Collider Collider { get; }

    public HitInfo(Vector3 position, Vector3 normal, float dmg, Collider collider) : this()
    {
        Position = position;
        Normal = normal;
        Dmg = dmg;
        Collider = collider;
    }
}
