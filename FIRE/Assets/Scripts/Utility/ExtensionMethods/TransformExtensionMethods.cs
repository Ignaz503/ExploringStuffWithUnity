using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RotateRelative
{
    World,
    Local
}

/// <summary>
/// extension methods for Unity transform class
/// </summary>
public static class TransformExtensionMethods
{
    /// <summary>
    /// rotates a transform rleative to a camera
    /// </summary>
    /// <param name="objToRotate">object to rotate</param>
    /// <param name="camRelativeTo">camera the object need relative rotating to</param>
    /// <param name="rotateLeftRight"> left right rotation</param>
    /// <param name="rotateUpDown">up down rotation</param>
    /// <param name="relative">decide if world or local rotation modified</param>
    public static void RotateRelativeToCamera(this Transform objToRotate, Camera camRelativeTo, float rotateLeftRight, float rotateUpDown, RotateRelative relative)
    {
        objToRotate.RotateRelativeToOtherTransform(camRelativeTo.transform, new Vector2(rotateLeftRight, rotateUpDown), relative);
    }

    public static void RotateRelativeToCamera(this Transform objToRotate, Camera cam, Vector2 dir,RotateRelative relative)
    {
        objToRotate.RotateRelativeToOtherTransform(cam.transform, dir, relative);
    }

    public static Quaternion GetRotationRelativeToCamera(this Transform objToRotate, Camera cam, Vector2 dir, RotateRelative relative)
    {
        Vector3 relativeUp = cam.transform.TransformDirection(Vector3.up);
        Vector3 relativeRight = cam.transform.TransformDirection(Vector3.right);

        Vector3 objRelativeUp = objToRotate.transform.InverseTransformDirection(relativeUp);
        Vector3 objRelativeRight = objToRotate.transform.InverseTransformDirection(relativeRight);

        Quaternion rot = Quaternion.AngleAxis(dir.x / objToRotate.localScale.x, objRelativeUp) * Quaternion.AngleAxis(-dir.y / objToRotate.localScale.x, objRelativeRight);

        return rot;
    }

    public static void RotateRelativeToOtherTransform(this Transform objToRotate, Transform transform, Vector2 dir, RotateRelative relative)
    {
        Vector3 relativeUp = transform.TransformDirection(Vector3.up);
        Vector3 relativeRight = transform.TransformDirection(Vector3.right);

        Vector3 objRelativeUp = objToRotate.transform.InverseTransformDirection(relativeUp);
        Vector3 objRelativeRight = objToRotate.transform.InverseTransformDirection(relativeRight);

        Quaternion rot = Quaternion.AngleAxis(dir.x / objToRotate.localScale.x, objRelativeUp) * Quaternion.AngleAxis(-dir.y / objToRotate.localScale.x, objRelativeRight);

        switch (relative)
        {
            case RotateRelative.Local:
                objToRotate.localRotation *= rot;
                break;
            case RotateRelative.World:
                objToRotate.rotation *= rot;
                break;
            default:
                throw new System.Exception("Relative Rotation not defined");
        }
    }

    public static void RotateRelativeToOtherTransform(this Transform objToRotate, Transform transform, float rotateLeftRight, float rotateUpDown, RotateRelative relative)
    {
        objToRotate.RotateRelativeToOtherTransform(transform, new Vector2(rotateLeftRight, rotateUpDown), relative);
    }
}
