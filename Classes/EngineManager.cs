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
	}
	
	public void Tick (float elapse, int game_frame) 
	{
        TimerManager.Instance.Tick(elapse, game_frame);
		MultyBuildManager.Instance.Tick(elapse, game_frame);
		DropSimulationManager.Instance.Tick(elapse, game_frame);
		InputSimulateManager.Instance.Tick(elapse, game_frame);
        GameConnect.Instance.Tick(elapse,game_frame);
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
