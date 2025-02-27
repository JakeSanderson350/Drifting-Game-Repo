using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class EdgeDetectionPostProcessor : MonoBehaviour
{
    public Color edgeColor = Color.black;
    [Range(0.0f, 1.0f)] public float threshold = 0.1f;

    private Material edgeDetectionMaterial;

    void OnEnable()
    { 
        Shader edgeDetectionShader = Shader.Find("Custom/EdgeDetection");
        if (edgeDetectionShader != null)
        {
            edgeDetectionMaterial = new Material(edgeDetectionShader);
        }
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (edgeDetectionMaterial != null)
        {
            // Make sure the shader accesses the main texture
            edgeDetectionMaterial.SetTexture("_MainTex", src);
            edgeDetectionMaterial.SetColor("_EdgeColor", edgeColor);
            edgeDetectionMaterial.SetFloat("_Threshold", threshold);

            // Blit with the edge detection shader applied
            Graphics.Blit(src, dest, edgeDetectionMaterial);
        }
        else
        {
            // If no material, just blit normally (just copy the screen texture)
            Graphics.Blit(src, dest);
        }
    }

}
