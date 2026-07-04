using UnityEngine;

public class SwordSlash : MonoBehaviour
{
    private float damage;
    private Transform owner;
    private float lifetime;

    /// <summary>
    /// 提供給外部初始化數值
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="owner"></param>
    /// <param name="lifetime"></param>
    public void Init(float damage, Transform owner, float lifetime)
    {
        this.damage = damage;
        this.owner = owner;
        this.lifetime = lifetime;

        Destroy(gameObject, this.lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // 敵人受傷邏輯
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                // 計算傷害
                DamageResult result = DamageCalculator.CalculatePlayerAttackDamage(damage);
                enemy.TakeDamage(result.finalDamage);
                print($"刀劍武器是否爆擊: {result.isCritical}，造成: {result.finalDamage}點傷害");

                // 如果該傷害觸發爆擊
                if (result.isCritical)
                {
                    // TODO: 爆擊特效
                }
            }

            
        }
    }
}
