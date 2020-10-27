using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CentralDataViewer : MonoBehaviour
{
    public GameObject RebirthPlayerObj;//重生玩家
    public Transform RebirthPos;//重生愛麗絲的座標
    public bool Player_isDead = false;//判斷愛麗絲死亡狀態
    public bool Player_isBorn = false;//判斷愛麗絲重生
    public int PlayerAlive = 1;//宣告靜態變數 愛麗絲數量
    private float WaitRebirthtime = 0.3f;//愛麗絲死亡重生等待時間

    public GameObject[] Item;
    public GameObject[] EnemyObjs;
    public Transform[] EnemyResetPos;
    public Transform Ground;//重生位置的父物件
    public Move_stuff_01[] Move_stuff_01_cs;//宣告機關cs

    public GameObject PnlPlayerDead;//設定panel 愛麗絲死亡
    public GameObject BtnReStart;//設定按鈕愛麗絲重生

    public GameObject[] CameraObj;
    public ParticleSystem Particle;

    public Animator Bridge_animator;//宣告橋的動畫


    void Update()
    {
        if (Player_isDead)
        {
            PnlPlayerDead.SetActive(true);//愛麗絲死亡執行愛麗絲重新開始畫面
        }

        if (Player_isBorn == true)
        {
            if (WaitRebirthtime > 0) //等待時間>0就等待時間-每禎的平均時間
            {
                WaitRebirthtime -= Time.deltaTime;//等待時間-1
                if (WaitRebirthtime <= 0f)
                {
                    if (PlayerAlive > -1 && PlayerAlive < 1)
                    {
                        PnlPlayerDead.SetActive(false);//關愛麗絲重新開始畫面
                        GameObject RebirthObj = Instantiate(RebirthPlayerObj, RebirthPos.position, RebirthPos.rotation, RebirthPos);//重生愛麗絲的座標
                        //RebirthObj.transform.localScale = new Vector3(1, 1, 1);//設定愛麗絲原本尺寸
                        //RebirthObj.transform.parent = Ground.transform;//重生愛麗絲在父物件節點下
                        PlayerAlive++;//愛麗絲計數++
                        Player_isBorn = false;//愛麗絲已重生 狀態關
                        Particle.Play();
                    }

                    Player_isDead = false;//重設愛麗絲死亡狀態 
                    WaitRebirthtime = 0.3f;//重設死亡等待時間
                    EnemyObjs[0].transform.position = EnemyResetPos[0].position;//重設小兵1座標
                    EnemyObjs[1].transform.position = EnemyResetPos[1].position;//重設小兵2座標
                    Bridge_animator.SetBool("IsBridge", false);//重設橋動畫
                    
                    //Item[0].SetActive(false);//重設過關道具

                    for (int i = 0; i < 3; i++)
                    {
                        Move_stuff_01_cs[i].Reset_MoveElement();//呼叫機關cs.的重置機關x3個
                    }

                }
            }
        }
        if (Input.GetKey(KeyCode.A))
        {
            if (CameraObj[0] != null)
            {
                CameraObj[0].transform.Rotate(new Vector3(0, 10, 0), 30 * Time.deltaTime);
            }
        }

        if (Input.GetKey(KeyCode.D))
        {
            if (CameraObj[0] != null)
            {
                CameraObj[0].transform.Rotate(new Vector3(0, 10, 0), -30 * Time.deltaTime);
            }
        }
    }


    public void Reborn()//玩家重生方法-按鈕呼叫
    {
        Player_isBorn = true;
    }
}
