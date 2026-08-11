using UnityEngine;

[CreateAssetMenu(fileName = "AudioConfig", menuName = "Audio/AudioConfig")]
public class AudioConfig : ScriptableObject
{
    [Header("BGM")]
    public AudioClip menuBGM;
    public AudioClip gameBGM;
    public AudioClip victoryBGM;
    public AudioClip loseBGM;

    [Header("SFX")]
    public AudioClip levelStartSFX;
    public AudioClip playerHurtSFX;
    public AudioClip playerDieSFX;
    public AudioClip swordSFX;
    public AudioClip fireballSFX;
    public AudioClip levelUpSFX;
}
