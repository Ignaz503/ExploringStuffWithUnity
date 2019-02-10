using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StatSystemInitializer : MonoBehaviour
{
    [SerializeField] StatInfluenceGraphStorage statInfluenceGraph = null;
    [SerializeField] string AssetPath  = "Assets/Resources/Stats/InfluenceGraph/StatsInfluenceStorage.asset";

    private void Awake()
    {
        if (statInfluenceGraph.ConnectionInfos == null)
        {
            statInfluenceGraph = Resources.Load<StatInfluenceGraphStorage>(AssetPath);
        }

        StatSheet.SetGraphStatInfluence(statInfluenceGraph);
    }
}
