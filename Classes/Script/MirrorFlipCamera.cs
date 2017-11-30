using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 镜像翻转相机，用于镜子等情况
/// @author hannibal
/// @time 2016-11-4
/// </summary>
[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class MirrorFlipCamera : MonoBehaviour
{
    new Camera camera;
    public bool flipHorizontal;
    void Awake()
    {
        camera = GetComponent<Camera>();
    }
    void OnPreCull()
    {
        camera.ResetWorldToCameraMatrix();
        camera.ResetProjectionMatrix();
        Vector3 scale = new Vector3(flipHorizontal ? -1 : 1, 1, 1);
        camera.projectionMatrix = camera.projectionMatrix * Matrix4x4.Scale(scale);
    }
    void OnPreRender()
    {
        GL.invertCulling = flipHorizontal;
    }

    void OnPostRender()
    {
        GL.invertCulling = false;
    }
}