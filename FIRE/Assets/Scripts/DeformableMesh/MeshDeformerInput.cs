using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshDeformerInput : MonoBehaviour
{
    [SerializeField] float forceOffset = .1f;
    [SerializeField] float force = 10f;
    public float Force { get { return force; } set { force = value; } }
    [SerializeField] KeyCode applyForceButton = KeyCode.Mouse0;
    [SerializeField] Camera raycastCamera = null;

    private void Update()
    {
        if (Input.GetKey(applyForceButton))
        {
            HandleInput();
        }
    }

    private void HandleInput()
    {
        Ray inputRay = raycastCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(inputRay,out hit))
        {
            MeshDeformer deformer = hit.collider.gameObject.GetComponent<MeshDeformer>();
            if (deformer)
            {
                Vector3 point = hit.point;
                point += hit.normal * forceOffset;
                deformer.AddDeformingForce(point,force);
            }
        }

    }
}
