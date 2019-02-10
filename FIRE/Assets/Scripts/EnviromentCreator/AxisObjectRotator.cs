using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisObjectRotator : MonoBehaviour
{
    [SerializeField] AxisRotator[] axisRotators = null;

    [SerializeField] Camera screenCamera = null;
    public Camera ScreenCamera { get { return screenCamera; } }

    [SerializeField] Transform rotationTarget = null;
    public Transform RotationTarget { get { return rotationTarget; } }

    public bool CanRotate { get; protected set; }

    bool isRotating;

    public void AxisRotationInform(AxisRotator.RotationAxis axis, bool isRotating)
    {
        if (isRotating)
        {
            for (int i = 0; i < axisRotators.Length; i++)
            {
                if ((int)axis != i)
                    axisRotators[i].gameObject.SetActive(false);
                else
                    axisRotators[i].gameObject.SetActive(true);

            }
        }
        else
        {
            //enanble all
            for (int i = 0; i < axisRotators.Length; i++)
            {
                axisRotators[i].gameObject.SetActive(true);
            }
        }
    }

}
