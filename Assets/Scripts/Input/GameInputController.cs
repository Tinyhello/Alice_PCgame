using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

//滑鼠與鍵盤操作：拖動機關、點兩下蘑菇障礙物、點擊讓愛麗絲移動、旋轉鏡頭
public class GameInputController : MonoBehaviour
{
    [FormerlySerializedAs("CameraObj")] public GameObject cameraPivot;//旋轉鏡頭時轉動的物件
    [FormerlySerializedAs("_obj")] public FloatingObject[] floatingObjects;
    [FormerlySerializedAs("house_plane")] public HouseTrigger houseTrigger;

    private const float HoldTime = 1f;//機關要按住多久才能拖動
    private const float DoubleClickTime = 0.3f;//蘑菇兩次點擊的間隔上限
    private const float CameraTurnSpeed = 30f;

    //====機關按住拖動
    private DragHandle pressedHandle;//按下滑鼠時選到的機關，放開前都不會變
    private bool pressedOnWalkable;//按下的位置是機關上可以走的部分（例如柱頂）
    private Vector3 pressedPoint;
    private float holdTimer = HoldTime;
    private MovableMechanism draggedMechanism;//正在拖動的機關
    private ClickEffect draggedEffect;

    //====蘑菇障礙物，點擊二下
    private bool firstClick;
    private bool secondClick;
    private float doubleClickTimer;

    private bool IsDragging => draggedMechanism != null;

    void Update()
    {
        bool isOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        bool isLocked = CubeWorld.IsRotating;//正方體旋轉中不能操作，正在拖的機關也會放開

        if (GameInput.PointerDown && !isOverUI && !isLocked)//點到UI就不處理
        {
            OnPointerDown();
        }

        if (GameInput.PointerHeld && !isLocked)
        {
            if (pressedHandle != null)
            {
                HoldHandle();
            }
        }
        else if (pressedHandle != null || IsDragging)
        {
            OnPointerUp();
        }

        UpdateDoubleClickTimer();
        TurnCamera(GameInput.CameraTurn);
    }

    //按下的那一刻決定這次操作是：拖機關、點蘑菇，還是叫愛麗絲移動
    private void OnPointerDown()
    {
        Ray ray = Camera.main.ScreenPointToRay(GameInput.PointerPosition);
        if (!Physics.Raycast(ray, out RaycastHit hit))
            return;

        pressedHandle = FindHandle(hit);
        if (pressedHandle != null)
        {
            pressedOnWalkable = hit.transform.GetComponent<DragHandle>() == null;
            pressedPoint = hit.point;
            return;
        }

        if (hit.transform.TryGetComponent(out MushroomObstacle mushroom))
        {
            ClickMushroom(mushroom);
            return;
        }

        if (AliceController.Current != null)
        {
            AliceController.Current.MoveToScreenPoint(GameInput.PointerPosition);
        }
    }

    //找出射線打到的物件屬於哪個機關：打到把手本身，或機關底下其他的實體碰撞體(例如柱頂)都算
    private DragHandle FindHandle(RaycastHit hit)
    {
        if (hit.transform.TryGetComponent(out DragHandle handle))
            return handle;

        if (hit.collider.isTrigger)//trigger是蘑菇、房子地板等其他用途，不算機關
            return null;

        MovableMechanism mechanism = hit.transform.GetComponentInParent<MovableMechanism>();
        if (mechanism == null)
            return null;

        foreach (DragHandle candidate in mechanism.GetComponentsInChildren<DragHandle>())
        {
            if (candidate.mechanismObject == mechanism.gameObject)
                return candidate;
        }
        return null;
    }

    //機關要按住一段時間才會開始拖動
    private void HoldHandle()
    {
        if (IsDragging)
            return;

        holdTimer -= Time.deltaTime;
        if (holdTimer >= 0)
            return;

        MovableMechanism mechanism = pressedHandle.Mechanism;
        if (mechanism == null)
            return;

        draggedMechanism = mechanism;
        draggedMechanism.BeginDrag();

        draggedEffect = pressedHandle.GetComponent<ClickEffect>();
        if (draggedEffect != null)
        {
            draggedEffect.Press();
        }
    }

    private void OnPointerUp()
    {
        if (IsDragging)
        {
            draggedMechanism.EndDrag();
            draggedMechanism = null;

            //愛麗絲在房子裡時房子要維持透明材質，不能被放開的材質蓋掉
            bool keepHouseMaterial = draggedEffect != null && houseTrigger != null && houseTrigger.IsAliceInside && draggedEffect.targetRenderer == houseTrigger.houseRenderer;
            if (draggedEffect != null && !keepHouseMaterial)
            {
                draggedEffect.Release();
            }
            draggedEffect = null;
        }
        else if (pressedOnWalkable && AliceController.Current != null)
        {
            //在機關可以走的部分短按（還沒按到開始拖動）就當成叫愛麗絲走過去
            AliceController.Current.MoveTo(pressedPoint);
        }

        pressedHandle = null;
        holdTimer = HoldTime;
    }

    //蘑菇要在時間內點兩下才會移開
    private void ClickMushroom(MushroomObstacle mushroom)
    {
        if (!firstClick)
        {
            firstClick = true;
            secondClick = false;
            doubleClickTimer = DoubleClickTime;
        }
        else if (doubleClickTimer > 0)
        {
            secondClick = true;
        }

        if (doubleClickTimer > 0 && firstClick && secondClick)
        {
            mushroom.Remove();
        }
    }

    //第一下點擊後開始倒數，時間到沒有點第二下就重來
    private void UpdateDoubleClickTimer()
    {
        if (firstClick && !secondClick)
        {
            doubleClickTimer -= Time.deltaTime;
            if (doubleClickTimer <= 0)
            {
                firstClick = false;
                secondClick = false;
            }
        }
    }

    //旋轉鏡頭時漂浮物會跟著上下飄
    private void TurnCamera(float direction)
    {
        if (direction == 0 || cameraPivot == null)
            return;

        cameraPivot.transform.Rotate(new Vector3(0, 10, 0), direction * CameraTurnSpeed * Time.deltaTime);
        foreach (FloatingObject floating in floatingObjects)
        {
            if (floating != null)
            {
                floating.Bob();
            }
        }
    }
}
