using UnityEngine;

public class EnemyContact : MonoBehaviour, IEnemyDeathHandler, IPoolable
{
    [SerializeField] private EnemyData data;
    [SerializeField] private Collider contactColider;
    private float timer;
    private bool canAttack;

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        if (timer > 0) return;
        if (!canAttack) return;

        PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();
        if (player != null)
        {
            DamageResult damage = DamageCalculator.CalculatePlayerReceiveDamage(data.contactDamage);
            
            if(damage.isDodged)
            {
                // TODO:閃避特效
                print($"敵人 {data.EnemyName} 的攻擊被閃避");
            }
            else 
            {
                player.TakeDamage(damage.finalDamage);
                print($"敵人 {data.EnemyName} 造成傷害: {damage.finalDamage}");
                // TODO:玩家受擊特效
            }
        }

        timer = data.attackCooldown;
    }

    public void OnEnemyDeath()
    {
        canAttack = false;
        contactColider.enabled = false;
    }

    public void OnSpawnFromPool()
    {
        canAttack = true;
        contactColider.enabled = true;

        timer = 0;
    }

    public void OnReturnToPool()
    {
        canAttack = false;
        contactColider.enabled = false;
    }
}
