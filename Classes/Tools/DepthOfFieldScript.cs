using UnityEngine;
using System.Collections;

public class DepthOfFieldScript : MonoBehaviour
{
    // 景深shader  
    public Shader dofShader;
    private Material dofMat = null;

    public Shader blurShader;
    private Material blurMat = null;

    private float onePixelWidth = 1.0f / 512.0f;
    private float onePixelHeight = 1.0f / 512.0f;

    void Start()
    {
        blurMat = new Material(blurShader);
        dofMat = new Material(dofShader);
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // 创建【临时的屏幕纹理】  
        RenderTexture blurRT = RenderTexture.GetTemporary(source.width, source.height, 16);
        // 将【临时的屏幕纹理】通过【模糊shader】变换为【模糊纹理】  
        blurMat.SetVector("offsets", new Vector4(onePixelWidth, onePixelWidth, 0.0f, 0.0f));
        Graphics.Blit(source, blurRT, blurMat);
        blurMat.SetVector("offsets", new Vector4(onePixelHeight, 0.0f, 0.0f, 0.0f));
        Graphics.Blit(blurRT, blurRT, blurMat);

        // 将【模糊纹理】的值传到【景深shader】中进行处理  
        dofMat.SetTexture("_BlurTex", blurRT);
        // 将【屏幕纹理】通过【景深shader】变换为【最终纹理】  
        Graphics.Blit(source, destination, dofMat);
        // 释放临时纹理  
        RenderTexture.ReleaseTemporary(blurRT);
    }
}