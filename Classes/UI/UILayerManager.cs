using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// UI层管理器
/// 一 坐标系:像素，原点在屏幕中心，x正方向右，y正方向上
/// 		^(Y)
///	 		|
///	 		|
///	 		|
///	 		|(0,0)
///	 -------|------>(X)
/// 		|
/// 		|
///			|
/// @author hannibal
/// @time 2014-12-5
/// </summary>
public class UILayerManager : Singleton<UILayerManager>
{
	/**layer*/
    private Transform m_RootLayer = null;
    private GameObject m_Canvas = null;
    private Dictionary<int, Transform> m_DicLayer;

	public UILayerManager()
	{
        m_DicLayer = new Dictionary<int, Transform>();
	}

	//～～～～～～～～～～～～～～～～～～～～～～～Layer~～～～～～～～～～～～～～～～～～～～～～～～//
    public void AddLayer(int layer_id, Transform layer)
	{
		if(m_DicLayer.ContainsKey(layer_id))
		{
			Log.Error("UILayerManager::AddLayer - same layer is register:"+layer_id.ToString());
			return;
		}

		m_DicLayer.Add(layer_id, layer);
	}
	public void RemoveLayer(int layer_id)
	{
		if(m_DicLayer.ContainsKey(layer_id) == false)
		{
			return;
		}
		
		m_DicLayer.Remove(layer_id);
	}
	public void ClearLayer()
	{
		foreach(var obj in m_DicLayer)
		{
			GameObject.Destroy(obj.Value);
		}
		m_DicLayer.Clear();
		m_RootLayer = null;
	}

    public Transform GetLayer(int layer)
	{
        Transform obj;
		if(m_DicLayer.TryGetValue(layer, out obj))
		{
			return obj;
		}
		return null;
	}

	public void SetCanvas(GameObject canvas, Transform layer)
	{
        m_Canvas = canvas;
		m_RootLayer = layer;
	}

	//～～～～～～～～～～～～～～～～～～～～～～～get/set~～～～～～～～～～～～～～～～～～～～～～～～//
    public Transform RootLayer
	{
		get { return m_RootLayer; }
	}
    public GameObject Canvas
    {
        get { return m_Canvas; }
    }
}
