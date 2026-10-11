using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.Serialization;

//可以用滑鼠沿著自己的Y軸拖動的機關
public class MovableMechanism : MonoBehaviour
{
    [FormerlySerializedAs("limithigh")] public float maxHeight = 3;//可移動距離
    public float dragSensitivity = 1f;//1代表機關在畫面上跟著滑鼠移動一樣的距離

    private Vector3 startLocalPosition;//用local座標記，機關所在的面轉動後才不會跑掉
    private float height;//目前沿著自己的Y軸拉了多遠
    private bool isDragged;
    private bool isAliceClimbing;

    private NavMeshSurface[] surfaces;//機關自己帶的NavMesh
    private Vector3 navMeshSyncedPosition;//上次載入NavMesh時機關的位置

    private void Start()
    {
        startLocalPosition = transform.localPosition;
        surfaces = GetComponentsInChildren<NavMeshSurface>();
        navMeshSyncedPosition = transform.position;
    }

    private void Update()
    {
        Drag();
        SyncNavMesh();
    }

    public void BeginDrag()
    {
        isDragged = true;
    }

    public void EndDrag()
    {
        isDragged = false;
    }

    //愛麗絲在梯子上時機關不能動
    public void SetAliceClimbing(bool isClimbing)
    {
        isAliceClimbing = isClimbing;
    }

    private void Drag()
    {
        if (isAliceClimbing || !isDragged)
            return;

        if (height < maxHeight)
        {
            height += PointerMovementAlongUp();
        }
        if (height >= maxHeight)//拉到最高就鎖定在最高的位置
        {
            height = maxHeight;
        }
        if (height < 0)//不讓機關低於原本位置
        {
            height = 0;
        }
        ApplyHeight();
    }

    private void ApplyHeight()
    {
        Vector3 offset = transform.up * height;
        if (transform.parent != null)
        {
            offset = transform.parent.InverseTransformVector(offset);
        }
        transform.localPosition = startLocalPosition + offset;
    }

    //把滑鼠這一幀的位移換算成機關沿著自己Y軸要移動的距離，讓機關在畫面上跟著滑鼠走
    private float PointerMovementAlongUp()
    {
        Camera cam = Camera.main;
        Vector2 from = cam.WorldToScreenPoint(transform.position);
        Vector2 to = cam.WorldToScreenPoint(transform.position + transform.up);
        Vector2 upOnScreen = to - from;//機關往上1單位，在螢幕上是往哪個方向移動幾個像素
        float pixelsPerUnit = upOnScreen.magnitude;
        if (pixelsPerUnit < 0.001f)
            return 0;

        float pixelsAlongUp = Vector2.Dot(GameInput.PointerDelta, upOnScreen / pixelsPerUnit);
        return pixelsAlongUp / pixelsPerUnit * dragSensitivity;
    }

    //NavMeshSurface的資料只會在啟用時定位一次，機關移動後要重新載入，NavMesh才會跟著機關走
    private void SyncNavMesh()
    {
        if (transform.position == navMeshSyncedPosition)
            return;

        navMeshSyncedPosition = transform.position;
        foreach (NavMeshSurface surface in surfaces)
        {
            if (!surface.isActiveAndEnabled)//所在的面沒有朝上時NavMesh是卸掉的
                continue;

            surface.RemoveData();
            surface.AddData();
        }
    }

    //回到初始位置，並重新開啓機關與第一個子物件
    public void ResetMechanism()
    {
        height = 0;
        transform.localPosition = startLocalPosition;
        gameObject.SetActive(true);
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
