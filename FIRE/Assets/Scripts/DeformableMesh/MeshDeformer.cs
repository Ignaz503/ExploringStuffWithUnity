using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshDeformer : MonoBehaviour
{
    [SerializeField] float springForce = 20f;
    [SerializeField] float damping = 5f;
    float uniformScale = 1f;

    Mesh deformationMesh;
    Vector3[] originalVertices;
    Vector3[] deformedVertices;
    Vector3[] vertexVelocities;

    private void Start()
    {
        deformationMesh = GetComponent<MeshFilter>().mesh;
        originalVertices = deformationMesh.vertices;
        deformedVertices = new Vector3[originalVertices.Length];
        vertexVelocities = new Vector3[originalVertices.Length];

        for (int i = 0; i < originalVertices.Length; i++)
        {
            deformedVertices[i] = originalVertices[i];
        }
    }

    private void Update()
    {
        uniformScale = transform.localScale.x;
        for (int i = 0; i < deformedVertices.Length; i++)
        {
            UpdateVertex(i);
        }
        deformationMesh.vertices = deformedVertices;
        deformationMesh.RecalculateNormals();
    }

    private void UpdateVertex(int i)
    {
        Vector3 velocity = vertexVelocities[i];

        Vector3 displacement = deformedVertices[i] - originalVertices[i];
        displacement *= uniformScale;
        velocity -= displacement * springForce * Time.deltaTime;
        velocity *= 1f - damping * Time.deltaTime;

        vertexVelocities[i] = velocity;

        deformedVertices[i] += velocity * (Time.deltaTime/uniformScale);
    }

    public void AddDeformingForce(Vector3 point, float force)
    {
        point = transform.InverseTransformPoint(point);
        for (int i = 0; i < deformedVertices.Length; i++)
        {
            AddForceToVertex(point, force, i);
        }
    }

    private void AddForceToVertex(Vector3 point, float force, int i)
    {
        Vector3 pointToVertex = deformedVertices[i] - point;
        pointToVertex *= uniformScale;
        float attanuatedforce = force / (1f + pointToVertex.sqrMagnitude);

        float velocity = attanuatedforce * Time.deltaTime;
        vertexVelocities[i] += pointToVertex.normalized * velocity;
    }
}
