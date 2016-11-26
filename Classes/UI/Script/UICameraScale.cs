using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public enum eScaleType
{
	EXACT_FIT=0, 	//屏幕宽 与 设计宽比 作为X方向的缩放因子，屏幕高 与 设计高比 作为Y方向的缩放因子,即x轴,y轴都自动缩放,保证了设计区域完全铺满屏幕，但是可能会出现图像拉伸。
	NO_BORDER, 		//与上面对应,取屏幕宽、高分别和设计分辨率宽、高计算缩放因子，较(大)者作为宽、高的缩放因子。保证了设计区域总能一个方向上铺满屏幕，而另一个方向一般会超出屏幕区域.即看哪个方向上的缩放因子大,来决定整个图片的缩放比例
	SHOW_ALL,  		//与上面对应,屏幕宽、高分别和设计分辨率宽、高计算缩放因子，取较(小)者作为宽、高的缩放因子。保证了设计区域全部显示到屏幕上，但可能会有黑边。即取二者中较小的缩放因子作为整个图片的缩放比例.因此会出现黑边
	FIXED_WIDTH,	//宽度不变,高度缩放
	FIXED_HEIGHT,	//根据字面意思理解即可,高度不变,宽度缩放
}

/// <summary>
/// 界面缩放
/// @author hannibal
/// @time 2016-8-29
/// </summary>
public class UICameraScale : MonoBehaviour 
{
	public eScaleType m_ScaleType = eScaleType.SHOW_ALL;

	void Start () 
	{
		float ManualWidth = UIID.DEFAULT_WIDTH;
		float ManualHeight = UIID.DEFAULT_HEIGHT;
		float ScaleX = 1, ScaleY = 1;
		switch(m_ScaleType)
		{
		case eScaleType.EXACT_FIT:
			ScaleX = Screen.width / ManualWidth;
			ScaleY = Screen.height / ManualHeight;
			break;

		case eScaleType.NO_BORDER:
			ScaleX = Screen.width / ManualWidth;
			ScaleY = Screen.height / ManualHeight;
			if(ScaleX >= ScaleY)ScaleY = ScaleX;
			else ScaleX = ScaleY;
			break;
			
		case eScaleType.SHOW_ALL:
			ScaleX = Screen.width / ManualWidth;
			ScaleY = Screen.height / ManualHeight;
			if(ScaleX < ScaleY)ScaleY = ScaleX;
			else ScaleX = ScaleY;
			break;
		}
		transform.localScale = new Vector3(ScaleX,ScaleY,1);
	}
}
