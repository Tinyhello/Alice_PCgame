using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class click_effect : MonoBehaviour
{
    public Renderer houserenderer;
    public Material[] housematerials;

    public void clickdown()
    {
        houserenderer.material = housematerials[0];
    }

    public void clickup()
    {
        houserenderer.material = housematerials[1];
    }




    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
