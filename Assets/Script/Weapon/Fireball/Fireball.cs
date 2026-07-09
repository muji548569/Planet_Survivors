using UnityEngine;

public class Fireball : MonoBehaviour
{
    private float damage;
    private Transform owner;
    private float lifetime;
    
    private float moveSpeed;
    private Vector3 moveDir;
    private Transform planet;
    private bool pierce;

    public void Init(float damage, 
                     Transform owner, 
                     float lifetime, 
                     float moveSpeed, 
                     Transform planet,
                     Vector3 fireDir,
                     bool pierce)
    {
        this.damage = damage;
        this.owner = owner;
        this.lifetime = lifetime;
        this.moveSpeed = moveSpeed;
        this.planet = planet;
        this.pierce = pierce;

        moveDir = fireDir;
        
        Destroy(this.gameObject, this.lifetime);
    }
    void Update()
    {
        // 每幀重新修正方向 讓子彈切星球平面位移
        Vector3 surfaceNormal = (transform.position - planet.position).normalized;
        moveDir = Vector3.ProjectOnPlane(moveDir, surfaceNormal).normalized;

        // 每幀更新計算出的位置跟旋轉
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        if(moveDir!=Vector3.zero) 
            transform.rotation = Quaternion.LookRotation(moveDir, transform.up);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // 敵人受傷邏輯
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                DamageResult result = DamageCalculator.CalculatePlayerAttackDamage(damage);
                enemy.TakeDamage(result.finalDamage);
                print($"火球武器是否爆擊: {result.isCritical}，造成: {result.finalDamage}點傷害");

                if (result.isCritical)
                {
                    // TODO: 爆擊特效
                }
            }

            if(!pierce)
                Destroy(gameObject);
        }
    }
}
