using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 定时器管理器
/// @author hannibal
/// @time 2016-9-19
/// </summary>
public class TimerManager : Singleton<TimerManager>
{
    private int                 m_idCounter = 0;
    private List<int>           m_RemovalPending = new List<int>();
    private List<TimerEntity>   m_Timers = new List<TimerEntity>();

    public void Setup()
    {
        m_idCounter = 0;
    }

    public void Destroy()
    {
        m_Timers.Clear();
        m_RemovalPending.Clear();
    }

    public void Tick(float elapse, int game_frame)
    {
        Remove();

        for (int i = 0; i < m_Timers.Count; i++)
        {
            m_Timers[i].Update(elapse);
        }
    }

    /// <summary>
    /// 增加定时器
    /// </summary>
    /// <param name="rate">触发频率(单位秒)</param>
    /// <param name="callBack">触发回调函数</param>
    /// <returns>新定时器id</returns>
    public int AddTimer(float rate, Action callBack)
    {
        return AddTimer(rate, 0, callBack);
    }
    /// <summary>
    /// 增加定时器，可以指定循环次数
    /// </summary>
    /// <param name="rate">触发频率(单位秒)</param>
    /// <param name="ticks">循环次数，如果是0则不会自动删除</param>
    /// <param name="callBack">触发回调函数</param>
    /// <returns>新定时器id</returns>
    public int AddTimer(float rate, int ticks, Action callBack)
    {
        TimerEntity newTimer = new TimerEntity(++m_idCounter, rate, ticks, callBack);
        m_Timers.Add(newTimer);
        return newTimer.id;
    }

    /// <summary>
    /// 移除定时器
    /// </summary>
    /// <param name="timerId">Timer GUID</param>
    public void RemoveTimer(int timerId)
    { 
        m_RemovalPending.Add(timerId);
    }

    /// <summary>
    /// 移除过期定时器
    /// </summary>
    private void Remove()
    {
        if (m_RemovalPending.Count > 0)
        {
            foreach (int id in m_RemovalPending)
            {
                for (int i = 0; i < m_Timers.Count; i++)
                {
                    if (m_Timers[i].id == id)
                    {
                        m_Timers.RemoveAt(i);
                        break;
                    }
                }
            }

            m_RemovalPending.Clear();
        }
    }
}

/// <summary>
/// 定时器
/// </summary>
class TimerEntity
{
    public int      id;
    public bool     isActive;

    public float    mRate;
    public int      mTicks;
    public int      mTicksElapsed;
    public float    mLast;
    public Action   mCallBack;

    public TimerEntity(int id_, float rate_, int ticks_, Action callback_)
    {
        id = id_;
        mRate = rate_ < 0 ? 0 : rate_;
        mTicks = ticks_ < 0 ? 0 : ticks_;
        mCallBack = callback_;
        mLast = 0;
        mTicksElapsed = 0;
        isActive = true;
    }

    public void Update(float elapse)
    {
        mLast += elapse;

        if (isActive && mLast >= mRate)
        {
            mCallBack.Invoke();
            mLast = 0;
            mTicksElapsed++;

            if (mTicks > 0 && mTicks == mTicksElapsed)
            {
                isActive = false;
                TimerManager.Instance.RemoveTimer(id);
            }
        }
    }
}