using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bridge_effect : MonoBehaviour
{
    public Renderer houserenderer;
    public Material[] housematerials;
    public Color HighlightColor;
    public float offset = -10;
    private bool isDisplay = false;

    public AnimationCurve curve;
    public float showtime = 3;
    private float time = 0;
    //=================================
    public Animator Bridge_animator;//宣告橋的動畫
    public bridge_effect[] bridge_effects;
    public GameObject obstacle_bridge;
    //=================================播特效
    public GameObject particle_bridge;

    public void bridgeup()
    {
        //houserenderer.material = housematerials[0];
        offset = -10;
        houserenderer.sharedMaterial.SetColor("_EmissionColor", HighlightColor * Mathf.Pow(2, offset));
        isDisplay = true;
        time = 0;
        particle_bridge.SetActive(true);
    }

    public void Bridge_reset()
    {
        particle_bridge.SetActive(false);
    }


    public void bridgeOff()
    {
        //houserenderer.material = housematerials[1];
    }
    
    // Update is called once per frame
    void Update()
    {
        
        if (isDisplay)
        {
            time += Time.deltaTime;
            offset = curve.Evaluate(time/showtime);
            houserenderer.sharedMaterial.SetColor("_EmissionColor", HighlightColor * Mathf.Pow(2, offset));
            


            if (time >= showtime)
            {
                isDisplay = false;
                Bridge_animator.SetBool("IsBridge", true);
                if (Bridge_animator.GetBool("IsBridge") == true)
                {
                    for (int i = 0; i < bridge_effects.Length; i++)
                    {
                        //bridge_effects[i].

                    }
                }
            }            
            obstacle_bridge.transform.Translate(0, -20f, 0);
        }
    }

    
}
