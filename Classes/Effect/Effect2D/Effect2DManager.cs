using UnityEngine;
using System.Collections;

/// <summary>
/// 特效管理
/// @author hannibal
/// @time 2014-12-8
/// </summary>
public class Effect2DManager : Singleton<Effect2DManager>
{
	/// <summary>
    /// 定点位置创建特效
	/// </summary>
	/// <param name="name"></param>
	/// <param name="pos_x"></param>
	/// <param name="pos_y"></param>
	/// <param name="pos_z"></param>
	/// <param name="is_loop"></param>
	/// <returns></returns>
    public Effect2DBase CreateEffect_Position(string name, float pos_x, float pos_y, float pos_z, bool is_loop)
	{
        Effect2DBase effect = NewObject<Effect2DBase>(name) as Effect2DBase;
		effect.transform.position = new Vector3(pos_x, pos_y, pos_z);
		effect.IsLoop = is_loop;
		
		return effect;
	}
	/// <summary>
    /// 挂节点创建特效 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="jointObj"></param>
	/// <param name="is_loop"></param>
	/// <returns></returns>
    public Effect2DBase CreateEffect_Joint(string name, Transform jointObj, bool is_loop)
	{
        Effect2DJoin effect = NewObject<Effect2DJoin>(name) as Effect2DJoin;
		effect.ParentNode = jointObj;
		effect.IsLoop = is_loop;
		
		return effect;
	}
    /// <summary>
    /// 移除特效
    /// </summary>
    /// <param name="eff"></param>
    public void RemoveEffect(Effect2DBase eff)
	{
        if (eff != null)
        {
            GameObject.Destroy(eff.gameObject);
        }
	}
    /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～对象构建～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
    /// <summary>
    /// 创建对象
    /// </summary>
    /// <typeparam name="T">脚本文件</typeparam>
    /// <param name="file">资源路径，不填会创建空对象</param>
    /// <returns></returns>
    private Effect2DBase NewObject<T>(string file = "")
    {
        GameObject obj = GameObjectUtils.BuildObject(file);
        if (obj == null) return null;
        Component gameObj = obj.GetComponent(typeof(T));
        if (gameObj == null) gameObj = obj.AddComponent(typeof(T));

        return gameObj as Effect2DBase;
    }
}
