using UnityEngine;
using UnityEngine.Serialization;

//終點：愛麗絲走進來後播放特效、讓愛麗絲消失，最後顯示過關畫面
public class LevelExit : MonoBehaviour
{
    [FormerlySerializedAs("CentralDataViewer")] public GameManager gameManager;
    [FormerlySerializedAs("pnlClear")] public GameObject clearPanel;
    [FormerlySerializedAs("Particle")] public ParticleSystem exitParticle;
    [FormerlySerializedAs("settime")] public float vanishDelay = 3f;//特效播放幾秒後讓愛麗絲消失

    private GameObject alice;
    private float timeLeft;//特效還剩多久
    private bool isFinishing;

    private void Update()
    {
        UpdateFinish();
    }

    private void UpdateFinish()
    {
        if (!isFinishing)
            return;

        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0)
        {
            clearPanel.SetActive(true);
            exitParticle.gameObject.SetActive(false);
            isFinishing = false;
        }
        else if (timeLeft <= exitParticle.main.startLifetimeMultiplier - vanishDelay)
        {
            gameManager.RemoveAlice(alice, 0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Alice"))
        {
            alice = other.gameObject;
            exitParticle.gameObject.SetActive(true);
            timeLeft = exitParticle.main.startLifetimeMultiplier;
            isFinishing = true;
            UpdateFinish();
            gameManager.FinishLevel();
        }
    }
}
