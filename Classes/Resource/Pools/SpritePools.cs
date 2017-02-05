using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Sprite对象池
/// @author hannibal
/// @time 2017-1-20
/// </summary>
public class SpritePools
{
    private static Dictionary<string, List<Sprite>> m_DicFile2Pool = new Dictionary<string, List<Sprite>>();

    /// <summary>
    /// 产生对象
    /// </summary>
    /// <param name="file">查找键值，注意：内部会修改sprite的name，如果使用了这个接口，外部就不要再修改name</param>
    /// <returns></returns>
    public static Sprite Get(string file)
    {
        if (file.Length == 0) return null;

        Sprite spawnItem = null;
        List<Sprite> itemArray = null;
        ///1.查找pools
        if (m_DicFile2Pool.TryGetValue(file, out itemArray))
        {
            if(itemArray.Count > 0)
            {
                spawnItem = itemArray[0];
                itemArray.RemoveAt(0);
            }
        }
        ///2.创建新的
        if (spawnItem == null)
        {
            spawnItem = ResourceLoaderManager.Instance.LoadSprite(file);
        }
        if (spawnItem != null) spawnItem.name = file;
        return spawnItem;
    }

    /// <summary>
    /// 回收
    /// </summary>
    public static void Recover(Sprite obj)
    {
        if (obj == null || string.IsNullOrEmpty(obj.name)) return;

        List<Sprite> itemArray = null;
        if (!m_DicFile2Pool.TryGetValue(obj.name, out itemArray))
        {
            itemArray = new List<Sprite>();
            m_DicFile2Pool[obj.name] = itemArray;
        }
        if (!itemArray.Contains(obj))itemArray.Add(obj);
    }

    public static void Clear()
    {
        foreach (var item_list in m_DicFile2Pool)
        {
            foreach (var obj in item_list.Value)
            {
                if (obj != null) Resources.UnloadAsset(obj);
            }
        }
        m_DicFile2Pool.Clear();
    }
}
