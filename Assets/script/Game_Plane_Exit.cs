using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Plane_Exit : MonoBehaviour
{
    public CentralDataViewer CentralDataViewer;//新增主控台cs
    public GameObject pnlClear;
    public GameObject TargetPlayerObj;
    
    void Start()
    {

    }
    void OnTriggerEnter(Collider TargetObj)
    {
        TargetPlayerObj = TargetObj.gameObject;
        if (TargetObj.gameObject.CompareTag("Alice"))
        {
            pnlClear.SetActive(true);

            if (CentralDataViewer.PlayerAlive == 1)
            {
                CentralDataViewer.Player_isBorn = false;
               
                TargetObj.gameObject.SetActive(false);//停用愛麗絲
                Destroy(TargetObj.gameObject, 2f);//2秒後消滅愛麗絲 
                CentralDataViewer.PlayerAlive--;//愛麗絲計數--
            }
        }
    }

}
