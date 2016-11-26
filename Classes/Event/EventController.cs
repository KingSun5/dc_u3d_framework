using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件控制器
/// @author hannibal
/// @time 2016-9-18
/// </summary>
public class EventController
{
    private Dictionary<string, Delegate> m_theRouter = new Dictionary<string, Delegate>();

    /// <summary>
    /// 判断是否已经包含事件
    /// </summary>
    /// <param name="eventType"></param>
    /// <returns></returns>
    public bool ContainsEvent(string eventType)
    {
        return m_theRouter.ContainsKey(eventType);
    }

    /// <summary>
    /// 清除非永久性注册的事件
    /// </summary>
    public void Cleanup()
    {
        m_theRouter.Clear();
    }

    /// <summary>
    /// 处理增加监听器前的事项， 检查 参数等
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="listenerBeingAdded"></param>
    private void OnListenerAdding(string eventType, Delegate listenerBeingAdded)
    {
        if (!m_theRouter.ContainsKey(eventType))
        {
            m_theRouter.Add(eventType, null);
        }

        Delegate d = m_theRouter[eventType];
        if (d != null && d.GetType() != listenerBeingAdded.GetType())
        {
            Log.Exception(string.Format(
                    "Try to add not correct event {0}. Current type is {1}, adding type is {2}.",
                    eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
        }
    }

    /// <summary>
    /// 移除监听器之前的检查
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="listenerBeingRemoved"></param>
    private bool OnListenerRemoving(string eventType, Delegate listenerBeingRemoved)
    {
        if (!m_theRouter.ContainsKey(eventType))
        {
            return false;
        }

        Delegate d = m_theRouter[eventType];
        if ((d != null) && (d.GetType() != listenerBeingRemoved.GetType()))
        {
            Log.Exception(string.Format(
                "Remove listener {0}\" failed, Current type is {1}, adding type is {2}.",
                eventType, d.GetType(), listenerBeingRemoved.GetType()));
            return false;
        }
        else
            return true;
    }

    /// <summary>
    /// 移除监听器之后的处理。删掉事件
    /// </summary>
    /// <param name="eventType"></param>
    private void OnListenerRemoved(string eventType)
    {
        if (m_theRouter.ContainsKey(eventType) && m_theRouter[eventType] == null)
        {
            m_theRouter.Remove(eventType);
        }
    }

    /// <summary>
    ///  增加监听器， 1个参数
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    public void AddEventListener<T>(string eventType, Action<T> handler)
    {
        OnListenerAdding(eventType, handler);
        m_theRouter[eventType] = (Action<T>)m_theRouter[eventType] + handler;
    }

    /// <summary>
    ///  移除监听器， 1个参数
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    public void RemoveEventListener<T>(string eventType, Action<T> handler)
    {
        if (OnListenerRemoving(eventType, handler))
        {
            m_theRouter[eventType] = (Action<T>)m_theRouter[eventType] - handler;
            OnListenerRemoved(eventType);
        }
    }

    /// <summary>
    /// 触发事件
    /// </summary>
    /// <param name="eventType"></param>
    public void TriggerEvent(string eventType, params object[] list)
    {
        GameEvent evt = new GameEvent(list);
        TriggerEvent(eventType, evt);
    }
    public void TriggerEvent(string eventType, GameEvent evt)
    {
        evt.type = eventType;
        Delegate d;
        if (!m_theRouter.TryGetValue(evt.type, out d))
        {
            return;
        }

        var callbacks = d.GetInvocationList();
        for (int i = 0; i < callbacks.Length; i++)
        {
            Action<GameEvent> callback = callbacks[i] as Action<GameEvent>;

            if (callback == null)
            {
                Log.Exception(string.Format("TriggerEvent {0} error: types of parameters are not match.", evt.type));
            }

            try
            {
                callback(evt);
            }
            catch (Exception ex)
            {
                Log.Exception(ex.ToString());
            }
        }
    }
}