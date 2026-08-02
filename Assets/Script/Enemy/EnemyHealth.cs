using UnityEngine;

public class EnemyHealth : MonoBehaviour, IPoolable
{
    [SerializeField] private EnemyData data;
    private float currentHealth;
    private bool isDead;

    private PoolableObject poolable;
    private IEnemyDeathHandler[] deathHandlers;
    
    // animator
    private Animator animator;
    private static readonly int IsDeadHash = Animator.StringToHash("IsDead");

    private void Awake()
    {
        currentHealth = data.maxHealth;
        poolable = GetComponent<PoolableObject>();
        animator = GetComponentInChildren<Animator>();

        deathHandlers = GetComponents<IEnemyDeathHandler>();
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        if (damage <= 0) return;

        currentHealth -= damage;
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
