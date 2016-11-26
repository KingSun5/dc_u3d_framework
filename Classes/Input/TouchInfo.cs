using UnityEngine;
using System.Collections;

/// <summary>
/// 触摸信息
/// @author hannibal
/// @time 2016-1-19
/// </summary>
public struct TouchInfo
{
	private bool m_startPointCaptured;
	private Vector3 m_startPoint;
	private Vector3 m_point;
	private Vector3 m_prevPoint;

	public void setTouchInfo(Vector3 pos)
	{
		m_prevPoint = m_point;
		m_point = pos;
		if (!m_startPointCaptured)
		{
			m_startPoint = m_point;
			m_startPointCaptured = true;
			m_prevPoint = m_point;
		}
	}

	public Vector3 getLocation()
	{
		return m_point;
	}
	public Vector3 getPreviousLocation()
	{
		return m_prevPoint;
	}
	public Vector3 getStartLocation()
	{
		return m_startPoint;
	}
	public Vector3 getDelta()
	{
		return m_point - m_prevPoint;
	}
}
