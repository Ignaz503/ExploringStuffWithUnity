using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ChunkMeshSplitter
{
    public static Mesh[] SplitMesh(Mesh originalMesh, int vertsPerLineOld, int numVertsPerLineNew)
    {
        Mesh[] newMeshes = new Mesh[4];

        RectMinMaxTracker[] minMaxTracker = new RectMinMaxTracker[4] { new RectMinMaxTracker(), new RectMinMaxTracker(), new RectMinMaxTracker(), new RectMinMaxTracker() };

        float vertHalf = vertsPerLineOld/2.0f;
        float halfSquare = vertHalf * vertHalf;

        Vector3[][] verticesSplit = new Vector3[4][] 
        {
            new Vector3[Mathf.RoundToInt(halfSquare)+Mathf.CeilToInt(vertHalf)],
            new Vector3[Mathf.RoundToInt(halfSquare)],
            new Vector3[Mathf.RoundToInt(halfSquare)],
            new Vector3[Mathf.RoundToInt(halfSquare)-Mathf.FloorToInt(vertHalf)]
        };

        Vector2[][] uvsNew = new Vector2[4][]
        {
            new Vector2[Mathf.RoundToInt(halfSquare)+Mathf.CeilToInt(vertHalf)],
            new Vector2[Mathf.RoundToInt(halfSquare)],
            new Vector2[Mathf.RoundToInt(halfSquare)],
            new Vector2[Mathf.RoundToInt(halfSquare)-Mathf.FloorToInt(vertHalf)]
        };

        float tri0 = ((Mathf.Sqrt(verticesSplit[0].Length)-1)* (Mathf.Sqrt(verticesSplit[0].Length) - 1))*6;

        float tri1 = ((Mathf.Sqrt(verticesSplit[1].Length)-1)* (Mathf.Sqrt(verticesSplit[1].Length) - 1))*6;
        float tri2 = ((Mathf.Sqrt(verticesSplit[2].Length)-1)* (Mathf.Sqrt(verticesSplit[2].Length) - 1))*6;
        float tri3 = ((Mathf.Sqrt(verticesSplit[3].Length)-1)* (Mathf.Sqrt(verticesSplit[3].Length) - 1))*6;

        Debug.Log($"tri0:{Mathf.RoundToInt(tri0)}, tri1: {Mathf.RoundToInt(tri1)}, tri2: {Mathf.RoundToInt(tri2)}, tri3: {Mathf.RoundToInt(tri3)}");

        int[][] newTriangles = new int[4][]
        {
            new int[Mathf.RoundToInt(tri0)],
            new int[Mathf.RoundToInt(tri1)],
            new int[Mathf.RoundToInt(tri2)],
            new int[Mathf.RoundToInt(tri3)],
        };

        int[] incrementer = new int[4] { 0, 0, 0, 0 };
        int[] triIncrement = new int[4] { 0, 0, 0, 0 };

        for (int y = 0; y < vertsPerLineOld; y++)
        {
            for (int x = 0; x < vertsPerLineOld; x++)
            {
                int meshIdx = GetVertexNewMeshIndex(x, y, vertsPerLineOld);
                if (meshIdx < 0)
                    throw new System.Exception("Mistakes where made when deciding on mesh index whilst spliting");

                Vector3 vert = originalMesh.vertices[y * vertsPerLineOld + x];

                verticesSplit[meshIdx][incrementer[meshIdx]] = vert;

                float length = verticesSplit[meshIdx].Length;
                int vertLine = (int)Mathf.Sqrt(length);//error
                int current = incrementer[meshIdx];

                Debug.Log($"{meshIdx}: {current}");

                float uY = (float)current / (Mathf.Sqrt(length));
                float uX = (float)current % (Mathf.Sqrt(length));

                try
                {
                    newTriangles[meshIdx][triIncrement[meshIdx]] = current;

                    newTriangles[meshIdx][triIncrement[meshIdx] + 1] = current + (vertLine+1);
                    newTriangles[meshIdx][triIncrement[meshIdx] + 2] = current+vertLine;

                    newTriangles[meshIdx][triIncrement[meshIdx] + 3] = current + (vertLine + 1);
                    newTriangles[meshIdx][triIncrement[meshIdx] + 4] = current;
                    newTriangles[meshIdx][triIncrement[meshIdx] + 5] = current+1;
                }
                catch (System.IndexOutOfRangeException ex)
                {
                    int posIncrement = (vertsPerLineOld-x) * (vertsPerLineOld-y) * 6;
                    Debug.Log($"max wished increment {triIncrement[meshIdx]+posIncrement}, size: {newTriangles[meshIdx].Length}, meshIdx: {meshIdx}");
                    Debug.Log($"vert: {Mathf.Sqrt(verticesSplit[meshIdx].Length)}");
                    throw ex;
                }
                triIncrement[meshIdx] += 3; 

                uvsNew[meshIdx][incrementer[meshIdx]] = new Vector2(uX, uY);

                incrementer[meshIdx]++;
                minMaxTracker[meshIdx].UpdateX(vert.x);
                minMaxTracker[meshIdx].UpdateY(vert.y);
            }
        }

        for (int i = 0; i < newMeshes.Length; i++)
        {
            newMeshes[i] = new Mesh()
            {
                vertices = verticesSplit[i],
                uv = uvsNew[i],
                triangles = newTriangles[i]
            };
            newMeshes[i].RecalculateNormals();
        }

        return newMeshes;
    }

    static int GetVertexNewMeshIndex(int x, int y, int vertsPerLine)
    {
        float xPercent = (float)(x) / ((float)vertsPerLine);
        float yPercent = (float)(y) / ((float)vertsPerLine);

        if (xPercent < .5f && yPercent < .5f)
            return 0;
        if (xPercent >= .5f && yPercent < .5f)
            return 1;
        if (xPercent < .5f && yPercent >= .5f)
            return 2;
        if (xPercent >= .5f && yPercent >= .5f)
            return 3;

        Debug.Log($"x: {xPercent}, y: {yPercent}, test x >= .5 and y < .5: {xPercent >= .5f && y < .5f}");

        return -1;
    }

}
