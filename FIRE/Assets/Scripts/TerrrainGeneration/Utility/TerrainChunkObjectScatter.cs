using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChunkObjectScatter
{
    TerrainChunk chunk;
    HeightMap heightMap;
    float minHeightValue;
    float maxHeightValue;

    Bounds chunkBounds;

    List<Vector2> scatterdPoints;
    public bool HasPoints { get; protected set; }

    //TEMP
    PointsAsGizmo pointDisplay;
    public bool IsCorrectlySctattered {get; protected set; }
    public Vector3[] ScatterdPoints { get; protected set; }

    public TerrainChunkObjectScatter(TerrainChunk chunk,HeightMap heightMap, float minHeightValue,float maxHeightValue,Bounds chunkBounds, PointsAsGizmo pointDisplay)
    {
        this.chunk = chunk;
        this.heightMap = heightMap;
        this.minHeightValue = minHeightValue;
        this.maxHeightValue = maxHeightValue;
        this.chunkBounds = chunkBounds;
        this.pointDisplay = pointDisplay;

        RequestScatter();
    }

    void RequestScatter()
    {
        ThreadedDataRequester.RequestData(() => ScatterPoints(), OnPointsScatterd);
    }

    Vector3[] ScatterPoints()
    {
        List<Vector2> points = GetPoints();
        this.scatterdPoints = points;
        return CreatePointsAtCorrectHeight(points);
    }

    Vector3[] CreatePointsAtCorrectHeight(List<Vector2> points)
    {
        return CreatePointsAtCorrectHeightUsingHeightMap(points);
    }

    Vector3[] CreatePointsAtCorrectHeightUsingHeightMap(List<Vector2> points)
    {
        List<Vector3> correctPoints = new List<Vector3>();

        for (int i = 0; i < points.Count; i++)
        {
            float pointX = Mathf.Lerp(chunkBounds.center.x - chunkBounds.extents.x, chunkBounds.center.x + chunkBounds.extents.x, points[i].x);
            float pointZ = Mathf.Lerp(chunkBounds.center.y - chunkBounds.extents.y, chunkBounds.center.y + chunkBounds.extents.y, points[i].y);

            //BIDIRECTIONAL INTERPOLATION
            float x = (heightMap.Width - 1) * points[i].x;
            //invert y  cause mesh generation y inverted
            float yInvert = ((heightMap.Height - 1.0f) * (1.0f - points[i].y));

            float height = heightMap.BidiractionalSample(new Vector2(x, yInvert));

            if (height >= minHeightValue && height <= maxHeightValue)
                correctPoints.Add(new Vector3(pointX, height, pointZ));
        }

        return correctPoints.ToArray();
    }

    List<Vector2> GetPoints()
    {
        //return PoissonDiscSampling.GeneratePoints(.025f, Vector2.one, 30,.7f);
        //return PoissonDiscSampling.GeneratePoints(.025f, Vector2.one, 30,.4f);
        return PoissonDiscSampling.GeneratePoints(.025f, Vector2.one, 30,.5f);
        //return PoissonDiscSampling.GeneratePoints(.025f, Vector2.one, 30,.6f);
        //return PoissonDiscSampling.GeneratePoints(.025f, Vector2.one, 30);
    }

    void OnPointsScatterd(object pointsData)
    {
        Vector3[] points = (Vector3[])pointsData;
        ScatterdPoints = points;
        IsCorrectlySctattered = true;
        //TEMP
        pointDisplay.SetPoints(ScatterdPoints);
    }

    void UpdateScatterPoints()
    {
        ThreadedDataRequester.RequestData(() => CreatePointsAtCorrectHeight(scatterdPoints), OnPointsScatterd);
    }
}
