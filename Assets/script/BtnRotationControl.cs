using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnRotationControl : MonoBehaviour
{
    public GameObject CameraObj;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Maze_LeftRotation()
    {
    if(CameraObj != null)
        {
            CameraObj.transform.Rotate(new Vector3(0, 10, 0), 30 * Time.deltaTime);
        }
    }

    public void Maze_RightRotation()
    {
        if (CameraObj != null)
        {
            CameraObj.transform.Rotate(new Vector3(0, 10, 0), -30 * Time.deltaTime);
        }
    }
}
