using UnityEngine;
using System.Threading;
using System.Collections.Generic;
using System.Net;
using System.ComponentModel;


/// <summary>
/// http方式下载文件
/// @author hannibal
/// @time 2016-9-19
/// </summary>
/*      
        string path = @"e:/test/pp1.fbx";
        if (File.Exists(path))
            File.Delete(path);
        HttpDownloadManager.Instance.AddDownLoadFile(@"d:/X_jijia_S_01+01.FBX", path);
        HttpDownloadManager.Instance.Start();
 */
public class HttpDownloadManager : Singleton<HttpDownloadManager> 
{
    public const string EVT_DOWNLOAD_PROGRESS   = "EVT_DOWNLOAD_PROGRESS";  //进度
    public const string EVT_DOWNLOAD_COMPLETED  = "EVT_DOWNLOAD_COMPLETED"; //完成
    public const string EVT_DOWNLOAD_FAILED     = "EVT_DOWNLOAD_FAILED";    //失败

	private string 			m_CurrDownFile = string.Empty;
	private int				m_TotalDownFile = 0;
	private List<string> 	m_ListDownFiles;

	private Thread 			m_Thread;
	static readonly object 	m_LockObj = new object();

    private Queue<DownloadFileInfo> m_LoadQueue = new Queue<DownloadFileInfo>();

    public void Setup()
    {
        m_ListDownFiles = new List<string>();
    }

    public void Destroy()
    {
        m_ListDownFiles.Clear();
        if(m_Thread != null)
        {
            m_Thread.Abort();
            m_Thread = null;
        }
    }

    /// <summary>
    /// 添加新下载项
    /// </summary>
    /// <param name="url">rul路径</param>
    /// <param name="path">存放路径</param>
    public void AddDownLoadFile(string url, string path)
    {
        lock (m_LockObj)
        {
            m_ListDownFiles.Add(url);
            DownloadFileInfo data = new DownloadFileInfo(url, path);
            m_LoadQueue.Enqueue(data);
        }
    }
    /// <summary>
    /// 启动下载
    /// </summary>
    public void Start()
    {
        Log.Info("[download]开始下载");
        m_CurrDownFile = string.Empty;
        m_Thread = new Thread(OnUpdate);
        m_Thread.Start();
    }

    private void OnUpdate()
    {
        while (true)
        {
            lock (m_LockObj)
            {
                if (m_LoadQueue.Count > 0)
                {
                    DownloadFileInfo data = m_LoadQueue.Dequeue();
                    HandleDownloadFile(data.url, data.path);
                }
            }
            Thread.Sleep(1);
        }
    }

    /// <summary>
    /// 下载文件
    /// </summary>
    private void HandleDownloadFile(string url, string path)
    {
        m_CurrDownFile = path;
        using (WebClient client = new WebClient())
        {
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(OnProgressChanged);
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(OnDownloadCompleted);
            client.DownloadFileAsync(new System.Uri(url), m_CurrDownFile);
        }
    }

    /// <summary>
    /// 下载一项
    /// </summary>
	public void OnDownloadCompleted(object sender, AsyncCompletedEventArgs e)
    {
        if (e.Error == null && e.Cancelled == false)
        {
            Log.Info("[download]下载一个文件：" + m_CurrDownFile);
            m_TotalDownFile += 1;
            if (IsDownFinish())
            {
                OnDownFinish();
            }
        }
        else
        {
            Log.Error("[download]下载失败,原因:" + e.Error.Message);
            EventDispatcher.TriggerEvent(EVT_DOWNLOAD_FAILED);
        }
    }

    /// <summary>
    /// 下载进度改变
    /// </summary>
    private DownloadProgressInfo progressInfo;
	private void OnProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
        progressInfo.TotalFiles = m_ListDownFiles.Count;
        progressInfo.ReceivedFiles = m_TotalDownFile;
        progressInfo.Percent = e.ProgressPercentage;
        progressInfo.TotalBytes = e.TotalBytesToReceive;
        progressInfo.ReceivedBytes = e.BytesReceived;
        EventDispatcher.TriggerEvent(EVT_DOWNLOAD_PROGRESS, progressInfo);
    }
    /// <summary>
    /// 下载完成
    /// </summary>
    private void OnDownFinish()
    {
        Log.Info("[download]下载完成");
        EventDispatcher.TriggerEvent(EVT_DOWNLOAD_COMPLETED);
        m_ListDownFiles.Clear();
        m_LoadQueue.Clear();
        if (m_Thread != null)
        {
            m_Thread.Abort();
            m_Thread = null;
        }
    }
    public bool IsDownFinish()
    {
        return m_ListDownFiles.Count == m_TotalDownFile;
    }
    public int GetTotalcount()
    {
        return m_ListDownFiles.Count;
    }
}

/// <summary>
/// 下载项
/// </summary>
struct DownloadFileInfo
{
    public string url;
    public string path;
    public DownloadFileInfo(string _url, string _path)
    {
        this.url = _url;
        this.path = _path;
    }
}
/// <summary>
/// 下载进度
/// </summary>
public struct DownloadProgressInfo
{
    public int  TotalFiles;         //总共需要下载的文件数
    public int  ReceivedFiles;      //已经下载的文件数

    public int  Percent;            //进度[1-100]
    public long TotalBytes;         //总共需要下载的bytes
    public long ReceivedBytes;      //已经下载的bytes
}