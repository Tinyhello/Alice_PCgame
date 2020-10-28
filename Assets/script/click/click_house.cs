using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class click_house : MonoBehaviour
{
    public Renderer houserenderer;
    public Material[] housematerials;

    

    private void OnMouseDown( )
    {
        houserenderer.material = housematerials[0];
    }

    private void OnMouseUp( )
    {
        houserenderer.material = housematerials[1];
    }


    public void changematerial()
    {
        houserenderer.material = housematerials[0];
    }

    public void changematerial2()
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
