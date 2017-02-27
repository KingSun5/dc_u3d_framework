using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// ab管理器
/// @author hannibal
/// @time 2017-2-27
/// </summary>
public class AssetBundleManager : Singleton<AssetBundleManager>
{
    private string m_BaseDownloadingURL = "";
    private string[] m_ActiveVariants = { };
    private AssetBundleManifest m_AssetBundleManifest = null;
    private Dictionary<string, LoadedAssetBundle> m_LoadedAssetBundles = new Dictionary<string, LoadedAssetBundle>();
    private Dictionary<string, WWW> m_DownloadingWWWs = new Dictionary<string, WWW>();
    private Dictionary<string, string> m_DownloadingErrors = new Dictionary<string, string>();
    private List<AssetBundleLoadOperation> m_InProgressOperations = new List<AssetBundleLoadOperation>();
    private Dictionary<string, string[]> m_Dependencies = new Dictionary<string, string[]>();

    public void Setup()
    {
        BaseDownloadingURL = AssetBundleUtil.GetRelativePath();
    }

    public void Destroy()
    {

    }

    public void Start()
    {
        string manifestAssetBundleName = AssetBundleID.PackageAssetDir;
        LoadAssetBundle(manifestAssetBundleName, true);
        var operation = new AssetBundleLoadManifestOperation(manifestAssetBundleName, "AssetBundleManifest", typeof(AssetBundleManifest));
        m_InProgressOperations.Add(operation);
    }

    public void Tick(float elapse, int game_frame)
    {
        var keysToRemove = new List<string>();
        foreach (var keyValue in m_DownloadingWWWs)
        {
            WWW download = keyValue.Value;

            if (download.error != null)
            {
                Log.Warning("[ab]加载失败:" + keyValue.Key);
                m_DownloadingErrors.Add(keyValue.Key, string.Format("Failed downloading bundle {0} from {1}: {2}", keyValue.Key, download.url, download.error));
                keysToRemove.Add(keyValue.Key);
                continue;
            }

            if (download.isDone)
            {
                AssetBundle bundle = download.assetBundle;
                if (bundle == null)
                {
                    Log.Warning("[ab]加载失败:" + keyValue.Key);
                    m_DownloadingErrors.Add(keyValue.Key, string.Format("{0} is not a valid asset bundle.", keyValue.Key));
                    keysToRemove.Add(keyValue.Key);
                    continue;
                }
                Log.Info("[ab]加载完资源:" + keyValue.Key);
                m_LoadedAssetBundles.Add(keyValue.Key, new LoadedAssetBundle(download.assetBundle));
                keysToRemove.Add(keyValue.Key);
            }
        }

        foreach (var key in keysToRemove)
        {
            WWW download = m_DownloadingWWWs[key];
            m_DownloadingWWWs.Remove(key);
            download.Dispose();
        }

        for (int i = 0; i < m_InProgressOperations.Count; )
        {
            if (!m_InProgressOperations[i].Update())
            {
                m_InProgressOperations.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }
    }
    /// <summary>
    /// manifest加载完成
    /// </summary>
    public void OnAssetBundleManifest(AssetBundleManifest manifest)
    {
        if (manifest != null)
        {
            m_AssetBundleManifest = manifest;
            LoadAllAssetAsync();
        }
    }

    public LoadedAssetBundle GetLoadedAssetBundle(string assetBundleName, out string error)
    {
        if (m_DownloadingErrors.TryGetValue(assetBundleName, out error))
        {
            return null;
        }

        LoadedAssetBundle bundle = null;
        m_LoadedAssetBundles.TryGetValue(assetBundleName, out bundle);
        if (bundle == null)
        {
            return null;
        }

        string[] dependencies = null;
        if (!m_Dependencies.TryGetValue(assetBundleName, out dependencies))
        {
            return bundle;
        }

        foreach (var dependency in dependencies)
        {
            if (m_DownloadingErrors.TryGetValue(assetBundleName, out error))
            {
                return bundle;
            }

            LoadedAssetBundle dependentBundle;
            m_LoadedAssetBundles.TryGetValue(dependency, out dependentBundle);
            if (dependentBundle == null)
            {
                return null;
            }
        }

        return bundle;
    }

    /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～加载～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
    /// <summary>
    /// 加载所有资源
    /// </summary>
    public void LoadAllAssetAsync()
    {
        string[] all_files = m_AssetBundleManifest.GetAllAssetBundles();
        if(all_files.Length > 0)
        {
            for(int i = 0; i < all_files.Length; ++i)
            {
                LoadAssetAsync(all_files[i], all_files[i], typeof(Object));
            }
        }
    }
    /// <summary>
    /// 加载单个
    /// </summary>
    /// <param name="assetBundleName"></param>
    /// <param name="assetName"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public AssetBundleLoadAssetOperation LoadAssetAsync(string assetBundleName, string assetName, System.Type type)
    {
        Log.Info("Loading " + assetName + " from " + assetBundleName + " bundle");

        AssetBundleLoadAssetOperation operation = null;
        assetBundleName = RemapVariantName(assetBundleName);
        LoadAssetBundle(assetBundleName);
        operation = new AssetBundleLoadAssetOperationFull(assetBundleName, assetName, type);
        m_InProgressOperations.Add(operation);
        return operation;
    }
    private void LoadAssetBundle(string assetBundleName, bool isLoadingAssetBundleManifest = false)
    {
        Log.Info("Loading Asset Bundle " + (isLoadingAssetBundleManifest ? "Manifest: " : ": ") + assetBundleName);
        if (!isLoadingAssetBundleManifest)
        {
            if (m_AssetBundleManifest == null)
            {
                Log.Error("[ab]Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
                return;
            }
        }

        bool isAlreadyProcessed = LoadAssetBundleInternal(assetBundleName, isLoadingAssetBundleManifest);

        if (!isAlreadyProcessed && !isLoadingAssetBundleManifest)
        {
            LoadDependencies(assetBundleName);
        }
    }

    public void UnloadAssetBundle(string assetBundleName)
    {
        UnloadAssetBundleInternal(assetBundleName);
        UnloadDependencies(assetBundleName);
    }

    private bool LoadAssetBundleInternal(string assetBundleName, bool isLoadingAssetBundleManifest)
    {
        LoadedAssetBundle bundle = null;
        m_LoadedAssetBundles.TryGetValue(assetBundleName, out bundle);
        if (bundle != null)
        {
            bundle.m_ReferencedCount++;
            return true;
        }
        if (m_DownloadingWWWs.ContainsKey(assetBundleName))
        {
            return true;
        }

        WWW download = null;
        string url = m_BaseDownloadingURL + assetBundleName;
        Log.Info("[ab]请求加载url:" + url);
        if (isLoadingAssetBundleManifest)
        {
            download = new WWW(url);
        }
        else
        {
            download = WWW.LoadFromCacheOrDownload(url, m_AssetBundleManifest.GetAssetBundleHash(assetBundleName), 0);
        }

        m_DownloadingWWWs.Add(assetBundleName, download);

        return false;
    }

    private void UnloadAssetBundleInternal(string assetBundleName)
    {
        string error;
        LoadedAssetBundle bundle = GetLoadedAssetBundle(assetBundleName, out error);
        if (bundle == null)
        {
            return;
        }

        if (--bundle.m_ReferencedCount == 0)
        {
            bundle.m_AssetBundle.Unload(false);
            m_LoadedAssetBundles.Remove(assetBundleName);
            Log.Info("[ab]" + assetBundleName + " has been unloaded successfully");
        }
    }

    private void LoadDependencies(string assetBundleName)
    {
        if (m_AssetBundleManifest == null)
        {
            Log.Error("[ab]Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
            return;
        }

        string[] dependencies = m_AssetBundleManifest.GetAllDependencies(assetBundleName);
        if (dependencies.Length == 0)
            return;

        for (int i = 0; i < dependencies.Length; i++)
        {
            dependencies[i] = RemapVariantName(dependencies[i]);
        }

        m_Dependencies.Add(assetBundleName, dependencies);
        for (int i = 0; i < dependencies.Length; i++)
        {
            LoadAssetBundleInternal(dependencies[i], false);
        }
    }

    private void UnloadDependencies(string assetBundleName)
    {
        string[] dependencies = null;
        if (!m_Dependencies.TryGetValue(assetBundleName, out dependencies))
        {
            return;
        }

        foreach (var dependency in dependencies)
        {
            UnloadAssetBundleInternal(dependency);
        }

        m_Dependencies.Remove(assetBundleName);
    }

    protected string RemapVariantName(string assetBundleName)
    {
        string[] bundlesWithVariant = m_AssetBundleManifest.GetAllAssetBundlesWithVariant();

        string[] split = assetBundleName.Split('.');

        int bestFit = int.MaxValue;
        int bestFitIndex = -1;
        for (int i = 0; i < bundlesWithVariant.Length; i++)
        {
            string[] curSplit = bundlesWithVariant[i].Split('.');
            if (curSplit[0] != split[0])
                continue;

            int found = System.Array.IndexOf(m_ActiveVariants, curSplit[1]);
            if (found == -1)
                found = int.MaxValue - 1;

            if (found < bestFit)
            {
                bestFit = found;
                bestFitIndex = i;
            }
        }

        if (bestFit == int.MaxValue - 1)
        {
            Log.Warning("[ab]Ambigious asset bundle variant chosen because there was no matching active variant: " + bundlesWithVariant[bestFitIndex]);
        }

        if (bestFitIndex != -1)
        {
            return bundlesWithVariant[bestFitIndex];
        }
        else
        {
            return assetBundleName;
        }
    }

    /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～设置路径～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
    public void SetSourceAssetBundleDirectory(string relativePath)
    {
        BaseDownloadingURL = AssetBundleUtil.GetStreamingAssetsPath() + relativePath;
    }

    public void SetSourceAssetBundleURL(string absolutePath)
    {
        BaseDownloadingURL = absolutePath + AssetBundleUtil.GetPlatformName() + "/";
    }

    public void SetDevelopmentAssetBundleServer()
    {
        TextAsset urlFile = Resources.Load("AssetBundleServerURL") as TextAsset;
        string url = (urlFile != null) ? urlFile.text.Trim() : null;
        if (url == null || url.Length == 0)
        {
            Log.Error("[ab]Development Server URL could not be found.");
        }
        else
        {
            SetSourceAssetBundleURL(url);
        }
    }
    /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～get/set～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
    public string BaseDownloadingURL
    {
        get { return m_BaseDownloadingURL; }
        set { m_BaseDownloadingURL = value; }
    }

    public string[] ActiveVariants
    {
        get { return m_ActiveVariants; }
        set { m_ActiveVariants = value; }
    }
}
public class LoadedAssetBundle
{
    public AssetBundle m_AssetBundle;
    public int m_ReferencedCount;

    public LoadedAssetBundle(AssetBundle assetBundle)
    {
        m_AssetBundle = assetBundle;
        m_ReferencedCount = 1;
    }
}
