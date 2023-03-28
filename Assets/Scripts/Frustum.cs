using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Camera))]
public class Frustum : MonoBehaviour
{
    private Mesh wireframeMesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Camera mainCam;
    
    public Material meshMaterial;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        wireframeMesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        DrawFrustum();
    }

    void DrawFrustum()
    {
        Vector3 camPosition = transform.position;
        var depth = 10 + mainCam.nearClipPlane;
        var xOffset = depth * Mathf.Tan(Mathf.Deg2Rad * mainCam.fieldOfView / 2);
        var yOffset = xOffset / mainCam.aspect;

        var nearClipPlaneVert = new Vector3[4];
        mainCam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), mainCam.nearClipPlane, Camera.MonoOrStereoscopicEye.Mono, nearClipPlaneVert);
        
        var farClipPlaneVert = new Vector3[4];
        mainCam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), mainCam.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, farClipPlaneVert);

        var frustumVert = new Vector3[8];
        nearClipPlaneVert.CopyTo(frustumVert, 0);
        farClipPlaneVert.CopyTo(frustumVert, 4);

        var idxBuf = new[]
        {
            0, 1,  1, 2,  2, 3,  3, 0, // near clip plane
            0, 4,  1, 5,  2, 6,  3, 7, // side
            4, 5,  5, 6,  6, 7,  7, 4  // far clip plane 
        };

        wireframeMesh.SetVertices(frustumVert);
        wireframeMesh.SetIndices(idxBuf, MeshTopology.Lines, 0);
        
        meshFilter.mesh = wireframeMesh;
        meshRenderer.material = meshMaterial;
    }
}
