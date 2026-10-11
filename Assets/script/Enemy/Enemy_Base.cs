using UnityEngine;
using UnityEngine.AI;

//小兵共用邏輯：巡邏、視野偵測、追擊、碰到愛麗絲
public abstract class Enemy_Base : MonoBehaviour
{
    public CentralDataViewer CentralDataViewer;//新增主控台cs
    public GameObject enemy;//目前要前往的巡邏點
    public GameObject Pos1;
    public GameObject Pos2;
    public Transform Ray;
    public float enemyduringtime = 0f;//到達巡邏點後的等待時間

    private const float PatrolWaitTime = 5f;
    private const float ChaseDuration = 4f;//小兵總共願意追擊的時間，用完後看不到愛麗絲就會放棄
    private const float SearchSpeed = 2.5f;//視線被其他物件擋住時的速度
    private const float ViewRadius = 7f;
    private const float ViewAngle = 150f;
    private const int ViewAngleStep = 20;

    private NavMeshAgent navMeshAgent;
    private GameObject[] patrolPoints;
    private float chaseTime = ChaseDuration;
    private bool isMove = true;
    private bool isFind = false;
    private GameObject Alice;

    protected abstract float ChaseSpeed { get; }//發現愛麗絲後的速度
    protected virtual float? PatrolSpeed => null;//巡邏時的速度，null代表不另外設定
    protected abstract GameObject[] GetPatrolPoints();

    protected virtual void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        patrolPoints = GetPatrolPoints();
        enemy = Pos1;
    }

    protected virtual void Update()
    {
        Patrol();
        Engage();

        if (Alice != null)
        {
            GotoALice();
        }
    }

    //巡邏：到點後換下一個巡邏點，並等待一段時間才會再換
    public void Patrol()
    {
        if (isMove)
        {
            if (PatrolSpeed.HasValue)
            {
                navMeshAgent.speed = PatrolSpeed.Value;
            }

            if (Vector3.Distance(transform.position, enemy.transform.position) <= navMeshAgent.stoppingDistance)
            {
                enemy = NextPatrolPoint();
                isMove = false;
            }
            navMeshAgent.SetDestination(enemy.transform.position);
        }
        else
        {
            enemyduringtime -= Time.deltaTime;
            if (enemyduringtime <= 0)
            {
                isMove = true;
                enemyduringtime = PatrolWaitTime;
            }
        }
    }

    private GameObject NextPatrolPoint()
    {
        int index = System.Array.IndexOf(patrolPoints, enemy);//直接比對物件參照，不用GetInstanceID
        return patrolPoints[(index + 1) % patrolPoints.Length];
    }

    //視野偵測：在前方扇形範圍內打多條射線找愛麗絲
    public void Engage()
    {
        Vector3 forwardLeft = Quaternion.Euler(0, -ViewAngle / 2f, 0) * Ray.forward * ViewRadius;
        for (int i = 0; i <= ViewAngleStep; i++)
        {
            Vector3 vector = Quaternion.Euler(0, (ViewAngle / ViewAngleStep) * i, 0) * forwardLeft;
            Debug.DrawLine(Ray.position, Ray.position + vector, Color.red);
            if (Physics.Raycast(Ray.position, vector, out RaycastHit hit, ViewRadius))
            {
                if (hit.transform.CompareTag("Alice"))
                {
                    Alice = hit.transform.gameObject;
                    Debug.DrawLine(Ray.position, hit.point, Color.green);
                    isFind = true;
                    navMeshAgent.speed = ChaseSpeed;//小兵要加速追愛麗絲
                }
                else
                {
                    navMeshAgent.speed = SearchSpeed;
                }
            }
        }
    }

    public void GotoALice()
    {
        if (isFind)
        {
            navMeshAgent.SetDestination(Alice.transform.position);
            chaseTime -= Time.deltaTime;
            if (chaseTime <= 0)
            {
                isFind = false;
            }
        }
    }

    private void OnCollisionEnter(Collision AttackTarget)
    {
        if (AttackTarget.gameObject.CompareTag("Alice"))
        {
            CentralDataViewer.KillAlice(AttackTarget.gameObject);
        }
    }

    public void EnemyPosReset()
    {
        if (navMeshAgent == null)//小兵還沒被開啓過時Start不會執行
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }
        navMeshAgent.Warp(Pos1.transform.position);//把小兵的agent 重生到Pos1座標才不會物件移位 *重點*
    }
}
