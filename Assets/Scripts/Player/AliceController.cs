using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class AliceController : MonoBehaviour
{
    //場上目前的愛麗絲，沒有的時候是null
    public static AliceController Current { get; private set; }

    [FormerlySerializedAs("CentralDataViewer")] public GameManager gameManager;
    public LayerMask moveMask;//滑鼠點擊可以移動的圖層
    [FormerlySerializedAs("anim")] public Animator animator;
    [FormerlySerializedAs("MoveBaseVelocit")] public float moveSpeed = 3.5f;
    [FormerlySerializedAs("stopOffset")] public float slowDownDistance = 2f;//離目標多近開始減速

    private const float LadderDownSpeed = 1.8f;
    private const float LadderUpSpeed = 1.3f;

    private static readonly int ClimbParam = Animator.StringToHash("climb");
    private static readonly int MoveVelocityParam = Animator.StringToHash("MoveVelocity");

    private enum LadderState
    {
        None,//沒有在爬梯子
        Down,
        Up
    }

    private NavMeshAgent agent;
    private Rigidbody body;
    private bool wasKinematic;
    private float moveVelocity;//0~1，目前走路速度的比例
    private bool isStopped;

    private LadderState ladderState = LadderState.None;
    private bool hasLinkData;
    private Vector3 ladderCenter;//梯子的中間點
    private Quaternion ladderRotation;//爬梯子時要面對的方向
    private MovableMechanism climbedMechanism;//目前梯子所在的機關

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        body = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        Current = this;
    }

    private void OnDisable()
    {
        if (Current == this)
        {
            Current = null;
        }
    }

    private void Update()
    {
        if (isStopped || !agent.isOnNavMesh)
            return;

        if (agent.remainingDistance > agent.stoppingDistance)
        {
            if (agent.isOnOffMeshLink)
            {
                UpdateClimb();
            }
            else
            {
                UpdateWalk();
            }
        }
        else
        {
            UpdateIdle();
        }
    }

    //走到滑鼠點的位置，只有moveMask裡的圖層可以點
    public void MoveToScreenPoint(Vector2 screenPoint)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPoint);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, moveMask))
        {
            MoveTo(hit.point);
        }
    }

    //設定目標後agent就會自己移動
    public void MoveTo(Vector3 point)
    {
        if (isStopped || !agent.isOnNavMesh)
            return;

        agent.SetDestination(new Vector3(point.x, transform.position.y, point.z));
    }

    //爬梯子：agent走在NavMeshLink上
    private void UpdateClimb()
    {
        if (!hasLinkData)
        {
            //愛麗絲在梯子上時機關不能動
            if (agent.navMeshOwner is Component linkOwner)
            {
                climbedMechanism = linkOwner.GetComponentInParent<MovableMechanism>();
                if (climbedMechanism != null)
                {
                    climbedMechanism.SetAliceClimbing(true);
                }
            }

            OffMeshLinkData link = agent.currentOffMeshLinkData;
            ladderCenter = (link.startPos + link.endPos) * 0.5f;
            //取梯子由下往上的方向，再投影到愛麗絲站立的平面
            Vector3 upward = link.startPos.y > ladderCenter.y ? link.startPos - link.endPos : link.endPos - link.startPos;
            ladderRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(upward, transform.up));

            hasLinkData = true;
        }

        if (ladderState != LadderState.None)
            return;

        if (transform.position.y > ladderCenter.y)
        {
            StartClimb(LadderState.Down, LadderDownSpeed);
        }
        else if (transform.position.y < ladderCenter.y)
        {
            StartClimb(LadderState.Up, LadderUpSpeed);
        }
    }

    private void StartClimb(LadderState state, float speed)
    {
        agent.updateRotation = false;//關agent自動旋轉 下梯才不會轉正
        transform.rotation = ladderRotation;
        ladderState = state;
        agent.speed = speed;
        animator.SetBool(ClimbParam, true);
    }

    //走路：離目標越近速度越慢
    private void UpdateWalk()
    {
        agent.nextPosition = transform.position;
        agent.updateRotation = true;
        agent.updatePosition = true;

        hasLinkData = false;
        ladderState = LadderState.None;
        animator.SetBool(ClimbParam, false);

        float targetVelocity = Mathf.Min((agent.remainingDistance - agent.stoppingDistance) / slowDownDistance, 1);
        moveVelocity = Mathf.Lerp(moveVelocity, targetVelocity, Time.deltaTime);
        agent.speed = moveVelocity * moveSpeed;
        animator.SetFloat(MoveVelocityParam, moveVelocity);

        if (climbedMechanism != null)
        {
            climbedMechanism.SetAliceClimbing(false);
            climbedMechanism = null;
        }
    }

    //到達目標：速度慢慢歸零切回idle
    private void UpdateIdle()
    {
        hasLinkData = false;
        animator.SetBool(ClimbParam, false);
        moveVelocity = Mathf.Lerp(Mathf.Min(moveVelocity, .5f), 0, Time.deltaTime);
        agent.speed = 0;
        animator.SetFloat(MoveVelocityParam, moveVelocity);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Key"))
        {
            gameManager.ShowBridge();
            other.gameObject.SetActive(false);
        }
    }

    //正方體換面時離開NavMesh，位置交給CubeWorld控制
    public void DetachFromNavMesh()
    {
        agent.enabled = false;
        if (body != null)
        {
            wasKinematic = body.isKinematic;
            body.isKinematic = true;//agent關掉後剛體會被重力拉走
        }
    }

    //換面結束，在目前的位置重新接上NavMesh
    public void AttachToNavMesh()
    {
        if (body != null)
        {
            body.isKinematic = wasKinematic;
        }

        agent.enabled = true;
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 3f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }
        hasLinkData = false;
        ladderState = LadderState.None;
    }

    //過關後停在原地
    public void StopMove()
    {
        hasLinkData = false;
        animator.SetBool(ClimbParam, false);
        moveVelocity = 0;
        agent.updateRotation = false;
        agent.updatePosition = false;
        agent.speed = 0;
        animator.SetFloat(MoveVelocityParam, moveVelocity);
        isStopped = true;
    }
}
