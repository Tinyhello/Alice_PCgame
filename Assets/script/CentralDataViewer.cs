using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentralDataViewer : MonoBehaviour
{
    //新增存取子
    public GameObject PlayerObj;//重生玩家
    public Transform RebirthPos;//重生玩家的座標
    public bool Player_isDead = false;//判斷玩家死亡狀態
    public int PlayerAlive = 1;//宣告靜態變數 玩家數量
    private float WaitRebirthtime = 3f;//主角死亡重生等待時間
    public GameObject[] EnemyObjs;
    public Transform[] EnemyResetPos;
    public Transform Ground;//重生位置的父物件
    public Move_stuff_01[] Move_stuff_01_cs;//宣告機關cs
    void Update()
    {
        if (Player_isDead == true)
        {
            if (WaitRebirthtime > 0) //等待時間>0就等待時間-每禎的平均時間
            {
                WaitRebirthtime -= Time.deltaTime;//等待時間-1
                if (WaitRebirthtime <= 0f)
                {
                    if(PlayerAlive > -1 && PlayerAlive < 1)
                    { 
                    GameObject RebirthObj = Instantiate(PlayerObj, RebirthPos.position, RebirthPos.rotation,Ground.transform);//重生愛麗絲的座標//重生愛麗絲在父物件節點下
                    //RebirthObj.transform.localScale = new Vector3(1, 1, 1);//設定愛麗絲原本尺寸
                    PlayerAlive++;//愛麗絲計數++
                    }

                    Player_isDead = false;//重設愛麗絲死亡狀態 
                    WaitRebirthtime = 3f;//重設死亡等待時間
                    EnemyObjs[0].transform.position = EnemyResetPos[0].position;//重設小兵1座標
                    EnemyObjs[1].transform.position = EnemyResetPos[1].position;//重設小兵2座標
                    
                    for(int i=0;i<3;i++)
                    { 
                    Move_stuff_01_cs[i].Reset_MoveElement();//呼叫機關cs.的重置機關x3個
                    }

                }
            }
        }
    }

   
}
