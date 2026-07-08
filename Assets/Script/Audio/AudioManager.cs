using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    [Header("SettingData")]
    public SoundData soundData = new SoundData();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if(clip == null) return;

        if(bgmSource.clip == clip && bgmSource.isPlaying) return;

        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.volume = soundData.bgmVolume;
        bgmSource.mute = !soundData.bgmOn;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        if(clip == null) return;
        sfxSource.volume = soundData.sfxVolume;
        sfxSource.mute = !soundData.sfxOn;
        sfxSource.PlayOneShot(clip);
    }

    public void SetBGMOn(bool isOn)
    {
        soundData.bgmOn = isOn;
        bgmSource.mute = !isOn;
    }

    public void SetSFXOn(bool isOn)
    {
        soundData.sfxOn = isOn;
        sfxSource.mute = !isOn;
    }

    public void SetBGMVolume(float volume)
    {
        soundData.bgmVolume = Mathf.Clamp01(volume);
        bgmSource.volume = soundData.bgmVolume;
    }

    public void SetSFXVolume(float volume)
    {
        soundData.sfxVolume = Mathf.Clamp01(volume);
        sfxSource.volume = soundData.bgmVolume;
    }
}
