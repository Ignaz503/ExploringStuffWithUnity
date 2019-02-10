using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveIfVisibleFromViewer : MonoBehaviour
{
    public Transform Viewer { get; set; }
    public GameObject Controller { get; set; }
    public TerrainChunk Chunk { get; set; }

    // Update is called once per frame
    void Update()
    {
        Vector3 vec = (transform.position-Viewer.position).normalized;
        
        if (Vector3.Dot(vec, Viewer.forward) <= 0f && !Chunk.ViewerInChunk)
            Controller.SetActive(false);
        else
            Controller.SetActive(true);

    }
}
