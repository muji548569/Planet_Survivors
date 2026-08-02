using UnityEngine;

public class Fireball : MonoBehaviour,IPoolable
{
    private PoolableObject poolable;

    private float damage;
    private float remainDuration;
    private float moveSpeed;
    private Vector3 moveDir;
    private bool pierce;

    private Transform owner;
    private Transform planet;
    private float surfaceRadius;
    private bool hasHit;

    private void Awake()
    {
        poolable = GetComponent<PoolableObject>();
    }

    public void Init(float damage, 
                     Transform owner, 
                     float duration, 
                     float moveSpeed, 
                     Transform planet,
                     Vector3 moveDir,
                     bool pierce)
    {
        this.damage = damage;
        this.owner = owner;
        this.remainDuration = duration;
        this.moveSpeed = moveSpeed;
        this.planet = planet;
        this.moveDir = moveDir;
        this.pierce = pierce;

        // 保存生成時角色中心與星球中心的距離
        surfaceRadius = Vector3.Distance(transform.position, planet.position);
    }

    void Update()
    {
        if(planet == null || owner == null)
        {
            poolable.Release();
            return;
        }

        remainDuration -= Time.deltaTime;

        if(remainDuration <= 0 )
        {
            poolable.Release();
            return;
        }

        // 每幀重新修正方向 讓子彈切星球平面位移
        Vector3 surfaceNormal = (transform.position - planet.position).normalized;
        // 根據跟星球的法向量 得出切線方向(也就是前進方向)
        moveDir = Vector3.ProjectOnPlane(moveDir, surfaceNormal).normalized;
        // 計算下一幀的位置
        Vector3 nextPosition = transform.position + moveDir * moveSpeed * Time.deltaTime;
        // 計算下一幀跟星球的法向量
        Vector3 nextNormal = (nextPosition - planet.position).normalized;
        // 應用新的位置 球面上高度用 nextNormal * surfaceRadius 綁定在初始化時的高度
        transform.position = planet.position + nextNormal * surfaceRadius;
        // 根據下一幀的法向量 計算切線方向
        moveDir = Vector3.ProjectOnPlane(moveDir, nextNormal).normalized;
        // 應用新的移動方向
        if(moveDir.sqrMagnitude > 0.0001f) 
            transform.rotation = Quaternion.LookRotation(moveDir, nextNormal);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        if(!other.CompareTag("Enemy")) return;

        // 敵人受傷邏輯
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (enemy == null) return;

        DamageResult result = DamageCalculator.CalculatePlayerAttackDamage(damage);
        enemy.TakeDamage(result.finalDamage);
        print($"火球武器是否爆擊: {result.isCritical}，對 {other.name} 造成: {result.finalDamage}點傷害");

        if (result.isCritical)
        {
            // TODO: 爆擊特效
        }

        if (!pierce)
        {
            hasHit = true;
            poolable.Release();
        }
            
    }

    public void OnSpawnFromPool()
    {
        // Init 會在取出後重新填入資料 這裡就不需要填

        hasHit = false;
    }

    public void OnReturnToPool()
    {
        damage = 0f;
        remainDuration = 0f;
        moveSpeed = 0f;
        moveDir = Vector3.zero;
        pierce = false;
        owner = null;
        planet = null;
    }
}
