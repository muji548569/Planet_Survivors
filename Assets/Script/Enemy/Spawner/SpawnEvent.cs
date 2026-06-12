using UnityEngine;

[System.Serializable]
public class SpawnEvent
{
    public string eventName;
    public GameObject enemyPrefab;
    public E_WaveType waveType;
    public float delay;             // 啟動延遲
    public float spawnInterval;     // 生成間隔
    public int spawnCount;          // 生成數量
    public int spawnSpread;         // 生成位置偏移
}


public enum E_WaveType
{
    RandomBackside,
    Circle,
}