using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 摇杆
/// @author hannibal
/// @time 2016-8-29
/// </summary>
public class RockerScrollScript : ScrollRect
{
    public delegate void RockerMove(float x, float y);
    public delegate void RockerStop();

    private float m_Radius = 0f;
    private bool m_IsDrag = false;

    [HideInInspector]
    public RockerMove OnRockerMove = null;
    [HideInInspector]
    public RockerStop OnRockerStop = null;

    protected override void Start()
    {
        base.Start();
        //计算摇杆块的半径
        m_Radius = (transform as RectTransform).sizeDelta.x * 0.5f;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEventListener.Get(gameObject).AddEventListener(eUIEventType.Down, OnDown);
        UIEventListener.Get(gameObject).AddEventListener(eUIEventType.Up, OnUp);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        UIEventListener.Get(gameObject).RemoveEventListener(eUIEventType.Down, OnDown);
        UIEventListener.Get(gameObject).RemoveEventListener(eUIEventType.Up, OnUp);
    }

    public void SetPosition(float x, float y)
    {
        GetComponent<RectTransform>().localPosition = new Vector3(x, y, 0);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        var contentPostion = this.content.anchoredPosition;
        if (contentPostion.magnitude > m_Radius)
        {
            contentPostion = contentPostion.normalized * m_Radius;
            SetContentAnchoredPosition(contentPostion);
        }
    }

    void OnDown(UIEvent evt)
    {
        m_IsDrag = true;
    }
    void OnUp(UIEvent evt)
    {
        m_IsDrag = false;
        if (OnRockerStop != null)
        {
            OnRockerStop();
        }
    }
    void Update()
    {
        if (!m_IsDrag) return;
        Vector3 drag_pos = content.transform.position - transform.position;
        if (OnRockerMove != null)
        {
            OnRockerMove(drag_pos.x, drag_pos.y);
        }
    }
    public bool IsDrag
    {
        get { return m_IsDrag; }
        set { m_IsDrag = value; }
    }
}
