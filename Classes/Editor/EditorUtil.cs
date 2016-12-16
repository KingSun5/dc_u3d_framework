using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;

public class EditorUtil : Editor
{
    [MenuItem ("Custom Editor/Memeory")]
    public static void menu()
    {
        Object target = Selection.activeObject as Object;
        var type = Types.GetType("UnityEditor.TextureUtil", "UnityEditor.dll");
        MethodInfo methodInfo = type.GetMethod("GetStorageMemorySize", BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);

        Debug.Log("内存占用：" + EditorUtility.FormatBytes(Profiler.GetRuntimeMemorySize(Selection.activeObject)));
        Debug.Log("硬盘占用：" + EditorUtility.FormatBytes((int)methodInfo.Invoke(null, new object[] { target })));
    }
}
