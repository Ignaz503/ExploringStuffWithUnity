using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    [SerializeField] Transform objectToRotate;
    [SerializeField] Camera cameraRelativeTo;

    Collider c;
    bool hasTempCollider;

    public void SetObjectToRotate(Transform trans)
    {
        objectToRotate = trans;
    }

    public void SetCamera(Camera cam)
    {
        cameraRelativeTo = cam;
    }

    private void Start()
    {
        //figure out if collider

        if (gameObject.GetComponent<Collider>() == null)
        {
            hasTempCollider = true;
            //see if mesh collider possible else box collider
            MeshFilter f = gameObject.GetComponent<MeshFilter>();
            if (f == null)
            {
                c = gameObject.AddComponent<BoxCollider>();
            }
            else
            {
                MeshCollider col = gameObject.AddComponent<MeshCollider>();
                col.sharedMesh = f.mesh;
                c = col;
            }
        }
    }

    private void OnMouseDrag()
    {
        objectToRotate.RotateRelativeToCamera(cameraRelativeTo, -1f*Input.GetAxis("Mouse X"), -1f* Input.GetAxis("Mouse Y"), RotateRelative.World);
    }

    private void OnDestroy()
    {
        if (hasTempCollider)
            Destroy(c);
    }
}
