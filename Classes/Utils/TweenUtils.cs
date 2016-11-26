using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// iTween动画封装
/// @author hannibal
/// @time 2015-1-14
/// </summary>
public class TweenUtils
{
	/**
	 * 移动到目标点动画
	 */ 
	static public void MoveTo(GameObject obj, Vector3 pos, float time,
	                          iTween.FunCallback OnUpdate = null, string updateParam = "",
	                          iTween.FunCallback OnComplete = null, string completeParam = "",
	                          iTween.EaseType type = iTween.EaseType.linear, string loopType = "none")
	{
		Hashtable args = new Hashtable();
		args.Add("easeType", type);
		args.Add("time",time);
		args.Add("loopType", loopType);

		//移动中调用，参数和上面类似
		args.Add("onupdate", updateParam);
		args.Add("onupdateparams", completeParam);
		
		args.Add("x",pos.x);
		args.Add("y",pos.y);
		args.Add("z",pos.z);
		
		//执行期回调函数
		Dictionary<string, iTween.FunCallback> dic = new Dictionary<string, iTween.FunCallback>();
		if(OnUpdate != null)
			dic.Add("onupdate", OnUpdate);
		if(OnComplete != null)
			dic.Add("oncomplete", OnComplete);
		
		//最终让改对象开始移动
		iTween.MoveTo(obj, args, dic);	
	}

	/**
	 * 缩放动画
	 */ 
	static public void ScaleTo(GameObject obj, Vector3 scale, float time,
	                          iTween.FunCallback OnUpdate = null, string updateParam = "",
	                          iTween.FunCallback OnComplete = null, string completeParam = "",
	                          string loopType = "none")
	{
		Hashtable args = new Hashtable();
		args.Add("easeType", iTween.EaseType.linear);
		args.Add("time",time);
		args.Add("loopType", loopType);

		args.Add("scale",scale);
		
		//移动中调用，参数和上面类似
		args.Add("onupdate", updateParam);
		args.Add("onupdateparams", completeParam);

		//执行期回调函数
		Dictionary<string, iTween.FunCallback> dic = new Dictionary<string, iTween.FunCallback>();
		if(OnUpdate != null)
			dic.Add("onupdate", OnUpdate);
		if(OnComplete != null)
			dic.Add("oncomplete", OnComplete);
		
		//最终让改对象开始移动
		iTween.ScaleTo(obj, args, dic);	
	}

	/**
	 * 渐变动画
	 */ 
	static public void FadeTo(GameObject obj, float alpha, float time,
	                           iTween.FunCallback OnComplete = null, string completeParam = "",
	                           string loopType = "none")
	{
		Hashtable args = new Hashtable();
		args.Add("easeType", iTween.EaseType.linear);
		args.Add("time",time);
		args.Add("loopType", loopType);

		//执行期回调函数
		Dictionary<string, iTween.FunCallback> dic = new Dictionary<string, iTween.FunCallback>();
		if(OnComplete != null)
			dic.Add("oncomplete", OnComplete);
		args.Add("onupdateparams", completeParam);
		
		//最终让改对象开始移动
		iTween.FadeTo(obj, alpha, time, dic);	
	}
}
