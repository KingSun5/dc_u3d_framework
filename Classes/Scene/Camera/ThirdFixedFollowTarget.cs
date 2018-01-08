using UnityEngine;
using System.Collections;

/// <summary>
/// 第三人称相机：固定视角+固定区域
/// @author hannibal
/// @time 2014-11-14
/// </summary>
public class ThirdFixedFollowTarget : MonoBehaviour 
{
    /// <summary>
    /// 摄像机平滑移动的时间
    /// </summary>
    public float m_smoothTime = 0.01f;
    /// <summary>
    /// 离注视目标的偏移距离
    /// </summary>
    public Vector3 m_offsetTarget = new Vector3(0, 0, -10);

    /// <summary>
    /// 摄像机要跟随的对象
    /// </summary>
    private Transform m_targetObj = null;
    /// <summary>
    /// 相机可移动区域
    /// </summary>
    private Bounds m_cameraArea;

    void Awake()
    {
    }

    void OnEnable()
    {
        EventController.AddEventListener(CameraEvent.FOLLOW_TARGET, OnBindTarget);
        EventController.AddEventListener(CameraEvent.MOVE_AREA, OnMapSize);
    }

    void OnDisable()
    {
        EventController.RemoveEventListener(CameraEvent.FOLLOW_TARGET, OnBindTarget);
        EventController.RemoveEventListener(CameraEvent.MOVE_AREA, OnMapSize);
    }
    /**绑定目标*/
    void OnBindTarget(GameEvent evt)
    {
        Log.Info("FllowTarget::OnBindTarget");

        m_targetObj = evt.Get<Transform>(0);
    }
    /**地图大小*/
    void OnMapSize(GameEvent evt)
    {
        m_cameraArea = evt.Get<Bounds>(0);
    }

    float off_x = 0;
    float off_y = 0;
    Vector3 camera_velocity = Vector3.zero;
    void Update()
    {
        if (m_targetObj != null && m_cameraArea.size.magnitude > 0)
        {
            Vector3 world_target = (m_targetObj.position + m_offsetTarget);
            Vector3 target_pos = m_targetObj.position;

            float half_w = Screen.width * 0.5f;
            float half_h = Screen.height * 0.5f;

            if (off_x == 0)
            {
                Vector3 origin = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
                Vector3 rigth_top = Camera.main.ScreenToWorldPoint(new Vector3(half_w, half_h, 0));
                off_x = rigth_top.x - origin.x;
                off_y = rigth_top.y - origin.y;
            }
            //超出有效区域
            if (world_target.x - off_x < m_cameraArea.min.x)
            {
                target_pos.x = m_cameraArea.min.x + off_x;
            }
            else if (world_target.x + off_x > m_cameraArea.max.x)
            {
                target_pos.x = m_cameraArea.max.x - off_x;
            }
            if (world_target.y - off_y < m_cameraArea.min.y)
            {
                target_pos.y = m_cameraArea.min.y + off_y;
            }
            else if (world_target.y + off_y > m_cameraArea.max.y)
            {
                target_pos.y = m_cameraArea.max.y - off_y;
            }
            //移动相机
            transform.position = Vector3.SmoothDamp(transform.position, target_pos + m_offsetTarget, ref camera_velocity, m_smoothTime);
        }
    }
}
