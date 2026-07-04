using UnityEngine;

public class Fireball : MonoBehaviour
{
    private float damage;
    private Transform owner;
    private float lifetime;

    private float moveSpeed;
    private Vector3 moveDir;
    private Transform planet;

    public void Init(float damage, Transform owner, float lifetime, float moveSpeed, Transform planet, float searchRadius)
    {
        this.damage = damage;
        this.owner = owner;
        this.lifetime = lifetime;
        this.moveSpeed = moveSpeed;
        this.planet = planet;

        moveDir = FindTargetDirection(searchRadius);

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

    /// <summary>
    /// 計算初始目標方向
    /// </summary>
    /// <param name="searchRadius"></param>
    /// <returns></returns>
    private Vector3 FindTargetDirection(float searchRadius)
    {
        // 得到鎖敵範圍內的所有敵人對象
        Collider[] hits = Physics.OverlapSphere(owner.position, searchRadius, 1 << LayerMask.NameToLayer("Enemy"));

        Transform nearestEnemy = null;
        float nearestDistance = Mathf.Infinity;

        // 遍歷鎖敵範圍內的所有碰撞器
        foreach (Collider hit in hits)
        {
            // 計算距離
            float distance = Vector3.Distance(transform.position, hit.transform.position);
            // 更新最近單位
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = hit.transform;
            }
        }

        // 得到最近敵人的方向 
        Vector3 rawDir = nearestEnemy == null? owner.forward : nearestEnemy.position - transform.position;
        // 計算星球表面法向
        Vector3 surfaceNormal = (transform.position - planet.position).normalized;
        // 把子彈移動方向投影到星球切平面，避免子彈往星球內部或外部飛
        Vector3 tangentDir = Vector3.ProjectOnPlane(rawDir, surfaceNormal).normalized;

        return tangentDir;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // 敵人受傷邏輯
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                DamageResult result = DamageCalculator.CalculatePlayerDamage(damage);
                enemy.TakeDamage(result.finalDamage);
                print($"火球武器是否爆擊: {result.isCritical}，造成: {result.finalDamage}點傷害");

                if (result.isCritical)
                {
                    // TODO: 爆擊特效
                }
            }
            Destroy(gameObject);
        }
    }
}
