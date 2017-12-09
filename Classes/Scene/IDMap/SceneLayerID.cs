using UnityEngine;
using System.Collections;

/// <summary>
/// 场景层级定义，对应unity layer
/// @author hannibal
/// @time 2016-12-19
/// </summary>
public class SceneLayerID
{
    public const string Default         = "Default";
    public const string UI              = "UI";
    public const string Scene           = "Scene";
    public const string SceneUnreal     = "SceneUnreal";
    public const string Effect          = "Effect";
    public const string RTT             = "RTT";
    public const string Role            = "Role";
    public const string Role1           = "Role1";
    public const string Role2           = "Role2";
    public const string Item            = "Item";
    public const string Item1           = "Item1";
    public const string Item2           = "Item2";
    public const string Bullet          = "Bullet";
    public const string Bullet1         = "Bullet1";
    public const string Bullet2         = "Bullet2";
    public const string Block_Unreal    = "Block_Unreal";
    public const string Block_Bounce    = "Block_Bounce";
    public const string Block_Destroy   = "Block_Destroy";

    static public int GetSceneMask()
    {
        return (1 << LayerMask.NameToLayer(SceneLayerID.Scene));
    }
    static public int GetSceneRoleMask()
    {
        return (1 << LayerMask.NameToLayer(SceneLayerID.Scene) | 1 << LayerMask.NameToLayer(SceneLayerID.Role));
    }
}


/// <summary>
/// 2d对象z-depth
/// </summary>
public class ObjectDepthID
{
    public const float TERRAIN          = -0.0f;
    public const float TERRAIN_EFFECT   = -0.1f;
    public const float MAP_OBJ          = -0.2f;
    public const float MAP_ITEM         = -0.3f;
    public const float ROLE             = -0.4f;
    public const float PLAYER           = -0.5f;
    public const float BULLET           = -0.6f;
    public const float EFFECT           = -0.7f;
    public const float TOP              = -0.8f;

}