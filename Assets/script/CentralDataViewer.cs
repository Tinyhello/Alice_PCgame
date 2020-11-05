using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CentralDataViewer : MonoBehaviour
{
    public GameObject RebirthPlayerObj;//重生玩家
    public GameObject StartPlayerObj;//愛麗絲開啓
    public Transform RebirthPos;//重生愛麗絲的座標
    public bool Player_isDead = false;//判斷愛麗絲死亡狀態
    public bool Player_isBorn = false;//判斷愛麗絲重生
    public int PlayerAlive = 1;//宣告靜態變數 愛麗絲數量
    private float WaitRebirthtime = 2f;//愛麗絲死亡重生等待時間

    public GameObject[] Item;
    public Enemy_Manger EnemyCS;//敵人cs
    public Enemy_Manger2 EnemyCS2;//敵人2 cs
    public Transform[] EnemyResetPos;//敵人重生點
    public Transform Ground;//重生位置的父物件
    public Move_stuff_01[] Move_stuff_01_cs;//宣告機關cs
    public bridge_effect bridge_;
    public Game_Plane_Exit Game_Plane_Exit;//宣告目的地的觸發cs
    public GameObject PnlPlayerDead;//設定panel 愛麗絲死亡
    public GameObject BtnReStart;//設定按鈕愛麗絲重生
    public GameObject[] StartEnemyObjs;//遊戲開始預設敵人
    public ParticleSystem Particle;//遊戲開始重生動畫
    public GameObject[] CameraObj;
    public Animator Bridge_animator;//宣告橋的動畫
    public Alice_Manger Alice_Manger;//宣告愛麗絲cs
    public bridge_effect bridge_Effect;
    private void Start()
    {
        Particle.Stop();
        if(StartPlayerObj != null)
            Alice_Manger = StartPlayerObj.GetComponent<Alice_Manger>();

    }

    void Update()
    {
        if (Player_isDead)
        {
            PnlPlayerDead.SetActive(true);//愛麗絲死亡執行愛麗絲重新開始畫面
        }

        if (Player_isBorn == true)
        {
            PnlPlayerDead.SetActive(false);//關愛麗絲重新開始畫面
            EnemyCS.EnemyPosReset();//叫小兵執行reset座標
            EnemyCS2.EnemyPosReset();//叫小兵2執行reset座標
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
                        Alice_Manger = RebirthObj.GetComponent<Alice_Manger>();
                        Alice_Manger.CentralDataViewer = this;//新生成愛麗絲Alice_Manger.cs把CentralDataViewer.cs給新生成愛麗絲用 非常重要非常重要

                    }
                    Player_isDead = false;//重設愛麗絲死亡狀態 
                    WaitRebirthtime = 2f;//重設死亡等待時間
                    for (int i = 0; i < 3; i++)
                    {
                        Move_stuff_01_cs[i].Reset_MoveElement();//呼叫機關cs.的重置機關x3個
                        bridge_.Bridge_reset();
                    }

                }
           
            }
        }
        Button();
    }

    public void Button()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Reborn();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            GameOver();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            GameStart();
        }




    }

    public void GameOver()
    {
        restartbridge();
        Debug.Log("GameOver嘿嘿嘿嘿嘿嘿嘿嘿");
        if (PlayerAlive == 1)
        {
            Player_isBorn = false;
            Player_isDead = true;

            Alice_Manger.gameObject.SetActive(false);//停用愛麗絲
            Destroy(Alice_Manger.gameObject, 2f);//2秒後消滅愛麗絲 
            PlayerAlive--;//愛麗絲計數--
        }
    }

    public void showBridge()
    {
        Bridge_animator.SetTrigger("finish");
        bridge_Effect.bridgeup();

    }




    public void Reborn()//玩家重生方法-按鈕呼叫
    {
        Player_isBorn = true;//玩家重生狀態開
        Particle.Play();//按下重新挑戰就播放重生動畫
    }
    public void ClearReStart()//遊戲重新開始
    {
        Player_isBorn = true;//玩家重生狀態開
        Particle.Play();//按下重新挑戰就播放重生動畫
    }
    public void GameStart()
    {
        Particle.Play();//按下遊戲開始就播放重生動畫
        Alice_Manger.gameObject.SetActive(true);//愛麗絲開啓
        for (int i = 0; i < 2; i++)//小兵'們'開啓
        {
            StartEnemyObjs[i].SetActive(true);
        }
    }
    public void restartbridge()
    {
        Bridge_animator.SetBool("IsBridge", false);//重設橋動畫
        Item[0].SetActive(true);//重設過關道具
    }

    public void GameFinish()
    {
        Alice_Manger.StopMove();
    }


}
