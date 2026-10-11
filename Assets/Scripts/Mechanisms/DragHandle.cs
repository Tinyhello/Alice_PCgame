using UnityEngine;
using UnityEngine.Serialization;

//機關上可以按住拖動的部分，指向它所帶動的機關
public class DragHandle : MonoBehaviour
{
    [FormerlySerializedAs("MoveObj")] public GameObject mechanismObject;

    public MovableMechanism Mechanism => mechanismObject != null ? mechanismObject.GetComponent<MovableMechanism>() : null;
}
