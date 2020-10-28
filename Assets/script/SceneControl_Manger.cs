using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class SceneControl_Manger : MonoBehaviour
{
    private bool part_control = true ;
    
    private Move_stuff_01 move_Stuff_01;

    //====機關按住滑動
    private bool mouse1 = true;//滑鼠選第一個
    private float mouse1durtime = 0;
    private float mouse1default = 1f;
    
    //====蘑菇障礙物，點擊二下
    private bool mouse2 = false;//滑鼠點第一下
    private bool mouse3 = false;//滑鼠點第二下
    private float mouse2durtime = 0f;
    private float mouse2default = 0.3f;
    //====判斷Alice的優先程度
    private bool alice_active1 = false;
    private bool alice_active2 = false;

    RaycastHit hit;
    public LayerMask mask = 1 << 8; //打開第8層

    public GameObject mushroom_obstacle;
    private Obstacle a;
    public Alice_Manger alice_manger;

    //public Renderer HouseRenderer;換材質註解
    //public Material[] HouseMaterials;換材質註解

    private click_house click;

    private GameObject alice;
    // Start is called before the first frame update
    void Start()
    {
        mouse1durtime = mouse1default;

        //HouseRenderer.material = GetComponent<Renderer>().material; 換材質註解
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return; //如果點到UI就跳出不執行後面的Raycast
        mousecontrol();
        mushroom_obstacle_tmiecontrol();
        Alicecontrol();
    }

    public void mousecontrol()
    {
        if (Input.GetMouseButton(0))
        {
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(r, out hit))   //,500f, mask  ,LayerMask.GetMask("Player")
            {
                //Debug.DrawLine(Camera.main.transform.position, hit.point, Color.red);
                //Debug.Log(hit.collider.gameObject.name);
                //if (hit.collider.gameObject.CompareTag("House_Plane"))  射線射到房子地板會替換房子的材質
                //{
                //    HouseRenderer.material = HouseMaterials[1];
                //}

                if (hit.transform.CompareTag("Move_Obj"))
                {
                    alice_active1 = false;
                    mouse1durtime -= Time.deltaTime;
                    click = hit.transform.GetComponent<click_house>();
                    if (click != null)
                    {
                        //click.changematerial();
                    }


                    if (mouse1durtime < 0)//這邊判斷要按住才算
                    {
                        if (mouse1 == true)
                        {
                            //Debug.Log("d");
                            MoveTarget mvTarget = hit.transform.GetComponent<MoveTarget>();
                            if (mvTarget != null)
                            {
                                //Debug.Log("e");
                                move_Stuff_01 = mvTarget.MoveObj.GetComponent<Move_stuff_01>();
                                move_Stuff_01.Move_stuff(hit.transform.gameObject);
                                mouse1 = false;
                            }
                        }
                    }
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    if (hit.transform.CompareTag("Mushroom_Obstacle"))//蘑菇障礙物
                    {
                        alice_active2 = false;
                        if (!mouse2) //if括號裡面的東西是運算子(包含!)，然後if判斷裡面的運算子算完後，是true才能進去，是false就不能進去
                        {
                            mouse2 = true;//這邊的mouse2 = ture跟上面的ture計算無關
                            mouse3 = false;
                            mouse2durtime = mouse2default;
                        }
                        else if (mouse2durtime > 0)
                        {
                            mouse3 = true;
                        }
                        if (mouse2durtime > 0 && mouse2 == true && mouse3 == true)//限制條件
                        {
                            a = hit.transform.gameObject.GetComponent<Obstacle>();
                            if (a != null)
                            {
                                mushroom_obstacle = hit.transform.gameObject;//射線打到磨菇要用變數存起來
                                a.obstacleOff();
                                
                            }
                        }
                    }
                }
                else
                {
                    alice_active1 = true;
                    alice_active2 = true;
                    
                }
            }
        }
        if(Input.GetMouseButtonUp(0))
        {
            //click.changematerial2();
            if (move_Stuff_01)
            {
                move_Stuff_01.Move_stuff(null);//機關儲存的value要切成null
                mouse1durtime = mouse1default;//滑鼠只要離開機關計時器就要回默認值
                mouse1 = true;//滑鼠放開機關就不能動了
                
            }
            
        }
    }

    public void Alicecontrol()
    {

        if(alice_active1 ==true && alice_active2 == true)
        {
            
            alice = GameObject.FindWithTag("Alice");
            if(alice != null)
            {
                alice.GetComponent<Alice_Manger>().Alice_navmeshlink();
            }
            
        }
    }
   
    public void mushroom_obstacle_tmiecontrol()
    {
        if (mouse2 == true && mouse3==false)
        {
            mouse2durtime -= Time.deltaTime;
            if (mouse2durtime <= 0)
            {
                mouse2 = false;
                mouse3 = false;
            }
        }
        
    }
}
