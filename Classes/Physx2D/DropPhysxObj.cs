using UnityEngine;
using System.Collections;

/// <summary>
/// 单个掉落对象:模拟带弹性的物品掉落在地上
/// @author hannibal
/// @time 2014-12-11
/// </summary>
public struct SDropInfo
{
	public float m_x;//位置
	public float m_y;

	public float m_gravity;//重力加速度
	public float m_friction;//摩擦力(0-1)
	public float m_altitude;//振幅

	public float m_speed_x;//速度
	public float m_speed_y;

	public float m_life;//掉落物体在地上弹多少次

	public DropPhysxObj.tick_fun m_tick_fun;//每帧回调函数
	public DropPhysxObj.end_fun m_end_fun;//结束回调函数
	
	public object m_userData;

	public void Init(float x, float y, 
	                 float gravity, float friction, float altitude,
	                 float speed_x, float speed_y, float life,
	                 object userData)
	{
		m_x = x;
		m_y = y;
		m_gravity = gravity;
		m_friction = friction;
		m_altitude = altitude;
		m_speed_x = speed_x;
		m_speed_y = speed_y;
		m_life = life;
		m_tick_fun = null;
		m_end_fun = null;
		m_userData = userData;
	}
}

public class DropPhysxObj : IPoolsObject
{
    public const string POOLS_DROP_PHYSX_OBJ = "POOLS_DROP_PHYSX_OBJ";

	public delegate void tick_fun(DropPhysxObj obj, float x, float y);
	public delegate void end_fun(DropPhysxObj obj);

	private uint m_id = 0;
	private bool m_active = false;
	
	private SDropInfo m_drop_info;

	private DropPhysxObj()
	{
	}
	
	/**统一创建接口，不要使用默认的构造函数实现*/		
	static public DropPhysxObj Create()
	{
		DropPhysxObj obj = ObjectPoolsManager.Instance.GetObj(POOLS_DROP_PHYSX_OBJ) as DropPhysxObj;
		if(obj == null)
		{
			obj = new DropPhysxObj();
		}
		obj.Init();
		return obj;
	}
	/**统一销毁接口*/		
	static public void Destroy(DropPhysxObj obj)
	{
		if(obj == null)return;

		obj.Release();
	}
	/** 初始化*/		
	public void Init()
	{
		m_id = DropSimulationManager.Instance.shareGUID();
		m_active = true;
		DropSimulationManager.Instance.Add(this);
	}
	
	public void Release()
	{
		if(!m_active)return;

		m_active = false;
		m_drop_info.m_tick_fun = null;
		m_drop_info.m_end_fun = null;
		
		DropSimulationManager.Instance.Remove(this);
		ObjectPoolsManager.Instance.RecoverObj(POOLS_DROP_PHYSX_OBJ,this);
	}
    public string GetPoolsType()
	{
		return POOLS_DROP_PHYSX_OBJ;
	}
	/**构建接口*/		
	public void Setup(SDropInfo info)
	{
		m_active = true;

		m_drop_info = info;
	}
	
	public bool Tick(float elapse, int game_frame)
	{
		if(!m_active)return false;
		
		bool ret = true;
		
		m_drop_info.m_speed_y = m_drop_info.m_speed_y + m_drop_info.m_gravity;
		
		float temp_x = m_drop_info.m_x + m_drop_info.m_speed_x;
		float temp_y = m_drop_info.m_y + m_drop_info.m_speed_y;
		if ((m_drop_info.m_gravity >= 0 && temp_y > m_drop_info.m_altitude) || 
		    (m_drop_info.m_gravity < 0 && temp_y < m_drop_info.m_altitude))
		{
			m_drop_info.m_speed_x = m_drop_info.m_speed_x * (1-m_drop_info.m_friction);
			m_drop_info.m_speed_y = m_drop_info.m_speed_y * -(1-m_drop_info.m_friction);
			temp_y = m_drop_info.m_altitude;
			
			//判断消失
			m_drop_info.m_life = m_drop_info.m_life - 1;
			if (--m_drop_info.m_life <= 0)
			{
				ret = false;
			}
		}
		m_drop_info.m_x = temp_x;
		m_drop_info.m_y = temp_y;
		//告诉位置
		if(m_drop_info.m_tick_fun != null)
		{
			m_drop_info.m_tick_fun(this, m_drop_info.m_x, m_drop_info.m_y);
		}
		if(!ret && m_drop_info.m_end_fun != null)
		{//回调
			m_drop_info.m_end_fun(this);
		}
		return ret;
	}

	public uint id
	{
		get{ return m_id; }
	}
}
