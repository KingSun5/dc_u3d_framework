using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AlertManager : Singleton<AlertManager>
{
	private AlertView m_AlertView;
	//资源
	private Object m_Res;

	public void Show(string content, Dictionary<AlertID.EBtnType, string> DicBtn, 
	                 AlertView.FunCallback fun, int layer_id)
	{
		//layer
        Transform layer = UILayerManager.Instance.GetLayer(layer_id);
		if(layer == null)
		{
			Log.Warning("AlertManager::Show - not find layer:"+layer_id.ToString());
			layer = UILayerManager.Instance.RootLayer;
		}
		if(layer == null)
		{
			Log.Error("AlertManager::Show - not set layer");
			return;
		}
		
		//构建
		GameObject obj = GameObject.Instantiate(m_Res) as GameObject;
		obj.transform.parent = layer.transform;
		obj.transform.localPosition = Vector3.zero;
		obj.transform.localScale = Vector3.one;
		obj.transform.Rotate(Vector3.zero);

		m_AlertView = obj.GetComponent<AlertView> ();
		m_AlertView.Content = content;
		m_AlertView.DicBtn = DicBtn;
		m_AlertView.Fun = fun;
	}

	public void Remove()
	{
        if (m_AlertView != null)
        {
            UIManager.Instance.Close(m_AlertView.screenID);
        }
	}

	public Object Res
	{
		set{ m_Res = value; }
	}
}
