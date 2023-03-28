using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class GoldenSpiralSphere : MonoBehaviour
{
    // topology type
    public enum Topology {Triangles = 0, Lines = 3, Points = 5}
    
    // mesh components
    private Mesh sphereMesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    // event vars
    private int prevPoints;
    private bool inc = true;
    private Topology prevTopo;

    [Header("Sphere Settings")] 
    [Range(5, 1000)] public int numPoints = 100;
    public Material meshMaterial;
    public Topology topology = Topology.Points;
    public bool enableRotation;

    // Start is called before the first frame update
    void Start()
    {
        sphereMesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        StartCoroutine(ChangePointNum());
    }

    // Update is called once per frame
    void Update()
    {
        if (enableRotation)
        {
            transform.Rotate(0, 50*Time.deltaTime, 0);
        }
        
        if (prevPoints != numPoints)
        {
            prevPoints = numPoints;
            DrawMesh();
        }
        
        if (prevTopo != topology)
        {
            prevTopo = topology;
            DrawMesh();
        }
    }

    IEnumerator ChangePointNum()
    {
        if (numPoints > 999)    inc = false;
        else if (numPoints < 6) inc = true;
        
        if (inc)    numPoints++;
        else        numPoints--;

        yield return new WaitForSeconds(0.005f);

        StartCoroutine(ChangePointNum());
    }

    void DrawMesh()
    {
        sphereMesh.Clear();
        
        var floatIdx = new float[numPoints];
        for (int i = 0; i < numPoints; i++)
        {
            floatIdx[i] = i + 0.5f;
        }
        var phi = new float[numPoints];
        var theta = new float[numPoints];
        for (int i = 0; i < numPoints; i++)
        {
            phi[i] = Mathf.Acos(1 - 2 * floatIdx[i] / numPoints);
            theta[i] = Mathf.PI * (1 + Mathf.Pow(5f, 0.5f)) * floatIdx[i];
        }
        var spherePoints = new Vector3[numPoints];
        for (int i = 0; i < numPoints; i++)
        {
            spherePoints[i].x = Mathf.Cos(theta[i]) * Mathf.Sin(phi[i]);
            spherePoints[i].y = Mathf.Sin(theta[i]) * Mathf.Sin(phi[i]);
            spherePoints[i].z = Mathf.Cos(phi[i]);
        }
        
        sphereMesh.SetVertices(spherePoints);
        
        var spherePointIndices = new int[numPoints];
        var sphereLineIndices = new int[2 * numPoints];
        
        if (topology == Topology.Points)
        {
            for (int i = 0; i < numPoints; i++)
            {
                spherePointIndices[i] = i;
            }
            sphereMesh.SetIndices(spherePointIndices, MeshTopology.Points, 0);
        }
        else if (topology == Topology.Lines)
        {
            for (int i = 0; i < numPoints - 1; i ++)
            {
                sphereLineIndices[2 * i] = i;
                sphereLineIndices[2 * i + 1] = i + 1;
            }
            sphereLineIndices[2 * numPoints - 2] = numPoints - 1;
            sphereLineIndices[2 * numPoints - 1] = 0;

            sphereMesh.SetIndices(sphereLineIndices, MeshTopology.Lines, 0);
        }
        else if (topology == Topology.Triangles)
        {
            var idx = new int[3 * (numPoints - 2)];

            for (int i = 0; i < numPoints - 3; i++)
            {
                idx[3 * i] = i;
                idx[3 * i + 1] = i + 1;
                idx[3 * i + 2] = i + 2;
            }

            sphereMesh.SetIndices(idx, MeshTopology.Triangles, 0);
        }

        meshFilter.mesh = sphereMesh;
        meshRenderer.material = meshMaterial;
    }
}