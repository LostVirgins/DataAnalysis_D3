using UnityEngine;
using UnityEngine.Rendering;

public class VolumetricHeatmap : MonoBehaviour
{
    public ComputeShader heatmapComputeShader; // Reference to the compute shader
    public int textureSize = 256; // Size of the 3D texture
    public Texture3D heatmapTexture; // The 3D texture we will generate
    public Material visualizationMaterial; // Material for visualizing the texture

    private CommandBuffer commandBuffer; // Command buffer to handle compute shader dispatch

    void Start()
    {
        // Generate a 3D texture
        heatmapTexture = new Texture3D(textureSize, textureSize, textureSize, TextureFormat.RGBA32, false);
        heatmapTexture.filterMode = FilterMode.Trilinear;
        heatmapTexture.wrapMode = TextureWrapMode.Repeat;

        // Create the command buffer
        commandBuffer = new CommandBuffer { name = "HeatmapCommandBuffer" };

        // Dispatch the compute shader to fill the texture with heatmap data
        DispatchComputeShader();
    }

    void DispatchComputeShader()
    {
        int kernelHandle = heatmapComputeShader.FindKernel("CSMain");

        // Set the 3D texture as a target for the compute shader
        heatmapComputeShader.SetTexture(kernelHandle, "Result", heatmapTexture);

        // Use CommandBuffer to dispatch the compute shader
        commandBuffer.Clear();
        commandBuffer.DispatchCompute(heatmapComputeShader, kernelHandle, textureSize / 8, textureSize / 8, textureSize / 8);
        Graphics.ExecuteCommandBuffer(commandBuffer);
    }

    void OnRenderObject()
    {
        // Set the texture for the material to visualize
        visualizationMaterial.SetTexture("_VolumeTex", heatmapTexture);
    }

    void OnDestroy()
    {
        // Release the command buffer
        commandBuffer.Release();
    }
}
