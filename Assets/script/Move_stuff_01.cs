using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_stuff_01 : MonoBehaviour
{
    private Vector3 pos;  //抓自己初始位置
    private Vector3 colliderSize;  //設定可移動距離
    private float mouseY;
    private float speed = 50f;
    //public MoveTarget MoveObj;
    private GameObject inobj;
    

    public float limithigh = 3;
    public CentralDataViewer CentralDataViewer;//新增主控台cs

    private bool isAliceHere = false;
    // Start is called before the first frame update
    void Start()
    {
        pos =new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);//機關初始座標



    }
    // Update is called once per frame
    void Update()
    {
        Move();
        
    }
   
    public void Move_stuff(GameObject inObj)
    {

        inobj = inObj;
    }

    public void Aliceclimb(bool istrue)
    {
        isAliceHere = istrue;
    }

    public void Move()
    {
        //Debug.Log("pos.y" + pos.y);
        //Debug.Log("this.gameObject" + this.gameObject.transform.position.y);
        if (isAliceHere)
            return;

        if (inobj == null)
        {
            return;
        }

        if (Vector3.Distance(pos, this.gameObject.transform.position) < limithigh)
        {
            mouseY = Input.GetAxis("Mouse Y");
            this.gameObject.transform.Translate(0, mouseY * speed * Time.deltaTime, 0);
            //Debug.Log("this.gameObject" + this.gameObject.transform.position.y);
            //Debug.Log("mouseY" + mouseY);
            //Debug.Log(this.gameObject);
        }
        else if (Vector3.Distance(pos, this.gameObject.transform.position) >= limithigh)
        {
            mouseY = 0;
        }
        else if (Vector3.Distance(pos, this.gameObject.transform.position) <= 0)
        {
            mouseY = 0f;
        }
        if (Vector3.Distance(pos, this.gameObject.transform.position) >= limithigh) //這行要放後面是因為要在同一個flame去移動或判斷
        {
            this.gameObject.transform.position = pos + limithigh*Vector3.up;
        }
        if (pos.y > this.gameObject.transform.position.y)//不讓此物件低於原本位置
        {

            this.gameObject.transform.position = new Vector3(pos.x, pos.y, pos.z);
            return;

        }
    }
    public void Reset_MoveElement()//重置機關方法
    {
            transform.position = pos;//機關位置重置
            gameObject.SetActive(true);//機關父物件重新開啓
            transform.GetChild(0).gameObject.SetActive(true);//機關子物件重新開啓
    }
}
