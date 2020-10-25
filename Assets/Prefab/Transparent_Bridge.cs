using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transparent_Bridge : MonoBehaviour
{
    public GameObject BridgeObj;
    public float WaitTime = 2f;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 10; i++)
        {
            if (WaitTime > 0) //等待時間>0就等待時間-每禎的平均時間
            {
                WaitTime -= Time.deltaTime;//等待時間-1
                if (WaitTime <= 0)
                {
                    Instantiate(BridgeObj, BridgeObj.transform.position*5, BridgeObj.transform.rotation);
                }

            }
        }
    }
}
