using UnityEngine;
using UnityEngine.Serialization;

//擋路的蘑菇，點兩下會移開
public class MushroomObstacle : MonoBehaviour
{
    [FormerlySerializedAs("child")] public GameObject visual;

    public void Remove()
    {
        if (visual != null)
        {
            transform.Translate(0, 0, 10f);//移開collider玩家才不會碰到
            visual.SetActive(false);
        }
    }
}
