using UnityEngine;
using Unity.AI.Navigation;

//可以用滑鼠上下拖動的機關
public class Move_stuff_01 : MonoBehaviour
{
    public float limithigh = 3;//可移動距離
    public CentralDataViewer CentralDataViewer;//新增主控台cs

    private const float speed = 50f;

    private Vector3 pos;  //抓自己初始位置
    private GameObject inobj;//滑鼠正在拖動的物件，null代表沒有在拖動
    private bool isAliceHere = false;

    private NavMeshSurface[] surfaces;//機關自己帶的NavMesh
    private Vector3 navMeshSyncedPos;//上次載入NavMesh時機關的位置

    void Start()
    {
        pos = transform.position;//機關初始座標
        surfaces = GetComponentsInChildren<NavMeshSurface>();
        navMeshSyncedPos = pos;
    }

    void Update()
    {
        Move();
        SyncNavMesh();
    }

    //NavMeshSurface的資料只會在啟用時定位一次，機關移動後要重新載入，NavMesh才會跟著機關走
    private void SyncNavMesh()
    {
        if (transform.position == navMeshSyncedPos)
            return;

        navMeshSyncedPos = transform.position;
        foreach (NavMeshSurface surface in surfaces)
        {
            surface.RemoveData();
            surface.AddData();
        }
    }

    public void Move_stuff(GameObject inObj)
    {
        inobj = inObj;
    }

    //愛麗絲在梯子上時機關不能動
    public void Aliceclimb(bool istrue)
    {
        isAliceHere = istrue;
    }

    public void Move()
    {
        if (isAliceHere)
            return;

        if (inobj == null)
        {
            return;
        }

        if (Vector3.Distance(pos, transform.position) < limithigh)
        {
            float mouseY = Input.GetAxis("Mouse Y");
            transform.Translate(0, mouseY * speed * Time.deltaTime, 0);
        }
        if (Vector3.Distance(pos, transform.position) >= limithigh) //這行要放後面是因為要在同一個frame去移動或判斷
        {
            transform.position = pos + limithigh * Vector3.up;
        }
        if (pos.y > transform.position.y)//不讓此物件低於原本位置
        {
            transform.position = pos;
        }
    }

    public void Reset_MoveElement()//重置機關方法
    {
        transform.position = pos;//機關位置重置
        gameObject.SetActive(true);//機關父物件重新開啓
        transform.GetChild(0).gameObject.SetActive(true);//機關子物件重新開啓
    }
}
