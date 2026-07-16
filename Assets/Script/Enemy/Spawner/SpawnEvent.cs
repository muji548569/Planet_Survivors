using UnityEngine;

[System.Serializable]
public class SpawnEvent
{
    public string eventName;
    public GameObject enemyPrefab;
    public E_WaveType waveType;
    public float delay;                 // 啟動延遲
    public float spawnInterval;         // 生成間隔
    public int spawnCount;              // 生成數量
    public float backsideConeAngle;     // 星球背面生成位置偏移 (E_WaveType.RandomBackside)  
    public float ringAngleFromPlayer;   // 圓環與玩家之間的球面角度 (E_WaveType.Circle)
}


public enum E_WaveType
{
    RandomBackside,
    Circle,
}