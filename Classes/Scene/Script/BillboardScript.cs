using UnityEngine;
using System.Collections;


/// <summary>
/// 公告板
/// @author hannibal
/// @time 2016-8-29
/// </summary>
public class BillboardScript : MonoBehaviour 
{
	public bool 		m_FollowScale = false;
	public Camera 		m_Camera = null;  
	private Quaternion 	m_Direction = new Quaternion();  
	
	// Use this for initialization  
	void Start () 
	{  
		m_Direction.x = transform.localRotation.x;  
		m_Direction.y = transform.localRotation.y;  
		m_Direction.z = transform.localRotation.z;  
		m_Direction.w = transform.localRotation.w;  
	}  
	
	// Update is called once per frame  
	void Update ()  
	{  
		if (m_Camera == null)  
		{  
			m_Camera = Camera.main;  
		}  
		transform.rotation = m_Camera.transform.rotation * m_Direction;
		if(m_FollowScale)
		{
			float dis = (m_Camera.transform.position - transform.position).magnitude*0.2f;
			transform.localScale = new Vector3(dis, dis, dis);
		}
	} 
}
