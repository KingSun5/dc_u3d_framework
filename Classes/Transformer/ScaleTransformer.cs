using UnityEngine;
using System.Collections;
/**
 * 缩放变换器
 */
public class ScaleTransformer : Transformer
{
    public int m_nStartType;
    public float m_fStartX;
    public float m_fStartY;
    public float m_fSpeedX;
    public float m_fSpeedY;
    public float m_fTargetX;
    public float m_fTargetY;

	public	static ScaleTransformer scaleTo(GameObject target, float x, float y, float time)
    {
        ScaleTransformer transformer = new ScaleTransformer();
        transformer.m_nStartType = 0;
        transformer.m_fTargetX = x;
        transformer.m_fTargetY = y;
        transformer.m_fTransformTime = time;
        transformer.target = target;
        return transformer;
    }
    public static ScaleTransformer scaleBy(GameObject target, float speedX, float speedY, float time)
    {
        ScaleTransformer transformer = new ScaleTransformer();
        transformer.m_nStartType = 1;
        transformer.m_fSpeedX = speedX;
        transformer.m_fSpeedY = speedY;
        transformer.m_fTransformTime = time;
        transformer.target = target;
        return transformer;
    }
    public override void OnTransformStarted()
    {
        //获得当前对象的缩放
		Vector3 scale = target.transform.localScale;
		m_fStartX = scale.x;
		m_fStartY = scale.y;
		if (m_nStartType == 0)
		{
			m_fSpeedX = (m_fTargetX - scale.x)/m_fTransformTime;
			m_fSpeedY = (m_fTargetY - scale.y)/m_fTransformTime;
		}
		else if (m_nStartType == 1)
		{
			m_fTargetX = scale.x + m_fSpeedX * m_fTransformTime;
			m_fTargetY = scale.x + m_fSpeedY * m_fTransformTime;
		}
        base.OnTransformStarted();
    }
    public override void runTransform(float currTime)
    {
        if (currTime >= m_fEndTime)
        {
            target.transform.localScale = new Vector3(m_fTargetX, m_fTargetY, target.transform.localScale.z);
        }
        else
        {
            float timeElapased = currTime - m_fStartTime;
            target.transform.localScale = new Vector3(m_fStartX + m_fSpeedX * timeElapased, m_fStartY + m_fSpeedY * timeElapased, target.transform.localScale.z);
        }
    }
}