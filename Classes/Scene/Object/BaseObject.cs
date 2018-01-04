using UnityEngine;
using System.Collections;

/// <summary>
/// 游戏逻辑单位基类
/// @author hannibal
/// @time 2014-11-1
/// </summary>
public abstract class BaseObject : MonoBehaviour, IEventBase
{
    [Header("BaseObject")]
    [SerializeField, Tooltip("对象类型(readonly)")]
    protected int   m_ObjectType = 0;

    [SerializeField, Tooltip("对象唯一ID(readonly)")]
    protected ulong m_ObjectUID = 0;
    [SerializeField, Tooltip("服务器id在客户端的备份(readonly)")]
    protected ulong m_ObjectServerID = 0;
    [SerializeField, Tooltip("是否激活中(readonly)")]
    protected bool m_Active = true;

    [SerializeField, Tooltip("包围盒大小(readonly)")]
    protected Vector3 m_BoundSize;

    /**Observer*/
    protected EventDispatcher m_Observer = new EventDispatcher();

    /**专门用于挂接其他对象的节点*/
    protected Transform m_AttachNode;

    /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～MonoBehaviour方法～～～～～～～～～～～～～～～～～～～～～～～～*/
    public virtual void Awake()
    {
        m_ObjectUID = ObjectManager.ShareGUID();
        gameObject.name = ObjectManager.AppendNameByID(gameObject.name, m_ObjectUID);

        m_AttachNode = transform;
        ObjectManager.Instance.AttachObject(this);
    }

    void Start()
    {
        //undo
    }
    void Update()
    {
        //undo
    }
    void OnEnable()
    {
        //undo
    }
    void OnDisable()
    {
        //undo
    }

    #region 基础方法
    public BaseObject()
    {
    }

    public virtual void Setup(object info)
    {
        RegisterEvent();
        CalBoundSize();
    }
    public virtual void Destroy()
    {
        UnRegisterEvent();
        m_Active = false;
        m_ObjectUID = 0;

        if (m_Observer != null)
        {
            m_Observer.Cleanup();
            m_Observer = null;
        }
        m_AttachNode = null;
    }

    public virtual bool Tick(float elapse, int game_frame)
    {
        return true;
    }

    /// <summary>
    /// 暂停
    /// </summary>
    public virtual void OnPauseGame()
    {
    }
    /// <summary>
    /// 恢复
    /// </summary>
    public virtual void OnResumeGame()
    {
    }

    public virtual void LoadResource(string file_name)
    {

    }

    public virtual void SetVisible(bool b)
    {
        gameObject.SetActive(b);
    }
    public virtual void SetColor(int r, int g, int b, int alpha)
    {
        if (GetComponent<SpriteRenderer>() != null)
        {
            GetComponent<SpriteRenderer>().color = new Color(r, g, b, alpha);
        }
    }
    public virtual void SetPosition(float x, float y, float z)
    {
        MathUtils.TMP_VECTOR3.Set(x, y, z);
        SetPosition(MathUtils.TMP_VECTOR3);
    }
    public virtual void SetPosition(Vector3 pos)
    {
        bool is_change = false;
        if (transform.localPosition != pos) is_change = true;

        transform.localPosition = pos;

        if (is_change) this.OnPositionChange();
    }
    public virtual void SetPosition(float x, float y)
    {
        MathUtils.TMP_VECTOR2.Set(x, y);
        SetPosition(MathUtils.TMP_VECTOR2);
    }
    public virtual void SetPosition(Vector2 pos)
    {
        bool is_change = false;
        if ((Vector2)transform.localPosition != pos) is_change = true;

        MathUtils.TMP_VECTOR3.Set(pos.x, pos.y, transform.localPosition.z);
        transform.localPosition = MathUtils.TMP_VECTOR3;

        if (is_change) this.OnPositionChange();
    }
    public virtual void OnPositionChange()
    {

    }
    public virtual void Translate(Vector3 vec)
    {
        transform.Translate(vec, Space.World);
        SetPosition(transform.position);
        this.OnPositionChange();
    }
    public virtual void LookAt(float x, float z)
    {
        MathUtils.TMP_VECTOR3.Set(x, transform.position.y, z);
        transform.LookAt(MathUtils.TMP_VECTOR3);
    }
    public virtual Vector3 GetScale()
    {
        return transform.localScale;
    }
    public virtual void SetScale(float x, float y, float z)
    {
        transform.localScale = new Vector3(x, y, z);
    }
    /// <summary>
    /// 朝向
    /// </summary>
    public virtual Vector3 Forward{get { throw new System.Exception("子类需要实现的借口"); }}
    public abstract void SetForward(Vector3 dir);
    public virtual void CalBoundSize()
    {
        Renderer render = GetComponentInChildren<Renderer>();
        if (render != null)
        {
            m_BoundSize = render.bounds.size;
        }
    }
    public virtual Vector3 BoundSize
    {
        get { return m_BoundSize; }
    }
    public virtual Vector3 HalfBoundSize
    {
        get { return m_BoundSize * 0.5f; }
    }
    public virtual Vector3 CenterPosition
    {
        get { return this.Position + new Vector3(0, BoundSize.y*0.5f, 0); ; }
    }
    public virtual Vector3 TopPosition
    {
        get { return this.Position + new Vector3(0, BoundSize.y, 0); }
    }
    #endregion

    #region 事件
    public virtual void RegisterEvent()
    {
    }
    public virtual void UnRegisterEvent()
    {
    }
    public void AddEventListener(string EventID, EventDispatcher.RegistFunction pFunction)
    {
        m_Observer.AddEventListener(EventID, pFunction);
    }
    public void RemoveEventListener(string EventID, EventDispatcher.RegistFunction pFunction)
    {
        m_Observer.RemoveEventListener(EventID, pFunction);
    }
    public void TriggerEvent(string EventID, GameEvent info)
    {
        m_Observer.TriggerEvent(EventID, info);
    }
    public void TriggerEvent(string eventType, params object[] list)
    {
        GameEvent info = new GameEvent();
        info.Init(list);
        info.type = eventType;
        m_Observer.TriggerEvent(eventType, info);
    }
    #endregion

    #region get/set
    public ulong ObjectUID
    {
        get { return m_ObjectUID; }
        set { m_ObjectUID = value; }
    }
    public ulong ObjectServerID
    {
        get { return m_ObjectServerID; }
        set { m_ObjectServerID = value; }
    }
    public bool Active
    {
        get { return m_Active; }
        set { m_Active = value; }
    }
    public int ObjectType
    {
        get { return m_ObjectType; }
        set { m_ObjectType = value; }
    }
    public Transform AttachNode
    {
        get { return m_AttachNode; }
    }
    public virtual Vector3 Position
    {
        get { return transform.position; }
    }
    public EventDispatcher Observer
    {
        get { return m_Observer; }
    }
    #endregion
}

public class ObjectEvent
{
    public const string MAP_OBJ_POS         = "MAP_OBJ_POS";							//位置改变
    public const string MAP_GRID_CHANGE     = "MAP_GRID_CHANGE";						//地图格子改变
}