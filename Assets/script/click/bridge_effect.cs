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
    private bool isUp = false;
    public float upTimeOffset=1f;
    public float max = 6;
    public float min = -10;

    public AnimationCurve curve;
    public float showtime = 3;
    private float time = 0;
    //=================================
    public Animator Bridge_animator;//宣告橋的動畫
    public bridge_effect[] bridge_effects;
    public GameObject obstacle_bridge;
    public void bridgeup()
    {
        //houserenderer.material = housematerials[0];
        offset = -10;
        houserenderer.sharedMaterial.SetColor("_EmissionColor", HighlightColor * Mathf.Pow(2, offset));
        isDisplay = true;
        isUp = true;
        time = 0;
    }


    public void bridgeOff()
    {
        //houserenderer.material = housematerials[1];
    }

    




    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    bridgeup();
        //}

        if (isDisplay)
        {
            time += Time.deltaTime;
            offset = curve.Evaluate(time/showtime);
            //if (isUp)
            //    offset += Time.deltaTime * upTimeOffset;//= Mathf.Lerp(offset, 10, Time.deltaTime * upTimeOffset);
            //else
            //    offset -= Time.deltaTime * upTimeOffset;//= Mathf.Lerp(offset, -10, Time.deltaTime * upTimeOffset);
            houserenderer.sharedMaterial.SetColor("_EmissionColor", HighlightColor * Mathf.Pow(2, offset));

            //if (isUp && offset >= max)
            //{
            //    //houserenderer.material = housematerials[0];
            //    isUp = false;
            //}
            //else if (!isUp && offset <= min)
            //    isDisplay = false;
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
            obstacle_bridge.transform.Translate(0, -50f, 0);
        }
    }
}
