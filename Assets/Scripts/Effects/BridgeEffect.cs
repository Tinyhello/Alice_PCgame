using UnityEngine;
using UnityEngine.Serialization;

//拿到鑰匙後橋的發光特效，播完才升起橋
public class BridgeEffect : MonoBehaviour
{
    [FormerlySerializedAs("houserenderer")] public Renderer targetRenderer;
    [FormerlySerializedAs("HighlightColor")] public Color highlightColor;
    public AnimationCurve curve;//發光強度隨時間的變化
    [FormerlySerializedAs("showtime")] public float duration = 3;
    [FormerlySerializedAs("Bridge_animator")] public Animator bridgeAnimator;
    [FormerlySerializedAs("obstacle_bridge")] public GameObject bridgeObstacle;//擋住橋的障礙物
    [FormerlySerializedAs("particle_bridge")] public GameObject particle;

    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    private const float StartIntensity = -10;

    private bool isPlaying;
    private float time;

    public void Play()
    {
        SetEmission(StartIntensity);
        isPlaying = true;
        time = 0;
        particle.SetActive(true);
    }

    public void ResetEffect()
    {
        particle.SetActive(false);
    }

    private void Update()
    {
        if (!isPlaying)
            return;

        time += Time.deltaTime;
        SetEmission(curve.Evaluate(time / duration));

        if (time >= duration)
        {
            isPlaying = false;
            bridgeAnimator.SetBool("IsBridge", true);
        }
        bridgeObstacle.transform.Translate(0, -20f, 0);
    }

    private void SetEmission(float intensity)
    {
        targetRenderer.sharedMaterial.SetColor(EmissionColor, highlightColor * Mathf.Pow(2, intensity));
    }
}
