using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointRenderer : MonoBehaviour
{
    public bool enableRotation;
    
    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter?.mesh.SetIndices(meshFilter.mesh.GetIndices(0), MeshTopology.Points, 0);
    }
    
    void Update()
    {
        if (enableRotation)
        {
            transform.Rotate(0, 0, 50*Time.deltaTime);
        }
    }
}
