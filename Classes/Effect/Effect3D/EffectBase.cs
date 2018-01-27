using UnityEngine;
using System.Collections;

/// <summary>
/// 特效
/// @author hannibal
/// @time 2014-12-8
/// </summary>
public class EffectBase : MonoBehaviour
{
    [SerializeField, Tooltip("对象唯一ID(readonly)")]
    protected ulong m_ObjectUID = 0;
    [SerializeField, Tooltip("开始激活时间(readonly)")]
    protected float m_StartTime = 0;
    [SerializeField, Tooltip("总时长")]
    protected float m_TotalTime = 0;
    [SerializeField, Tooltip("位置偏移")]
    protected Vector3 m_OffsetPos = Vector3.zero;
    [SerializeField, Tooltip("是否激活中")]
    protected bool m_Active = false;
    [SerializeField, Tooltip("是否准备完成(readonly)")]
    protected bool m_IsLoadComplete = false;
    [SerializeField, Tooltip("正在加载的文件(readonly)")]
    protected string m_FilePath = "";

    protected Transform m_RootNode = null;
    private System.Action OnComplete = null;

    public EffectBase()
    {
    }
    public virtual void Awake()
    {
    }
    public virtual void Start()
    {
        m_StartTime = Time.realtimeSinceStartup;
        transform.localPosition += m_OffsetPos;
        AddDestroyComponent();
    }
    public virtual void PreDestroy()
    {
    }
    public virtual void OnDestroy()
    {
        //不能放在OnDisable
        ResourceManager.Instance.RemoveAsync(m_FilePath);
        OnComplete = null;
        m_Active = false;
    }
    public virtual void OnEnable()
    {
        m_Active = true;
        RegisterEvent();
    }
    public virtual void OnDisable()
    {
        m_Active = false;
        UnRegisterEvent();
    }
    /// <summary>
    /// 注册事件
    /// </summary>
    public virtual void RegisterEvent()
    {
    }

    public virtual void UnRegisterEvent()
    {
    }

    public virtual void Update()
    {
    }
    /// <summary>
    /// 加载内部资源
    /// </summary>
    public virtual void LoadResource(string file)
    {
        m_IsLoadComplete = false;
        m_FilePath = file;
        ResourceManager.Instance.AddAsync(file, eResType.PREFAB, delegate(sResLoadResult info)
        {
            if (!m_Active || gameObject == null) return;
            Object res = ResourceLoaderManager.Instance.GetResource(file);
            if (res == null) return;

            GameObject obj = GameObject.Instantiate(res) as GameObject;
            if (obj == null) return;
            OnLoadComplete(obj.transform);
        }
        );
    }
    public virtual void OnLoadComplete(Transform obj)
    {
        if (obj == null || gameObject == null) return;

        m_IsLoadComplete = true;
        m_RootNode = obj;
        m_RootNode.SetParent(this.transform, false);
        if (this is EffectUI)
        {
            GameObjectUtils.SetLayer(gameObject, LayerMask.NameToLayer(SceneLayerID.UI));
        }
        else
        {
            GameObjectUtils.SetLayer(gameObject, LayerMask.NameToLayer(SceneLayerID.Effect));
        }
    }

    protected virtual void AddDestroyComponent()
    {
        if (m_TotalTime <= 0)
        {
            ParticleAutoDestroyScript componet = gameObject.GetComponentInChildren<ParticleAutoDestroyScript>();
            if (componet == null) componet = gameObject.AddComponent<ParticleAutoDestroyScript>();
            componet.m_AutoDestroy = false;
            componet.m_DestroyCallback = OnComponentDestroy;
        }
        else
        {
            AutoDestroyScript componet = gameObject.GetComponentInChildren<AutoDestroyScript>();
            if (componet == null) componet = gameObject.AddComponent<AutoDestroyScript>();
            componet.m_AutoDestroy = false;
            componet.m_TotalTime = m_TotalTime;
            componet.m_DestroyCallback = OnComponentDestroy;
        }
    }
    /// <summary>
    /// 组件自动销毁回调
    /// </summary>
    private void OnComponentDestroy()
    {
        m_Active = false;

        if (OnComplete != null) OnComplete();
        EffectManager.Instance.RemoveEffect(this);
    }
    public void OnCompleted(System.Action callback)
    {
        OnComplete = callback;
    }
    public ulong ObjectUID
    {
        get { return m_ObjectUID; }
        set { m_ObjectUID = value; }
    }
    /// <summary>
    /// 相对于中心点、父节点的偏移值
    /// </summary>
    public virtual void SetOffset(Vector3 offset_pos)
    {
        m_OffsetPos = offset_pos;
    }
    public virtual void SetVisible(bool b)
    {
        gameObject.SetActive(b);
    }
    public float TotalTime
    {
        get { return m_TotalTime; }
        set { m_TotalTime = value; }
    }
    public EventDispatcher Observer
    {
        get { return m_Observer; }
    }
    public bool IsLoadComplete
    {
        get { return m_IsLoadComplete; }
    }
}
