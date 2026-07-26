using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform planet;
    [Header("Spawn Setting")]
    [SerializeField] private float planetRadius = 50f;  // 星球半徑
    [SerializeField] private float enemySurfaceOffset = 0.5f;
    public void Spawn(SpawnEvent spawnEvent, int index)
    {
        if(player == null || planet == null) return;
        if(spawnEvent.enemyPrefab == null) return;

        switch (spawnEvent.waveType)
        {
            case E_WaveType.RandomBackside:
                SpawnRandomBackside(spawnEvent.enemyPrefab, spawnEvent.backsideConeAngle);
                break;
            case E_WaveType.Circle:
                SpawnCircle(spawnEvent.enemyPrefab, spawnEvent.spawnCount, index, spawnEvent.ringAngleFromPlayer);
                break;
        }
    }

    /// <summary>
    /// 在相對玩家的星球背面附近生成敵人
    /// </summary>
    /// <param name="enemyPrefab"></param>
    /// <param name="maxAngleFromBackside">相對背面的最大偏移角度 0°在正背面 20°會生成在背面周圍20°範圍內 90°會在背面半圓內生成 180°整個球面都可能生成</param>
    private void SpawnRandomBackside(GameObject enemyPrefab, float maxAngleFromBackside)
    {
        // 玩家方向
        Vector3 playerDir = (player.position - planet.position).normalized;
        // 星球背面方向
        Vector3 backDir = -playerDir;
        // 給背面方向添加一些隨機量
        Vector3 spawnDir = GetRandomDirectionInCone(backDir, maxAngleFromBackside);

        SpawnEnemy(enemyPrefab, spawnDir);
    }

    /// <summary>
    /// 在球面指定緯度生成一圈敵人
    /// </summary>
    /// <param name="enemyPrefab"></param>
    /// <param name="count">總生成數量</param>
    /// <param name="index">生成索引(第幾個)</param>
    /// <param name="ringAngleFromPlayer">圓環生成位置 0°在角色位置 90°在半圓處 180°在球體背面 </param>
    private void SpawnCircle(GameObject enemyPrefab, int count, int index, float ringAngleFromPlayer)
    {
        if (count <= 0) return;

        // 將玩家所在位置視為北極方向
        Vector3 playerDir = (player.position - planet.position).normalized;

        // 取得垂直於playerDir的兩條切線
        GetTangentAxes(playerDir, out Vector3 axisA, out Vector3 axisB);

        // 敵人在圓環上的水平角度
        float azimuth = Mathf.PI * 2f * index / count;

        // 相對玩家方向的球面角度
        float polarAngle = Mathf.Clamp(ringAngleFromPlayer, 0f, 180f) * Mathf.Deg2Rad;

        // 圓環上的切線方向
        Vector3 ringDir = axisA * Mathf.Cos(azimuth) + axisB * Mathf.Sin(azimuth);

        // 使用球面座標計算方向
        Vector3 spawnDir = playerDir * Mathf.Cos(polarAngle) + ringDir * Mathf.Sin(polarAngle);

        spawnDir.Normalize();

        SpawnEnemy(enemyPrefab, spawnDir);
    }

    /// <summary>
    /// 取得相對玩家星球背面的隨機偏移角度
    /// </summary>
    /// <param name="centerDir"></param>
    /// <param name="maxAngle"></param>
    /// <returns></returns>
    private Vector3 GetRandomDirectionInCone(Vector3 centerDir, float maxAngle) 
    {
        GetTangentAxes(centerDir, out Vector3 axisA, out Vector3 axisB);

        float maxAngleRad = Mathf.Clamp(maxAngle, 0f, 180f) * Mathf.Deg2Rad;

        // 在球面圓錐範圍內均勻取樣
        float cosTheta = Mathf.Lerp(1f, Mathf.Cos(maxAngleRad), Random.value);
        float sinTheta = Mathf.Sqrt(Mathf.Max(0, 1f - cosTheta*cosTheta));
        // 方位角
        float azimuth = Random.Range(0, Mathf.PI * 2f);
        Vector3 tangentDir = axisA * Mathf.Cos(azimuth) + axisB * Mathf.Sin(azimuth);

        return (centerDir * cosTheta + tangentDir * sinTheta).normalized;
    }

    /// <summary>
    /// 計算切線
    /// </summary>
    /// <param name="normal"></param>
    /// <param name="axisA"></param>
    /// <param name="axisB"></param>
    private void GetTangentAxes(Vector3 normal, out Vector3 axisA, out Vector3 axisB)
    {
        // 避免 normal 和 Vector3.up 平行
        Vector3 reference = Mathf.Abs(Vector3.Dot(normal, Vector3.up)) > 0.99f 
                            ? Vector3.right 
                            : Vector3.up;

        axisA = Vector3.Cross(normal, reference).normalized;
        axisB = Vector3.Cross(normal, axisA).normalized;
    }

    private void SpawnEnemy(GameObject enemyPrefab, Vector3 spawnDir)
    {
        spawnDir.Normalize();
        // 將敵人up方向朝向星球外側
        Quaternion spawnRot = Quaternion.FromToRotation(Vector3.up, spawnDir);

        Vector3 spawnPos = planet.position + spawnDir * (planetRadius + enemySurfaceOffset);

        PoolableObject poolable = PoolManager.Instance.Get(enemyPrefab, spawnPos, spawnRot);

        InitializeEnemy(poolable);
    }

    /// <summary>
    /// 利用 IEnemyInitializable 介面 統一初始化接口
    /// </summary>
    /// <param name="enemy"></param>
    private void InitializeEnemy(PoolableObject enemy)
    {
        IEnemyInitializable[] initializables = enemy.GetComponents<IEnemyInitializable>();

        foreach (var initializable in initializables)
        {
            initializable.Init(planet, player);
        }
    }
}
