using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "Enemy/WaveData")]
public class WaveData : ScriptableObject
{
    public string waveName;

    public float startTime;         // 啟動時間

    public bool repeat;             // 是否重複
    public float repeatInterval;    // 重複間隔

    public SpawnEvent[] spawnEvents;
}
