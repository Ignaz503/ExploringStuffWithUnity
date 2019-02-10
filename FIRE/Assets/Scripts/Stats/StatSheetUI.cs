using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatSheetUI : MonoBehaviour
{
    
    [SerializeField] Canvas statSheetCanvas = null;
    [SerializeField] EntityWithStats entity = null;

    [Header("Prefabs")]
    [SerializeField] GameObject StatUIPrefab = null;

    public float dick { get; set; }

    private void Start()
    {

        BuildUI();
    }

    public void BuildUI()
    {
        if(entity!= null)
        {
            PropertyInfo[] infos = entity.entityStats.GetType().GetProperties();

            for (int i = 0; i < infos.Length; i++)
            {
                SetUpStatUI(Instantiate(StatUIPrefab), i, infos[i],entity.entityStats);
            }
        }
    }

    void SetUpStatUI(GameObject statUI,int idx, PropertyInfo info,StatSheet statSheet)
    {
        StatUI ui = statUI.GetComponent<StatUI>();

        ui.SetStatResponsible(GetStatIncreaseAmount, info.GetValue(statSheet) as Stat);
        ui.OnStatIncreaseRemainderReturn += StatIncreaseRemainderReturn;
        idx = 0;//temp
        //set position and parent
        statUI.transform.SetParent(statSheetCanvas.transform);
    }

    void StatIncreaseRemainderReturn(float remainder)
    {
        Debug.Log(remainder);
    }

    float GetStatIncreaseAmount()
    {
        return 1;
    }

}
