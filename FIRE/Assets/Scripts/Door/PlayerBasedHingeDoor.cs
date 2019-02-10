using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBasedHingeDoor : SingleHingedDoor
{

    float defualtDirection;
    bool changedMotion = false;

    private void Awake()
    {
        defualtDirection = movementDirection;
        OnInteract += WhenInteract;
        OnClosed += WhenClosed;
    }

    void WhenClosed(BaseDoor d)
    {
        movementDirection = defualtDirection;
        if(changedMotion)
            openRotation = Quaternion.Euler(openRotation.eulerAngles * -1);
    }

    void WhenInteract(ControllableEntity ent)
    {
        if (IsOpenOrOpening)
            return;

        changedMotion = false;

        Vector3 toDoor = (transform.position - ent.transform.position).normalized;

        Vector3 toOpenPos = (transform.TransformPoint(openPosition) - transform.position).normalized;

        Debug.Log(Vector3.Dot(toDoor, toOpenPos));
        if(Vector3.Dot(toDoor,toOpenPos) < 0)
        {
            movementDirection *= -1;
            changedMotion = true;
            openRotation = Quaternion.Euler(openRotation.eulerAngles * -1);
        }

    }

}


