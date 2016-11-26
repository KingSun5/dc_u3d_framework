using UnityEngine;
using System.Collections;

/// <summary>
/// 资源加载
/// @author hannibal
/// @time 2014-10-22
/// </summary>
public class ResourceLoaderManager : Singleton<ResourceLoaderManager>
{
	public Object Load(string path)
	{
		return Resources.Load (path);
	}
	public void UnloadAsset(Object obj)
	{
		Resources.UnloadAsset(obj);
	}
    /// <summary>
    /// 文本
    /// </summary>
	public TextAsset LoadTextAssetInResources(string textName)
	{
		TextAsset textAsset = Load(textName) as TextAsset;
		return textAsset;
	}
    /// <summary>
    /// 场景
    /// </summary>
	public AsyncOperation LoadScene(string scene_name)
	{
		return Application.LoadLevelAsync (scene_name);
	}
    /// <summary>
    /// sprite
    /// </summary>
	public Sprite LoadSprite(string name)
	{
		return Resources.Load<Sprite> (name);
	}
    public Sprite[] LoadAllSprite(string path)
    {
        return Resources.LoadAll<Sprite>(path);
    }
    /// <summary>
    /// 特效
    /// </summary>
    public Object LoadEffect(string name)
    {
        return Resources.Load(name);
    }
    /// <summary>
    /// 子弹、枪口特效、命中特效
    /// </summary>
    public Object LoadShot(string name)
    {
        return Resources.Load(name);
    }
    /// <summary>
    /// 声音
    /// </summary>
    public AudioClip LoadSount(string name)
    {
        return Resources.Load<AudioClip>(name);
    }
    /// <summary>
    /// 贴图
    /// </summary>
    public Texture LoadTexture(string name)
    {
        return Resources.Load(name) as Texture;
    }
}
