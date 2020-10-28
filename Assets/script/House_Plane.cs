using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House_Plane : MonoBehaviour
{
    public Renderer HouseRenderer;
    public Material[] HouseMaterials;
    public Collider HouseCollider; //新增house的collider變數

    public GameObject aaa;
    void Start()
    {
       
        //HouseRenderer.material = GetComponent<Renderer>().material;
    }
    void OnTriggerEnter(Collider TargetObj)
    {
        if (TargetObj.gameObject.CompareTag("Alice"))
        {
            HouseRenderer.material = HouseMaterials[1];
            HouseCollider.enabled = false;
            aaa = TargetObj.gameObject;

            //Alice_Manger alice = TargetObj.GetComponent<Alice_Manger>();
        }
    }


    void OnTriggerExit(Collider TargetObj)
    {
        if (TargetObj.gameObject.CompareTag("Alice"))
        {
            HouseRenderer.material = HouseMaterials[0];
            HouseCollider.enabled = true;
        }
    }
}
