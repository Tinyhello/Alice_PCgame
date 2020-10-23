using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Patrol = 0,//
    Seek,//
    Chase,//
    Attack,//
    Dead,//
}


public class Enemy : MonoBehaviour
{
    public Player Player_CS;
    public GameObject PlayerObj;
    public NavMeshAgent agent;    //宣告NavMeshAgent
    public GameObject target_obj;    //目標物件
    public GameObject Pos1;//敵人巡邏點1
    public GameObject Pos2;//敵人巡邏點2

    public Vector3 tar_pos;
    public Vector3 pos;

    public float lookAngle = 45f;//敵人巡邏角度
    public float enemyduringtime = 3f;//敵人巡邏定點等待時間
    
    

    // Start is called before the first frame update
    void Start()
    {
        //target_obj = null;
        agent = GetComponent<NavMeshAgent>();        //接收NavMeshAgent
        target_obj = Pos1;
        //pos = agent.transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        Enemy1(EnemyState.Patrol);
        Enemy1(EnemyState.Seek);
        


    }

    


    public void Enemy1(EnemyState state)
    {
        //if (anim == null) //anim為空就跳出
        //    return;

        switch (state)
        {
            case EnemyState.Patrol:
                {
                    UpdatePatrolState();
                    break;
                }
            case EnemyState.Seek:
                {
                    UpdateSeekState();
                    break;
                }
            case EnemyState.Chase:
                {
                    UpdateChaseState();
                    break;
                }
            case EnemyState.Attack:
                {
                    UpdateAttackState();
                    break;
                }
            case EnemyState.Dead:
                {
                    UpdateDeadState();
                    break;
                }


        }

    }

    public void UpdatePatrolState()
    {
        if (target_obj != null)
        {

        }

        if (Vector3.Distance(transform.position, target_obj.transform.position) <= agent.stoppingDistance)
        {
            if (target_obj.GetInstanceID() == Pos1.GetInstanceID())
            {
                target_obj = Pos2;
                Debug.Log("切換導航點2");
            }
            else
            {
                target_obj = Pos1;
                Debug.Log("切換導航點1");
            }
        }
        agent.SetDestination(target_obj.transform.position);//讓紅方塊往目標物的座標移動
    }
    public void UpdateSeekState()
    {
        
        for (int i=-45;i < lookAngle; i+=45)
        {
        RaycastHit hit;
            Debug.DrawLine(transform.position + transform.up * .5f, transform.position + transform.up * .5f + Quaternion.Euler(0, i, 0) * transform.forward * 5f, Color.red);//先加.5高度
            if (Physics.Raycast(transform.position + transform.up * .5f,  Quaternion.Euler(0, i, 0) * transform.forward , out hit, 8f/*,LayerMask.GetMask("Player")*/)) //因該物件Layer是設定Player 所以射線只會射到Layer為Player的物件
            {

                if (hit.collider.CompareTag("Player"))
                {
                    //if (senseTimer <= SensorInterval)
                    //{
                    //    senseTimer = 1;
                    //}
                    //senseTimer += Time.deltaTime;
                   
                    Debug.DrawLine(transform.position, hit.point, Color.green);
                    target_obj.transform.position = hit.transform.position;
                    //transform.transform.LookAt(hit.transform.position);//看向目標
                    agent.SetDestination(target_obj.transform.position);//取得目標的位置
                    Debug.Log("看到你囉");
                }


            }
        }
    }
    public void UpdateChaseState()
    {

    }
    public void UpdateAttackState()
    {

    }
    public void UpdateDeadState()
    {

    }

}
