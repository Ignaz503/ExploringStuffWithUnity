using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    [SerializeField]KeyCode useKey = KeyCode.Mouse0;
    [SerializeField] Transform trackingTransform = null;
    IHoldeableItem heldItem;
    public Transform ParentTransform { get; protected set; }
    public Camera PlayerCamera { get; protected set; }

    public bool HasItem { get { return heldItem != null; } }

    //delay??
    bool pickedUpThisFrame = false;

    public void HoldItem(IHoldeableItem item)
    {
        if (heldItem != null)
            DropItem();
        heldItem = item;
        pickedUpThisFrame = true;
        heldItem.OnPlacedInItemHolder(this);
    }

    public void DropItem()
    {
        if(heldItem != null)
        {
            heldItem.DropHeldItem();
            heldItem.GameObject.transform.SetParent(null);
            heldItem = null; 
        }
    }

    private void Start()
    {
        transform.position = trackingTransform.position;
    }

    private void Update()
    {
        transform.position = trackingTransform.position;
        transform.rotation = trackingTransform.rotation;
        if (!pickedUpThisFrame)
            TryUseItem();
        else
            pickedUpThisFrame = false;
    }

    public void TryUseItem()
    {
        heldItem?.UpdateRoutineWhenHeld();
    }

    public void SetParentTransform(Transform parent)
    {
        ParentTransform = parent;
    }

    public void SetPlayerCamera(Camera cam)
    {
        PlayerCamera = cam;
    }

    public void SetUseKey(KeyCode k)
    {
        useKey = k;
    }    

    public bool UseKeyDown()
    {
        return Input.GetKeyDown(useKey);
    }

    public bool UseKeyUp()
    {
        return Input.GetKeyUp(useKey);
    }

    public bool UseKeyHeld()
    {
        return Input.GetKey(useKey);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;

        Gizmos.DrawSphere(transform.position,.1f);
    }

}


