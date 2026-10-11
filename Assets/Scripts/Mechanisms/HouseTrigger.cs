using UnityEngine;
using UnityEngine.Serialization;

//愛麗絲走進房子時把房子換成透明材質並關掉房子的collider，離開時換回來
public class HouseTrigger : MonoBehaviour
{
    [FormerlySerializedAs("HouseRenderer")] public Renderer houseRenderer;
    [FormerlySerializedAs("HouseMaterials")] public Material[] houseMaterials;//0是一般，1是透明
    [FormerlySerializedAs("HouseCollider")] public Collider houseCollider;

    private GameObject aliceInside;//目前在房子裡的愛麗絲

    public bool IsAliceInside => aliceInside != null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Alice"))
        {
            houseRenderer.material = houseMaterials[1];
            houseCollider.enabled = false;
            aliceInside = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Alice"))
        {
            houseRenderer.material = houseMaterials[0];
            houseCollider.enabled = true;
            aliceInside = null;
        }
    }
}
