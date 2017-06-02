using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class UnityTestDll : MonoBehaviour
{

    [DllImport("framework_dll")]
    private static extern int TestAdd(int x, int y);


    // Use this for initialization  
    void Start()
    {

    }

    // Update is called once per frame  
    void Update()
    {

    }

    void OnGUI()
    {
        int i = TestAdd(5, 7);
        GUI.Button(new Rect(1, 1, 200, 100), "this dll i = 5+7, i is" + i);
    }
}  
