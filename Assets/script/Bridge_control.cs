using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge_control : MonoBehaviour
{
    public GameObject bridge_obstacle;
    public CentralDataViewer CentralDataViewer;//新增主控台cs

    private Vector3 pos;
    // Start is called before the first frame update
    void Start()
    {
        pos = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        
    }

    // Update is called once per frame
    void Update()
    {
        
        

    }

    public void bridge()
    {
        //if (CentralDataViewer.Item[1].SetActive(true))
        //{
        //    CentralDataViewer.CameraObj[0].SetActive(false);//關掉迷宮1攝影機
        //    CentralDataViewer.CameraObj[3].SetActive(true);//開啓橋動畫攝影機
        //                                                   //bridge_obstacle.transform.Translate(0, -50f, 0);//障礙物移開
        //                                                   //bridge_obstacle.SetActive(false);//障礙物關掉
        //                                                   //播動畫
        //}
    }


    public void Reset_MoveElement()//重置機關方法
    {
        transform.position = pos;//機關位置重置
        gameObject.SetActive(true);//機關父物件重新開啓
        transform.GetChild(0).gameObject.SetActive(true);//機關子物件重新開啓
    }

}
