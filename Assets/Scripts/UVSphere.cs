using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class UVSphere : MonoBehaviour
{
    // topology type
    public enum Topology {Triangles = 0, Lines = 3, Points = 5}
    
    // mesh components
    private Mesh sphereMesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    // event variables
    private int prevPoints;
    private bool inc = true;
    private Topology prevTopo;
    private bool prevAnim;

    [Header("Sphere Settings")] 
    [Range(2, 50)] public int numPoints = 25;
    public Material meshMaterial;
    public Material litMaterial;
    public Topology topology = Topology.Points;
    public bool enableRotation;
    public bool enableAnimation;

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

        if (prevAnim != enableAnimation)
        {
            prevAnim = enableAnimation;
            if(enableAnimation)
                StartCoroutine(ChangePointNum());
        }
    }

    IEnumerator ChangePointNum()
    {
        while (enableAnimation)
        {
            if (numPoints > 49)
            {
                inc = false;
                if(numPoints == prevPoints)
                    numPoints--;
            }
            else if (numPoints < 3)
            {
                inc = true;
                if(numPoints == prevPoints)
                    numPoints++;
            }

            if (inc)    numPoints++;
            else        numPoints--;
            
            yield return new WaitForSeconds(0.2f);
        }

        yield return null;
    }

    void DrawMesh()
    {
        sphereMesh.Clear();

        // set vertices
        var spherePoints = new List<Vector3>();
        
        for( int k=0; k <= numPoints; k++ )
        {
            float theta=Mathf.PI*k/numPoints, c_t=Mathf.Cos(theta), s_t=Mathf.Sin(theta);
            for (int l = 0; l < 2 * numPoints; l++)
            {
                float phi= Mathf.PI * 2.0f * l / (2 * numPoints), c_p = Mathf.Cos(phi), s_p = Mathf.Sin(phi);
                spherePoints.Add(new Vector3(s_t * c_p, c_t, s_t * s_p));
            }
        }

        sphereMesh.SetVertices(spherePoints);
        
        // set indices
        if (topology == Topology.Points) // points
        {
            var spherePointIndices = new List<int>();
            for (int i = 0; i < spherePoints.Count; i++)
            {
                spherePointIndices.Add(i);
            }
            sphereMesh.SetIndices(spherePointIndices, MeshTopology.Points, 0);
        }
        else if (topology == Topology.Lines) // lines
        {
            var sphereLineIndices = new List<int>();
            
            for (int i = 0; i < numPoints; i++)
            {
                var k1 = i  * 2 * numPoints;
                var k2 = k1 + 2 * numPoints;

                for (var j = 0; j < 2 * numPoints; j++)
                {
                    if (i != 0) {
                        sphereLineIndices.Add(k1);
                        sphereLineIndices.Add(k1 + 1);
                        sphereLineIndices.Add(k1 + 1);
                        sphereLineIndices.Add(k2);
                    }

                    if (i != numPoints - 1) {
                        sphereLineIndices.Add(k1 + 1);
                        sphereLineIndices.Add(k2 + 1);
                        sphereLineIndices.Add(k2 + 1);
                        sphereLineIndices.Add(k2);
                    }

                    k1++;
                    k2++;
                }
                sphereLineIndices.Add(0);
                sphereLineIndices.Add(2 * numPoints);
                sphereLineIndices.Add(2 * numPoints);
                sphereLineIndices.Add(4 * numPoints - 1);
            }

            sphereMesh.SetIndices(sphereLineIndices, MeshTopology.Lines, 0);
        }
        else if(topology == Topology.Triangles) // triangles
        {
            List<Vector3> normals = new List<Vector3>();
            for( int k=0; k <= numPoints; k++ )
            {
                float theta=Mathf.PI*k/numPoints, c_t=Mathf.Cos(theta), s_t=Mathf.Sin(theta);
                for (int l = 0; l < 2 * numPoints; l++)
                {
                    float phi= Mathf.PI * 2.0f * l / (2 * numPoints), c_p = Mathf.Cos(phi), s_p = Mathf.Sin(phi);
                    spherePoints.Add(new Vector3(s_t * c_p, c_t, s_t * s_p));
                }
            }
            sphereMesh.SetNormals(normals);
            
            List<int> indices = new List<int>();
            
            for (int i = 0; i < numPoints; i++)
            {
                var k1 = i  * 2 * numPoints;
                var k2 = k1 + 2 * numPoints;

                for (var j = 0; j < 2 * numPoints; j++)
                {
                    if (i != 0) {
                        indices.Add(k1);
                        indices.Add(k1 + 1);
                        indices.Add(k2);
                    }

                    if (i != numPoints - 1) {
                        indices.Add(k1 + 1);
                        indices.Add(k2 + 1);
                        indices.Add(k2);
                    }

                    k1++;
                    k2++;
                }
            }
            indices.Add(0);
            indices.Add(2 * numPoints);
            indices.Add(4 * numPoints - 1);

            sphereMesh.SetIndices(indices, MeshTopology.Triangles, 0);
        }

        meshFilter.mesh = sphereMesh;
        meshRenderer.material = meshMaterial;
        if (topology == Topology.Triangles)
            meshRenderer.material = litMaterial;
    }
}