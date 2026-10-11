using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.Serialization;

//讓NavMeshLink的起點/終點跟著指定的物件，機關移動時連結才會接到正確的位置
public class NavMeshLinkFollower : MonoBehaviour
{
    public NavMeshLink link;
    [FormerlySerializedAs("pos_start")] public Transform startAnchor;
    [FormerlySerializedAs("pos_end")] public Transform endAnchor;

    private void Update()
    {
        if (startAnchor)
        {
            link.startPoint = transform.InverseTransformPoint(startAnchor.position);
        }

        if (endAnchor)
        {
            link.endPoint = transform.InverseTransformPoint(endAnchor.position);
        }
    }
}
