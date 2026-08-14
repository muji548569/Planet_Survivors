using UnityEngine;

public class EnemyHealth : MonoBehaviour, IPoolable
{
    [SerializeField] private EnemyData data;
    private float currentHealth;
    private bool isDead;

    private PoolableObject poolable;
    private IEnemyDeathHandler[] deathHandlers;
    private IKnockbackable[] knockbackables;
    
    // animator
    private Animator animator;
    private static readonly int IsDeadHash = Animator.StringToHash("IsDead");

    private void Awake()
    {
        currentHealth = data.maxHealth;
        poolable = GetComponent<PoolableObject>();
        animator = GetComponentInChildren<Animator>();

        deathHandlers = GetComponents<IEnemyDeathHandler>();
        knockbackables = GetComponents<IKnockbackable>();
    }

    public void TakeDamage(float damage, Transform attacker)
    {
        if (isDead) return;
        if (damage <= 0) return;

        // 減去生命值
        currentHealth -= damage;

        // 計算擊退力度
        float KnockbackForce = 1 - Mathf.Clamp01(data.knockbackResistance);
        // 調用身上實作的擊退介面
        foreach(IKnockbackable knockback in knockbackables)
        {
            knockback.ApplyKnockback(attacker.position, KnockbackForce);
        }

        // 生命 <= 0 調用死亡函數
        if(currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die() 
    {
        if(isDead) return;

        isDead = true;

        SpawnDrop();

        foreach (var handler in deathHandlers)
        {
            handler.OnEnemyDeath();
        }

        if (animator != null)
        {
            animator.SetBool(IsDeadHash, true);
        }
        else
        {
            ReleaseAfterDeathAnimation();
        }
    }

    // 由 Death Animation 最後一幀的 Animation Event 呼叫
    public void ReleaseAfterDeathAnimation()
    {
        poolable.Release();
    }

    private void SpawnDrop()
    {
        EnemyDropper dropper = GetComponent<EnemyDropper>();
        if (dropper != null)
        {
            dropper.Drop();
        }
    }

    public void OnSpawnFromPool()
    {
        currentHealth = data.maxHealth;
        isDead = false;
        if (animator != null)
        {
            animator.SetBool(IsDeadHash, false);
        }
    }

    public void OnReturnToPool()
    {
        currentHealth = 0f;
        isDead = true;
    }
}
