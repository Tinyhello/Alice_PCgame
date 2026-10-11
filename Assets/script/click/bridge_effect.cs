using UnityEngine;

//拿到鑰匙後橋的發光特效，播完才升起橋
public class bridge_effect : MonoBehaviour
{
    public Renderer houserenderer;
    public Material[] housematerials;
    public Color HighlightColor;
    public float offset = -10;
    private bool isDisplay = false;

    public AnimationCurve curve;
    public float showtime = 3;
    private float time = 0;
    //=================================
    public Animator Bridge_animator;//宣告橋的動畫
    public bridge_effect[] bridge_effects;
    public GameObject obstacle_bridge;
    //=================================播特效
    public GameObject particle_bridge;

    public void bridgeup()
    {
        offset = -10;
        SetEmission();
        isDisplay = true;
        time = 0;
        particle_bridge.SetActive(true);
    }

    public void Bridge_reset()
    {
        particle_bridge.SetActive(false);
    }

    void Update()
    {
        if (isDisplay)
        {
            time += Time.deltaTime;
            offset = curve.Evaluate(time / showtime);
            SetEmission();

            if (time >= showtime)
            {
                isDisplay = false;
                Bridge_animator.SetBool("IsBridge", true);
            }
            obstacle_bridge.transform.Translate(0, -20f, 0);
        }
    }

    private void SetEmission()
    {
        houserenderer.sharedMaterial.SetColor("_EmissionColor", HighlightColor * Mathf.Pow(2, offset));
    }
}
