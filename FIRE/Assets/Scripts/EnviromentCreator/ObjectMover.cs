using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    [SerializeField] Transform objectToMove = null;
    public Transform ObjectToMove { get { return objectToMove; } }

    [SerializeField] Camera screenCamera;
    public Camera ScreenCamera { get { return screenCamera; } }

    public void SetObjectToMove(Transform transform)
    {
        objectToMove = transform;
    }

    public void SetCamera(Camera cam)
    {
        screenCamera = cam;
    }

    Collider c;
    bool hasTempCollider = false;

    private void Start()
    {
        //make sure we have a collider

        if(gameObject.GetComponent<Collider>() == null)
        {
            hasTempCollider = true;
            //see if mesh collider possible else box collider
            MeshFilter f = gameObject.GetComponent<MeshFilter>();
            if(f == null)
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

    Vector3 screenPoint;
    Vector3 offset;
    
    private void OnMouseDown()
    {
        
        screenPoint = ScreenCamera.WorldToScreenPoint(ObjectToMove.position);

        offset = ObjectToMove.position - ScreenCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    private void OnMouseDrag()
    {
        Vector3 currentScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

        Vector3 newPos = ScreenCamera.ScreenToWorldPoint(currentScreenPos) + offset;

        if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
            newPos = new Vector3(Mathf.Round(newPos.x), Mathf.Round(newPos.y), Mathf.Round(newPos.z));

        ObjectToMove.position = newPos;
    }

    private void OnDestroy()
    {
        if (hasTempCollider)
            Destroy(c);
    }
}
