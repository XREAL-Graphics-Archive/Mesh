using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireframeRenderer : MonoBehaviour
{
    void Start()
    {
        SkinnedMeshRenderer skinnedMesh = GetComponent<SkinnedMeshRenderer>();
        var indices = skinnedMesh?.sharedMesh.GetIndices(0);
        List<int> lineIndices = new List<int>();
        for (int i = 0; i < indices.Length / 3; i++)
        {
            lineIndices.Add(indices[3 * i]);
            lineIndices.Add(indices[3 * i + 1]);
            lineIndices.Add(indices[3 * i + 1]);
            lineIndices.Add(indices[3 * i + 2]);
            lineIndices.Add(indices[3 * i + 2]);
            lineIndices.Add(indices[3 * i]);
        }
        skinnedMesh?.sharedMesh.SetIndices(lineIndices, MeshTopology.Lines, 0);
    }
}
