using UnityEngine;
using UnityEngine.Serialization;

//三個巡邏點的小兵
public class ThreePointPatrolEnemy : EnemyBase
{
    [FormerlySerializedAs("Pos3")] public GameObject point3;

    protected override float ChaseSpeed => 3.5f;
    protected override float? PatrolSpeed => 2f;

    protected override GameObject[] GetPatrolPoints()
    {
        return new[] { point1, point2, point3 };
    }
}
