using UnityEngine;

public class EnemyHealth : MonoBehaviour, IPoolable
{
    [SerializeField] private EnemyData data;
    private float currentHealth;
    private bool isDead;
    private PoolableObject poolable;

    private void Awake()
    {
        currentHealth = data.maxHealth;
        poolable = GetComponent<PoolableObject>();
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
    }

    public void OnReturnToPool()
    {
        currentHealth = 0f;
        isDead = true;
    }
}
