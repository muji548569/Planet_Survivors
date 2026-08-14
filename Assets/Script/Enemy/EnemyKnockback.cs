using UnityEngine;

public class EnemyKnockback : MonoBehaviour, IPoolable, IKnockbackable
{
    [SerializeField] private EnemyData data;

    private Transform planet;
    private float surfaceRadius;

    private Vector3 knockbackAxis;
    private float currentKnockbackSpeed;

    public bool IsKnockbacking => currentKnockbackSpeed > 0f;

    public void Init(Transform planet)
    {
        this.planet = planet;

        surfaceRadius = Vector3.Distance(transform.position, planet.position);
    }

    public void ApplyKnockback(Vector3 sourcePosition, float knockbackMultiplier)
    {
        if (planet == null) return;

        Vector3 surfaceNormal = (transform.position - planet.position).normalized;

        // 計算擊退方向
        Vector3 knockbackDir = Vector3.ProjectOnPlane(transform.position - sourcePosition, surfaceNormal);
        if (knockbackDir.sqrMagnitude < 0.0001f) return;
        knockbackDir.Normalize();

        // 找到旋轉軸
        knockbackAxis = Vector3.Cross(surfaceNormal, knockbackDir).normalized;

        // 計算擊退速度
        currentKnockbackSpeed = data.knockbackSpeed * knockbackMultiplier;
    }

    public void Tick()
    {
        if(!IsKnockbacking) return;

        // 線速度 v = 角速度 ω × 半徑 r
        // 所以 角速度 ω = 線速度 v / 半徑 r
        float angularSpeed = currentKnockbackSpeed / surfaceRadius;

        // 計算每一幀應該移動的角度 (弧度轉角度)
        float angle = angularSpeed * Mathf.Rad2Deg * Time.deltaTime;

        // 根據旋轉軸旋轉
        // 以哪個位置 根據哪個軸 旋轉多少度
        transform.RotateAround(planet.position, knockbackAxis, angle);

        // 計算現在擊退速度
        currentKnockbackSpeed =
            Mathf.MoveTowards(currentKnockbackSpeed,
            0,
            data.knockbackDeceleration * Time.deltaTime);
    }

    public void OnReturnToPool()
    {
        planet = null;
        currentKnockbackSpeed = 0;
        knockbackAxis = Vector3.zero;
    }

    public void OnSpawnFromPool()
    {
        currentKnockbackSpeed = 0;
        knockbackAxis = Vector3.zero;
    }
}
