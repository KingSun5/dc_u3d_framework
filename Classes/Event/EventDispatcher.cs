using System;
using System.Collections.Generic;

/// <summary>
/// 事件控制器
/// @author hannibal
/// @time 2016-9-18
/// </summary>
public class EventDispatcher
{
    private static EventController m_eventController = new EventController();

    /// <summary>
    /// 清除非永久性注册的事件
    /// </summary>
    public static void Cleanup()
    {
        m_eventController.Cleanup();
    }

    /// <summary>
    ///  增加监听器， 固定GameEvent类型参数
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    public static void AddEventListener(string eventType, Action<GameEvent> handler)
    {
        m_eventController.AddEventListener(eventType, handler);
    }

    /// <summary>
    ///  移除监听器， 带固定GameEvent参数类型
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    public static void RemoveEventListener(string eventType, Action<GameEvent> handler)
    {
        m_eventController.RemoveEventListener(eventType, handler);
    }

    /// <summary>
    ///  触发事件， GameEvent对象传递方式
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    static public GameEvent m_DefaultGameEvent = new GameEvent(0);
    public static void TriggerEvent(string eventType,params object[] list)
    {
        GameEvent gameEvent = new GameEvent(list);
        gameEvent.type = eventType;
        m_eventController.TriggerEvent(eventType, gameEvent);
    }
}
