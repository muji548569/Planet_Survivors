using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private EnemyData data;
    private float currentHealth;

    private void Awake()
    {
        currentHealth = data.maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die() 
    {
        EnemyDropper dropper = GetComponent<EnemyDropper>();
        if(dropper != null)
        {
            dropper.Drop();
        }

        Destroy(gameObject);
    }
}
