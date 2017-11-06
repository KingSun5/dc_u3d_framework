
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// UI特效自适应
/// @author hannibal
/// @time 2016-12-16
/// </summary>
public class UIParticleScaleScript : MonoBehaviour
{
    private List<ScaleData> scaleDatas = null;
    void Awake()
    {
        scaleDatas = new List<ScaleData>();
        foreach( ParticleSystem p in transform.GetComponentsInChildren<ParticleSystem>(true))
        {
            scaleDatas.Add(new ScaleData(){transform = p.transform,beginScale = p.transform.localScale});
        }
    }
 
	void Start () 
    {
        float designWidth   = UIID.DEFAULT_WIDTH;
        float designHeight  = UIID.DEFAULT_HEIGHT;
        float designScale   = designWidth/designHeight;
        float scaleRate     = (float)Screen.width/(float)Screen.height;
 
        foreach(ScaleData scale in scaleDatas)
        {
            if(scale.transform != null)
            {
                if(scaleRate<designScale)
                {
                    float scaleFactor = scaleRate / designScale;
                    scale.transform.localScale = scale.beginScale * scaleFactor;
                }
                else
                {
                    scale.transform.localScale  = scale.beginScale;
                }
            }
        }
	}
	
#if UNITY_EDITOR
	void Update () 
    {
        Start();
	}
#endif
    class ScaleData
    {
        public Transform transform;
        public Vector3 beginScale = Vector3.one;
    }
}