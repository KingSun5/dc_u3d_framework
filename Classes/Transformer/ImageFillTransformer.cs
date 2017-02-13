using UnityEngine;
using UnityEngine.UI;
using System.Collections;
/**
 * 图片进度填充变换器
 */
public class ImageFillTransformer : Transformer
{
    public float m_FillCount;
    public float m_FillSpeed;
    private float m_StartFill = 0;
    public Image m_FillImage = null;
    public static ImageFillTransformer FillTo(GameObject target, float fillCount, float time)
    {
        ImageFillTransformer transformer = new ImageFillTransformer();
        transformer.m_FillCount = fillCount;
        transformer.m_fTransformTime = time;
        transformer.target = target;
        return transformer;
    }
    public override void OnTransformStarted()
    {
        m_FillImage = target.GetComponent<Image>();
        if (m_FillImage == null)
            return;
        m_StartFill = m_FillImage.fillAmount;
        m_FillSpeed = (m_FillCount - m_StartFill) / m_fTransformTime;
    }
    public override void runTransform(float currTime)
    {
        if (m_FillImage == null)
            return;
        if (currTime >= m_fEndTime)
        {
            m_FillImage.fillAmount = m_FillCount;
        }
        else
        {
            float timeElapased = currTime - m_fStartTime;
            m_FillImage.fillAmount = (m_StartFill + m_FillSpeed * timeElapased);
        }
    }
}