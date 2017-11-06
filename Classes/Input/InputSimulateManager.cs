using UnityEngine;
using System.Collections;

/// <summary>
/// 输入模拟
/// @author hannibal
/// @time 2014-11-21
/// </summary>
public class InputSimulateManager : Singleton<InputSimulateManager>
{
	private bool m_Enable;
	private bool m_IsTouchDown;
	private TouchInfo m_TouchInfo;

	public InputSimulateManager()
	{
		m_Enable = false;
		m_IsTouchDown = false;
		m_TouchInfo = new TouchInfo();
	}

	public void Setup()
	{
		m_Enable = true;
	}
	
	public void Destroy()
	{
		m_Enable = false;
	}
	
	public void Tick(float elapse, int game_frame)
	{
		if(!m_Enable)return;

		if(PlatformUtils.IsPCPlatform())
		{
            string event_type = InputID.TOUCH_NONE;
			if(Input.GetButtonDown ("Fire1"))
			{
				if(!m_IsTouchDown)
				{
                    event_type = InputID.TOUCH_BEGIN;
				}
				m_IsTouchDown = true;
			}
			else if(Input.GetButtonUp ("Fire1"))
			{
				if(m_IsTouchDown)
				{
                    event_type = InputID.TOUCH_END;
				}
				m_IsTouchDown = false;
			}
			else if(Input.GetButton ("Fire1"))
			{
				if(m_IsTouchDown && Input.mousePosition != m_TouchInfo.getLocation())
				{
                    event_type = InputID.TOUCH_MOVED;
				}
			}
            if (event_type != InputID.TOUCH_NONE)
			{
				m_TouchInfo.setTouchInfo(Input.mousePosition);
				EventController.TriggerEvent(event_type, m_TouchInfo);
			}
		}
		else
		{

		}
	}

	public void Clear()
	{
		if(!m_Enable)return;

	}

	public bool Enable
	{
		get { return m_Enable; }
		set { m_Enable = value; }
	}
}
