using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundData
{
    public bool sfxOn = true;
    public bool bgmOn = true;
    [Range(0,1)] public float bgmVolume = 1f;
    [Range(0,1)] public float sfxVolume = 1f;
}
