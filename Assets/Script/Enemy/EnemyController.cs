using UnityEngine;

public class EnemyController : MonoBehaviour, IPoolable
{
    [SerializeField] private EnemyData data;
    private Transform planet;
    private Transform target;
    private float surfaceRadius;

    void Update()
    {
        if (target == null || planet == null) return;
        
        Vector3 targetDir = target.position - transform.position;
        Vector3 normal = (transform.position - planet.position).normalized;
        Vector3 moveDir = Vector3.ProjectOnPlane(targetDir, normal);
        if (moveDir.sqrMagnitude < 0.0001f)
        {
            return;
        }
        moveDir.Normalize();
        
        // 先沿目前位置的切線移動
        Vector3 nextPosition = transform.position + moveDir * data.moveSpeed * Time.deltaTime;
        // 重新計算新位置的星球法線
        Vector3 nextNormal = (nextPosition - planet.position).normalized;
        // 將敵人重新貼回固定半徑
        transform.position = planet.position + nextNormal * surfaceRadius;
        // 使用新位置重新計算面向方向
        Vector3 nextMoveDir = Vector3.ProjectOnPlane(target.position - transform.position, nextNormal);

        if (nextMoveDir.sqrMagnitude < 0.0001f) return;

        Quaternion targetRot = Quaternion.LookRotation(nextMoveDir.normalized, nextNormal);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 25 * Time.deltaTime);
    }

    public void Init(Transform planet, Transform target)
    {
        this.planet = planet;
        this.target = target;

        // 保存生成時角色中心與星球中心的距離
        surfaceRadius = Vector3.Distance(
            transform.position,
            planet.position);
    }

    public void OnSpawnFromPool()
    {
        // 重設敵人每次生成時需要恢復的狀態
        // 例如血量、受傷狀態、動畫、攻擊冷卻等
    }

    public void OnReturnToPool()
    {
        planet = null;
        target = null;

        // 不需要清除 rigidBody 的 linearVelocity 跟 angularVelocity
        // 因為此 Rigidbody 是 Kinematic。
    }
}
