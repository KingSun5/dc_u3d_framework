using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 掉落物模拟
/// @author hannibal
/// @time 2014-12-11
/// </summary>
public class DropSimulationManager : Singleton<DropSimulationManager>
{
	static private uint m_share_id = 0;
	
	private bool m_active = false;
	/**对象集合*/
	private Dictionary<uint, DropPhysxObj> m_dic_drop_obj;
	
	public DropSimulationManager()
	{
		m_dic_drop_obj = new Dictionary<uint, DropPhysxObj>();
	}

	public void Setup()
	{
		m_active = true;
	}
	
	public void Destroy()
	{
		m_active = false;

		RemoveAll();
	}
	
	public uint shareGUID()
	{
		return ++m_share_id;
	}
	
	public void Tick(float elapse, int game_frame)
	{
		if(!m_active)return;

		var list_values = new List<DropPhysxObj>(m_dic_drop_obj.Values);
		foreach(var obj in list_values)
		{
			if(obj.Tick(elapse, game_frame) == false)
			{
				obj.Release();
			}
		}
	}
	
	public void Add(DropPhysxObj obj)
	{
		if(obj == null)return;

		m_dic_drop_obj.Add(obj.id, obj);
	}
	
	public void Remove(DropPhysxObj obj)
	{
		if(obj == null)return;

		m_dic_drop_obj.Remove(obj.id);
	}

	public void RemoveAll()
	{
		var list_values = new List<DropPhysxObj>(m_dic_drop_obj.Values);
		foreach(var obj in list_values)
		{
			DropPhysxObj.Destroy(obj);
		}
		m_dic_drop_obj.Clear();
	}
}
