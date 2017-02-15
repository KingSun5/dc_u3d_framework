using UnityEngine;
using System.Collections;

/// <summary>
/// 管理器
/// @author hannibal
/// @time 2014-11-11
/// </summary>
public class EngineManager : Singleton<EngineManager> 
{
	public void Setup()
	{
        TimerManager.Instance.Setup();
		ObjectPoolsManager.Instance.Setup();
		ObjectFactoryManager.Instance.Setup();
		MultyBuildManager.Instance.Setup();
		UIManager.Instance.Setup();
		DropSimulationManager.Instance.Setup();
		SoundManager.Instance.Setup ();
		InputSimulateManager.Instance.Setup();
		InputSimulateManager.Instance.Enable = false;
        GameConnect.Instance.SetUp();
        HttpDownloadManager.Instance.Setup();
        TransformerManager.Instance.Setup();
	}
	
	public void Destroy()
	{
        TimerManager.Instance.Destroy();
		ObjectPoolsManager.Instance.Destroy();
		ObjectFactoryManager.Instance.Destroy();
		MultyBuildManager.Instance.Destroy();
		UIManager.Instance.Destroy();
		DropSimulationManager.Instance.Destroy();
		SoundManager.Instance.Destroy ();
		InputSimulateManager.Instance.Destroy();
        HttpDownloadManager.Instance.Destroy();
        GameConnect.Instance.Destroy();
        TransformerManager.Instance.Destroy();
	}
	
	public void Tick (float elapse, int game_frame)
    {
        CheckEscapeGame();

        TimerManager.Instance.Tick(elapse, game_frame);
		MultyBuildManager.Instance.Tick(elapse, game_frame);
		DropSimulationManager.Instance.Tick(elapse, game_frame);
		InputSimulateManager.Instance.Tick(elapse, game_frame);
        GameConnect.Instance.Tick(elapse, game_frame);
        TransformerManager.Instance.Tick(elapse, game_frame);
	}

    public void ProcessGC()
    {
        SpritePools.Clear();
        GameobjectPools.Clear();
        SoundManager.Instance.Clear();
        ResourceManager.Instance.ProcessGC();
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
                , LangManager.lang[5], LangManager.lang[6]);
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
