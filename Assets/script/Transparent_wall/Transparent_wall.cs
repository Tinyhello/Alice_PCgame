using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transparent_wall : MonoBehaviour
{
    public Transform BackGround;
    public Transform Player;
    public Player Player_sc;
       
    

   
    // Start is called before the first frame update
    void Start()
    {
       
    }

    private bool isturn = false;
    private Quaternion targetRot;
   
    void Update()
    {
        //if (isRotate_Foward)
        //    Rotate_F();
        //if (isRotate_Back)
        //    Rotate_B();
        //if (isRotate_Left)
        //    Rotate_L();
        //if (isRotate_Right)
        //    Rotate_R();

        if (Input.GetKeyDown(KeyCode.A)&&!isturn)
        {
            
            targetRot = BackGround.transform.rotation*Quaternion.Euler(Vector3.right*90);//指定變數要旋轉到的角度
            isturn = true;
        }
        else if (Input.GetKeyDown(KeyCode.D) && !isturn)
        {

            targetRot = BackGround.transform.rotation * Quaternion.Euler(Vector3.right * -90);//指定變數要旋轉到的角度
            isturn = true;
        }
        else if (Input.GetKeyDown(KeyCode.W) && !isturn)
        {

            targetRot = BackGround.transform.rotation * Quaternion.Euler(Vector3.up * 90);//指定變數要旋轉到的角度
            isturn = true;
        }
        else if (Input.GetKeyDown(KeyCode.S) && !isturn)
        {

            targetRot = BackGround.transform.rotation * Quaternion.Euler(Vector3.up * -90);//指定變數要旋轉到的角度
            isturn = true;
        }





        if (isturn)
        {

            //BackGround.transform.rotation = Quaternion.Lerp(BackGround.transform.rotation, targetRot, Time.deltaTime);//Quaternion.Lerp 將目標物以一個位置比例放置在兩物件之間
            BackGround.transform.rotation = Quaternion.RotateTowards(BackGround.transform.rotation, targetRot, Time.deltaTime*25f);//Quaternion.RotateTowards 讓物件旋轉至某個角度

            if (Quaternion.Angle(BackGround.transform.rotation, targetRot) <= 1f)//(Quaternion.Angle歐拉角(原本物件旋轉值,你指定的旋轉值 ) <= 1f 只要原旋轉值比指定旋轉值<=1f就執行)
            {

                BackGround.transform.rotation = targetRot;
                isturn = false;
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        //if (other.CompareTag("Player") && Player_sc.isFoward == true)
        //{
        //    Debug.Log("碰到前面");
        //    isRotate_Foward = true;
        //    isRotate_Back = false;
        //    BackGround.transform.Rotate(new Vector3(0f, 0f, 1f));
        //}
        //else if (other.CompareTag("Player") )
        //{
        //    Debug.Log("碰到後面");
        //    isRotate_Back = true;
        //    isRotate_Foward = false;
        //    BackGround.transform.Rotate(new Vector3(-1f, 0f, 0f));
        //}
        //else if (other.CompareTag("Player") && Player_sc.isLeft == true)
        //{
        //    Debug.Log("碰到左邊");
        //    isRotate_Left = true;
        //    isRotate_Right = false;
        //    BackGround.transform.Rotate(new Vector3(-1f, 0f, 0f));
        //}
        //else if (other.CompareTag("Player") && Player_sc.isRight == true)
        //{
        //    Debug.Log("碰到右邊");
        //    isRotate_Right = true;
        //    isRotate_Left = false;
        //    BackGround.transform.Rotate(new Vector3(1f, 0f, 0f));
        //}
    }
    //private void Rotate_F(bool R1 = false)
    //{
    //    if (isRotate_Foward == true && BackGround.transform.localEulerAngles.z >= 0 && BackGround.transform.localEulerAngles.z <= 360)
    //    {
    //        BackGround.transform.Rotate(new Vector3(0, 0, 1), 30 * Time.deltaTime);
    //        Player.transform.Translate(Vector3.forward * 1.11f * Time.deltaTime);
    //        Player.transform.Rotate(new Vector3(1, 0, 0), 30 * Time.deltaTime);

    //        if (BackGround.transform.localEulerAngles.z >= 90 && BackGround.transform.localEulerAngles.z <= 91)
    //        {
    //            isRotate_Foward = R1;
    //        }
    //        else if (BackGround.transform.localEulerAngles.z >= 180 && BackGround.transform.localEulerAngles.z <= 181)
    //        {
    //            isRotate_Foward = R1;
    //        }
    //        else if (BackGround.transform.localEulerAngles.z >= 270 && BackGround.transform.localEulerAngles.z <= 271)
    //        {
    //            isRotate_Foward = R1;
    //        }
    //        else if (BackGround.transform.localEulerAngles.z >= 0 && BackGround.transform.localEulerAngles.z <= 1)
    //        {
    //            isRotate_Foward = R1;
    //        }
    //    }
    //}
    //private void Rotate_B(bool R2 = false)
    //{
    //    if (isRotate_Back == true && BackGround.transform.localEulerAngles.x >= 0 && BackGround.transform.localEulerAngles.x <= 360)
    //    {
    //        BackGround.transform.Rotate(new Vector3(-1, 0, 0), 30 * Time.deltaTime);
    //        Player.transform.Translate(Vector3.back * 1.11f * Time.deltaTime);
    //        Player.transform.Rotate(new Vector3(-1, 0, 0), 30 * Time.deltaTime);
    //        Debug.Log(BackGround.transform.localEulerAngles);
    //        if (BackGround.transform.localEulerAngles.x >= -90 && BackGround.transform.localEulerAngles.x <= -88)
    //        {
    //            isRotate_Back = R2;
    //        }
    //        //else if (BackGround.transform.localEulerAngles.x >= 180 && BackGround.transform.localEulerAngles.x >= 0)
    //        //{
    //        //    isRotate_Back = R2;
    //        //}
    //        //else if (BackGround.transform.localEulerAngles.x >= -89 && BackGround.transform.localEulerAngles.x <= -90)
    //        //{
    //        //    isRotate_Back = R2;
    //        //}
    //        //else if (BackGround.transform.localEulerAngles.x <= -180 /*&& BackGround.transform.localEulerAngles.x <= -90*/)
    //        //{
    //        //    isRotate_Back = R2;
    //        //}

    //    }
    //}

    //private void Rotate_L(bool R3 = false)//NG待修正
    //{
    //    if (isRotate_Left == true && BackGround.transform.localEulerAngles.x >= 0 && BackGround.transform.localEulerAngles.x <= 360)
    //    {
    //        BackGround.transform.Rotate(new Vector3(-1, 0, 0), 30 * Time.deltaTime);
    //        Player.transform.Translate(Vector3.forward * 1f * Time.deltaTime);
    //        Player.transform.Rotate(new Vector3(0, 0, 1), 30 * Time.deltaTime);

    //        if (BackGround.transform.localEulerAngles.x >= 89 && BackGround.transform.localEulerAngles.x <= 91)
    //        {
    //            isRotate_Left = R3;
    //        }
    //        else if (BackGround.transform.localEulerAngles.x >= 179 && BackGround.transform.localEulerAngles.x <= 181)
    //        {
    //            isRotate_Left = R3;
    //        }
    //        else if (BackGround.transform.localEulerAngles.x >= 269 && BackGround.transform.localEulerAngles.x <= 271)
    //        {
    //            isRotate_Left = R3;
    //        }
    //        else if (BackGround.transform.localEulerAngles.x >= 1 && BackGround.transform.localEulerAngles.x <= 1)
    //        {
    //            isRotate_Left = R3;
    //        }
    //    }
    //}

    //private void Rotate_R(bool R4 = false)
    //{
    //    if (isRotate_Right == true && BackGround.transform.localEulerAngles.x >= 0 && BackGround.transform.localEulerAngles.z <= 360)
    //    {
    //        BackGround.transform.Rotate(new Vector3(1, 0, 0), 30 * Time.deltaTime);
    //        Player.transform.Translate(Vector3.back * 1f * Time.deltaTime);
    //        Player.transform.Rotate(new Vector3(0, 0, -1), 30 * Time.deltaTime);

    //        if (BackGround.transform.localEulerAngles.x >= 89 && BackGround.transform.localEulerAngles.x <= 90)
    //        {
    //            isRotate_Right = R4;
    //        }
    //        else if (BackGround.transform.localEulerAngles.x >= 180 && BackGround.transform.localEulerAngles.x >= 0)
    //        {
    //            isRotate_Right = R4;
    //        }
    //        else if (BackGround.transform.localEulerAngles.x >= -89 && BackGround.transform.localEulerAngles.x <= -90)
    //        {
    //            isRotate_Right = R4;
    //        }
    //        else if (BackGround.transform.localEulerAngles.x <= -180 /*&& BackGround.transform.localEulerAngles.x <= -90*/)
    //        {
    //            isRotate_Right = R4;
    //        }
    //    }
    //}
}
