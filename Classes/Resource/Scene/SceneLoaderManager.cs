using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// u3d场景加载
/// @author hannibal
/// @time 2017-3-2
/// </summary>
public class SceneLoaderManagerr : Singleton<SceneLoaderManagerr>
{
    static public string LOAD_PROGRESS = "LOAD_PROGRESS";		//加载中
    static public string LOAD_COMPLATE = "LOAD_COMPLATE";		//完成

    private string m_CurScene = "";
    private AsyncOperation m_LoadAsync;

    public void Setup()
    {
        m_CurScene = "";
        m_LoadAsync = null;
    }

    public void Destroy()
    {
        RemoveScene();
    }

    public void Tick(float elapse, int game_frame)
    {
        if (m_LoadAsync == null) return;

        if (m_LoadAsync.isDone)
        {
            SetSceneActive(m_CurScene);
            EventDispatcher.TriggerEvent(LOAD_COMPLATE);
            m_LoadAsync = null;
        }
        else
        {
            EventDispatcher.TriggerEvent(LOAD_PROGRESS, m_LoadAsync.progress);
        }
    }

    public void LoadScene(string scene_name)
    {
        if (string.IsNullOrEmpty(scene_name)) return;

        ResourceLoaderManager.Instance.LoadScene(scene_name);
        m_CurScene = scene_name;
        EventDispatcher.TriggerEvent(LOAD_COMPLATE);
    }

    public void AsyncLoadScene(string scene_name)
    {
        if (string.IsNullOrEmpty(scene_name)) return;

        m_LoadAsync = ResourceLoaderManager.Instance.AsyncLoadScene(scene_name);
        m_CurScene = scene_name;
    }

    public void RemoveScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene != null)
        {
            SceneManager.UnloadScene(scene);
        }
    }

    private void SetSceneActive(string scene_name)
    {
        Scene scene = SceneManager.GetSceneByName(scene_name);
        SceneManager.SetActiveScene(scene);
    }
}
