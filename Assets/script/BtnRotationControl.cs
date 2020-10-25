using System.Collections;
using System.Collections.Generic;
using UnityEngine;
<<<<<<< HEAD
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class BtnRotationControl : MonoBehaviour  
{
    public GameObject CameraObj;
    public bool isOver = false;
    public bool LeftRotation = false;
    public bool RightRotation = false;
    public void MouseEnter()
    {
        isOver = true;
    }
    public void MouseExit()
    {
        isOver = false;
        LeftRotation = false;
        RightRotation = false;
    }
    public void BtnLeftRotation()
    {
        LeftRotation = true;
    }
    public void BtnRightRotation()
    {
        RightRotation = true;
    }

    private void Update()
    {
        if (isOver)
        {
            //鼠標移動到物體上時的事件
            if (Input.GetMouseButton(0))
            {
                if (CameraObj != null&& RightRotation == true)
                {
                    CameraObj.transform.Rotate(new Vector3(0, 10, 0), -30 * Time.deltaTime);
                }
                else if (CameraObj != null && LeftRotation == true)
                {
                    CameraObj.transform.Rotate(new Vector3(0, 10, 0), 30 * Time.deltaTime);
                }
            }
        }
    }


    //public void OnPointerEnter()    //滑鼠移入
    //{
    //    if (CameraObj != null)
    //    {
    //        CameraObj.transform.Rotate(new Vector3(0, 10, 0), 30 * Time.deltaTime);
    //    }
    //}

    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    if (CameraObj != null)
    //    {
    //        CameraObj.transform.Rotate(new Vector3(0, 10, 0), -30 * Time.deltaTime);
    //    }
    //}
=======

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
>>>>>>> 116c5f4e0ef031839a103887efccaa5c6483f4c2
}
