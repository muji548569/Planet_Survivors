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
                DamageResult result = DamageCalculator.CalculatePlayerDamage(damage);
                enemy.TakeDamage(result.finalDamage);
                print($"環繞武器是否爆擊: {result.isCritical}，造成: {result.finalDamage}點傷害");

                if (result.isCritical)
                {
                    // TODO: 爆擊特效
                }
            }
            
        }
    }
}
