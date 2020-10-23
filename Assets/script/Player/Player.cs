using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    public CentralDataViewer CentralDataViewer;//新增主控台cs
    public float angle = 180;
    //public float speed = 20;

    public Vector3 jump;
    public float jumpForce = 0.5f;
    public bool isGrounded;
    Rigidbody rb;
    
    public float speed2 = 0f;//物體的移動速度

    public Animator anim;
    public NavMeshAgent agent;    //宣告NavMeshAgent
    public Transform target_obj;    //目標物件

    public Ground Ground_CS;
    public Transform Ground;
    //private bool isturn = false;
    //private Quaternion targetRot;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        jump = new Vector3(0.0f, 2.0f, 0.0f);

        anim.SetBool("isWalking", false);
    }

    void OnCollisionStay()
    {
        isGrounded = true;
    }
    void OnCollisionExit()
    {
        isGrounded = false;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Translate(0, 0, 0.02f);
        //anim.SetBool("param_idletowalk", true);


        if (Input.GetMouseButtonDown(0))
        {
            Ray r_ = Camera.main.ScreenPointToRay(Input.mousePosition);//朝螢幕射ray回傳滑鼠坐標
            RaycastHit hit_;

            anim.SetBool("isWalking", true);
            if (Physics.Raycast(r_, out hit_, 500))//(Ray坐標,回傳變數,最遠距離)
            {
                //target_obj = hit_.transform;
                agent.speed = 6f;
                agent.SetDestination(hit_.point);//Nav導航照回傳變數的point(點)走

            }
            else if (Vector3.Distance(agent.transform.position, target_obj.position) <= agent.stoppingDistance)//到達目的地 停止移動
            {

            //播放愛麗絲發呆動畫    anim.SetBool("isWalking", false);
            }
        }


        if (Input.GetKey(KeyCode.W))
        {
            if (!Ground_CS.isturn)
            {

                //targetRot = Ground.transform.rotation * Quaternion.Euler(Vector3.up * -90);//指定變數要旋轉到的角度
                //targetRot = Ground.transform.rotation * Quaternion.Euler(Vector3.left * 90);//指定變數要旋轉到的角度
                Ground_CS.isturn = true;
            }

            //transform.Translate(0, 0, 0.2f);
        }  // 按住 上鍵 時，物件每個 frame 朝自身 z 軸方向移動 0.1 公尺

       

        if (Input.GetKeyDown(KeyCode.Q))
        {


            if (!agent.isStopped)
            {
                agent.ResetPath();//清除路徑
                agent.isStopped = true;//navMeshAgent自動尋路停止:開
                agent.updatePosition = false;////更新座標
                agent.updateRotation = false;//更新旋轉值
            }
            else
            {
                agent.isStopped = false;//navMeshAgent自動尋路停止:關 
                agent.nextPosition = transform.position;////導航座標設為愛麗絲座標
                agent.updatePosition = true;//更新座標
                agent.updateRotation = true;//更新旋轉值
            }


        }

       


        //if (Input.GetKey(KeyCode.E))
        //{ transform.Rotate(0, 1, 0); } // 按住 左鍵 時，物件每個 frame 以自身 y 軸為軸心旋轉 -3 度
        //if (Input.GetKey(KeyCode.Q))
        //{ transform.Rotate(0, -1, 0); } // 按住 右鍵 時，物件每個 frame 以自身 y 軸為軸心旋轉 3 度 

        //if (transCamera != null) //鏡頭跟隨player視角連動
        //{
        //    if ((transCamera.localEulerAngles.y < 180) || (transCamera.localEulerAngles.x > 180 && Input.GetAxis("Mouse Y") < 0)) //判斷y<180(前方)為true 或 
        //        transCamera.Rotate(Vector3.right * (-Input.GetAxis("Mouse Y")) * angle * Time.deltaTime);//相機上下 旋轉
        //}
        //if ((transCamera.localEulerAngles.y < 180) || (transCamera.localEulerAngles.x < 90 && Input.GetAxis("Mouse Y") > 0)) //只判斷往下是否到底
        //    transCamera.Rotate(Vector3.right * (-Input.GetAxis("Mouse Y")) * angle * Time.deltaTime);//相機上下 旋轉

        //transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * angle * Time.deltaTime);//相機旋轉 左右旋轉

        ////Debug.Log(transCamera.localEulerAngles);





    }


       
   
    
   
}

