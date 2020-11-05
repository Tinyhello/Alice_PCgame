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
    private float cf;
    private float ccc = 0.03f;

    // Start is called before the first frame update
    void Start()
    {
        startPos = this.gameObject .transform.position; // 将最初的位置保存到oldPos
        perRadian = Random.Range(0.005f, 0.04f);
        radius = Random.Range(0.5f, 6.5f);
        x = Random.Range(0f, 1f);     
    }

    public void Float()
    {
        Debug.Log(x);
        radian += perRadian; // 弧度每次加0.03
        dy = -Mathf.Cos(radian) * radius; //隨機換方向
        dyy = Mathf.Cos(radian) * radius;
        
        if (x <= 0.5)
        {
            cf = dy;
            Debug.Log("a");
        }
        else if(x > 0.5)
        {
            
            cf = dyy;
            Debug.Log("b");

        }

        this.transform.position +=  new Vector3(0, cf * ccc, 0);
    }



}
