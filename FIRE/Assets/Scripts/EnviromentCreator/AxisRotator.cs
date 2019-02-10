using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisRotator : MonoBehaviour
{
    public enum RotationAxis
    {
        X,Y,Z
    }

    [SerializeField] AxisObjectRotator objectRotator = null;
    [SerializeField] RotationAxis rotationAxis = RotationAxis.X;
    Vector3 rotationAxisVector;
    Vector3 otherAxis;

    private void Start()
    {
        switch (rotationAxis)
        {
            case RotationAxis.X:
                rotationAxisVector = Vector3.right;
                otherAxis = Vector3.forward + Vector3.up;
                break;
                case RotationAxis.Y:
                rotationAxisVector = Vector3.up;
                otherAxis = Vector3.right + Vector3.forward;
                break;
                case RotationAxis.Z:
                rotationAxisVector = Vector3.forward;
                otherAxis = Vector3.right + Vector3.up;
                break;
            default:
                rotationAxisVector = Vector3.zero;
                otherAxis = Vector3.zero;
                break;
        }

    }

    private void OnMouseDown()
    {
        objectRotator.AxisRotationInform(rotationAxis, true);
    }

    private void OnMouseDrag()
    {
        Quaternion rot = transform.GetRotationRelativeToCamera(objectRotator.ScreenCamera, -1f * new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")), RotateRelative.World);

        Vector3 otherAxisRotation = transform.eulerAngles;
        otherAxisRotation.Scale(otherAxis);


        Vector3 newRot = rot.eulerAngles;
        newRot.Scale(rotationAxisVector);

        Quaternion newRotQuat = Quaternion.Euler(newRot);
        Quaternion oldRotQuat = Quaternion.Euler(otherAxisRotation);

        objectRotator.RotationTarget.rotation *= newRotQuat;

    }

    private void OnMouseUp()
    {
        objectRotator.AxisRotationInform(rotationAxis, false);
    }
}
