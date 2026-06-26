using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private WaveData[] waveDatas;
    [SerializeField] private EnemySpawner enemySpawner;

    private readonly List<WaveRuntime> waves = new();
    private float elapsedTime;  // 經過時間

    void Start()
    {
        // 遍歷所有WaveData
        foreach (WaveData data in waveDatas)
        {
            // 加到WaveRuntime等待使用
            waves.Add(new WaveRuntime(data));
        }
    }

    void Update()
    {
        if (!GameSessionManager.Instance.IsPlaying) return; 
        // 遍歷WaveRuntime數組
        foreach (WaveRuntime wave in waves)
        {
            if (elapsedTime >= wave.nextTriggerTime)
            {
                // 啟動波次
                StartCoroutine(RunWave(wave.data));

                if (wave.data.repeat)
                {
                    // 如果是重複波次 下次觸發時間為
                    wave.nextTriggerTime += wave.data.repeatInterval;
                }
                else
                {
                    // 如果不需要重複啟動的波次
                    // 直接把下次執行時間拉到無限
                    wave.nextTriggerTime = float.MaxValue;
                }
            }
        }
    }

    private void OnEnable()
    {
        GameSessionManager.Instance.OnTimeChanged += getTime;
    }

    private void OnDisable()
    {
        GameSessionManager.Instance.OnTimeChanged -= getTime;
    }

    /// <summary>
    /// 啟動一個波次(wavedata)
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private IEnumerator RunWave(WaveData data)
    {
        // 遍歷WaveData中的WaveEvent數組
        foreach (SpawnEvent spawnEvent in data.spawnEvents)
        {
            // 啟動所有SpawnEvent
            StartCoroutine(RunSpawnEvent(spawnEvent));
        }
        yield return null;
    }
    /// <summary>
    /// 啟動波次中的一個生成事件
    /// </summary>
    /// <param name="spawnEvent"></param>
    /// <returns></returns>
    private IEnumerator RunSpawnEvent(SpawnEvent spawnEvent)
    {
        // 根據設定的時間 延遲執行
        yield return new WaitForSeconds(spawnEvent.delay);

        for(int i = 0; i < spawnEvent.spawnCount; ++i)
        {
            // 呼叫敵人生成器
            enemySpawner.Spawn(spawnEvent, i);
            // 等待生成間隔
            yield return new WaitForSeconds(spawnEvent.spawnInterval);
        }
    }

    private void getTime(float time)
    {
        elapsedTime = time;
    }
}
