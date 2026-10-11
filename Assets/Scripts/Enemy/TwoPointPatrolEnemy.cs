using UnityEngine;

//兩個巡邏點的小兵
public class TwoPointPatrolEnemy : EnemyBase
{
    protected override float ChaseSpeed => 4f;

    protected override GameObject[] GetPatrolPoints()
    {
        return new[] { point1, point2 };
    }
}
