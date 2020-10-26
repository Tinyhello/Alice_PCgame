using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Nav_mesh_link : MonoBehaviour
{
    public NavMeshLink link;
    //public Transform pos_cur;
    public Transform pos_start;
    public Transform pos_end;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (pos_start)
        {
            link.startPoint = transform.InverseTransformPoint(pos_start.position);
        }

        if (pos_end)
        {
            link.endPoint = transform.InverseTransformPoint(pos_end.position);
        }
           
    }
}
