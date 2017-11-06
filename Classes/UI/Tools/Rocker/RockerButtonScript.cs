using UnityEngine;
using System.Collections;

/// <summary>
/// 摇杆btn
/// @author hannibal
/// @time 2016-9-13
/// </summary>
public class RockerButtonScript : MonoBehaviour
{
    public delegate void RockerDown();
    public delegate void RockerUp();
    public delegate void RockerPress();

    private bool m_IsDrag = false;

    [HideInInspector]
    public RockerDown OnRockerDown = null;
    [HideInInspector]
    public RockerUp OnRockerUp = null;
    [HideInInspector]
    public RockerPress OnRockerPress = null;

    void Start()
    {
    }

    void OnEnable()
    {
        UIEventListener.Get(gameObject).onDown = OnDown;
        UIEventListener.Get(gameObject).onUp = OnUp;
    }
    void OnDisable()
    {
    }

    void OnDown(GameObject go, Vector2 pos)
    {
        m_IsDrag = true;
        if (OnRockerDown != null)
        {
            OnRockerDown();
        }
    }
    void OnUp(GameObject go, Vector2 pos)
    {
        m_IsDrag = false;
        if (OnRockerUp != null)
        {
            OnRockerUp();
        }
    }
    void Update()
    {
        if (!m_IsDrag) return;

        if (OnRockerPress != null)
        {
            OnRockerPress();
        }
    }
}
