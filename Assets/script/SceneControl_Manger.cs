using UnityEngine;
using UnityEngine.EventSystems;

//滑鼠與鍵盤操作：拖動機關、點兩下蘑菇障礙物、控制愛麗絲移動、旋轉鏡頭
public class SceneControl_Manger : MonoBehaviour
{
    //====旋轉鏡頭
    public GameObject CameraObj;
    public Float_Obj[] _obj;

    public LayerMask mask = 1 << 8; //打開第8層

    public GameObject mushroom_obstacle;
    public Alice_Manger alice_manger;

    public House_Plane house_plane;

    private const float HoldTime = 1f;//機關要按住多久才能拖動
    private const float DoubleClickTime = 0.3f;//蘑菇兩次點擊的間隔上限
    private const float CameraTurnSpeed = 30f;

    //====機關按住滑動
    private Move_stuff_01 move_Stuff_01;
    private click_effect click;
    private bool isDragging = false;
    private float holdTimer = HoldTime;
    private GameObject pressedHandle;//按下滑鼠時選到的機關，放開前都不會變
    private bool pressedOnWalkable = false;//按下的位置是機關上可以走的部分（例如柱頂）
    private Vector3 pressedPoint;

    //====蘑菇障礙物，點擊二下
    private bool firstClick = false;//滑鼠點第一下
    private bool secondClick = false;//滑鼠點第二下
    private float doubleClickTimer = 0f;

    //====判斷Alice的優先程度，兩個都是true才能控制愛麗絲
    private bool alice_active1 = false;//滑鼠沒有按在機關上
    private bool alice_active2 = false;//滑鼠沒有點在蘑菇上

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return; //如果點到UI就跳出不執行後面的Raycast
        mousecontrol();
        mushroom_obstacle_tmiecontrol();
        Alicecontrol();
        Cameraturn();
    }

    public void mousecontrol()
    {
        if (Input.GetMouseButton(0))
        {
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool isHit = Physics.Raycast(r, out RaycastHit hit);

            if (Input.GetMouseButtonDown(0))
            {
                //按下的那一刻決定選到哪個機關，之後滑鼠滑開也算按住同一個
                pressedHandle = isHit ? FindHandle(hit) : null;
                pressedOnWalkable = isHit && !hit.transform.CompareTag("Move_Obj");
                pressedPoint = hit.point;
            }

            if (pressedHandle != null)
            {
                alice_active1 = false;//停止控制Alice
                HoldMoveObj(pressedHandle);
                return;
            }

            if (!isHit)
                return;

            if (hit.transform.CompareTag("Move_Obj"))
            {
                alice_active1 = false;
            }
            else if (Input.GetMouseButtonDown(0))
            {
                if (hit.transform.CompareTag("Mushroom_Obstacle"))//蘑菇障礙物
                {
                    alice_active2 = false;
                    ClickMushroom(hit.transform.gameObject);
                }
            }
            else
            {
                alice_active1 = true;
                alice_active2 = true;
            }
        }
        else if (Input.GetMouseButtonUp(0) || pressedHandle != null || isDragging)//在UI上放開時收不到MouseUp，所以沒按著也要放開
        {
            ReleaseMoveObj();
        }
    }

    //找出射線打到的物件屬於哪個機關：打到機關本身(Move_Obj)，或機關底下其他的實體碰撞體(例如柱頂)都算
    private GameObject FindHandle(RaycastHit hit)
    {
        if (hit.transform.CompareTag("Move_Obj"))
            return hit.transform.gameObject;

        if (hit.collider.isTrigger)//trigger是蘑菇、房子地板等其他用途，不算機關
            return null;

        Move_stuff_01 mechanism = hit.transform.GetComponentInParent<Move_stuff_01>();
        if (mechanism == null)
            return null;

        foreach (MoveTarget target in mechanism.GetComponentsInChildren<MoveTarget>())
        {
            if (target.MoveObj == mechanism.gameObject)
                return target.gameObject;
        }
        return null;
    }

    //機關要按住一段時間才會開始拖動
    private void HoldMoveObj(GameObject target)
    {
        holdTimer -= Time.deltaTime;
        if (holdTimer >= 0 || isDragging)
            return;

        MoveTarget mvTarget = target.GetComponent<MoveTarget>();
        if (mvTarget == null)
            return;

        click = target.GetComponent<click_effect>();
        move_Stuff_01 = mvTarget.MoveObj.GetComponent<Move_stuff_01>();
        move_Stuff_01.Move_stuff(target);//存滑鼠點到的物件
        isDragging = true;

        if (click != null)
        {
            click.clickdown();
        }
    }

    private void ReleaseMoveObj()
    {
        //在機關可以走的部分短按（還沒按到開始拖動）就當成叫愛麗絲走過去
        if (pressedHandle != null && pressedOnWalkable && !isDragging)
        {
            GameObject alice = GameObject.FindWithTag("Alice");
            if (alice != null)
            {
                alice.GetComponent<Alice_Manger>().MoveTo(pressedPoint);
            }
        }

        pressedHandle = null;
        holdTimer = HoldTime;//滑鼠放開計時器就要回默認值
        alice_active1 = true;//放開機關後恢復控制Alice

        if (move_Stuff_01)
        {
            move_Stuff_01.Move_stuff(null);//機關儲存的value要切成null
            isDragging = false;//滑鼠放開機關就不能動了

            //愛麗絲在房子裡時房子要維持透明材質，不能被放開的材質蓋掉
            bool keepHouseMaterial = house_plane.IsAliceInside && click != null && click.houserenderer == house_plane.HouseRenderer;
            if (click != null && !keepHouseMaterial)
            {
                click.clickup();
            }
        }
    }

    //蘑菇要在時間內點兩下才會移開
    private void ClickMushroom(GameObject target)
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
            Obstacle obstacle = target.GetComponent<Obstacle>();
            if (obstacle != null)
            {
                mushroom_obstacle = target;//射線打到磨菇要用變數存起來
                obstacle.obstacleOff();
            }
        }
    }

    public void Alicecontrol()
    {
        if (alice_active1 && alice_active2)
        {
            GameObject alice = GameObject.FindWithTag("Alice");
            if (alice != null)
            {
                alice.GetComponent<Alice_Manger>().Alice_navmeshmove();
            }
        }
    }

    //第一下點擊後開始倒數，時間到沒有點第二下就重來
    public void mushroom_obstacle_tmiecontrol()
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

    public void Cameraturn()
    {
        if (Input.GetKey(KeyCode.A))
        {
            TurnCamera(CameraTurnSpeed);
        }

        if (Input.GetKey(KeyCode.D))
        {
            TurnCamera(-CameraTurnSpeed);
        }
    }

    //旋轉鏡頭時漂浮物會跟著上下飄
    private void TurnCamera(float speed)
    {
        if (CameraObj == null)
            return;

        CameraObj.transform.Rotate(new Vector3(0, 10, 0), speed * Time.deltaTime);
        for (int i = 0; i < _obj.Length; i++)
        {
            if (_obj[i] != null)
            {
                _obj[i].Float();
            }
        }
    }
}
