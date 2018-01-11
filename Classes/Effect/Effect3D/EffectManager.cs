using UnityEngine;
using System.Collections;

/// <summary>
/// 特效管理
/// @author hannibal
/// @time 2014-12-8
/// </summary>
public class EffectManager : Singleton<EffectManager>
{
    private static ulong m_ShareObjID = 0;

    /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～创建特效～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
	/// <summary>
    /// 定点位置创建特效
	/// </summary>
	/// <param name="file">资源文件</param>
	/// <param name="pos">位置</param>
    /// <param name="time">播放时长(秒)：
    /// 1.对于非循环特效，且未设置time，特效播放结束后，会自动销毁；
    /// 2.对于循环特效，到点之后自动销毁
    /// 3.只要这个参数大于0，不管是循环特效还是非循环特效，指定时间一到，自动销毁</param>
    /// 4.循环特效，且未指定time，则需要外部调用RemoveEffect接口销毁
	/// <returns></returns>
	public EffectBase CreateEffect_Position(string file, Vector3 pos, float time = 0)
	{
        EffectBase effect = NewObject<EffectBase>(file) as EffectBase;
        if(effect != null)
        {
            effect.ObjectUID = ShareGUID();
            effect.transform.position = pos;
            effect.TotalTime = time;
            GameObjectUtils.SetLayer(effect.gameObject, LayerMask.NameToLayer(SceneLayerID.Effect));
        }

        return effect;
	}
    /// <summary>
    /// 挂节点创建特效 
    /// </summary>
    /// <param name="file"></param>
    /// <param name="parent_node">父节点</param>
    /// <param name="is_loop"></param>
    /// <param name="time">参考接口CreateEffect_Position的说明</param>
    /// <returns></returns>
    public EffectBase CreateEffect_Joint(string file, Transform parent_node, float time = 0)
	{
        EffectJoint effect = NewObject<EffectJoint>(file) as EffectJoint;
        if(effect != null)
        {
            effect.ObjectUID = ShareGUID();
            effect.transform.SetParent(parent_node, false);
            effect.ParentNode = parent_node;
            effect.TotalTime = time;
            GameObjectUtils.SetLayer(effect.gameObject, LayerMask.NameToLayer(SceneLayerID.Effect));
        }

        return effect;
	}
    /// <summary>
    /// 创建UI特效
    /// </summary>
    /// <param name="file"></param>
    /// <param name="parent_node">父节点</param>
    /// <param name="is_loop"></param>
    /// <param name="time">参考接口CreateEffect_Position的说明</param>
    /// <returns></returns>
    public EffectBase CreateEffect_UI(string file, Transform parent_node, float time = 0)
    {
        EffectUI effect = NewObject<EffectUI>(file) as EffectUI;
        if (effect != null)
        {
            if (parent_node == null)
                parent_node = UILayerUtils.RootLayer;
            effect.transform.SetParent(parent_node, false);
            effect.ParentNode = parent_node;
            effect.TotalTime = time;
            GameObjectUtils.SetLayer(effect.gameObject, LayerMask.NameToLayer(SceneLayerID.UI));
        }

        return effect;
    }
	public void RemoveEffect(EffectBase eff)
	{
		if(eff != null)
		{
            eff.PreDestroy();
            GameObject.Destroy(eff.gameObject);
		}
	}
    static public ulong ShareGUID()
    {
        return ++m_ShareObjID;
    }

    /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～对象构建～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
    /// <summary>
    /// 创建对象
    /// </summary>
    /// <typeparam name="T">脚本文件</typeparam>
    /// <param name="file">资源路径，不填会创建空对象</param>
    /// <returns></returns>
    private EffectBase NewObject<T>(string file = "")
    {
        GameObject obj = GameObjectUtils.BuildObject("Prefab/EmptyObject");
        if (obj == null) return null;
        SceneLayerUtils.AddChild(obj.transform);
        Component gameObj = obj.GetComponent(typeof(T));
        if (gameObj == null) gameObj = obj.AddComponent(typeof(T));

        EffectBase eff = gameObj as EffectBase;
        eff.name = file;
        eff.LoadResource(file);
        return eff;
    }
}
