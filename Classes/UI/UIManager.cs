using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public struct SUILoaderInfo
{
	public int 			mID;
	/**资源*/
	public string 	    mPathName;
    /**层级*/
    public int          mLayer;
	/**预加载*/
	public bool   		mIsPreLoader;
	/**隐藏销毁*/
	public bool 		mHideDestroy;
	/**对齐*/
	public eAligeType	mAlige;

	public SUILoaderInfo(int id, string path, int layer, bool pre_loader, bool destroy, eAligeType alige)
	{
		mID = id;
		mPathName = path;
        mLayer = layer;
		mIsPreLoader = pre_loader;
		mHideDestroy = destroy;
		mAlige = alige;
	}
}

/// <summary>
/// UI管理器
/// @author hannibal
/// @time 2014-12-4
/// </summary>
public class UIManager : Singleton<UIManager>
{
	/**加载信息*/
	private Dictionary<int, SUILoaderInfo> m_DicLoaderInfo;

	/**构建过的UI*/
	private Dictionary<int, GameObject> m_DicUIView;

	public UIManager()
	{
		m_DicLoaderInfo = new Dictionary<int, SUILoaderInfo>();
		m_DicUIView = new Dictionary<int, GameObject>();
	}

	public void Setup()
	{

	}

	public void Destroy()
	{
		m_DicLoaderInfo.Clear();

		foreach(var obj in m_DicUIView)
		{
			GameObject.Destroy(obj.Value);
		}
		m_DicUIView.Clear();
	}

    public GameObject Find(int id)
    {
        GameObject obj;
        if (m_DicUIView.TryGetValue(id, out obj))
        {
            return obj;
        }
        return null;
    }

    public int GetUILayerID(int id)
    {
        SUILoaderInfo info;
        if (m_DicLoaderInfo.TryGetValue(id, out info) == false)
        {
            return -1;
        }
        return info.mLayer;
    }
	//～～～～～～～～～～～～～～～～～～～～～～～显示~～～～～～～～～～～～～～～～～～～～～～～～//
    /// <summary>
    /// 显示入口
    /// </summary>
	public GameObject Show(int id)
	{
		//从缓存中查找
		GameObject obj;
		if(m_DicUIView.TryGetValue(id, out obj))
		{
			obj.SetActive(true);
			return obj;
		}

		//获取数据
		SUILoaderInfo info;
		if(m_DicLoaderInfo.TryGetValue(id, out info) == false)
		{
			Log.Error("UIManager::Show - not find id:"+info.mID.ToString());
			return null;
		}

		//加载
		Object res = LoaderResByName(info.mPathName);
		if(res == null)
		{
			Log.Error("UIManager::Show - res loader error:"+info.mPathName);
			return null;
		}

		//layer
        Transform layer = UILayerManager.Instance.GetLayer(info.mLayer);
		if(layer == null)
		{
            Log.Warning("UIManager::Show - not find layer:" + info.mLayer.ToString());
			layer = UILayerManager.Instance.RootLayer;
		}
		if(layer == null)
		{
			Log.Error("UIManager::Show - not set layer");
			return null;
		}
 
		//构建
		obj = GameObject.Instantiate(res) as GameObject;
        obj.transform.SetParent(layer.transform, false);
		m_DicUIView.Add(id, obj);

        //设置层级
        ResetSortingOrder((int)id);

		return obj;
	}
    /// <summary>
    /// 关闭入口
    /// </summary>
	public bool Close(int id)
	{
		SUILoaderInfo loaderInfo;
		loaderInfo = UIManager.Instance.GetLoaderInfo(id);

		GameObject gameObject;
		if(m_DicUIView.TryGetValue(id, out gameObject) == false)
		{
			return false;
		}
		
		if(loaderInfo.mHideDestroy)
		{
			GameObject.Destroy(gameObject);
            m_DicUIView.Remove(id);
            return true;
		}
		else
		{
            gameObject.SetActive(false);
            return false;
		}
	}
    /// <summary>
    /// 关闭所有面板
    /// </summary>
    public void CloseAll(List<int> exclude_list)
    {
        bool has_close = false;
        while (m_DicUIView.Count > 0)
        {
            has_close = false;
            foreach (var obj in m_DicUIView)
            {
                if (exclude_list != null && exclude_list.Contains(obj.Key)) continue;
                if (Close(obj.Key))
                {
                    has_close = true;
                    break;
                }
            }
            if (!has_close) break;
        }
    }
    //～～～～～～～～～～～～～～～～～～～～～～～渲染层级~～～～～～～～～～～～～～～～～～～～～～～～//
    /// <summary>
    /// 重设UI渲染层级
    /// </summary>
    /// <param name="id"></param>
    public void ResetSortingOrder(int id)
    {
        GameObject obj = UIManager.Instance.Find(id);
        if (obj == null) return;
        UIWindowBase currView = obj.GetComponent<UIWindowBase>();
        if (currView == null) return;

        currView.MaxSortingOrder = 0;
        int UILayerID = UIManager.Instance.GetUILayerID(id);
        GameObject layer = UILayerManager.Instance.GetLayer(UILayerID).gameObject;
        int maxCanvasSortingOrder = GetMaxCanvasSortingOrder(layer);
        Canvas currCanvas = obj.GetComponent<Canvas>();
        if (currCanvas == null)
        {
            currCanvas = obj.AddComponent<Canvas>();
        }
        if (maxCanvasSortingOrder != -1)
        {
            currCanvas.overrideSorting = true;
            if (maxCanvasSortingOrder == 0)
            {
                currCanvas.sortingOrder = UILayerID * UIID.OrderLyaerInterval;
                currView.MaxSortingOrder = UILayerID * UIID.OrderLyaerInterval;
            }
            else
            {
                currCanvas.sortingOrder = maxCanvasSortingOrder + 1;
                currView.MaxSortingOrder = maxCanvasSortingOrder + 1;
            }
        }

        // 加射线
        GraphicRaycaster cast = obj.GetComponent<GraphicRaycaster>();
        if (cast == null)
        {
            cast = obj.AddComponent<GraphicRaycaster>();
        }

        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = obj.AddComponent<CanvasGroup>();
        }
    }

    /// <summary>
    /// 获取当前节点下的最大sortingOrder值
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="UILayerID"></param>
    /// <returns></returns>
    public int GetMaxCanvasSortingOrder(GameObject obj)
    {
        if (obj == null) return -1;
        int maxSortingOrder = 0;
        int childCounts = obj.transform.childCount;
        for (int i = 0; i < childCounts; ++i)
        {
            GameObject child = obj.transform.GetChild(i).gameObject;
            UIWindowBase currView = child.GetComponent<UIWindowBase>();
            if (currView == null) continue;
            if (maxSortingOrder < currView.MaxSortingOrder)
            {
                maxSortingOrder = currView.MaxSortingOrder;
            }
        }
        return maxSortingOrder;
    }
	//～～～～～～～～～～～～～～～～～～～～～～～加载~～～～～～～～～～～～～～～～～～～～～～～～//
	public void PushLoaderInfo(SUILoaderInfo info)
	{
		if(m_DicLoaderInfo.ContainsKey(info.mID))
		{
			Log.Error("UIManager::PushLoaderInfo - same id is register:"+info.mID.ToString());
			return;
		}

		m_DicLoaderInfo.Add(info.mID, info);
	}

	public SUILoaderInfo GetLoaderInfo(int id)
	{
		SUILoaderInfo info;
		m_DicLoaderInfo.TryGetValue(id, out info);
		return info;
	}

	private Object LoaderResByName(string name)
	{
		Object res = ResourceLoaderManager.Instance.Load(name);
		return res;
	}
}
