using UnityEngine;
using UnityEngine.UI;

//背景音樂，音量由滑桿控制
public class SoundControl : MonoBehaviour
{
    public AudioClip music;
    public Slider slider;

    private AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        source.loop = true;
        source.volume = 0.5f;
        source.clip = music;
        source.Play();
    }

    private void Update()
    {
        source.volume = slider.value;
    }
}
