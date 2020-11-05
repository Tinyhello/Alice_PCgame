using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Float_Obj : MonoBehaviour
{
    float radian = 0; // 弧度
    float perRadian = 0.03f; // 每次变化的弧度
    float radius = 0.8f; // 半径
    Vector3 startPos; // 开始时候的坐标
    private float dy;
    private float dyy;
    private float x;
    private float a;
    private float cf;
    private float ccc = 0.03f;

    // Start is called before the first frame update
    void Start()
    {
        startPos = this.gameObject .transform.position; // 将最初的位置保存到oldPos
        perRadian = Random.Range(0.005f, 0.01f);
        radius = Random.Range(0.5f, 2.2f);
        //x = Random.Range(0f, 1f); 不能用這個，因為數值會固定
        x = Random.value;
        a = x <= 0.5 ? -1 : 1;
        //if (x <= 0.5)
        //{
        //    a = -1;
        //}
        //else if(x > 0.5)
        //{
        //    a = 1;
        //}
        
    }

    public void Float()
    {
        radian += perRadian; // 弧度每次加0.03
        //dy = -Mathf.Cos(radian) * radius; //隨機換方向
        //dyy = Mathf.Cos(radian) * radius;
        
        //if (x <= 0.5)
        //{
        //    cf = dy;
        //}
        //else if(x > 0.5)
        //{
            
        //    cf = dyy;
        //}

        //this.transform.position +=  new Vector3(0, cf * ccc, 0);
        this.transform.position +=  new Vector3(0, Mathf.Cos(radian) * radius*a * ccc, 0);
    }



}
