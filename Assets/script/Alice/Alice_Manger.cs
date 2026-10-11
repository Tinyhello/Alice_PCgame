using UnityEngine;
using UnityEngine.AI;

public enum OffMeshLinkFlag
{
    //沒有在爬梯子的時候
    None,
    //正在下樓梯
    LadderDwon,
    //正在爬樓梯
    LadderUp
}

public class Alice_Manger : MonoBehaviour
{
    public CentralDataViewer CentralDataViewer;//新增主控台cs
    public NavMeshAgent navMeshAgent;
    public LayerMask moveMask;//滑鼠點擊可以移動的圖層

    public Animator anim;

    public Animator Bridge_animator;//宣告橋的動畫
    public GameObject bridge;

    public float MoveBaseVelocit = 3.5f;
    public float stopOffset = 2f;

    public GameObject bridge01;
    public GameObject bridge02;
    public GameObject bridge03;
    public GameObject bridge04;
    public bridge_effect[] bridge_effects;
    public GameObject Item;//宣告key變數

    private const float LadderDownSpeed = 1.8f;
    private const float LadderUpSpeed = 1.3f;

    private float MoveVelocit = 0;
    private OffMeshLinkFlag flag = OffMeshLinkFlag.None;

    private bool IsHasLinkData = false;
    private Vector3 center;//梯子的中間點
    private Quaternion ladderDir;//爬梯子時要面對的方向
    private Move_stuff_01 currectLinkObj;//目前梯子所在的機關

    private bool isStop = false;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void SetAlicePos(Vector3 pos)
    {
        navMeshAgent.Warp(pos);
    }

    //設定目標後agent就會自己移動
    public void MoveTo(Vector3 point)
    {
        if (isStop)
            return;

        navMeshAgent.SetDestination(new Vector3(point.x, transform.position.y, point.z));
    }

    public void Alice_navmeshmove()
    {
        if (isStop)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, moveMask))
            {
                MoveTo(hit.point);
            }
        }

        if (!navMeshAgent.isOnNavMesh)
        {
            return;
        }

        if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
        {
            if (navMeshAgent.isOnOffMeshLink)
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

    //爬梯子：agent走在NavMeshLink上
    private void UpdateClimb()
    {
        if (!IsHasLinkData)
        {
            //愛麗絲在梯子上時機關不能動
            if (navMeshAgent.navMeshOwner != null)
            {
                GameObject c_ = (navMeshAgent.navMeshOwner as Component).gameObject;
                currectLinkObj = c_.GetComponentInParent<Move_stuff_01>();
                currectLinkObj?.Aliceclimb(true);
            }

            OffMeshLinkData link = navMeshAgent.currentOffMeshLinkData;
            center = (link.startPos + link.endPos) * 0.5f;
            //取梯子由下往上的方向，再用ProjectOnPlane投影到愛麗絲站立的平面
            Vector3 upward = link.startPos.y > center.y ? link.startPos - link.endPos : link.endPos - link.startPos;
            ladderDir = Quaternion.LookRotation(Vector3.ProjectOnPlane(upward, transform.up));

            IsHasLinkData = true;
        }

        if (flag != OffMeshLinkFlag.None)
            return;

        if (transform.position.y > center.y)
        {
            StartClimb(OffMeshLinkFlag.LadderDwon, LadderDownSpeed);
        }
        else if (transform.position.y < center.y)
        {
            StartClimb(OffMeshLinkFlag.LadderUp, LadderUpSpeed);
        }
    }

    private void StartClimb(OffMeshLinkFlag climbFlag, float speed)
    {
        navMeshAgent.updateRotation = false;//關agent自動旋轉 下梯才不會轉正
        transform.rotation = ladderDir;
        flag = climbFlag;
        navMeshAgent.speed = speed;
        anim.SetBool("climb", true);
    }

    //走路：離目標越近速度越慢
    private void UpdateWalk()
    {
        navMeshAgent.nextPosition = transform.position;
        navMeshAgent.updateRotation = true;
        navMeshAgent.updatePosition = true;

        IsHasLinkData = false;
        anim.SetBool("climb", false);
        float targetVelocit_ = Mathf.Min((navMeshAgent.remainingDistance - navMeshAgent.stoppingDistance) / stopOffset, 1);
        MoveVelocit = Mathf.Lerp(MoveVelocit, targetVelocit_, Time.deltaTime);

        navMeshAgent.speed = MoveVelocit * MoveBaseVelocit;
        flag = OffMeshLinkFlag.None;
        SetMoveVelocity(MoveVelocit);
        currectLinkObj?.Aliceclimb(false);
        currectLinkObj = null;
    }

    //到達目標：速度慢慢歸零切回idle
    private void UpdateIdle()
    {
        IsHasLinkData = false;
        anim.SetBool("climb", false);
        MoveVelocit = Mathf.Lerp(Mathf.Min(MoveVelocit, .5f), 0, Time.deltaTime);
        navMeshAgent.speed = 0;
        SetMoveVelocity(MoveVelocit);
    }

    public void SetMoveVelocity(float val)
    {
        anim.SetFloat("MoveVelocity", val);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Key"))
        {
            CentralDataViewer.showBridge();
            other.gameObject.SetActive(false);
        }
    }

    public void StopMove()
    {
        IsHasLinkData = false;
        anim.SetBool("climb", false);
        MoveVelocit = 0;
        navMeshAgent.updateRotation = false;
        navMeshAgent.updatePosition = false;
        navMeshAgent.speed = 0;
        SetMoveVelocity(MoveVelocit);
        isStop = true;
    }
}
