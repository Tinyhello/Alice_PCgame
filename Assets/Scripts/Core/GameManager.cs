using UnityEngine;
using UnityEngine.Serialization;

//遊戲流程：開始、愛麗絲死亡與重生、拿到鑰匙升橋、過關
public class GameManager : MonoBehaviour
{
    [FormerlySerializedAs("RebirthPlayerObj")] public GameObject alicePrefab;//重生時生成的愛麗絲
    [FormerlySerializedAs("StartPlayerObj")] public GameObject startAlice;//場景裡一開始的愛麗絲
    [FormerlySerializedAs("RebirthPos")] public Transform respawnPoint;//重生愛麗絲的座標
    [FormerlySerializedAs("StartEnemyObjs")] public GameObject[] startEnemies;//遊戲開始時要開啓的敵人
    [FormerlySerializedAs("Move_stuff_01_cs")] public MovableMechanism[] mechanisms;//重生時要重置的機關
    [FormerlySerializedAs("Item")] public GameObject[] items;//過關道具，第0個是鑰匙
    [FormerlySerializedAs("bridge_Effect")] public BridgeEffect bridgeEffect;
    [FormerlySerializedAs("Bridge_animator")] public Animator bridgeAnimator;//橋的動畫
    [FormerlySerializedAs("PnlPlayerDead")] public GameObject deadPanel;//愛麗絲死亡的畫面
    [FormerlySerializedAs("Particle")] public ParticleSystem respawnParticle;//遊戲開始與重生的特效

    private const float RespawnDelay = 2f;//按下重生後等多久才生成愛麗絲
    private const float DeadDestroyDelay = 2f;//愛麗絲死亡後幾秒消滅物件

    private AliceController currentAlice;
    private EnemyBase[] enemies;
    private bool isAliceAlive = true;
    private bool isDead;
    private bool isRespawning;
    private float respawnTimer = RespawnDelay;

    private void Start()
    {
        respawnParticle.Stop();
        enemies = FindObjectsByType<EnemyBase>(FindObjectsInactive.Include);
        if (startAlice != null)
        {
            currentAlice = startAlice.GetComponent<AliceController>();
        }
    }

    private void Update()
    {
        if (isDead)
        {
            deadPanel.SetActive(true);
        }

        if (isRespawning)
        {
            UpdateRespawn();
        }

        HandleDebugKeys();
    }

    //重生倒數期間把敵人固定在起點，時間到再生成新的愛麗絲並重置機關
    private void UpdateRespawn()
    {
        deadPanel.SetActive(false);
        foreach (EnemyBase enemy in enemies)
        {
            enemy.ResetPosition();
        }

        respawnTimer -= Time.deltaTime;
        if (respawnTimer > 0f)
            return;

        if (!isAliceAlive)
        {
            if (CubeWorld.Current != null)
            {
                CubeWorld.Current.ResetToStartFace();//重生點在起始面上
            }

            GameObject alice = Instantiate(alicePrefab, respawnPoint.position, respawnPoint.rotation, respawnPoint);
            currentAlice = alice.GetComponent<AliceController>();
            currentAlice.gameManager = this;//prefab存不了場景物件的參照，生成後要自己給
            isAliceAlive = true;
        }

        isDead = false;
        isRespawning = false;
        respawnTimer = RespawnDelay;

        foreach (MovableMechanism mechanism in mechanisms)
        {
            mechanism.ResetMechanism();
        }
        bridgeEffect.ResetEffect();
    }

    private void HandleDebugKeys()
    {
        if (GameInput.DebugRespawnPressed)
        {
            Reborn();
        }
        if (GameInput.DebugGameOverPressed)
        {
            GameOver();
        }
        if (GameInput.DebugGameStartPressed)
        {
            GameStart();
        }
    }

    //====以下三個方法由UI按鈕呼叫，名稱不能改

    public void GameStart()
    {
        respawnParticle.Play();
        currentAlice.gameObject.SetActive(true);
        foreach (GameObject enemy in startEnemies)
        {
            enemy.SetActive(true);
        }
    }

    public void Reborn()//死亡後重新挑戰
    {
        isRespawning = true;
        respawnParticle.Play();
    }

    public void ClearReStart()//過關後重新開始
    {
        Reborn();
    }

    //====

    public void GameOver()
    {
        KillAlice(currentAlice.gameObject);
    }

    //愛麗絲死亡：重設橋與過關道具，並顯示重新開始畫面
    public void KillAlice(GameObject alice)
    {
        ResetBridge();
        if (RemoveAlice(alice, DeadDestroyDelay))
        {
            isDead = true;
        }
    }

    //停用並消滅場上的愛麗絲，有成功移除才回傳true
    public bool RemoveAlice(GameObject alice, float destroyDelay)
    {
        if (!isAliceAlive)
            return false;

        isRespawning = false;
        alice.SetActive(false);
        Destroy(alice, destroyDelay);
        isAliceAlive = false;
        return true;
    }

    //拿到鑰匙：播放橋的特效，播完橋會升起
    public void ShowBridge()
    {
        bridgeAnimator.SetTrigger("finish");
        bridgeEffect.Play();
    }

    private void ResetBridge()
    {
        bridgeAnimator.SetBool("IsBridge", false);
        items[0].SetActive(true);
    }

    public void FinishLevel()
    {
        currentAlice.StopMove();
    }
}
