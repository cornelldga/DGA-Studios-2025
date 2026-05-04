using UnityEngine;

[ExecuteAlways]
public class DustCameraBinder : MonoBehaviour
{
    void LateUpdate()
    {
        Vector3 p = Camera.main.transform.position;
        Shader.SetGlobalVector("_CameraWorldPos", new Vector4(p.x, p.y, 0, 0));
    }
}