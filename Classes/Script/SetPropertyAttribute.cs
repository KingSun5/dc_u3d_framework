using UnityEngine;
using System.Collections;

/// <summary>
/// 与SetPropertyDrawer配合使用
/// @author hannibal
/// @time 2017-1-17
/// </summary>
public class SetPropertyAttribute : PropertyAttribute
{
    public string Name { get; private set; }
    public bool IsDirty { get; set; }

    public SetPropertyAttribute(string name)
    {
        this.Name = name;
    }
}