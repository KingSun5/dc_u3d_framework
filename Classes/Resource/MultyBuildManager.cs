using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 资源加载类型
/// </summary>
public enum eResourceType
{
    UNDEFIED = 0,
    PREFAB,         //预制件
    SPRITE,         //2d图片
    TEXTURE,        //纹理
    MATERIAL,       //材质
    SHADER,         //着色器
    SOUND,          //声音：mp3、ogg、wav
//     MOVIE,          //视频，电影： .mov、.mpg、 .mpeg、.mp4、.avi、.asf
    SCENE,          //AsyncOperation场景类不继承Object，所以暂时没写，以后可加上，但在分帧构建上用处不大，用协程更好
}

public struct SBuildObjectInfo
{
	public delegate void FunComplate(Object obj);
	public string mResPath;
    public eResourceType Type;   //资源类型
	public int mFrameOffset;
	public FunComplate OnComplate;

    public SBuildObjectInfo(string res, FunComplate fun, int frame = 1, eResourceType resType = eResourceType.UNDEFIED)
	{
		mResPath = res;
		OnComplate = fun;
		mFrameOffset = frame;
        Type = resType;
	}
}

/// <summary>
/// 分帧构建
/// @author hannibal
/// @time 2014-12-1
/// </summary>
public class MultyBuildManager : Singleton<MultyBuildManager>
{
	private List<SBuildObjectInfo> m_ListBuildObject;
	private int m_MaxOffsetFrame = 1;
	private int m_CurOffsetFrame = 1;
	private int m_CurFrame = 0;

	public MultyBuildManager()
	{
		m_ListBuildObject = new List<SBuildObjectInfo>();
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
		if(info.mResPath.Length == 0)
		{
			Log.Error("MultyBuildManager::Add param error");
			return;
		}
		m_ListBuildObject.Add(info);

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
        SBuildObjectInfo info = m_ListBuildObject[0];
		ProcessBuild(info);
        m_ListBuildObject.Remove(info);

        //修改加载间隔
		if(m_ListBuildObject.Count > 0 && m_ListBuildObject[0].mFrameOffset > 0)
		{
            m_CurOffsetFrame = m_ListBuildObject[0].mFrameOffset;
		}
		else
		{
			m_CurOffsetFrame = m_MaxOffsetFrame;
		}
	}

	private void ProcessBuild(SBuildObjectInfo info)
	{
        Object res = null;
        switch (info.Type)
        {
            case eResourceType.UNDEFIED:
            case eResourceType.PREFAB:
                res = ResourceLoaderManager.Instance.Load(info.mResPath);
                break;
            case eResourceType.SPRITE:
                res = ResourceLoaderManager.Instance.LoadSprite(info.mResPath);
                break;
            case eResourceType.TEXTURE:
                res = ResourceLoaderManager.Instance.LoadTexture(info.mResPath);
                break;
            case eResourceType.MATERIAL:
                res = ResourceLoaderManager.Instance.LoadMaterial(info.mResPath);
                break;
            case eResourceType.SHADER:
                res = ResourceLoaderManager.Instance.LoadShader(info.mResPath);
                break;
            case eResourceType.SOUND:
                res = ResourceLoaderManager.Instance.LoadSound(info.mResPath);
                break;
        }
		if(res == null)
		{
			Log.Error("MultyBuildManager::ProcessBuild - not build file:" + info.mResPath);
			return;
		}
        Object obj = GameObject.Instantiate(res, Vector3.zero, Quaternion.identity);
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
