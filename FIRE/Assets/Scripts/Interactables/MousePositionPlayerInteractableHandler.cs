using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePositionPlayerInteractableHandler : PlayerInteractableHandler
{
    protected override Ray RaycastRay { get { return playerCamera.ScreenPointToRay(Input.mousePosition); } }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(RaycastRay);
    }

}
