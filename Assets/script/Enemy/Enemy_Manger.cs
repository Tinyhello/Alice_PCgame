using UnityEngine;

//三個巡邏點的小兵
public class Enemy_Manger : Enemy_Base
{
    public GameObject Pos3;

    protected override float ChaseSpeed => 3.5f;
    protected override float? PatrolSpeed => 2f;

    protected override GameObject[] GetPatrolPoints()
    {
        return new[] { Pos1, Pos2, Pos3 };
    }
}
