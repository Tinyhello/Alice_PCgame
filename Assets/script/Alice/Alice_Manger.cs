using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AliceState
{
    Idle,
    walk,
    climb,
}

public enum OffMeshLinkFlag
{
    //默认OffMeshLink没有开启的时候
    None,
    //OffMeshLink处于下楼的状态
    LadderDwon,
    //OffMeshLink处于爬楼梯的的状态
    LadderUp
}


public class Alice_Manger : MonoBehaviour
{
    public CentralDataViewer CentralDataViewer;//新增主控台cs
    public NavMeshAgent navMeshAgent;
    public LayerMask moveMask;//
    private Bridge_control Bridge_control;

    private float MoveVelocit = 0; 

    Ray ray;
    RaycastHit hit;
    Vector3 target;
    private NavMeshAgent agent;
    private OffMeshLinkFlag flag = OffMeshLinkFlag.None;


    private Rigidbody rg;
    //private float anim_speed = 0;

    public Animator anim;

    private bool IsHasLinkData = false;
    private OffMeshLinkData link;
    private Vector3 center;
    private Quaternion ladderDir;
    // Start is called before the first frame update
    private Quaternion targetRot;
    
    public Animator Bridge_animator;//宣告橋的動畫
    public GameObject bridge;

    private GameObject climbnodrag;

    private Move_stuff_01 currectLinkObj;
    public float MoveBaseVelocit = 3.5f;
    public float stopOffset = 2f;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        target = transform.position;
        rg = GetComponent<Rigidbody>();
    }

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        targetRot = transform.rotation * Quaternion.Euler(Vector3.up * 180);
        Bridge_control = GetComponent<Bridge_control>();

    }
    // Update is called once per frame
    void Update()
    {
        //AliceMovement();
        //Alice_navmeshlink();
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, Time.deltaTime * 50f);//愛麗絲轉180度
    }
    public void AliceMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(r, out hit,moveMask))
            {
                navMeshAgent.SetDestination(hit.point);
                //anim.SetBool("walk" , true); 已用blendtree控制idle/walk動畫
            }
            else
            {
                //anim.SetBool("walk", false);已用blendtree控制idle/walk動畫
            }
        }
    }
    public void SetAlicePos(Vector3 pos)
    {
        agent.Warp(pos);
    }
    public void Alice_navmeshlink()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit,moveMask))
            {
                Debug.DrawLine(Camera.main.transform.position, hit.point, Color.green);
                target = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                //设置目标后就可以移动了
                //agent.destination = target;

                agent.SetDestination(target);
                //Debug.Log(agent.path.corners.Length);
            }
        }
        //自带移动效果
        //agent.SetDestination(target);

        if (!agent.isOnNavMesh)
        {
            return;
        }
        if (agent.remainingDistance > agent.stoppingDistance)
        {

            //判断OffMeshLink是否开启 有没有开始上楼或下楼的状态
            if (agent.isOnOffMeshLink)
            {
                //這邊叫alice不能走
                if(!IsHasLinkData)//老師打的
                {
                    GameObject c_ = (agent.navMeshOwner as Component).gameObject;//這邊型轉
                    currectLinkObj = c_.GetComponentInParent<Move_stuff_01>();
                    currectLinkObj?.Aliceclimb(true);
                    link = agent.currentOffMeshLinkData;
                    center = (link.startPos + link.endPos) * 0.5f;
                    if (link.startPos.y > center.y)
                    {
                        ladderDir = Quaternion.LookRotation(Vector3.ProjectOnPlane(link.startPos - link.endPos, transform.up));//取start-end pos的方向,再用ProjectOnPlane(start-end pos的方向,依哪邊的Y軸取物件平面)
                    }
                    else
                        ladderDir = Quaternion.LookRotation(Vector3.ProjectOnPlane(link.endPos - link.startPos, transform.up));//取start-end pos的方向,再用ProjectOnPlane(start-end pos的方向,依哪邊的Y軸取物件平面)

                    IsHasLinkData = true;
                }
                //用来标记任务的状态
                if (flag == OffMeshLinkFlag.None)
                {
                    
                    if (transform.position.y > center.y)
                    {
                        agent.updateRotation = false;//關agent自動旋轉 下梯才不會轉正
                        //transform.rotation = transform.rotation * Quaternion.Euler(Vector3.up * 180);//愛麗絲強制跳到180度 佳志-只有一邊方向的下樓梯可行
                        transform.rotation = ladderDir;//老師打的

                        //agent.GetComponent<MeshRenderer>().material.color = Color.red;
                        flag = OffMeshLinkFlag.LadderDwon;
                        //下楼的速度是1
                        agent.speed = 1.8f;
                        anim.SetBool("climb", true);
                      
                        //agent.transform.Rotate(new Vector3(0, 180f, 0));

                    }
                    if (transform.position.y < center.y)
                    {
                        agent.updateRotation = false;//關agent自動旋轉 下梯才不會轉正
                        //transform.rotation = transform.rotation * Quaternion.Euler(Vector3.up * -180);//愛麗絲強制跳到180度 佳志-只有一邊方向的下樓梯可行
                        transform.rotation = ladderDir;

                        //agent.GetComponent<MeshRenderer>().material.color = Color.green;
                        flag = OffMeshLinkFlag.LadderUp;
                        //上楼的速度是0.5f
                        agent.speed = 1.3f;
                        anim.SetBool("climb", true);
                    }
                }
            }
            else //判断OffMeshLink是否关闭 //滑鼠點擊目標 walk狀態
            {
                agent.nextPosition = transform.position;
                agent.updateRotation = true;
                agent.updatePosition = true;


                IsHasLinkData = false;
                anim.SetBool("climb", false);
                float targetVelocit_ = Mathf.Min(((agent.remainingDistance- agent.stoppingDistance) / stopOffset), 1);
                MoveVelocit = Mathf.Lerp(MoveVelocit, targetVelocit_, Time.deltaTime); //距離很遠的時候+速，很近減速  跟  agent.remainingDistance 有關  ,walk狀態加速
                
                agent.speed = MoveVelocit* MoveBaseVelocit;
                flag = OffMeshLinkFlag.None;
                SetMoveVelocity(MoveVelocit);
                currectLinkObj?.Aliceclimb(false);
                currectLinkObj = null;
            }
        }
        else //idle狀態 MoveVelocit歸零
        {
            //MoveVelocit = 0; //愛麗絲走到目標點切回idle狀態0
            MoveVelocit = Mathf.Lerp(Mathf.Min(MoveVelocit, .5f), 0, Time.deltaTime); //距離很遠的時候+速，很近減速  跟  agent.remainingDistance 有關  ,walk狀態加速
            agent.updateRotation = false;
            agent.updatePosition = false;
            //agent.speed = MoveVelocit * 3.5f; //不需要多打
            SetMoveVelocity(MoveVelocit);
        }
    }

    public void SetMoveVelocity(float val)
    {
        anim.SetFloat("MoveVelocity", val);
    }

    private void OnCollisionEnter(Collision item)//碰到過關道具
    {
        if (item.gameObject.CompareTag("Key"))
        {
            item.gameObject.SetActive(false);
            //CentralDataViewer.Item[0].SetActive(true);
            //Bridge_control.bridge();
            CentralDataViewer.Item[1].SetActive(true);
            
            Bridge_animator.SetBool("IsBridge", true);
            bridge.transform.Translate(0, -50f, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Key"))
        {
            other.gameObject.SetActive(false);
            //CentralDataViewer.Item[0].SetActive(true);
            //Bridge_control.bridge();
            CentralDataViewer.Item[1].SetActive(true);
            
            Bridge_animator.SetBool("IsBridge", true);
            bridge.transform.Translate(0, -50f, 0);

        }
    }

}


