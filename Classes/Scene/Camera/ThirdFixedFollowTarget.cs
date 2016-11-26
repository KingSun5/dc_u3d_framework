using UnityEngine;
using System.Collections;

/// <summary>
/// 第三人称相机：固定视角
/// @author hannibal
/// @time 2014-11-14
/// </summary>
public class ThirdFixedFollowTarget : MonoBehaviour 
{
	/**摄像机平滑移动的时间*/
	public float		m_smoothTime;  	
	public Vector3 		m_cameraOffset;
	private Vector3		m_cameraVelocity = Vector3.zero;
	
	/**摄像机要跟随的对象*/
	private Transform	m_targetObj = null; 

	
	void  Awake () 
	{  
	}
	
	void OnEnable()
	{
        EventDispatcher.AddEventListener(CameraID.CAMERA_FOLLOW_TARGET, OnBindTarget);
	}
	
	void OnDisable()
	{
        EventDispatcher.RemoveEventListener(CameraID.CAMERA_FOLLOW_TARGET, OnBindTarget);
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
			//移动相机
			transform.position = Vector3.SmoothDamp(transform.position, m_targetObj.position + m_cameraOffset, ref m_cameraVelocity, m_smoothTime);
			Quaternion angel = Quaternion.LookRotation(m_targetObj.position - this.transform.position);//获取旋转角度
			this.transform.rotation = Quaternion.Slerp(this.transform.rotation, angel, Time.deltaTime);
		}
	}
}
