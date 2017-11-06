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
    private static EventDispatcher m_Observer = new EventDispatcher();

    static public void AddEventListener(string EventID, EventDispatcher.RegistFunction pFunction)
    {
        m_Observer.AddEventListener(EventID, pFunction);
    }
    static public void RemoveEventListener(string EventID, EventDispatcher.RegistFunction pFunction)
    {
        m_Observer.RemoveEventListener(EventID, pFunction);
    }
    static public void TriggerEvent(string EventID, GameEvent info)
    {
        m_Observer.TriggerEvent(EventID, info);
    }
    static public GameEvent m_DefaultGameEvent = new GameEvent();
    public static void TriggerEvent(string eventType, params object[] list)
    {
        m_DefaultGameEvent.Init(list);
        m_DefaultGameEvent.type = eventType;
        m_Observer.TriggerEvent(eventType, m_DefaultGameEvent);
    }
    public static void Cleanup()
    {
        m_Observer.Cleanup();
    }
}