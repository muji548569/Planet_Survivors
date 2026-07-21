using UnityEngine;

[System.Serializable]
public class AudioSaveData
{
    public bool sfxOn = true;
    public bool bgmOn = true;
    [Range(0, 1)] public float bgmVolume = 1f;
    [Range(0, 1)] public float sfxVolume = 1f;

    public void Validate()
    {
        bgmVolume = Mathf.Clamp01(bgmVolume);
        sfxVolume = Mathf.Clamp01(sfxVolume);
    }
}
