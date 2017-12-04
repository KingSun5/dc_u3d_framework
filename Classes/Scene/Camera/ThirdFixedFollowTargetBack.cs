using UnityEngine;
using System.Collections;

public class ThirdFixedFollowTargetBack : MonoBehaviour 
{
	public float m_CameraOffsetY = 0;
	public float m_CameraDistance = 5;
	
	/**摄像机要跟随的对象*/
	private Transform	m_targetObj = null; 

	void  Awake () 
	{
	}
	
	void OnEnable()
	{
        EventController.AddEventListener(CameraEvent.FOLLOW_TARGET, OnBindTarget);
	}
	
	void OnDisable()
	{
        EventController.RemoveEventListener(CameraEvent.FOLLOW_TARGET, OnBindTarget);
	}
	/**绑定目标*/
	void OnBindTarget(GameEvent evt)
	{
        m_targetObj = evt.Get<Transform>(0);
	}

	void  Update()
	{
		if(m_targetObj != null)
		{
			float targetAngleY = m_targetObj.transform.eulerAngles.y * Mathf.PI / 180f;
			float targetAngleX = m_targetObj.transform.eulerAngles.x * Mathf.PI / 180f;

			float unitOffsetX = Mathf.Sin(targetAngleY);
			float unitOffsetZ = Mathf.Cos(targetAngleY);
			float unitOffsetY = Mathf.Abs(Mathf.Sin(targetAngleX));
			Vector3 unitOffset = new Vector3(unitOffsetX, -unitOffsetY, unitOffsetZ);

			transform.position = m_targetObj.transform.position - unitOffset*m_CameraDistance + new Vector3(0, m_CameraOffsetY, 0);
			transform.LookAt(m_targetObj.position);
		}
	}
}
