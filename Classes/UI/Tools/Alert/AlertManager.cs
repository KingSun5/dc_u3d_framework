﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 弹框
/// @author hannibal
/// @time 2016-2-15
/// </summary>
public class AlertManager : Singleton<AlertManager>
{
	private AlertView m_AlertView;

    public void ShowAlert(int id, string content, string btn_name, System.Action<eAlertBtnType> fun)
	{
		//layer
        Transform layer = UILayerUtils.GetLayer((int)eUILayer.TOP);
		if(layer == null)
		{
            Log.Warning("AlertManager::Show - not find layer:" + eUILayer.TOP);
			layer = UILayerUtils.RootLayer;
		}

		//构建
        GameObject obj = UIManager.Instance.Show(id);
        if (obj == null)
        {
            return;
        }

        GameObjectUtils.SetLayer(obj, LayerMask.NameToLayer(SceneLayerID.UI));
        m_AlertView = obj.GetComponent<AlertView>() as AlertView;
        if (m_AlertView == null) m_AlertView = obj.AddComponent<AlertView>() as AlertView;

        //更新数据
		m_AlertView = obj.GetComponent<AlertView>();
		m_AlertView.Content = content;
        m_AlertView.DicBtn.Add(eAlertBtnType.OK, btn_name);
		m_AlertView.Fun = fun;
        m_AlertView.Show();
	}

    public void ShowConfirm(int id, string content, string ok_name, string cancel_name, System.Action<eAlertBtnType> fun)
    {
        //layer
        Transform layer = UILayerUtils.GetLayer((int)eUILayer.TOP);
        if (layer == null)
        {
            Log.Warning("AlertManager::Show - not find layer:" + eUILayer.TOP);
            layer = UILayerUtils.RootLayer;
        }

        //构建
        GameObject obj = UIManager.Instance.Show(id);
        if (obj == null)
        {
            return;
        }

        GameObjectUtils.SetLayer(obj, LayerMask.NameToLayer(SceneLayerID.UI));
        m_AlertView = obj.GetComponent<AlertView>() as AlertView;
        if (m_AlertView == null) m_AlertView = obj.AddComponent<AlertView>() as AlertView;

        //更新数据
        m_AlertView = obj.GetComponent<AlertView>();
        m_AlertView.Content = content;
        m_AlertView.DicBtn.Add(eAlertBtnType.OK, ok_name);
        m_AlertView.DicBtn.Add(eAlertBtnType.CANCEL, cancel_name);
        m_AlertView.Fun = fun;
        m_AlertView.Show();
    }

	public void Remove()
	{
        if (m_AlertView != null)
        {
            GameObject.Destroy(m_AlertView.gameObject);
            m_AlertView = null;
        }
	}
}
