using UnityEngine;

public class CentralDataViewer : MonoBehaviour
{
    public GameObject RebirthPlayerObj;//重生玩家
    public GameObject StartPlayerObj;//愛麗絲開啓
    public Transform RebirthPos;//重生愛麗絲的座標
    public bool Player_isDead = false;//判斷愛麗絲死亡狀態
    public bool Player_isBorn = false;//判斷愛麗絲重生
    public int PlayerAlive = 1;//愛麗絲數量

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

    private const float RebirthWaitTime = 2f;//愛麗絲死亡重生等待時間
    private const float DeadDestroyDelay = 2f;//愛麗絲死亡後幾秒消滅物件

    private float WaitRebirthtime = RebirthWaitTime;

    private void Start()
    {
        Particle.Stop();
        if (StartPlayerObj != null)
            Alice_Manger = StartPlayerObj.GetComponent<Alice_Manger>();
    }

    void Update()
    {
        if (Player_isDead)
        {
            PnlPlayerDead.SetActive(true);//愛麗絲死亡執行愛麗絲重新開始畫面
        }

        if (Player_isBorn)
        {
            UpdateRebirth();
        }
        Button();
    }

    private void UpdateRebirth()
    {
        PnlPlayerDead.SetActive(false);//關愛麗絲重新開始畫面
        EnemyCS.EnemyPosReset();//叫小兵執行reset座標
        EnemyCS2.EnemyPosReset();//叫小兵2執行reset座標

        if (WaitRebirthtime <= 0)
            return;

        WaitRebirthtime -= Time.deltaTime;
        if (WaitRebirthtime > 0f)
            return;

        if (PlayerAlive == 0)
        {
            GameObject RebirthObj = Instantiate(RebirthPlayerObj, RebirthPos.position, RebirthPos.rotation, RebirthPos);//重生愛麗絲的座標
            PlayerAlive++;//愛麗絲計數++
            Player_isBorn = false;//愛麗絲已重生 狀態關
            Alice_Manger = RebirthObj.GetComponent<Alice_Manger>();
            Alice_Manger.CentralDataViewer = this;//新生成愛麗絲Alice_Manger.cs把CentralDataViewer.cs給新生成愛麗絲用 非常重要非常重要
        }
        Player_isDead = false;//重設愛麗絲死亡狀態
        WaitRebirthtime = RebirthWaitTime;//重設死亡等待時間

        for (int i = 0; i < Move_stuff_01_cs.Length; i++)
        {
            Move_stuff_01_cs[i].Reset_MoveElement();//呼叫機關cs.的重置機關
        }
        bridge_.Bridge_reset();
    }

    //測試用快捷鍵
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
        KillAlice(Alice_Manger.gameObject);
    }

    //愛麗絲死亡：重設橋與過關道具，並顯示重新開始畫面
    public void KillAlice(GameObject alice)
    {
        restartbridge();
        if (RemoveAlice(alice, DeadDestroyDelay))
        {
            Player_isDead = true;
        }
    }

    //停用並消滅場上的愛麗絲，有成功移除才回傳true
    public bool RemoveAlice(GameObject alice, float destroyDelay)
    {
        if (PlayerAlive != 1)
            return false;

        Player_isBorn = false;
        alice.SetActive(false);//停用愛麗絲
        Destroy(alice, destroyDelay);
        PlayerAlive--;//愛麗絲計數--
        return true;
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

    public void ClearReStart()//遊戲重新開始-按鈕呼叫
    {
        Reborn();
    }

    public void GameStart()
    {
        Particle.Play();//按下遊戲開始就播放重生動畫
        Alice_Manger.gameObject.SetActive(true);//愛麗絲開啓
        for (int i = 0; i < StartEnemyObjs.Length; i++)//小兵'們'開啓
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
