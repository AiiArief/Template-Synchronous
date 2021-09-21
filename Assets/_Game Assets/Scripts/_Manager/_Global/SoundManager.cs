using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    [HideInInspector]
    public AudioSource musicSource;

    public AudioMixer audioMixer;

    private void Awake()
    {
        musicSource = GetComponent<AudioSource>();

    }

    public void ToggleMute(bool isOn)
    {
        audioMixer.SetFloat("vol", (isOn) ? -80.0f : 0.0f);
    }
}
