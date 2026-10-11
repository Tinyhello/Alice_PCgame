using UnityEngine;

//面的邊界（trigger）：愛麗絲踩進來就越界到target所在的面
[RequireComponent(typeof(Collider))]
public class CubeEdge : MonoBehaviour
{
    public CubeEdge target;//越界後抵達的邊界，在相鄰的面上
    public Transform arrivalPoint;//從別的面過來時愛麗絲站的位置與面向，要在trigger範圍外

    public CubeFace Face { get; private set; }

    private CubeWorld world;

    private void Awake()
    {
        Face = GetComponentInParent<CubeFace>();
        world = GetComponentInParent<CubeWorld>();
    }

    private void OnTriggerStay(Collider other)
    {
        AliceController alice = other.GetComponentInParent<AliceController>();
        if (alice != null && alice == AliceController.Current)
        {
            world.Cross(this, alice);
        }
    }
}
