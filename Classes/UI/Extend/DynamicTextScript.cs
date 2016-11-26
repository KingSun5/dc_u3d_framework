using UnityEngine;
using System.Collections;
using UnityEngine.UI;



/// <summary>
/// 动态显示数值文本
/// @author hannibal
/// @time 2015-2-1
/// </summary>
public class DynamicTextScript : MonoBehaviour
{
	/**时间：控制改变速度*/
	public float 	m_TotalTime = 1f;
	public bool 	m_FixedTime = true;
	public float 	m_Speed = 0.03f;
	private float 	m_StartTime = 0f;

	/**颜色*/
	public bool 	m_EnableColor = false;
	public Color 	m_Color = Color.white;

	/**数值前后字符串*/
	public string 	m_PreData = "";
	public string 	m_EndData = "";
	
	private Text 	m_OwnerText;
	private bool 	m_Active = false;

	/**值*/
	private int 	m_Value = 200;
	private int 	m_InitValue;
	private int 	m_EndValue;

	public System.Action<Text> m_OnComplete = null;

	// Use this for initialization
	void Start () 
	{
		m_OwnerText = gameObject.GetComponent<Text>();
		if(m_OwnerText == null)
		{
			Log.Error("DynamicTextScript::Start - attach object not Text");
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(!m_Active)return;

		//线性插值
		int to_value = m_InitValue + (int)((m_EndValue-m_InitValue)*((Time.realtimeSinceStartup-m_StartTime)/m_TotalTime));
		int side = MathUtils.Sign(m_EndValue - m_InitValue);
		if((m_Value == m_EndValue) || (side == 1 && to_value >= m_EndValue) || (side == -1 && to_value <= m_EndValue))
		{
			to_value = m_EndValue;
			m_Active = false;
		}
		Value = to_value;

		if(!m_Active && m_OnComplete != null)
		{
			m_OnComplete(m_OwnerText);
		}
	}

	/**
	 * 显示动画
	 * to_value 目标值
	 * time 动画时间
	 */
	public void To(int to_value, System.Action<Text> on_end = null)
	{
		m_Active = true;

		m_OnComplete = on_end;
		m_InitValue = m_Value;
		m_EndValue = to_value;

		if(to_value == m_Value)return;

		m_StartTime = Time.realtimeSinceStartup;
		if(!m_FixedTime)
		{//计算动画时间
			float num = Mathf.Abs(to_value - m_Value);
			float time = num * m_Speed;
			m_TotalTime = time > m_TotalTime ? m_TotalTime : time;
		}
	}
	
	public void Destroy()
	{
		if(!m_Active)return;
		
		m_Active = false;
	}

	public int Value
	{
		get{return m_Value;}
		set
		{
			m_Value = value;
			if(m_OwnerText != null)
			{
				string str_value = m_Value.ToString();
				if(m_EnableColor)
					str_value = StringUtils.SetFontColor( m_Value.ToString(),"#"+ColorUtils.Color2RGBA(m_Color));
				m_OwnerText.text = m_PreData + str_value + m_EndData;
			}
		}
	}
}
