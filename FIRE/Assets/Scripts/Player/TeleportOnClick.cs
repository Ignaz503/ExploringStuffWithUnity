using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportOnClick : MonoBehaviour
{
    [SerializeField] KeyCode teleportKey = KeyCode.Mouse0;
    [SerializeField] float charHeight = 2f;
    [SerializeField] Camera rayCastCamera = null;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(teleportKey))
            TryTeleport();
    }

    void TryTeleport()
    {
        RaycastHit hit;
        if(Physics.Raycast(rayCastCamera.ViewportPointToRay(new Vector3(.5f,.5f)),out hit))
        {
            transform.position = hit.point + (Vector3.up * charHeight);
        }
    }

}
