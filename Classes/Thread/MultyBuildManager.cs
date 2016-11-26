using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public struct SBuildObjectInfo
{
	public delegate void FunComplate(Object obj);
	public string mResName;
	public int mFrameOffset;
	public FunComplate OnComplate;

	public SBuildObjectInfo(string res, FunComplate fun, int frame = 1)
	{
		mResName = res;
		OnComplate = fun;
		mFrameOffset = frame;
	}
}

/// <summary>
/// 分帧构建
/// @author hannibal
/// @time 2014-12-1
/// </summary>
public class MultyBuildManager : Singleton<MultyBuildManager>
{
	private LinkedList<SBuildObjectInfo> m_ListBuildObject;
	private int m_MaxOffsetFrame = 1;
	private int m_CurOffsetFrame = 1;
	private int m_CurFrame = 0;

	public MultyBuildManager()
	{
		m_ListBuildObject = new LinkedList<SBuildObjectInfo>();
	}

	public void Setup()
	{

	}

	public void Destroy()
	{
		m_ListBuildObject.Clear();
	}

	public void Tick(float elapse, int game_frame)
	{
		++m_CurFrame;
		if(m_ListBuildObject.Count > 0 && m_CurFrame%m_CurOffsetFrame == 0)
		{
			BuildOne();
			m_CurFrame = 0;
		}
	}

	public void Add(SBuildObjectInfo info)
	{
		if(info.mResName.Length == 0)
		{
			Log.Error("MultyBuildManager::Add param error");
			return;
		}
		m_ListBuildObject.AddLast(info);

		if(info.mFrameOffset == 0 || (m_ListBuildObject.Count == 1 && m_CurFrame >= info.mFrameOffset))
		{
			BuildOne();
			m_CurFrame = 0;
		}
	}

	public void ClearAll()
	{
        m_ListBuildObject.Clear();
        m_CurFrame = 0;
	}

	private void BuildOne()
	{
		SBuildObjectInfo info = m_ListBuildObject.First.Value;
		m_ListBuildObject.RemoveFirst();
		ProcessBuild(info);

		//修改加载间隔
		if(m_ListBuildObject.Count > 0 && m_ListBuildObject.First.Value.mFrameOffset > 0)
		{
			m_CurOffsetFrame = m_ListBuildObject.First.Value.mFrameOffset;
		}
		else
		{
			m_CurOffsetFrame = m_MaxOffsetFrame;
		}
	}

	private void ProcessBuild(SBuildObjectInfo info)
	{
		Object res = ResourceLoaderManager.Instance.Load(info.mResName);
		if(res == null)
		{
			Log.Error("MultyBuildManager::ProcessBuild - not build file:" + info.mResName);
			return;
		}
		Object obj = GameObject.Instantiate(res);
		if(info.OnComplate != null)
		{
			info.OnComplate(obj);
		}
	}

	public int MaxOffsetFrame
	{
		set 
		{
			m_MaxOffsetFrame = value; 
			m_CurOffsetFrame = m_MaxOffsetFrame;
		}
	}
}
