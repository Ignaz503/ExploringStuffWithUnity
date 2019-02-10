using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisRotationCollider : MonoBehaviour
{
    public event System.Action OnDown;
    public event System.Action OnDrag;

    private void OnMouseDown()
    {
        OnDown?.Invoke();
    }

    private void OnMouseDrag()
    {
        OnDrag?.Invoke();
    }

}
