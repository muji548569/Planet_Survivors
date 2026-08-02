using UnityEngine;

public class TurretEnemy : MonoBehaviour, IPoolable, IEnemyInitializable, IEnemyDeathHandler
{
    [SerializeField] private TurretEnemyData data;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float rotateSpeed;
    private Transform planet;
    private Transform target;
    private float attackTimer;
    private bool isActive;

    private Animator animator;
    private static readonly int AttackTriggerHash = Animator.StringToHash("Attack");

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void Init(Transform planet, Transform target)
    {
        this.planet = planet;
        this.target = target;

        isActive = true;
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

        LookToTarget();
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

        if(animator != null)
        {
            animator.SetTrigger(AttackTriggerHash);
        }
    }

    private void LookToTarget()
    {
        if (planet == null || target == null) return;
        // 敵人所在位置的球面法線
        Vector3 surfaceNormal = (transform.position - planet.position).normalized;
        // 敵人指向玩家
        Vector3 directionToTarget = target.position - transform.position;
        // 投影到當前球面的切平面
        Vector3 lookDirection = Vector3.ProjectOnPlane(directionToTarget, surfaceNormal);
        if(lookDirection.sqrMagnitude < 0.0001f)
        {
            return ;
        }
        lookDirection.Normalize();

        // Y 軸維持沿著球面法線，只旋轉朝向
        transform.rotation = Quaternion.LookRotation(lookDirection, surfaceNormal);
    }

    public void OnSpawnFromPool()
    {
        attackTimer = data.attackCooldown;
        isActive = false;
    }

    public void OnReturnToPool()
    {
        isActive = false;
        planet = null;
        target = null;
        attackTimer = 0f;
    }

    public void OnEnemyDeath()
    {
        isActive = false;
    }
}
