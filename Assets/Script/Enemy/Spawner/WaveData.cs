using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "Enemy/WaveData")]
public class WaveData : ScriptableObject
{
    public string waveName;

    public GameObject enemyPrefab;
    public E_WaveType waveType;

    public float startTime;         // 啟動時間
    public float spawnInterval;     // 單波次內生成間隔
    public int spawnCount;          // 單波次生成數量

    public bool repeat;             // 是否重複
    public float repeatInterval;    // 重複間隔
}

public enum E_WaveType
{
    Random,
    Circle,
} 
