using System.Collections;
using UnityEngine;

//正方體關卡：只有朝上的那一面可以走，愛麗絲越過邊界時把正方體轉到下一面朝上
public class CubeWorld : MonoBehaviour
{
    //場上的正方體，一般的平面關卡是null
    public static CubeWorld Current { get; private set; }

    public CubeFace startFace;//遊戲開始與重生時朝上的面
    public float rotateDuration = 1.2f;

    private const float EdgeCooldown = 0.3f;//轉完後多久內不會再觸發越界
    private const float CornerLift = 0.7f;//愛麗絲翻過邊角時離開表面的高度

    private CubeFace[] faces;
    private CubeFace currentFace;
    private Quaternion startRotation;
    private bool isRotating;
    private float edgeReadyTime;

    //旋轉中不接受玩家操作
    public static bool IsRotating => Current != null && Current.isRotating;

    private void Awake()
    {
        faces = GetComponentsInChildren<CubeFace>(true);
        startRotation = transform.rotation;
        currentFace = startFace;
    }

    private void OnEnable()
    {
        Current = this;
    }

    private void OnDisable()
    {
        if (Current == this)
        {
            Current = null;
        }
    }

    private void Start()
    {
        foreach (CubeFace face in faces)
        {
            if (face != startFace)
            {
                face.Deactivate();
            }
        }
    }

    //愛麗絲踩到邊界：轉到邊界另一頭所在的面
    public void Cross(CubeEdge from, AliceController alice)
    {
        if (isRotating || Time.time < edgeReadyTime)
            return;
        if (from.target == null || from.Face != currentFace)
            return;

        StartCoroutine(CrossRoutine(from.target, alice));
    }

    private IEnumerator CrossRoutine(CubeEdge to, AliceController alice)
    {
        isRotating = true;
        CubeFace fromFace = currentFace;
        CubeFace toFace = to.Face;

        fromFace.Deactivate();
        alice.DetachFromNavMesh();
        Transform aliceTransform = alice.transform;
        aliceTransform.SetParent(transform, true);//跟著正方體一起轉

        //愛麗絲在正方體座標裡從出發點翻過邊角走到抵達點，以下都是正方體的local座標
        Vector3 fromPosition = aliceTransform.localPosition;
        Quaternion fromRotation = aliceTransform.localRotation;
        Vector3 toPosition = transform.InverseTransformPoint(to.arrivalPoint.position);
        Quaternion toRotation = Quaternion.Inverse(transform.rotation) * to.arrivalPoint.rotation;
        Vector3 fromUp = transform.InverseTransformDirection(fromFace.transform.up);
        Vector3 toUp = transform.InverseTransformDirection(toFace.transform.up);
        Vector3 edgePoint = fromPosition + toUp * Vector3.Dot(toPosition - fromPosition, toUp);
        Vector3 corner = edgePoint + (fromUp + toUp) * CornerLift;

        Quaternion fromCube = transform.rotation;
        Quaternion toCube = Quaternion.FromToRotation(toFace.transform.up, Vector3.up) * fromCube;

        for (float time = 0; time < rotateDuration; time += Time.deltaTime)
        {
            float t = Mathf.SmoothStep(0, 1, time / rotateDuration);
            transform.rotation = Quaternion.Slerp(fromCube, toCube, t);
            if (alice != null)
            {
                Vector3 a = Vector3.Lerp(fromPosition, corner, t);
                Vector3 b = Vector3.Lerp(corner, toPosition, t);
                aliceTransform.localPosition = Vector3.Lerp(a, b, t);
                aliceTransform.localRotation = Quaternion.Slerp(fromRotation, toRotation, t);
            }
            yield return null;
        }

        transform.rotation = toCube;
        currentFace = toFace;
        currentFace.Activate();

        if (alice != null)
        {
            aliceTransform.SetParent(toFace.transform, true);
            aliceTransform.SetPositionAndRotation(to.arrivalPoint.position, to.arrivalPoint.rotation);
            if (alice.isActiveAndEnabled)//旋轉途中死掉就不用接回NavMesh
            {
                alice.AttachToNavMesh();
            }
        }

        edgeReadyTime = Time.time + EdgeCooldown;
        isRotating = false;
    }

    //重生時直接轉回起始面，不播旋轉過程
    public void ResetToStartFace()
    {
        StopAllCoroutines();
        isRotating = false;

        foreach (CubeFace face in faces)
        {
            face.Deactivate();
        }
        transform.rotation = startRotation;
        currentFace = startFace;
        currentFace.Activate();
    }
}
