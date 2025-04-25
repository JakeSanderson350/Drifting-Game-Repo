using UnityEngine;

public class TempCamera : MonoBehaviour
{
    void Start()
    {
        Camera mainCamera = Camera.main;
        mainCamera.allowMSAA = false; // Disable multi-sampling anti-aliasing
        mainCamera.depthTextureMode = DepthTextureMode.Depth; // Enable depth texture

        // Adjust the z-buffer precision
        mainCamera.nearClipPlane = 0.01f; // Increase near clip plane for better z-precision
    }
}
