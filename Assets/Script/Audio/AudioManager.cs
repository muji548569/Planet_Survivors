using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    [Header("SettingData")]
    public AudioSaveData saveData;
    [Header("AudioConfigData")]
    [SerializeField] private AudioConfig audioConfig;

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

    public void PlayBGM(E_BGM bgm, bool loop = true)
    {
        AudioClip clip = null;
        switch (bgm)
        {
            case E_BGM.Menu:
                clip = audioConfig.menuBGM;
                break;
            case E_BGM.Game:
                clip = audioConfig.gameBGM;
                break;
            case E_BGM.Victory:
                clip = audioConfig.victoryBGM;
                break;
            case E_BGM.Lose:
                clip = audioConfig.loseBGM;
                break;
        }

        if (clip == null)
        {
            Debug.LogError($"[AudioManager] 找不到 {bgm.ToString()} 的音頻文件");
            return;
        }
        bgmSource.clip = clip;
        bgmSource.loop = loop;
        ApplyBgmSettings();
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void PlaySFX(E_SFX sfx)
    {
        AudioClip clip = null;
        switch (sfx)
        {
            case E_SFX.LevelStart:
                clip = audioConfig.levelStartSFX;
                break;
            case E_SFX.PlayerHurt:
                clip = audioConfig.playerHurtSFX;
                break;
            case E_SFX.PlayerDie:
                clip = audioConfig.playerDieSFX;
                break;
            case E_SFX.Sword:
                clip = audioConfig.swordSFX;
                break;
            case E_SFX.Fireball:
                clip = audioConfig.fireballSFX;
                break;
            case E_SFX.LevelUp:
                clip = audioConfig.levelUpSFX;
                break;
        }

        if (clip == null) return;
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

public enum E_BGM
{
    Menu,
    Game,
    Victory,
    Lose,
}

public enum E_SFX
{
    LevelStart,
    PlayerHurt,
    PlayerDie,
    EnemyHurt,
    Sword,
    Fireball,
    LevelUp,
}
