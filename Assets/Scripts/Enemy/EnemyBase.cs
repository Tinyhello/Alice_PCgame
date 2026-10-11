using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

//小兵共用邏輯：巡邏、視野偵測、追擊、碰到愛麗絲
public abstract class EnemyBase : MonoBehaviour
{
    [FormerlySerializedAs("CentralDataViewer")] public GameManager gameManager;
    [FormerlySerializedAs("Pos1")] public GameObject point1;//巡邏點，也是重生的位置
    [FormerlySerializedAs("Pos2")] public GameObject point2;
    [FormerlySerializedAs("Ray")] public Transform eye;//視野射線的起點與方向
    [FormerlySerializedAs("enemyduringtime")] public float waitTimer = 0f;//到達巡邏點後的等待時間

    private const float PatrolWaitTime = 5f;
    private const float ChaseDuration = 4f;//小兵總共願意追擊的時間，用完後看不到愛麗絲就會放棄
    private const float SearchSpeed = 2.5f;//視線被其他物件擋住時的速度
    private const float ViewRadius = 7f;
    private const float ViewAngle = 150f;
    private const int ViewRayCount = 20;

    private NavMeshAgent agent;
    private GameObject[] patrolPoints;
    private GameObject currentPoint;//目前要前往的巡邏點
    private float chaseTime = ChaseDuration;
    private bool isMoving = true;
    private bool isChasing;
    private GameObject alice;

    protected abstract float ChaseSpeed { get; }//發現愛麗絲後的速度
    protected virtual float? PatrolSpeed => null;//巡邏時的速度，null代表不另外設定
    protected abstract GameObject[] GetPatrolPoints();

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        patrolPoints = GetPatrolPoints();
        currentPoint = point1;
    }

    protected virtual void Update()
    {
        Patrol();
        LookForAlice();

        if (alice != null)
        {
            Chase();
        }
    }

    //巡邏：到點後換下一個巡邏點，並等待一段時間才會再換
    private void Patrol()
    {
        if (isMoving)
        {
            if (PatrolSpeed.HasValue)
            {
                agent.speed = PatrolSpeed.Value;
            }

            if (Vector3.Distance(transform.position, currentPoint.transform.position) <= agent.stoppingDistance)
            {
                currentPoint = NextPatrolPoint();
                isMoving = false;
            }
            agent.SetDestination(currentPoint.transform.position);
        }
        else
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                isMoving = true;
                waitTimer = PatrolWaitTime;
            }
        }
    }

    private GameObject NextPatrolPoint()
    {
        int index = System.Array.IndexOf(patrolPoints, currentPoint);
        return patrolPoints[(index + 1) % patrolPoints.Length];
    }

    //視野偵測：在前方扇形範圍內打多條射線找愛麗絲
    private void LookForAlice()
    {
        Vector3 leftEdge = Quaternion.Euler(0, -ViewAngle / 2f, 0) * eye.forward * ViewRadius;
        for (int i = 0; i <= ViewRayCount; i++)
        {
            Vector3 direction = Quaternion.Euler(0, (ViewAngle / ViewRayCount) * i, 0) * leftEdge;
            Debug.DrawLine(eye.position, eye.position + direction, Color.red);
            if (Physics.Raycast(eye.position, direction, out RaycastHit hit, ViewRadius))
            {
                if (hit.transform.CompareTag("Alice"))
                {
                    alice = hit.transform.gameObject;
                    Debug.DrawLine(eye.position, hit.point, Color.green);
                    isChasing = true;
                    agent.speed = ChaseSpeed;//小兵要加速追愛麗絲
                }
                else
                {
                    agent.speed = SearchSpeed;
                }
            }
        }
    }

    private void Chase()
    {
        if (isChasing)
        {
            agent.SetDestination(alice.transform.position);
            chaseTime -= Time.deltaTime;
            if (chaseTime <= 0)
            {
                isChasing = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Alice"))
        {
            gameManager.KillAlice(collision.gameObject);
        }
    }

    //回到第一個巡邏點，要用Warp移動agent才不會物件移位
    public void ResetPosition()
    {
        if (agent == null)//小兵還沒被開啓過時Start不會執行
        {
            agent = GetComponent<NavMeshAgent>();
        }

        if (!agent.enabled)//所在的面沒有朝上時agent是關的，直接搬物件
        {
            transform.position = point1.transform.position;
            return;
        }
        agent.Warp(point1.transform.position);
    }
}
