using UnityEngine;

public class TurretEnemy : MonoBehaviour, IPoolable, IEnemyInitializable
{
    [SerializeField] private TurretEnemyData data;
    [SerializeField] private Transform firePoint;
    private Transform planet;
    private Transform target;
    private float attackTimer;
    private bool isActive;

    public void Init(Transform planet, Transform target)
    {
        this.planet = planet;
        this.target = target;
    }

    void Update()
    {
        if (!isActive) return;
        if (planet == null || target == null) return;
        
        attackTimer -= Time.deltaTime;
        if(attackTimer <= 0)
        {
            Attack();
            attackTimer = data.attackCooldown;
        }
    }

    private void Attack()
    {
        // 球面法線
        Vector3 surfaceNormal = (transform.position - planet.position).normalized;
        // 目標方向
        Vector3 directionToTarget = target.position - firePoint.position;
        // 只保留沿球面切線的方向
        Vector3 fireDirection = Vector3.ProjectOnPlane(directionToTarget, surfaceNormal);

        if(fireDirection.sqrMagnitude < 0.0001f)
        {
            return;
        }

        fireDirection.Normalize();

        // 從物件池中取出子彈
        PoolableObject poolable = PoolManager.Instance.Get(data.projectilePrefab,
            firePoint.position,
            Quaternion.LookRotation(fireDirection, surfaceNormal));

        TurretProjectile projectile = poolable.GetComponent<TurretProjectile>();
        if(projectile == null)
        {
            Debug.LogError($"{projectile.name} 缺少 TurretProjectile");
            poolable.Release();
            return;
        }

        projectile.Init(planet, fireDirection, data.projectileSpeed, data.projectileDamage, data.projectileLifetime);
    }

    public void OnSpawnFromPool()
    {
        attackTimer = data.attackCooldown;
        isActive = true;
    }

    public void OnReturnToPool()
    {
        isActive = false;
        planet = null;
        target = null;
        attackTimer = 0f;
    }
}
