using UnityEngine;

public class ChargeEnemy : MonoBehaviour, IPoolable, IEnemyInitializable, IEnemyDeathHandler
{
    [SerializeField] private ChargeEnemyData data;  

    private Transform planet;
    private Transform target;
    private PoolableObject poolable;

    private Vector3 rotationAxis;       // 敵人繞著星球移動時使用的旋轉軸
    private float surfaceRadius;        // 敵人距離星球中心的距離
    private float angularSpeed;         // 角速度
    private float remainLifetime;       // 敵人剩餘存活時間

    private bool isCharge;              // 敵人目前是否正在執行衝刺行為
    private bool hasLockedDirection;    // 敵人是否已成功鎖定衝刺方向

    private bool isActive;              // 是否已死亡

    private void Awake()
    {
        poolable = GetComponent<PoolableObject>();
    }

    public void Init(Transform planet, Transform target)
    {

        this.planet = planet;
        this.target = target;

        LookTargetRotation();

        isActive = true;

    }

    /// <summary>
    /// 計算衝刺路徑
    /// </summary>
    private void LookTargetRotation()
    {
        if(target == null || planet == null)
        {
            Debug.LogError($"{name} 無法鎖定目標：target 或 planet 為空");
            return;
        }

        // 取得自身的球面法線
        Vector3 spawnNormal = (transform.position - planet.position).normalized;
        // 取得目標的球面法線
        Vector3 targetNormal = (target.position - planet.position).normalized;

        // 計算旋轉軸
        // 用 叉積Cross 計算出同時垂直於 自身法線 跟 目標法線 形成的平面 的 向量
        // 形成的平面穿過球心 因此對應球面上的一條大圓
        // 所以 rotationAxis 就是自身沿大圓前進時的旋轉軸
        rotationAxis = Vector3.Cross(spawnNormal, targetNormal).normalized;

        // 處理叉積接近零
        if (rotationAxis.sqrMagnitude < 0.0001f)
        {
            // 可能是敵人與玩家方向完全相同 或剛好位於球體正對面
            // 這兩種情況叉積都可能接近零 因此要準備備用切線軸
            rotationAxis = GetFallbackRotationAxis(spawnNormal);
        }

        // 計算表面半徑
        surfaceRadius = Vector3.Distance(transform.position, planet.position);

        // 線速度轉角速度
        // 星球越大 同樣的線速度所對應的角速度越小
        angularSpeed = data.moveSpeed / surfaceRadius * Mathf.Rad2Deg;

        // 初始化狀態
        remainLifetime = data.remainLifetime;
        hasLockedDirection = true;
        isCharge = true;

        // 立即校正朝向
        UpdateRotation(spawnNormal);
    }

    void Update()
    {
        if (!isCharge || !hasLockedDirection || !isActive) return;
        if (planet == null) return;

        if (remainLifetime <= 0)
        {
            poolable.Release();
            return;
        }

        MoveAlongLockedDirection();

        remainLifetime -= Time.deltaTime;
    }

    /// <summary>
    /// 負責讓敵人每幀沿球面前進
    /// </summary>
    private void MoveAlongLockedDirection()
    {
        // 取得敵人目前位於星球哪個方向
        // 因為每幀都會移動 所以要重新計算目前的表面法線
        Vector3 currentNormal = (transform.position - planet.position).normalized;

        // 計算這一幀的旋轉角度
        float angle = angularSpeed * Time.deltaTime;

        // 旋轉目前法線 得到下一幀的表面法線
        Vector3 nextNormal = Quaternion.AngleAxis(angle, rotationAxis) * currentNormal;

        // 根據下一個法線更新位置
        transform.position = planet.position + nextNormal * surfaceRadius;

        // 更新朝向
        UpdateRotation(nextNormal);
    }

    /// <summary>
    /// 負責調整敵人的朝向
    /// </summary>
    /// <param name="surfaceNormal"></param>
    private void UpdateRotation(Vector3 surfaceNormal)
    {
        // 計算移動切線方向
        // rotationAxis：繞哪一根軸旋轉
        // surfaceNormal：自身所在位置的表面法線
        // 兩者叉積會得到球面的切線方向 也就是前進方向
        Vector3 moveDirection = Vector3.Cross(rotationAxis, surfaceNormal).normalized;

        // Debug 畫線
        // 實際鎖定的初始移動方向
        //Debug.DrawRay(transform.position, moveDirection * 10f, Color.red, 5f);
        // 到玩家的空間直線
        //Debug.DrawLine(transform.position, target.position, Color.green, 5f);

        if (moveDirection.sqrMagnitude < 0.0001f)
        {
            return;
        }

        // 應用旋轉
        transform.rotation = Quaternion.LookRotation(moveDirection, surfaceNormal);
    }

    /// <summary>
    /// 生成一個合法的備用旋轉軸
    /// </summary>
    /// <param name="normal"></param>
    /// <returns></returns>
    private Vector3 GetFallbackRotationAxis(Vector3 normal)
    {
        // 參考方向
        // 首先檢查 normal 是否與世界 up 太接近平行
        Vector3 referenceAxis = Mathf.Abs(Vector3.Dot(normal, Vector3.up)) > 0.9f
                ? Vector3.right
                : Vector3.up;

        // 用參考軸和表面法線做叉積 取得一條垂直於 normal 的切線
        Vector3 tangent = Vector3.Cross(referenceAxis, normal).normalized;

        // 再用 normal × tangent 得到另一條同樣垂直於 normal 的方向
        // 這個方向可以作為合法的旋轉軸
        return Vector3.Cross(normal, tangent).normalized;
    }

    public void OnSpawnFromPool()
    {
        // 此時 EnemySpawner 可能還沒呼叫 Init 所以先保持未啟動
        isCharge = false;
        hasLockedDirection = false;
        isActive = false;
        remainLifetime = 0f;
    }

    public void OnReturnToPool()
    {
        isCharge = false;
        hasLockedDirection = false;
        isActive = false;

        target = null;
        planet = null;

        rotationAxis = Vector3.zero;
        remainLifetime = 0f;
    }

    public void OnEnemyDeath()
    {
        isActive = false;
    }
}
