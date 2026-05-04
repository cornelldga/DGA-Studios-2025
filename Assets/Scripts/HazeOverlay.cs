using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshRenderer))]
public class DustOverlay : MonoBehaviour
{
    public Camera targetCamera;
    public float distanceFromCamera = 1f;
    
    [Header("Sorting")]
    public string sortingLayerName = "Default";
    public int orderInLayer = 32000;  

    Material _mat;
    MeshRenderer _renderer;
    static readonly int CamPosID = Shader.PropertyToID("_CameraWorldPos");

    void OnEnable()
    {
        if (targetCamera == null) targetCamera = Camera.main;
        _renderer = GetComponent<MeshRenderer>();
        _mat = Application.isPlaying ? _renderer.material : _renderer.sharedMaterial;
        
        ApplySorting();
    }
    
    void ApplySorting()
    {
        if (_renderer == null) return;
        _renderer.sortingLayerName = sortingLayerName;
        _renderer.sortingOrder = orderInLayer;
    }

    void LateUpdate()
    {
        if (targetCamera == null || _mat == null) return;

        ApplySorting();

        transform.position = targetCamera.transform.position 
                           + targetCamera.transform.forward * distanceFromCamera;
        transform.rotation = targetCamera.transform.rotation;

        if (targetCamera.orthographic)
        {
            float h = targetCamera.orthographicSize * 4f;
            float w = h * targetCamera.aspect;
            transform.localScale = new Vector3(w, h, 1f);
        }
        else
        {
            float h = 2f * distanceFromCamera * Mathf.Tan(targetCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            float w = h * targetCamera.aspect;
            transform.localScale = new Vector3(w, h, 1f);
        }

        Vector3 p = targetCamera.transform.position;
        _mat.SetVector(CamPosID, new Vector4(p.x, p.y, 0, 0));
    }
}