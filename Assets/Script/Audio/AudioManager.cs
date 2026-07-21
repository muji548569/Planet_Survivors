using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    [Header("SettingData")]
    public AudioSaveData saveData;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Init()
    {
        if(SaveManager.Instance == null)
        {
            Debug.LogError("[AudioManager] SaveManager 不存在。");
            return;
        }

        saveData = SaveManager.Instance.Data.audio;
        ApplySettings();
    }

    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if(clip == null) return;

        if(bgmSource.clip == clip && bgmSource.isPlaying) return;

        bgmSource.clip = clip;
        bgmSource.loop = loop;
        ApplyBgmSettings();
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        if(clip == null) return;
        ApplySfxSettings();
        sfxSource.PlayOneShot(clip);
    }

    public void SetBGMOn(bool isOn)
    {
        saveData.bgmOn = isOn;
        ApplyBgmSettings();
        SaveManager.Instance.Save();
    }

    public void SetSFXOn(bool isOn)
    {
        saveData.sfxOn = isOn;
        ApplySfxSettings();
        SaveManager.Instance.Save();
    }

    public void SetBGMVolume(float volume)
    {
        saveData.bgmVolume = Mathf.Clamp01(volume);
        ApplyBgmSettings();
    }

    public void SetSFXVolume(float volume)
    {
        saveData.sfxVolume = Mathf.Clamp01(volume);
        ApplySfxSettings();
    }

    public void SaveSettings()
    {
        SaveManager.Instance.Save();
    }

    private void ApplySettings()
    {
        ApplyBgmSettings();
        ApplySfxSettings();
    }

    private void ApplyBgmSettings()
    {
        bgmSource.mute = !saveData.bgmOn;
        bgmSource.volume = saveData.bgmVolume;
    }

    private void ApplySfxSettings()
    {
        sfxSource.mute = !saveData.sfxOn;
        sfxSource.volume = saveData.sfxVolume;
    }
}
