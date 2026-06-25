using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform planet;
    [Header("Spawn Setting")]
    [SerializeField] private float planetRadius = 50f;
    public void Spawn(SpawnEvent spawnEvent, int index)
    {
        if(player == null || planet == null) return;
        if(spawnEvent.enemyPrefab == null) return;

        switch (spawnEvent.waveType)
        {
            case E_WaveType.RandomBackside:
                SpawnRandomBackside(spawnEvent.enemyPrefab, spawnEvent.spawnSpread);
                break;
            case E_WaveType.Circle:
                SpawnCircle(spawnEvent.enemyPrefab, spawnEvent.spawnCount, index, spawnEvent.spawnSpread);
                break;
        }
    }

    /// <summary>
    /// 在相對玩家的星球背面附近生成敵人
    /// </summary>
    /// <param name="enemyPrefab"></param>
    private void SpawnRandomBackside(GameObject enemyPrefab, float spawnSpread)
    {
        // 玩家方向
        Vector3 playerDir = (player.position - planet.position).normalized;
        // 星球背面方向
        Vector3 backDir = -playerDir;
        // 給背面方向添加一些隨機量
        Vector3 spawnDir = (backDir + Random.insideUnitSphere *  spawnSpread).normalized;

        // 將方向轉換成星球表面座標
        Vector3 spawnPos = planet.position + spawnDir * planetRadius;
        // 將敵人up方向朝向星球外側
        Quaternion spawmRot = Quaternion.FromToRotation(Vector3.up, spawnDir);
        
        // 生成敵人
        GameObject obj = Instantiate(enemyPrefab, spawnPos, spawmRot);
        EnemyController enemy = obj.GetComponent<EnemyController>();
        enemy.Init(planet, player);
    }

    /// <summary>
    /// 在相對玩家的星球背面生成一個圓環波次敵人
    /// </summary>
    /// <param name="enemyPrefab"></param>
    /// <param name="count"></param>
    /// <param name="index"></param>
    private void SpawnCircle(GameObject enemyPrefab, int count, int index, float spawnSpread)
    {
        // 玩家方向
        Vector3 playerDir = (player.position - planet.position).normalized;

        // 圓環中心方向 (相對玩家的星球背面)
        Vector3 centerDir = -playerDir;

        // 求以中心方向為法向的平面在星球上的切面
        // 求第一條切線
        Vector3 axisA = Vector3.Cross(centerDir, Vector3.up);

        // 避免與 Up 平行造成 Cross 結果為零
        if(axisA == Vector3.zero)
        {
            axisA = Vector3.Cross(centerDir, Vector3.right);
        }

        // 求第二條切線
        Vector3 axisB = Vector3.Cross(centerDir, axisA);

        // 計算怪物在圓環上的角度
        float angle = 360 / count * index;

        // 計算圓環上的方向
        Vector3 circleDir = centerDir 
                            + axisA * Mathf.Cos(angle * Mathf.Deg2Rad) * spawnSpread
                            + axisB * Mathf.Sin(angle * Mathf.Deg2Rad) * spawnSpread;

        circleDir.Normalize();

        // 投影到星球表面
        Vector3 spawnPos = planet.position + circleDir * planetRadius;
        // 將敵人up方向朝向星球外側
        Quaternion spawmRot = Quaternion.FromToRotation(Vector3.up, circleDir);

        GameObject obj = Instantiate(enemyPrefab, spawnPos, spawmRot);
        EnemyController enemy = obj.GetComponent<EnemyController>();
        enemy.Init(planet, player);
    }
}
