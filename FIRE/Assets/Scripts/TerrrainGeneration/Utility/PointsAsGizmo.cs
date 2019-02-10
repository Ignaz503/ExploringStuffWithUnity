using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsAsGizmo : MonoBehaviour
{
    [SerializeField]Vector3[] points;
    int length { get { return points.Length; } }

    bool pointsSet;

    bool StuffSpawned;
    bool spawningStuff;

    bool visible;

    public Transform objectParent;
    ActiveIfVisibleFromViewer objectParentActivController;

    public TerrainChunk Chunk { get; set; }

    public void SetViewer(Transform viewer)
    {
        GameObject parent = new GameObject();
        parent.transform.SetParent(transform);
        parent.name = gameObject.name + " object controller";
        objectParent = parent.transform;

        objectParentActivController =  gameObject.AddComponent<ActiveIfVisibleFromViewer>();
        objectParentActivController.Viewer = viewer;
        objectParentActivController.Chunk = Chunk;
        objectParentActivController.Controller = parent;
    }

    public void SetPoints(Vector3[] points)
    {
        this.points = points;
        pointsSet = true;
        if (!StuffSpawned && gameObject.activeSelf&&!spawningStuff)
            StartCoroutine(SpawnStuffAtPoints());
    }

    public void SetVisible(bool visible)
    {
        this.visible = visible;
        if (!StuffSpawned && visible && pointsSet && gameObject.activeSelf&&!spawningStuff)
            StartCoroutine(SpawnStuffAtPoints());
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //if (pointsSet && visible)
        //{
        //    for (int i = 0; i < length; i++)
        //    {
        //        Gizmos.DrawSphere(points[i], 1);
        //    }
        //}
    }
    
    IEnumerator SpawnStuffAtPoints()
    {
        spawningStuff = true;
        int i = 0;
        while(i < points.Length)
        {
            GameObject instance = Instantiate(TerrainGenerator.RandomTree, objectParent);

            instance.GetComponent<MeshRenderer>().material = TerrainGenerator.RandomTreeMaterial;

            instance.transform.position = points[i];// + Vector3.up*(10f*.75f);
            instance.transform.localScale = Vector3.one*5f;
            //instance.transform.localScale = (Vector3.right + Vector3.forward)*7.5f +Vector3.up * 10f;
            instance.transform.eulerAngles = Vector3.up * Random.value * Mathf.PI * 2f*Mathf.Rad2Deg;
            i++;
            yield return null;
        }
        StuffSpawned = true;
    }

}
