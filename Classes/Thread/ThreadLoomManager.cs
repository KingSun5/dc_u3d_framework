using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

/// <summary>
/// 线程函数处理器，可以让一个函数一部分逻辑在多线程中处理
/// @author hannibal
/// @time 2017-8-11
/// </summary>
public class ThreadLoomManager : MonoBehaviour
{
    public static int maxThreads = 8;
    static int numThreads;

    private int _count;
    private static ThreadLoomManager _current;
    public static ThreadLoomManager Current
    {
        get
        {
            Initialize();
            return _current;
        }
    }

    void Awake()
    {
        _current = this;
        initialized = true;
    }

    static bool initialized;

    static void Initialize()
    {
        if (!initialized)
        {

            if (!Application.isPlaying)
                return;
            initialized = true;
            var g = new GameObject("ThreadLoomManager");
            _current = g.AddComponent<ThreadLoomManager>();
        }

    }

    private List<Action> _actions = new List<Action>();
    public struct DelayedQueueItem
    {
        public float time;
        public Action action;
    }
    private List<DelayedQueueItem> _delayed = new List<DelayedQueueItem>();

    List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();

    public static void QueueOnMainThread(Action action)
    {
        QueueOnMainThread(action, 0f);
    }
    public static void QueueOnMainThread(Action action, float time)
    {
        if (time != 0)
        {
            lock (Current._delayed)
            {
                Current._delayed.Add(new DelayedQueueItem { time = Time.time + time, action = action });
            }
        }
        else
        {
            lock (Current._actions)
            {
                Current._actions.Add(action);
            }
        }
    }

    public static Thread RunAsync(Action a)
    {
        Initialize();
        while (numThreads >= maxThreads)
        {
            Thread.Sleep(1);
        }
        Interlocked.Increment(ref numThreads);
        ThreadPool.QueueUserWorkItem(RunAction, a);
        return null;
    }

    private static void RunAction(object action)
    {
        try
        {
            ((Action)action)();
        }
        catch
        {
        }
        finally
        {
            Interlocked.Decrement(ref numThreads);
        }

    }


    void OnDisable()
    {
        if (_current == this)
        {

            _current = null;
        }
    }



    // Use this for initialization  
    void Start()
    {

    }

    List<Action> _currentActions = new List<Action>();

    // Update is called once per frame  
    void Update()
    {
        lock (_actions)
        {
            _currentActions.Clear();
            _currentActions.AddRange(_actions);
            _actions.Clear();
        }
        foreach (var a in _currentActions)
        {
            a();
        }
        lock (_delayed)
        {
            _currentDelayed.Clear();
            _currentDelayed.AddRange(_delayed.Where(d => d.time <= Time.time));
            foreach (var item in _currentDelayed)
                _delayed.Remove(item);
        }
        foreach (var delayed in _currentDelayed)
        {
            delayed.action();
        }
    }
}

/**
//Scale a mesh on a second thread  
void ScaleMesh(Mesh mesh, float scale)  
{  
    //Get the vertices of a mesh  
    var vertices = mesh.vertices;  
    //Run the action on a new thread  
    ThreadLoomManager.RunAsync(()=>{  
        //Loop through the vertices  
        for(var i = 0; i < vertices.Length; i++)  
        {  
            //Scale the vertex  
            vertices[i] = vertices[i] * scale;  
        }  
        //Run some code on the main thread  
        //to update the mesh  
        ThreadLoomManager.QueueOnMainThread(()=>{  
            //Set the vertices  
            mesh.vertices = vertices;  
            //Recalculate the bounds  
            mesh.RecalculateBounds();  
        });  
   
    });  
}  
 */