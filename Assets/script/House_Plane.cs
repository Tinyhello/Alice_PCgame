using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House_Plane : MonoBehaviour
{
    public Renderer HouseRenderer;
    public Material[] HouseMaterials;
    void Start()
    {
       
        //HouseRenderer.material = GetComponent<Renderer>().material;
    }
    void OnTriggerEnter(Collider TargetObj)
    {
        if (TargetObj.gameObject.CompareTag("Alice"))
        {
            HouseRenderer.material = HouseMaterials[1];
        }
    }


    void OnTriggerExit(Collider TargetObj)
    {
        if (TargetObj.gameObject.CompareTag("Alice"))
        {
            HouseRenderer.material = HouseMaterials[0];
        }
    }
}
