using UnityEngine;
using System.Collections;

/// <summary>
/// 2D平行相机调整size
/// @author hannibal
/// @time 2016-12-22
/// </summary>
public class Camera2DAdjustSizeScript : MonoBehaviour
{
    public float devHeight = 9.6f;
    public float devWidth = 6.4f;

    // Use this for initialization
    void Start()
    {

        float screenHeight = Screen.height;

        Debug.Log("screenHeight = " + screenHeight);

        //this.GetComponent<Camera>().orthographicSize = screenHeight / 200.0f;

        float orthographicSize = this.GetComponent<Camera>().orthographicSize;

        float aspectRatio = Screen.width * 1.0f / Screen.height;

        float cameraWidth = orthographicSize * 2 * aspectRatio;

        Debug.Log("cameraWidth = " + cameraWidth);

        if (cameraWidth < devWidth)
        {
            orthographicSize = devWidth / (2 * aspectRatio);
            Debug.Log("new orthographicSize = " + orthographicSize);
            this.GetComponent<Camera>().orthographicSize = orthographicSize;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}