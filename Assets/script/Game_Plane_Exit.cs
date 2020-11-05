using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Plane_Exit : MonoBehaviour
{
    public CentralDataViewer CentralDataViewer;//新增主控台cs
    public GameObject pnlClear;
    public GameObject TargetPlayerObj;
    public ParticleSystem Particle;

    public float settime = 3f; 
    private float _time = 0f;
    
    private bool die =false ;
    private bool diee = false;


    void Start()
    {

    }

    void Update()
    {
        delete();



    }

    private void delete()
    {
        if(die == true)
        {

            _time -= Time.deltaTime;

            if (_time <= 0)
            {
                //Debug.Log(_time);
                pnlClear.SetActive(true);
                Particle.gameObject.SetActive(false);
                die = false;
            }
            else if(_time <= Particle.main.startLifetimeMultiplier - settime)
            { 
                if (CentralDataViewer.PlayerAlive == 1)
                {
                    CentralDataViewer.Player_isBorn = false;

                    TargetPlayerObj.gameObject.SetActive(false);//停用愛麗絲
                    Destroy(TargetPlayerObj.gameObject/*, 2f*/);//2秒後消滅愛麗絲 
                    CentralDataViewer.PlayerAlive--;//愛麗絲計數--
                }
            }
        }
        
        
    }





    void OnTriggerEnter(Collider TargetObj)
    {
        TargetPlayerObj = TargetObj.gameObject;
        if (TargetObj.gameObject.CompareTag("Alice"))
        {
            Particle.gameObject.SetActive(true);
            _time = Particle.main.startLifetimeMultiplier;
            //Debug.Log("OnTriggerEnter:"+_time);
            die = true;
            delete();
            CentralDataViewer.GameFinish();







        }
    }

}
