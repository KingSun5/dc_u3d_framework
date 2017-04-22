using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;

/// <summary>
/// 目的：当场景里的物体保存成Prefab之后，LightMap的信息就会丢失。
/// 所以最后就写了个脚本，把每个Render里的lightmap纪录下来，当prefab实例化之后，重新设置
/// 使用方式：当场景中LightMap烘培好了之后，在perfab根节点上面挂上这个脚本，右键－》SaveData，之后apply prefab。
/// 在运行时场景初始化完成之后。调用Setup方法，即可挂载lightMap。
/// @author hannibal
/// @time 2016-10-8
/// </summary>
public class PrefabLightmapScript : MonoBehaviour
{
    //LightMap信息  
    [System.Serializable]
    public struct RendererInfo
    {
        public Renderer renderer;
        public int lightmapIndex;
        public Vector4 lightmapOffsetScale;
    }

    //场景中的Fog信息  
    [System.Serializable]
    public struct FogInfo
    {
        public bool fog;
        public FogMode fogMode;
        public Color fogColor;
        public float fogStartDistance;
        public float fogEndDistance;
        public float fogDensity;
    }


    public FogInfo fogInfo;
    public List<RendererInfo> m_RendererInfo;
    public List<Texture2D> lightmapNear;
    public List<Texture2D> lightmapFar;
    public LightmapData[] lightmapData;
    public LightmapsMode lightmapsMode;

    //地形的LightMap信息  
    public Terrain terrain;
    public RendererInfo terrainRendererInfo;

    //设置光照信息  
    public void Setup()
    {
        lightmapData = new LightmapData[lightmapNear.Count > lightmapFar.Count ? lightmapNear.Count : lightmapFar.Count];
        Log.Debug("PrefabLightmapScript::Setup lightmapData length:" + lightmapData.Length);
        for (int i = 0; i < lightmapData.Length; i++)
        {
            lightmapData[i] = new LightmapData();
            lightmapData[i].lightmapColor = i < lightmapFar.Count ? lightmapFar[i] : null;
            lightmapData[i].lightmapDir = i < lightmapNear.Count ? lightmapNear[i] : null;
        }
        LightmapSettings.lightmapsMode = lightmapsMode;
        LightmapSettings.lightmaps = lightmapData;
        LoadLightmap();
        RenderSettings.fog = fogInfo.fog;
        RenderSettings.fogMode = fogInfo.fogMode;
        RenderSettings.fogColor = fogInfo.fogColor;
        RenderSettings.fogStartDistance = fogInfo.fogStartDistance;
        RenderSettings.fogEndDistance = fogInfo.fogEndDistance;
        RenderSettings.fogDensity = fogInfo.fogDensity;
    }
    private void LoadLightmap()
    {
        Log.Debug("PrefabLightmapScript::LoadLightmap RendererInfo Count:" + m_RendererInfo.Count);
        if (m_RendererInfo.Count <= 0) return;

        if (terrain != null)
        {
            terrain.lightmapScaleOffset = terrainRendererInfo.lightmapOffsetScale;
            terrain.lightmapIndex = terrainRendererInfo.lightmapIndex;
        }

        foreach (var item in m_RendererInfo)
        {
            item.renderer.lightmapIndex = item.lightmapIndex;
            item.renderer.lightmapScaleOffset = item.lightmapOffsetScale;
        }
    }

    //保存光照信息  
    [ContextMenu("SaveData")]
    public void Build()
    {
        SaveLightmap();
    }
    private void SaveLightmap()
    {
        fogInfo = new FogInfo();
        fogInfo.fog = RenderSettings.fog;
        fogInfo.fogMode = RenderSettings.fogMode;
        fogInfo.fogColor = RenderSettings.fogColor;
        fogInfo.fogStartDistance = RenderSettings.fogStartDistance;
        fogInfo.fogEndDistance = RenderSettings.fogEndDistance;

        lightmapNear = new List<Texture2D>();
        lightmapFar = new List<Texture2D>();
        for (int i = 0; i < LightmapSettings.lightmaps.Length; i++)
        {
            LightmapData data = LightmapSettings.lightmaps[i];
            if (data.lightmapDir != null)
            {
                lightmapNear.Add(data.lightmapDir);
            }

            if (data.lightmapColor != null)
            {
                lightmapFar.Add(data.lightmapColor);
            }
        }
        m_RendererInfo = new List<RendererInfo>();
        var renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer r in renderers)
        {
            if (r.lightmapIndex != -1)
            {
                RendererInfo info = new RendererInfo();
                info.renderer = r;
                info.lightmapOffsetScale = r.lightmapScaleOffset;
                info.lightmapIndex = r.lightmapIndex;
                m_RendererInfo.Add(info);
            }
        }

        terrain = GetComponentInChildren<Terrain>();
        if (terrain != null)
        {
            terrainRendererInfo = new RendererInfo();
            terrainRendererInfo.lightmapOffsetScale = terrain.lightmapScaleOffset;
            terrainRendererInfo.lightmapIndex = terrain.lightmapIndex;
        }
        lightmapsMode = LightmapSettings.lightmapsMode;
    }
}