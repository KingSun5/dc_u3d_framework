using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// 资源加载
/// @author hannibal
/// @time 2014-10-22
/// </summary>
public class ResourceLoaderManager : Singleton<ResourceLoaderManager>
{
    private bool m_EnableLog = false;
    private bool m_EnablePools = true;
    private Dictionary<string, Object> m_DicFile2Pool = new Dictionary<string, Object>();

    #region 加载
    /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～加载～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
	public Object Load(string path)
	{
        Object obj = null;
        if(m_EnablePools)m_DicFile2Pool.TryGetValue(path, out obj);
		if(obj == null)
        {
            float time = Time.realtimeSinceStartup;
            obj = Resources.Load(path);
            if (obj == null)Log.Error("[res]load failed：" + path);
            else if (m_EnablePools)
                m_DicFile2Pool[path] = obj;
            if (m_EnableLog) Log.Debug("[load]load resource:" + path + " Time:" + (Time.realtimeSinceStartup - time));
        }
        return obj;
	}
    public ResourceRequest LoadAsync(string path)
    {
        ResourceRequest req = Resources.LoadAsync(path);
        if (req == null)Log.Error("[res]async load failed：" + path);
        return req;
    }
    /// <summary>
    /// 文本
    /// </summary>
    public TextAsset LoadTextAsset(string path)
    {
        float time = Time.realtimeSinceStartup;
        TextAsset textAsset = Load(path) as TextAsset;
        if (m_EnableLog) Log.Debug("[load]load resource:" + path + " Time:" + (Time.realtimeSinceStartup - time));
		return textAsset;
	}
    /// <summary>
    /// 场景
    /// </summary>
    public void LoadScene(string path, LoadSceneMode mode = LoadSceneMode.Single)
    {
        float time = Time.realtimeSinceStartup;
        UnityEngine.SceneManagement.SceneManager.LoadScene(path);
        if (m_EnableLog) Log.Debug("[load]load scene:" + path + " Time:" + (Time.realtimeSinceStartup - time));
    }
    public AsyncOperation AsyncLoadScene(string path, LoadSceneMode mode = LoadSceneMode.Single)
    {
        float time = Time.realtimeSinceStartup;
        AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(path);
        if (m_EnableLog) Log.Debug("[load]async load scene:" + path + " Time:" + (Time.realtimeSinceStartup - time));
        return async;
    }
    /// <summary>
    /// sprite
    /// </summary>
    public Sprite LoadSprite(string path)
    {
        float time = Time.realtimeSinceStartup;
        Sprite res = Resources.Load<Sprite>(path);
        if (m_EnableLog)Log.Debug("[load]load sprite:" + path + " Time:" + (Time.realtimeSinceStartup - time));
        return res;
	}
    public Sprite[] LoadAllSprite(string path)
    {
        float time = Time.realtimeSinceStartup;
        Sprite[] res = Resources.LoadAll<Sprite>(path);
        if (m_EnableLog) Log.Debug("[load]load resource:" + path + " Time:" + (Time.realtimeSinceStartup - time));
        return res;
    }
    /// <summary>
    /// 声音
    /// </summary>
    public AudioClip LoadSound(string path)
    {
        Object obj = null;
        if (m_EnablePools) m_DicFile2Pool.TryGetValue(path, out obj);
        if (obj == null)
        {
            float time = Time.realtimeSinceStartup;
            obj = Resources.Load<AudioClip>(path);
            if (obj == null) Log.Error("[res]load sound failed：" + path);
            else if (m_EnablePools)
                m_DicFile2Pool[path] = obj;
            if (m_EnableLog) Log.Debug("[load]load resource:" + path + " Time:" + (Time.realtimeSinceStartup - time));
        }

        return obj as AudioClip;
    }
    public ResourceRequest LoadAsyncSound(string path)
    {
        ResourceRequest req = Resources.LoadAsync<AudioClip>(path);
        if (req == null) Log.Error("[res]async load sound failed：" + path);
        return req;
    }
    /// <summary>
    /// 贴图
    /// </summary>
    public Texture LoadTexture(string path)
    {
        Texture res = Load(path) as Texture;
        return res;
    }
    /// <summary>
    /// 材质
    /// </summary>
    public Material LoadMaterial(string path)
    {
        Material res = Load(path) as Material;
        return res;
    }

    public Shader LoadShader(string path)
    {
        Shader res = Load(path) as Shader;
        return res;
    }
    /// <summary>
    /// 根据文件名获取资源
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public Object GetResource(string path)
    {
        if (!m_EnablePools) return null;
        Object obj = null;
        m_DicFile2Pool.TryGetValue(path, out obj);
        return obj;
    }

    public void AddResource(string path, Object res)
    {
        if (!m_EnablePools) return;
        if (path.Length == 0 || res == null) return;
        if(!m_DicFile2Pool.ContainsKey(path))
        {
            m_DicFile2Pool[path] = res;
            //Log.Debug("[load]add res to pools:" + path);
        }
    }
    #endregion

    #region 释放
    /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～释放～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
    public void UnloadAsset(Object obj)
    {
        Resources.UnloadAsset(obj);
    }
    public void UnloadUnusedAssets()
    {
        Resources.UnloadUnusedAssets();
    }
    public void Clear()
    {
        m_DicFile2Pool.Clear();
    }
    #endregion

    public bool EnableLog
    {
        set { m_EnableLog = value; }
    }
}
