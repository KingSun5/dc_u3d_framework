using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 全屏泛光
/// </summary>
public class MoblieBloomScript : MonoBehaviour
{
	public Shader BloomShader = null;
	private Material BloomMaterial = null;
	private RenderTextureFormat rtFormat = RenderTextureFormat.Default;

	public Color colorMix = new Color(1.0f, 1.0f, 1.0f, 1.0f);

	[Range(0.0f, 1.0f)]
    public float threshold = 0.25f;

    [Range(0.0f, 2.5f)]
    public float intensity = 0.75f;

    [Range(0.2f, 1.0f)]
    public float BlurSize = 1.0f;

	void Start () 
    {
		CheckSupport ();
		CreateMaterials ();
	}

	void CreateMaterials()
    {
		if(!BloomMaterial)
        {
			BloomMaterial = new Material(BloomShader);
			BloomMaterial.hideFlags = HideFlags.HideAndDontSave;	
		}
	}

	bool CheckSupport() 
    {
        if (!(SystemInfo.supportsImageEffects && SystemInfo.supportsRenderTextures && BloomShader.isSupported))
        {
			enabled = false;
			return false;
		}
		rtFormat = SystemInfo.SupportsRenderTextureFormat (RenderTextureFormat.RGB565) ? RenderTextureFormat.RGB565 : RenderTextureFormat.Default;
		return true;
	}


	void OnRenderImage (RenderTexture sourceTexture, RenderTexture destTexture)
	{	
		#if UNITY_EDITOR
			CheckSupport ();
			CreateMaterials ();	
		#endif

		if(threshold != 0 && intensity != 0)
        {
			int rtW = sourceTexture.width/4;
	        int rtH = sourceTexture.height/4;
	
	        BloomMaterial.SetColor ("_ColorMix", colorMix);
	        BloomMaterial.SetVector ("_Parameter", new Vector4(BlurSize*1.5f, 0.0f, intensity,0.8f - threshold));	
	        // material.SetFloat("_blurSize",BlurSize);

	        RenderTexture rtTempA = RenderTexture.GetTemporary (rtW, rtH, 0,rtFormat);
            rtTempA.filterMode = FilterMode.Bilinear;

            RenderTexture rtTempB = RenderTexture.GetTemporary (rtW, rtH, 0,rtFormat);
            rtTempA.filterMode = FilterMode.Bilinear;

            Graphics.Blit (sourceTexture, rtTempA,BloomMaterial,0);


            Graphics.Blit (rtTempA, rtTempB, BloomMaterial,1);
            RenderTexture.ReleaseTemporary(rtTempA);
 

            rtTempA = RenderTexture.GetTemporary (rtW, rtH, 0, rtFormat);
            rtTempB.filterMode = FilterMode.Bilinear;
            Graphics.Blit (rtTempB, rtTempA, BloomMaterial,2);


            BloomMaterial.SetTexture ("_Bloom", rtTempA);
            Graphics.Blit (sourceTexture, destTexture, BloomMaterial,3);
       

            RenderTexture.ReleaseTemporary(rtTempA);
            RenderTexture.ReleaseTemporary(rtTempB);
		}
		else
        {
			Graphics.Blit(sourceTexture, destTexture);
			
		}
	}
	
	 public void OnDisable ()
     {
        if (BloomMaterial)
            DestroyImmediate (BloomMaterial);
            // BloomMaterial = null;
    }
}
