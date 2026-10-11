using UnityEngine;

//愛麗絲走進房子時切換房子材質並關掉房子的collider
public class House_Plane : MonoBehaviour
{
    public Renderer HouseRenderer;
    public Material[] HouseMaterials;
    public Collider HouseCollider; //新增house的collider變數

    public GameObject aaa;//目前在房子裡的愛麗絲，不在房子裡時是null

    public bool IsAliceInside => aaa != null;

    void OnTriggerEnter(Collider TargetObj)
    {
        if (TargetObj.gameObject.CompareTag("Alice"))
        {
            HouseRenderer.material = HouseMaterials[1];
            HouseCollider.enabled = false;
            aaa = TargetObj.gameObject;
        }
    }

    void OnTriggerExit(Collider TargetObj)
    {
        if (TargetObj.gameObject.CompareTag("Alice"))
        {
            HouseRenderer.material = HouseMaterials[0];
            HouseCollider.enabled = true;
            aaa = null;
        }
    }
}
