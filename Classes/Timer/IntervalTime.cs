using UnityEngine;
using System.Collections;

public class IntervalTime 
{
	private bool m_is_reset_time;
	private float m_interval_time;//毫秒
	private float m_now_time;

	public IntervalTime()
	{
		m_is_reset_time = true;
	}

	public void Init(float interval)
	{
		m_interval_time = interval;
		m_now_time = 0.0f;
	}

	public bool Update(float elapse_time)
	{
		m_now_time += elapse_time;
		if (m_now_time >= m_interval_time)
		{
			if (m_is_reset_time)m_now_time = 0.0f;
			else m_now_time -= m_interval_time;
			return true;
		}
		return false;
	}

	public void SetResetTime(bool b)
	{
		m_is_reset_time = b;
	}
}
