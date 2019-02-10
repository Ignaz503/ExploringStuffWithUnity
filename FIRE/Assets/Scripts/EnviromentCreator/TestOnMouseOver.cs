using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOnMouseOver : MonoBehaviour
{
    private void OnMouseOver()
    {
        Debug.Log(gameObject.name);
    }
}
