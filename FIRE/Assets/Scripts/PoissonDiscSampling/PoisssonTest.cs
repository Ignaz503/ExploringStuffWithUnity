using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisssonTest : MonoBehaviour {

    [SerializeField]float radius = 1f;
    [SerializeField] Vector3 RegionSize = Vector2.one;
    [Range(0,100)]public int RejectionSamples = 30;
    [SerializeField] bool draw = true;
    [SerializeField] [Range(0f, 1f)] float decimationPercent=0f;

    [SerializeField] GameObject rockPrefab = null;

    float displayRadius = .5f;

    List<Vector2> points = null;

    [Header("HeightMap")]
    [SerializeField][Range(1,100)] int width = 1;
    [SerializeField][Range(1, 100)] int height = 1;
    float[,] heightMap;

    private void Start()
    {
        decimationPercent = Random.value;
    }

    void CreateRocks(List<Vector2> locations)
    {
        foreach (Vector2 point in locations)
        {
            GameObject o = Instantiate(rockPrefab, ToXZ(point, 0),Quaternion.identity, transform);
            float avg = (transform.localScale.x+ transform.localScale.y+ transform.localScale.z)/3;
            o.transform.localScale = Vector3.one * (radius/avg);
        }
    }

    void CreateRocks(List<Vector3> points)
    {
        foreach (Vector3 point in points)
        {

            GameObject o = Instantiate(rockPrefab, transform);

            Vector3 p = point;
            p -= ((Vector3.right + Vector3.forward) * .5f);
            p.Scale(transform.localScale);
            o.transform.localPosition = p;
            float avg = (transform.localScale.x + transform.localScale.y + transform.localScale.z) / 3;
            o.transform.localScale = Vector3.one * (1 /avg);
        }
    }

    private void OnDrawGizmos()
    {
        if (!draw)
            return;

        Gizmos.color = Color.white;

        Gizmos.DrawWireCube(RegionSize/2, RegionSize);

        if (points != null)
        {
            foreach (Vector2 point in points)
            {
                Gizmos.DrawSphere(ToXZ(point,0), displayRadius);
            }
        }

    }

    void GenerateHeightMap()
    {
        heightMap = new float[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heightMap[x, y] = Random.value;
                Debug.Log(x + y);
                //heightMap[x, y] = Mathf.PerlinNoise((float)x / width, (float)y / height);
            }
        }
    }

    Vector3 ToXZ(Vector2 p, float y)
    {
        return new Vector3(p.x, 0, p.y);
    }

    Vector2 ToXZ(Vector3 p)
    {
        return new Vector2(p.x, p.z);
    }

}
