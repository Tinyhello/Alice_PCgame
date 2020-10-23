using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Manger : MonoBehaviour
{
    public CentralDataViewer CentralDataViewer;//新增主控台cs
    public GameObject PlayerObj;
    public GameObject enemy;
    public GameObject Pos1;
    public GameObject Pos2;
    public Transform Ray;
    public float enemyduringtime = 3f;



    private float Chasetime = 4f;


    private float viewRadius = 7f;
    private float viewAngleStep = 20f;
    private NavMeshAgent navMeshAgent;
    //private float curtime = 0f;
    private bool isMove = true;
    private bool isFind = false;
    private GameObject Alice;
    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        Patrol();
        Engage();
        //LookAround(Ray);

        if (Alice != null) { 
            GotoALice();
        }
    }

    public void Patrol()
    {
        //巡邏&鎖敵
        //發現並追蹤
        //接觸Alice後Alice要陣亡
        if (isMove) //true
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) <= navMeshAgent.stoppingDistance)
            {
                if (enemy.GetInstanceID() == Pos1.GetInstanceID())
                {
                    enemy = Pos2;
                }
                else
                {
                    enemy = Pos1;
                }
                isMove = false;
            }
            navMeshAgent.SetDestination(enemy.transform.position);
        }
        else //false
        {
            if (enemy.GetInstanceID() == Pos1.GetInstanceID() || enemy.GetInstanceID() == Pos2.GetInstanceID())//計時器沒辦法到點後才計算，因為判斷式有成功通過(因為ID已經相等)
            {
                enemyduringtime -= Time.deltaTime;
                //Debug.Log(duringtime);
                if (enemyduringtime <= 0)
                {
                    //Debug.Log("Pass");
                    isMove = true;
                    enemyduringtime = 5f;
                }
            }
        }
    }
    public void Engage()
    {
        //Ray ray = transform.forward
        RaycastHit hit;
        Vector3 forwardLeft = Quaternion.Euler(0, -75, 0) * Ray.forward * viewRadius;
        //Vector3 pos = new Vector3(0, 0, 0);
        for (int i = 0; i <= viewAngleStep; i++)
        {
            Vector3 vector = Quaternion.Euler(0, (150 / viewAngleStep) * i, 0) * forwardLeft;
            Vector3 pos = Ray.position + vector;
            Debug.DrawLine(Ray.position, pos, Color.red);
            if (Physics.Raycast(Ray.transform.position, vector, out hit, viewRadius))
            {
                if (hit.transform.CompareTag("Alice"))
                {
                    Alice = hit.transform.gameObject;
                    Debug.DrawLine(Ray.position, hit.point, Color.green);
                    //Debug.Log("Find Alice");
                    isFind = true;
                    navMeshAgent.speed = 4f;
                    //Debug.Log("Yes");
                    //小兵要加速
                    //追愛麗絲
                }
                else
                {
                    
                    navMeshAgent.speed = 2.5f;
                    //Debug.Log("No");
                    //不追愛麗絲
                }
            }



        }

        //Debug.DrawLine(Ray.transform.position, Ray.transform.position + Ray.transform.forward * 10f, Color.red);//終點是 起點位置+起點到終點的向量距離
    }

    public void GotoALice()
    {
        if (isFind == true)
        {
            navMeshAgent.SetDestination(Alice.transform.position);
            Chasetime -= Time.deltaTime;
            //Debug.Log(Chasetime);
            if (Chasetime <= 0)
            {
                isFind = false; //isfind 需要另外宣告
            }
        }

    }

    public void LookAround(Transform targets)
    {
        Vector3 forwardLeft = Quaternion.Euler(0, -45, 0) * targets.forward * viewRadius;

        for (int i = 0; i <= viewAngleStep; i++)
        {
            Vector3 vector = Quaternion.Euler(0, (90 / viewAngleStep) * i, 0) * forwardLeft;
            Vector3 pos = targets.position + vector;
            Debug.DrawLine(targets.position, pos, Color.red);
        }

    }

    public void Attack()
    {

    }

    private void OnCollisionEnter(Collision AttackTarget)
    {
        //Debug.Log("Attack1");
        if (AttackTarget.gameObject.CompareTag("Alice"))
        {
            //Debug.Log("Attack2");
            Attack();
            //播放撲克士兵攻擊動畫
            if (CentralDataViewer.PlayerAlive == 1)
            {
                CentralDataViewer.Player_isDead = true;
                //播放愛麗絲死亡動畫
                AttackTarget.gameObject.SetActive(false);//停用愛麗絲
                Destroy(AttackTarget.gameObject, 2f);//2秒後消滅愛麗絲 
                CentralDataViewer.PlayerAlive--;//愛麗絲計數--
            }
        }

    }
}


