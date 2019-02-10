using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] heightMap, MeshSettings settings, int levelOfDetail)
    {
        int skipIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;

        int numVertsPerLine = settings.NumberOfVerticesPerLine;

        Vector2 topLeft = new Vector2(-1, 1) * settings.MeshWorldSize / 2f;

        MeshData meshData = new MeshData(numVertsPerLine, skipIncrement,settings.UseFlatShading);

        int[,] vertexIdicesMap = new int[numVertsPerLine, numVertsPerLine];
        int meshVertexIndex = 0;
        int outOfMeshVertexIndex = -1;

        //create vertex indices map
        for (int y = 0; y < numVertsPerLine; y++)
        {
            for (int x = 0; x < numVertsPerLine; x++)
            {
                bool isOutOfMeshVertex = y == 0 || y == numVertsPerLine - 1 || x == 0 || x == numVertsPerLine - 1;

                bool isSkippedVertex = x > 2 && 
                    x < numVertsPerLine - 3 && 
                    y > 2 && 
                    y < numVertsPerLine - 3 && 
                    ((x - 2) % skipIncrement != 0 || (y - 2) % skipIncrement != 0);

                if (isOutOfMeshVertex)
                {
                    vertexIdicesMap[x, y] = outOfMeshVertexIndex;
                    outOfMeshVertexIndex--;
                }// end if isOutOfMeshVertex
                else if(!isSkippedVertex)
                {
                    vertexIdicesMap[x, y] = meshVertexIndex;
                    meshVertexIndex++;
                }// end else if isSkippedVertex
            }// end for x
        }// end for y

        //create mesh
        for (int y = 0; y < numVertsPerLine; y++)
        {
            for (int x = 0; x < numVertsPerLine; x++)
            {
                bool isSkippedVertex = x > 2 &&
                    x < numVertsPerLine - 3 &&
                    y > 2 &&
                    y < numVertsPerLine - 3 &&
                    ((x - 2) % skipIncrement != 0 || (y - 2) % skipIncrement != 0);

                if (!isSkippedVertex)
                {
                    bool isOutOfMeshVertex = y == 0 || y == numVertsPerLine - 1 || x == 0 || x == numVertsPerLine - 1;
                    bool isMeshEdgeVertex = (y == 1 || y == numVertsPerLine - 2 || x == 1 || x == numVertsPerLine - 2) && !isOutOfMeshVertex;
                    bool isMainVertex = (x - 2) % skipIncrement == 0 && (y - 2) % skipIncrement == 0 && !isOutOfMeshVertex && !isMeshEdgeVertex;
                    bool isEdgeConnectionVertex = (y == 2 || y == numVertsPerLine - 3 || x == 2 || x == numVertsPerLine - 3) && !isOutOfMeshVertex && !isMeshEdgeVertex && !isMainVertex;

                    int vertIdx = vertexIdicesMap[x, y];

                    Vector2 percent = new Vector2(x-1,y-1) /(numVertsPerLine-3);

                    float height = heightMap[x, y];

                    if (isEdgeConnectionVertex)
                    {
                        bool isVertical = x == 2 || x == numVertsPerLine - 3;
                        int distMainVertA = (isVertical ? y - 2 : x - 2 ) % skipIncrement;
                        int distMainVertB = skipIncrement - distMainVertA;
                        float distPercentAB = distMainVertA / (float)skipIncrement;

                        float heightMainVertA = heightMap[isVertical?x:x-distMainVertA, isVertical ? y-distMainVertA:y];

                        float heightMainVertB = heightMap[isVertical ? x : x + distMainVertB, isVertical ? y + distMainVertB : y];

                        height = heightMainVertA * (1 - distPercentAB) + heightMainVertB * distPercentAB;

                    }

                    Vector2 vertPos2D = topLeft + new Vector2(percent.x,-percent.y) * settings.MeshWorldSize;

                    meshData.AddVertex(new Vector3(vertPos2D.x, height, vertPos2D.y), percent, vertIdx);

                    bool createTriangle = x < numVertsPerLine - 1 && y < numVertsPerLine - 1 && (!isEdgeConnectionVertex || (x != 2 && y != 2));

                    if (createTriangle)
                    {
                        int currentIncrement = (isMainVertex && x != numVertsPerLine - 3 && y != numVertsPerLine - 3) ? skipIncrement : 1;

                        int a = vertexIdicesMap[x, y];
                        int b = vertexIdicesMap[x + currentIncrement, y];
                        int c = vertexIdicesMap[x, y + currentIncrement];
                        int d = vertexIdicesMap[x + currentIncrement, y + currentIncrement];

                        //not on right or bottom edge vertices
                        meshData.AddTriangle(a, d, c);
                        meshData.AddTriangle(d, a, b);
                    }
                }// end if not skipped vertex
            }
        }
        meshData.ProcessMesh();
        return meshData;
    }
}

public class MeshData
{
    Vector3[] vertices;
    public Vector3[] Vertices { get { return vertices; } }
    int[] triangles;
    public int[] Triangles { get{ return triangles; } }
    Vector2[] uvs;
    Vector3[] bakedNormals;

    Vector3[] outOfMeshVertices;
    int[] outOfMeshTriangles;
    int triIdx;
    int outOfMeshTriangleIndex;

    bool useFlatShading;

    public MeshData(int numVertsPerLine, int skipIncrement,bool useFlatShading)
    {

        int numMeshEdgeVertices = (numVertsPerLine - 2) * 4 - 4;
        int numEdgeConnectionVertices = (skipIncrement - 1) * (numVertsPerLine - 5) / skipIncrement * 4;
        int numMainVerticesPerLine = (numVertsPerLine - 5) / skipIncrement + 1;
        int numMainVertices = numMainVerticesPerLine * numMainVerticesPerLine;

        vertices = new Vector3[numMeshEdgeVertices + numEdgeConnectionVertices + numMainVertices];
        uvs = new Vector2[vertices.Length];

        int numMeshEdgeTriangles = 8 * (numVertsPerLine - 4);
        int numMainTriangles = (numMainVerticesPerLine - 1) * (numMainVerticesPerLine - 1) * 2;
        triangles = new int[(numMeshEdgeTriangles + numMainTriangles) * 3];

        outOfMeshVertices = new Vector3[numVertsPerLine * 4 - 4];
        outOfMeshTriangles = new int[24 * (numVertsPerLine - 2)];

        this.useFlatShading = useFlatShading;
    }

    public MeshData(Vector3[] vertices, int[] triangles, Vector2[] uv)
    {
        this.vertices = vertices;
        this.triangles = triangles;
        this.uvs = uv;
    }

    public void AddTriangle(int a, int b, int c)
    {
        if (a < 0 || b < 0 || c < 0)
        {
            outOfMeshTriangles[outOfMeshTriangleIndex] = a;
            outOfMeshTriangles[outOfMeshTriangleIndex + 1] = b;
            outOfMeshTriangles[outOfMeshTriangleIndex + 2] = c;
            outOfMeshTriangleIndex += 3;
        }
        else
        {
            triangles[triIdx] = a;
            triangles[triIdx + 1] = b;
            triangles[triIdx + 2] = c;
            triIdx += 3;
        }
    }

    public void AddVertex(Vector3 vertexPos, Vector2 uv, int idx)
    {
        if (idx < 0)
        {
            outOfMeshVertices[-idx - 1] = vertexPos;
        }
        else
        {
            vertices[idx] = vertexPos;
            uvs[idx] = uv;
        }
    }

    Vector3[] CalculateNormals()
    {
        Vector3[] vertexNormals = new Vector3[vertices.Length];
        int triCount = triangles.Length / 3;

        for (int i = 0; i < triCount; i++)
        {
            int normalTriIndex = i * 3;

            int vertexIdxA = triangles[normalTriIndex];
            int vertexIdxB = triangles[normalTriIndex + 1];
            int vertexIdxC = triangles[normalTriIndex + 2];

            Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIdxA, vertexIdxB, vertexIdxC);

            vertexNormals[vertexIdxA] += triangleNormal;
            vertexNormals[vertexIdxB] += triangleNormal;
            vertexNormals[vertexIdxC] += triangleNormal;

        }

        int borderTriCount = outOfMeshTriangles.Length / 3;
        for (int i = 0; i < borderTriCount; i++)
        {
            int normalTriIndex = i * 3;

            int vertexIdxA = outOfMeshTriangles[normalTriIndex];
            int vertexIdxB = outOfMeshTriangles[normalTriIndex + 1];
            int vertexIdxC = outOfMeshTriangles[normalTriIndex + 2];

            Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIdxA, vertexIdxB, vertexIdxC);

            if (vertexIdxA >= 0)
                vertexNormals[vertexIdxA] += triangleNormal;
            if (vertexIdxB >= 0)
                vertexNormals[vertexIdxB] += triangleNormal;
            if (vertexIdxC >= 0)
                vertexNormals[vertexIdxC] += triangleNormal;

        }

        for (int i = 0; i < vertexNormals.Length; i++)
        {
            vertexNormals[i].Normalize();
        }

        return vertexNormals;
    }

    Vector3 SurfaceNormalFromIndices(int idxA, int idxB, int idxC)
    {
        Vector3 pointA = (idxA < 0) ? outOfMeshVertices[-idxA - 1] : vertices[idxA];
        Vector3 pointB = (idxB < 0) ? outOfMeshVertices[-idxB - 1] : vertices[idxB];
        Vector3 pointC = (idxC < 0) ? outOfMeshVertices[-idxC - 1] : vertices[idxC];

        Vector3 ab = pointB - pointA;
        Vector3 ac = pointC - pointA;

        return Vector3.Cross(ab, ac).normalized;
    }

    public void ProcessMesh()
    {
        if (useFlatShading)
            FlatShade();
        else
            BakeNormals();
    }

    void BakeNormals()
    {
        bakedNormals = CalculateNormals();
    }

    public void FlatShade()
    {
        Vector3[] flatShadedVertices = new Vector3[triangles.Length];
        Vector2[] flatShadedUvs = new Vector2[triangles.Length];

        for (int i = 0; i < triangles.Length; i++)
        {
            flatShadedVertices[i] = vertices[triangles[i]];
            flatShadedUvs[i] = uvs[triangles[i]];
            triangles[i] = i;
        }
        vertices = flatShadedVertices;
        uvs = flatShadedUvs;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh()
        {
            vertices = vertices,
            triangles = triangles,
            uv = uvs,
        };

        if (useFlatShading)
            mesh.RecalculateNormals();
        else
            mesh.normals = bakedNormals;
        //mesh.RecalculateNormals();
        //mesh.RecalculateTangents();
        //mesh.RecalculateBounds();
        return mesh;
    }
}