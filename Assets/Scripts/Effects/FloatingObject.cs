using UnityEngine;

//旋轉鏡頭時會上下飄動的物件
public class FloatingObject : MonoBehaviour
{
    private const float AmplitudeScale = 0.03f;

    private float radian;
    private float radianStep;//每次變化的弧度
    private float radius;
    private float direction;//飄動方向，1或-1

    private void Start()
    {
        radianStep = Random.Range(0.005f, 0.01f);
        radius = Random.Range(0.5f, 2.2f);
        direction = Random.value <= 0.5 ? -1 : 1;
    }

    public void Bob()
    {
        radian += radianStep;
        transform.position += new Vector3(0, Mathf.Cos(radian) * radius * direction * AmplitudeScale, 0);
    }
}
