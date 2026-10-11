using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

//正方體的一面，自己的Y軸朝外。沒有朝上的時候NavMesh要卸掉、面上的敵人要停住
public class CubeFace : MonoBehaviour
{
    private readonly List<Behaviour> disabledBehaviours = new List<Behaviour>();
    private readonly List<Rigidbody> frozenBodies = new List<Rigidbody>();
    private bool isActive = true;

    public void Deactivate()
    {
        if (!isActive)
            return;
        isActive = false;

        //先停敵人再卸NavMesh，Activate時會倒過來開
        Disable(GetComponentsInChildren<EnemyBase>(true));
        Disable(GetComponentsInChildren<NavMeshAgent>(true));
        Disable(GetComponentsInChildren<NavMeshLink>(true));
        Disable(GetComponentsInChildren<NavMeshSurface>(true));

        //面轉到側邊後剛體會被重力拉下去，先固定住
        foreach (Rigidbody body in GetComponentsInChildren<Rigidbody>(true))
        {
            if (!body.isKinematic && !IsAlice(body))
            {
                body.isKinematic = true;
                frozenBodies.Add(body);
            }
        }
    }

    public void Activate()
    {
        if (isActive)
            return;
        isActive = true;

        for (int i = disabledBehaviours.Count - 1; i >= 0; i--)
        {
            if (disabledBehaviours[i] != null)
            {
                disabledBehaviours[i].enabled = true;
            }
        }
        disabledBehaviours.Clear();

        foreach (Rigidbody body in frozenBodies)
        {
            if (body != null)
            {
                body.isKinematic = false;
            }
        }
        frozenBodies.Clear();
    }

    //只關原本有開的，Activate時才不會把本來就關著的打開
    private void Disable<T>(T[] behaviours) where T : Behaviour
    {
        foreach (T behaviour in behaviours)
        {
            if (behaviour.enabled && !IsAlice(behaviour))
            {
                behaviour.enabled = false;
                disabledBehaviours.Add(behaviour);
            }
        }
    }

    //愛麗絲換面時由CubeWorld處理，不跟著面一起停
    private static bool IsAlice(Component component)
    {
        return component.GetComponentInParent<AliceController>(true) != null;
    }
}
