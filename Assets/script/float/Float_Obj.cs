using UnityEngine;

//旋轉鏡頭時會上下飄動的物件
public class Float_Obj : MonoBehaviour
{
    private const float ccc = 0.03f;//飄動幅度的縮放

    float radian = 0; // 弧度
    float perRadian = 0.03f; // 每次變化的弧度
    float radius = 0.8f; // 半徑
    private float a;//飄動方向，1或-1

    void Start()
    {
        perRadian = Random.Range(0.005f, 0.01f);
        radius = Random.Range(0.5f, 2.2f);
        a = Random.value <= 0.5 ? -1 : 1;//隨機決定方向
    }

    public void Float()
    {
        radian += perRadian;
        transform.position += new Vector3(0, Mathf.Cos(radian) * radius * a * ccc, 0);
    }
}
