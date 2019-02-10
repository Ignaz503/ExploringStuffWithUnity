using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDummy : MonoBehaviour, IDamagable
{
    [SerializeField] float health = 50f;
    [SerializeField] MeshRenderer mRenderer;

    public void Damage(HitInfo hitInfo)
    {
        mRenderer.material.color = Random.ColorHSV();

        health -= hitInfo.Dmg;
        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        mRenderer = GetComponent<MeshRenderer>();
    }
}
