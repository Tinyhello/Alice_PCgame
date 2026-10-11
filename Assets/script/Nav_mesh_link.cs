using UnityEngine;
using Unity.AI.Navigation;

//讓NavMeshLink的起點/終點跟著指定的物件移動
public class Nav_mesh_link : MonoBehaviour
{
    public NavMeshLink link;
    public Transform pos_start;
    public Transform pos_end;

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
