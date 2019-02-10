using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FireWoodObject : MonoBehaviour
{
    [Tooltip("Value added to fire when log is added to fire")]
    [SerializeField]FireWood fireWood;
    public FireWood FireWood { get { return fireWood; } set { fireWood = value; } }

    //probably throwable so idk

    private void OnCollisionEnter(Collision collision)
    {
        CheckCollisionWithFire(collision);
    }

    void CheckCollisionWithFire(Collision col)
    {
        Fire f = col.gameObject.GetComponent<Fire>();
        if (f != null)
        {
            f.AddFireWood(FireWood);
            Destroy(gameObject);
        }
    }
}

