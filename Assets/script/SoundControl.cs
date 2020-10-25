using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundControl: MonoBehaviour
{
    public AudioClip music;
    private AudioSource back;
    public Slider slider;
    void Start()
    {
        back = this.GetComponent<AudioSource>();
        back.loop = true; //設置循環播放  
        back.volume = 0.5f;//設置音量最大，區間在0-1之間
        back.clip = music;
        back.Play(); //播放背景音樂
    }

    void Update()
    {
        back.volume = slider.value;
    }
}