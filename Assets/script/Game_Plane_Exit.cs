using UnityEngine;

public class Game_Plane_Exit : MonoBehaviour
{
    public CentralDataViewer CentralDataViewer;//新增主控台cs
    public GameObject pnlClear;
    public GameObject TargetPlayerObj;
    public ParticleSystem Particle;

    public float settime = 3f;//特效播放幾秒後讓愛麗絲消失
    private float _time = 0f;

    private bool die = false;

    void Update()
    {
        delete();
    }

    private void delete()
    {
        if (!die)
            return;

        _time -= Time.deltaTime;

        if (_time <= 0)
        {
            pnlClear.SetActive(true);
            Particle.gameObject.SetActive(false);
            die = false;
        }
        else if (_time <= Particle.main.startLifetimeMultiplier - settime)
        {
            CentralDataViewer.RemoveAlice(TargetPlayerObj, 0f);
        }
    }

    void OnTriggerEnter(Collider TargetObj)
    {
        if (TargetObj.gameObject.CompareTag("Alice"))
        {
            TargetPlayerObj = TargetObj.gameObject;
            Particle.gameObject.SetActive(true);
            _time = Particle.main.startLifetimeMultiplier;
            die = true;
            delete();
            CentralDataViewer.GameFinish();
        }
    }
}
