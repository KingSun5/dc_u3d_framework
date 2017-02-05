using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 声音对象池
/// @author hannibal
/// @time 2016-12-16
/// </summary>
public class AudioPools : MonoBehaviour
{
    public static AudioPools instance;

    public Transform SceneAudioSourcePrefab;    // 场景音效
    public Transform AudioParentNode = null;

    private Dictionary<string, List<AudioSource>> m_DicFile2AudioPool;
    private Dictionary<AudioClip, List<AudioSource>> m_DicClip2AudioPool;

    void Awake()
    {
        if (instance != null) Log.Exception("实例已经存在:AudioPools");
        instance = this;

        m_DicFile2AudioPool = new Dictionary<string, List<AudioSource>>();
        m_DicClip2AudioPool = new Dictionary<AudioClip, List<AudioSource>>();
    }

    /// <summary>
    /// 产生声音
    /// </summary>
    /// <param name="file"></param>
    /// <param name="pos"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public AudioSource SpawnAudioByFile(string file, Vector3 pos, Transform parent)
    {
        if (file.Length == 0) return null;

        AudioSource spawnItem = null;
        List<AudioSource> itemArray = null;
        ///1.查找pools
        if (m_DicFile2AudioPool.TryGetValue(file, out itemArray))
        {
            for (int i = 0; i < itemArray.Count; ++i)
            {
                var item = itemArray[i];
                if (item != null && !item.gameObject.activeSelf)
                {
                    spawnItem = item;
                    break;
                }
            }
        }
        ///2.创建新的
        if (spawnItem == null)
        {
            AudioClip clip = ResourceLoaderManager.Instance.LoadSound(file);
            if (clip == null)
            {
                Log.Error("声音加载失败：" + file);
                return null;
            }
            spawnItem = ((Transform)Instantiate(SceneAudioSourcePrefab, Vector3.zero, Quaternion.identity)).GetComponent<AudioSource>();
            spawnItem.clip = clip;
            if (!m_DicFile2AudioPool.TryGetValue(file, out itemArray))
            {
                itemArray = new List<AudioSource>();
                m_DicFile2AudioPool.Add(file, itemArray);
            }
            itemArray.Add(spawnItem);
        }
        spawnItem.transform.SetParent(parent == null ? AudioParentNode : parent, false);
        spawnItem.transform.localPosition = pos;
        spawnItem.gameObject.SetActive(true);

        return spawnItem;
    }
    /// <summary>
    /// 产生声音
    /// </summary>
    public AudioSource SpawnAudioByClip(AudioClip clip, Vector3 pos, Transform parent = null)
    {
        if (clip == null) return null;

        AudioSource spawnItem = null;
        List<AudioSource> itemArray = null;
        ///1.查找pools
        if (m_DicClip2AudioPool.TryGetValue(clip, out itemArray))
        {
            for (int i = 0; i < itemArray.Count; ++i)
            {
                var item = itemArray[i];
                if (item != null && !item.gameObject.activeSelf)
                {
                    spawnItem = item;
                    break;
                }
            }
        }
        ///2.创建新的
        if (spawnItem == null)
        {
            spawnItem = ((Transform)Instantiate(SceneAudioSourcePrefab, Vector3.zero, Quaternion.identity)).GetComponent<AudioSource>();
            spawnItem.clip = clip;
            if (!m_DicClip2AudioPool.TryGetValue(clip, out itemArray))
            {
                itemArray = new List<AudioSource>();
                m_DicClip2AudioPool.Add(clip, itemArray);
            }
            itemArray.Add(spawnItem);
        }
        spawnItem.transform.SetParent(parent == null ? AudioParentNode : parent, false);
        spawnItem.transform.localPosition = pos;
        spawnItem.gameObject.SetActive(true);

        return spawnItem;
    }
    public void DespawnAudio(Transform obj)
    {
        obj.SetParent(AudioParentNode);
        obj.gameObject.SetActive(false);
    }
    public void StopAll()
    {
        foreach(var list in m_DicFile2AudioPool)
        {
            foreach(var obj in list.Value)
            {
                DespawnAudio(obj.transform);
            }
        }
    }
    /// <summary>
    /// 清除所有音效
    /// </summary>
    public void Clear()
    {
        foreach (var list in m_DicFile2AudioPool)
        {
            foreach (var obj in list.Value)
            {
                GameObject.Destroy(obj.gameObject);
            }
        }
        m_DicFile2AudioPool.Clear();
    }
}
