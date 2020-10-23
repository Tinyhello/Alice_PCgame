using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transparent_wall_Back : MonoBehaviour
{
    public Transform Ground;//迷宮物件
    public Ground Ground_CS;//迷宮玩家script
    public Player Player_CS;//玩家script
    private Quaternion targetRot;//讓迷宮的指定角度

    void Start()
    {
        
    }

    
    void Update()
    {
        if (Ground_CS.isturn)
        {
            if (!Player_CS.agent.isStopped)//agent.isStopped為false
            {
                Player_CS.agent.ResetPath();//清除路徑
                Player_CS.agent.isStopped = true;//navMeshAgent自動尋路停止:開
                Player_CS.agent.updatePosition = false;////更新座標
                Player_CS.agent.updateRotation = false;//更新旋轉值
            }

            //Ground.transform.rotation = Quaternion.Lerp(Ground.transform.rotation, targetRot, Time.deltaTime);//Quaternion.Lerp 將目標物以一個位置比例放置在兩物件之間
            Ground.transform.rotation = Quaternion.RotateTowards(Ground.transform.rotation, targetRot, Time.deltaTime * 25f);//Quaternion.RotateTowards 讓物件旋轉至指定角度

            if (Quaternion.Angle(Ground.transform.rotation, targetRot) <= 1f)//(Quaternion.Angle歐拉角(原本物件旋轉值,你指定的旋轉值 ) <= 1f 只要原旋轉值比指定旋轉值<=1f就執行)
            {
                if (Player_CS.agent.isStopped) //agent.isStopped為true
                {
                    Player_CS.agent.isStopped = false;//navMeshAgent自動尋路停止:關 
                    Player_CS.agent.nextPosition = transform.position;////導航座標設為愛麗絲座標
                    Player_CS.agent.updatePosition = true;//更新座標
                    Player_CS.agent.updateRotation = true;//更新旋轉值
                }
                Debug.Log("開啓Navmesh");
                Ground.transform.rotation = targetRot;
                Ground_CS.isturn = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Alice") && !Ground_CS.isturn)
        {
            Debug.Log("Touch back");
            targetRot = Ground.transform.rotation * Quaternion.Euler(Vector3.right * -90);//指定變數要旋轉到的角度
            Ground_CS.isturn = true;
        }
    }
}
