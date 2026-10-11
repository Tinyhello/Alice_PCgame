using UnityEngine;

//兩個巡邏點的小兵
public class Enemy_Manger2 : Enemy_Base
{
    protected override float ChaseSpeed => 4f;

    protected override GameObject[] GetPatrolPoints()
    {
        return new[] { Pos1, Pos2 };
    }
}
