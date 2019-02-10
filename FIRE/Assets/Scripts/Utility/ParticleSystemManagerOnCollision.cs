using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;


public class ParticleSystemManagerOnCollision : MonoBehaviour
{
    [SerializeField]new ParticleSystem particleSystem = null;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Enter");
        if (collision.gameObject.GetComponent<FirstPersonController>() != null)
        {
            particleSystem.Play();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("Exit");
        if (collision.gameObject.GetComponent<FirstPersonController>() != null)
        {
            //start particle system
            particleSystem.Stop();
        }
    }
}
