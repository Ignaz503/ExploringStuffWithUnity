using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityWithStats : MonoBehaviour
{
    [SerializeField] float min = 0;
    [SerializeField] float max = 10;
    [HideInInspector]public StatSheet entityStats;
    [SerializeField] StatSheetUI entityUI = null;

    // Start is called before the first frame update
    void Awake()
    {
        entityStats = StatSheet.Creator.CreateRandomStatSheetInRange(min, max);
        entityUI.gameObject.SetActive(true);//temp
    }
}
