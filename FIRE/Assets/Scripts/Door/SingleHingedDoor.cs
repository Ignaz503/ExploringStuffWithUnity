using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleHingedDoor : BaseDoor
{
    [SerializeField] bool drawGizmos = true;
    [SerializeField]Vector3 hinge = Vector3.zero;
    protected Vector3 openPosition;
    protected Vector3 closePosition;


    [SerializeField] float rotationSpeed = .2f;
    [SerializeField][Range(-1,1)] protected float movementDirection = 1f;
    protected Quaternion openRotation;

    private void Start()
    {
        movementDirection = Mathf.Sign(movementDirection);
        openPosition = new Vector3(hinge.x, hinge.y, movementDirection*20);
        closePosition = new Vector3(-hinge.x, hinge.y, 0);
        openRotation = Quaternion.Euler(new Vector3(0, movementDirection*90, 0));
    }

    protected override IEnumerator CloseCoroutine()
    {
        Vector3 hingeWorld = transform.TransformPoint(hinge);

        float compareRot = transform.localEulerAngles.y;
        while (compareRot>0)//todo
        {
            transform.RotateAround(hingeWorld, transform.up, -movementDirection*rotationSpeed * Time.deltaTime);

            if(movementDirection > 0)
                compareRot = transform.localEulerAngles.y > 90 ? transform.localEulerAngles.y - 360 : transform.localEulerAngles.y;
            else
                compareRot= transform.localEulerAngles.y < 270 ? -compareRot:compareRot;
            yield return null;
        }
        transform.RotateAround(hingeWorld, transform.up, 0 - transform.localEulerAngles.y);
    }

    protected override IEnumerator OpenCoroutine()
    {
        Vector3 hingeWorld = transform.TransformPoint(hinge);
        float compareRot = 0f;
        if (movementDirection > 0 || previousState == DoorState.Closed)
            compareRot = transform.localEulerAngles.y;
        else
            compareRot = Mathf.Abs(transform.localEulerAngles.y - 360);

        while (compareRot < 90)
        {

            transform.RotateAround(hingeWorld, transform.up, movementDirection*rotationSpeed * Time.deltaTime);

            if (movementDirection > 0)
                compareRot = transform.localEulerAngles.y;
            else
                compareRot = Mathf.Abs(transform.localEulerAngles.y - 360);

            yield return null;
        }
        transform.RotateAround(hingeWorld, transform.up, openRotation.eulerAngles.y - transform.localEulerAngles.y);
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos)
            return;

        DrawLocalPoint(hinge, Color.black);

        Vector3 openPos = openPosition;
        openPos.z *= Mathf.Sign(hinge.x); 
        DrawLocalPoint(openPos, Color.green);

        DrawLocalPoint(closePosition, Color.red);


        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.TransformPoint(hinge), transform.TransformPoint(openPos));
        Gizmos.DrawLine(transform.TransformPoint(hinge), transform.TransformPoint(closePosition));

    }

    void DrawLocalPoint(Vector3 point, Color c)
    {
        Gizmos.color = c;
        Vector3 pointWorld = transform.TransformPoint(point);
        Gizmos.DrawSphere(pointWorld, .1f);

    }

    private void OnValidate()
    {
        Start();
    }

}
