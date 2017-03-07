using UnityEngine;
using System.Collections;

/// <summary>
/// 管理器
/// @author hannibal
/// @time 2014-11-11
/// </summary>
public class EngineManager : Singleton<EngineManager> 
{
    public void Setup(int default_screen_w, int default_screen_h, int max_screen_w=1920)
    {
        SetResolution(default_screen_w, default_screen_h, max_screen_w);

        TimerManager.Instance.Setup();
		ObjectFactoryManager.Instance.Setup();
		MultyBuildManager.Instance.Setup();
        ResourceManager.Instance.Setup();
		UIManager.Instance.Setup();
		DropSimulationManager.Instance.Setup();
		SoundManager.Instance.Setup ();
		InputSimulateManager.Instance.Setup();
		InputSimulateManager.Instance.Enable = false;
        GameConnect.Instance.SetUp();
        HttpDownloadManager.Instance.Setup();
        TransformerManager.Instance.Setup();
        AssetBundleManager.Instance.Setup();
	}
	
	public void Destroy()
	{
        TimerManager.Instance.Destroy();
		ObjectFactoryManager.Instance.Destroy();
        MultyBuildManager.Instance.Destroy();
        ResourceManager.Instance.Destroy();
		UIManager.Instance.Destroy();
		DropSimulationManager.Instance.Destroy();
		SoundManager.Instance.Destroy ();
		InputSimulateManager.Instance.Destroy();
        HttpDownloadManager.Instance.Destroy();
        GameConnect.Instance.Destroy();
        TransformerManager.Instance.Destroy();
        AssetBundleManager.Instance.Destroy();
	}
	
	public void Tick (float elapse, int game_frame)
    {
        CheckEscapeGame();

        TimerManager.Instance.Tick(elapse, game_frame);
        ResourceManager.Instance.Tick(elapse, game_frame);
		MultyBuildManager.Instance.Tick(elapse, game_frame);
		DropSimulationManager.Instance.Tick(elapse, game_frame);
		InputSimulateManager.Instance.Tick(elapse, game_frame);
        GameConnect.Instance.Tick(elapse, game_frame);
        TransformerManager.Instance.Tick(elapse, game_frame);
        AssetBundleManager.Instance.Tick(elapse, game_frame);
	}

    public void ProcessGC()
    {
        SpritePools.Clear();
        GameobjectPools.Clear();
        SoundManager.Instance.Clear();
        ResourceManager.Instance.ProcessGC();
    }
    /// <summary>
    /// 设置最大分辨率
    /// </summary>
    private void SetResolution(int default_screen_w, int default_screen_h, int max_screen_w)
    {
        UIID.DEFAULT_WIDTH = default_screen_w;
        UIID.DEFAULT_HEIGHT = default_screen_h;

        int screen_w = Screen.width;
        int screen_h = Screen.height;
        int w = max_screen_w;
        int h = w * screen_h / screen_w;
        if (w <= screen_w && h <= screen_h)
        {
            SetResolution(w, h);
            screen_w = w; screen_h = h;
        }
        else
        {
            SetResolution(screen_w, screen_h);
        }
    }
    void SetResolution(int w, int h)
    {
#if UNITY_STANDALONE && !UNITY_EDITOR
#else
        Screen.SetResolution(w, h, true);
#endif
    }
    /// <summary>
    /// 退出游戏
    /// </summary>
    static private int m_ClickCounts = 0;
    static private float m_LastClickTick = 0;
    static private void CheckEscapeGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (m_ClickCounts == 0)
            {
                m_LastClickTick = Time.time;
            }
            if (Time.time - m_LastClickTick >= 1.0f)
            {
                m_ClickCounts = 0;
                m_LastClickTick = Time.time;
            }
            m_ClickCounts++;

            // 点击两次才有反应
            if (m_ClickCounts >= 2)
            {
                m_ClickCounts = 0;
                AlertManager.Instance.ShowConfirm((int)eInternalUIID.ID_ALERT, LangManager.lang[4], (eAlertBtnType type) =>
                {
                    if (type == eAlertBtnType.OK)
                        Application.Quit();
                }
                , LangManager.lang[8], LangManager.lang[9]);
            }
        }
    }
	//～～～～～～～～～～～～～～～～～～～～～～～暂停～～～～～～～～～～～～～～～～～～～～～～～//
    private void OnPauseGame(GameEvent evt)
	{
		//bool is_pause = (bool)info;
	}

	public void HandlePauseGame(bool is_pause)
	{
		//Time.timeScale = is_pause ? 0f : 1f;
	}
}
