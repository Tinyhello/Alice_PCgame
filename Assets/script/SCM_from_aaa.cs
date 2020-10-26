using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SCM_from_aaa : MonoBehaviour
{
    private bool part_control = true ;
    
    private Move_stuff_01 move_Stuff_01;

    //====機關按住滑動
    private bool mouse1 = true;//滑鼠選第一個
    private float mouse1durtime = 0;
    private float mouse1default = 0.5f;
    
    //====蘑菇障礙物，點擊二下
    private bool mouse2 = false;//滑鼠點第一下
    private bool mouse3 = false;//滑鼠點第二下
    private float mouse2durtime = 0f;
    private float mouse2default = 0.3f;
    //====判斷Alice的優先程度
    private bool alice_active1 = false;
    private bool alice_active2 = false;

    
    private bool choose1stuff = true; 
    RaycastHit hit;
    public LayerMask mask = 1 << 8; //打開第8層

    public GameObject mushroom_obstacle;
    private Obstacle a;
    public Alice_Manger alice_manger;

    private bool drag = false;

    private GameObject alice;
    // Start is called before the first frame update
    void Start()
    {
        mouse1durtime = 0;
        
    } 

    // Update is called once per frame
    void Update()
    {
        mousecontrol();
        mushroom_obstacle_tmiecontrol();
        ConfirmMoveObj();
        // Alicecontrol();
        //mouse1 = true的情形下只有Move_Obj能進去
        

    }
    
    private int mode = -1;       //-1=none,0=alice,1=蘑菇,2=柱子
    private GameObject hitObj = null;
    private Vector3 hitPoint = Vector3.zero;

    public void mousecontrol()
    {
        // mode=0;

        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(r, out hit))   //,500f, mask
        {
             
            if (Input.GetMouseButtonDown(0))
            {

                hitObj =  hit.transform.gameObject;
                if (hit.transform.CompareTag("Move_Obj"))
                {  
                    //初始設定
                    drag=false;
                    mouse1durtime=0;
                        // alice_active1 = false;
                    mode=2;
                        //停止控制Alice. 
                        //存滑鼠點到的物件
                }
                else if (hit.transform.CompareTag("Mushroom_Obstacle"))//蘑菇障礙物
                {
                    mode=1;
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
                else
                {
                    mode=0;
                    // alice_active1 = true;
                    // alice_active2 = true;
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                
                Debug.Log(mode);
                //判斷柱子
                if(mode==2) {
                    if(drag){
                        if (move_Stuff_01)
                        {
                            move_Stuff_01.Move_stuff(null);//機關儲存的value要切成null
                            mouse1durtime = mouse1default;//滑鼠只要離開機關計時器就要回默認值
                            // mouse1 = true;//滑鼠放開機關就不能動了
                        }
                        
                            // alice_active1 = true;
                        
                        //各項bool初始化\ 
                        mode = -1;
                        // mouse1 = false;
                    }else{
                        mode = 0;
                    }
                }

                if(mode == 0)
                {
                    hitPoint = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                    Alicecontrol(hitPoint);
                }

            }
                
                
                
            // }
                // Alicecontrol();
            
            

        }
        
        








        // if (Input.GetMouseButton(0))
        // {
        //     Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
        //     if (Physics.Raycast(r, out hit))   //,500f, mask
        //     {
        //         if (hit.transform.CompareTag("Move_Obj"))
        //         {  
        //             mouse1durtime -= Time.deltaTime;
                    
        //             if (mouse1durtime < 0)//這邊判斷要按住才算
        //             {
        //                 if(mouse1 == true)
        //                 {
        //                     MoveTarget mvTarget = hit.transform.GetComponent<MoveTarget>();
        //                     if (mvTarget != null)
        //                     {
                //                 move_Stuff_01 = mvTarget.MoveObj.GetComponent<Move_stuff_01>();
                //                 move_Stuff_01.Move_stuff(hit.transform.gameObject);
                //                 mouse1 = false;
                //                 alice_active1 = false;
                //             }
                //         }
                //     }
                //     //停止控制Alice. 
                //     //存滑鼠點到的物件
                // }
                // else if (Input.GetMouseButtonDown(0))
                // {
                //     if (hit.transform.CompareTag("Mushroom_Obstacle"))//蘑菇障礙物
                //     {
                //         alice_active2 = false;
                //         if (!mouse2) //if括號裡面的東西是運算子(包含!)，然後if判斷裡面的運算子算完後，是true才能進去，是false就不能進去
                //         {
                //             mouse2 = true;//這邊的mouse2 = ture跟上面的ture計算無關
                //             mouse3 = false;
                //             mouse2durtime = mouse2default;
                //         }
                //         else if (mouse2durtime > 0)
        //                 {
        //                     mouse3 = true;
        //                 }
        //                 if (mouse2durtime > 0 && mouse2 == true && mouse3 == true)//限制條件
        //                 {
        //                     a = hit.transform.gameObject.GetComponent<Obstacle>();
        //                     if (a != null)
        //                     {
        //                         mushroom_obstacle = hit.transform.gameObject;//射線打到磨菇要用變數存起來
        //                         a.obstacleOff();
        //                     }
        //                 }
        //             }
        //         }
        //         else
        //         {
        //             alice_active1 = true;
        //             alice_active2 = true;
        //         }
        //     }
        // }
        // else if (Input.GetMouseButtonUp(0))
        // {
        //     if (move_Stuff_01)
        //     {
        //         move_Stuff_01.Move_stuff(null);//機關儲存的value要切成null
        //         mouse1durtime = mouse1default;//滑鼠只要離開機關計時器就要回默認值
        //         mouse1 = true;//滑鼠放開機關就不能動了
        //     }
            
        // }





    }

    public void Alicecontrol(Vector3 inPoint)
    {
        //Debug.Log("mode="+mode);
        if(mode == 0)
        // if(alice_active1 ==true && alice_active2 == true)
        {
            alice = GameObject.FindWithTag("Alice");
            //alice.GetComponent<Alice_Manger>().Alice_navmeshlink(true, inPoint);
            //mode=-1;
        }
        // alice_active1 = false;
        // alice_active2 = false;
      
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
                mode=-1;
            }
        }else{
            //mode=-1;
        }
        
    }

    private void ConfirmMoveObj()
    {
        if(hitObj == null)
        {
            return; 
        }
        //只有
        if(mode == 2)
        {
            mouse1durtime += Time.deltaTime;
            if (mouse1durtime > mouse1default){
                drag = true;
                MoveTarget mvTarget = hitObj.transform.GetComponent<MoveTarget>();
                if (mvTarget != null)
                {
                    move_Stuff_01 = mvTarget.MoveObj.GetComponent<Move_stuff_01>();
                    move_Stuff_01.Move_stuff(hitObj);
                    // mouse1 = false;
                                    // alice_active1 = false;
                }
            }
        }
    }
}
