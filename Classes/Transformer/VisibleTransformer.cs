using UnityEngine;
using System.Collections;
/**
 * 可视变换器
 */
public class VisibleTransformer : Transformer
{
    bool m_boVisible;//变换完成后目标可视状态

    public static VisibleTransformer visible(GameObject target, bool visible, float time)
    {
        VisibleTransformer transformer = new VisibleTransformer();
        transformer.m_boVisible = visible;
        transformer.m_fTransformTime = time;
        transformer.target = target;
        return transformer;
    }
    public VisibleTransformer()
    {
        m_Type = eTransformerID.Visible;
    }
    public override void runTransform(float currTime)
	{
		if (currTime >= m_fEndTime)
		{
            target.SetActive(m_boVisible);
		}
	}
}