using UnityEngine;

public class OrbitBullet : MonoBehaviour
{
    private float damage;
    private Transform owner;
    public void Init(float damage, Transform owner)
    {
        this.damage = damage;
        this.owner = owner;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if(enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            
        }
    }
}
