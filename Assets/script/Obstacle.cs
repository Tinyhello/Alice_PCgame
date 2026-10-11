using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public GameObject child;
    public CentralDataViewer CentralDataViewer;//新增主控台cs
    private Vector3 pos;

    private void Start()
    {
        pos = transform.position;//機關初始座標
    }

    public void obstacleOff()
    {
        if (child != null)
        {
            transform.Translate(0, 0, 10f);  //之後設100玩家就不會碰到collider
            child.SetActive(false);
        }
    }

    public void Reset_MoveElement()//重置機關方法
    {
        transform.position = pos;//機關位置重置
        gameObject.SetActive(true);//機關父物件重新開啓
        transform.GetChild(0).gameObject.SetActive(true);//機關子物件重新開啓
    }
}
