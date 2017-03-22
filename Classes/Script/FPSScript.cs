using UnityEngine;
using System.Collections;

/// <summary>
/// 求FPS
/// @author hannibal
/// @time 2014-12-27
/// </summary>
public class FPSScript : MonoBehaviour
{
	/**fps*/
	public int 			m_FrameRate = 60;
	
	/**计算时间间隔(秒)*/
	private float 		m_TimeOffset = 1f;
	private int 		m_FrameCount;
	private int 		m_FPS;

	private GUIStyle 	m_TextStyle = new GUIStyle();
	private Rect 		m_RectText;

	void Awake()
    {
        m_FPS = 0;

        m_RectText = new Rect(Screen.width - 200 * UIID.ScreenScaleX, 120 * UIID.ScreenScaleY, 80 * UIID.ScreenScaleX, 30 * UIID.ScreenScaleY);

        m_TextStyle.alignment = TextAnchor.MiddleRight;
        m_TextStyle.fontSize = (int)(15.0f * (UIID.ScreenScaleX + UIID.ScreenScaleY) * 0.5f);

        SetFrameRate(m_FrameRate);
	}

	void Start()
	{
		InvokeRepeating("UpdateFPS", 0, m_TimeOffset);
	}

	void Update ()
	{
		++m_FrameCount;
	}

	// FPS
	void OnGUI()
	{
		if (m_FPS > 20)m_TextStyle.normal.textColor = Color.green;
		else if (m_FPS > 15)m_TextStyle.normal.textColor = Color.yellow;
		else m_TextStyle.normal.textColor = Color.red;

		string str = System.String.Format("FPS: {0:F2},Mem: {1:F2}", m_FPS,Profiler.GetTotalAllocatedMemory()/(1024f*1024f));
		GUI.Label(m_RectText, str, m_TextStyle);
	}

	void UpdateFPS()
	{
		m_FPS = (int)(m_FrameCount*(1/m_TimeOffset));
		m_FrameCount = 0;
	}

    public void SetFrameRate(int fps)
    {
        Log.Info("SetFrameRate:" + fps + " rect:" + m_RectText);
        m_FrameRate = fps;
        //fps:需要注意的是在Edit/Project Setting/QualitySettings下,若vsync被设置了，则targetFrameRate设置的将无效
        Application.targetFrameRate = m_FrameRate;
    }
}
