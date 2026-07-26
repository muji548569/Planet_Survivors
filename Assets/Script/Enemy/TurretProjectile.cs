using UnityEngine;

public class TurretProjectile : MonoBehaviour, IPoolable
{
    [SerializeField] private float surfaceOffset;

    private Transform planet;
    private PoolableObject poolable;

    private Vector3 rotationAxis;
    private float surfaceRadius;
    private float angularSpeed;
    private float damage;
    private float remainingLifetime;

    private bool isActive;

    private void Awake()
    {
        poolable = GetComponent<PoolableObject>();
        if (poolable == null)
        {
            Debug.LogError($"{name} 缺少 PoolableObject 元件");
        }
    }

    public void Init(Transform planet, Vector3 moveDirection, float moveSpeed, float damage, float lifetime)
    {
        this.planet = planet;
        this.damage = damage;
        remainingLifetime = lifetime;

        Vector3 surfaceNormal = (transform.position - planet.position).normalized;

        surfaceRadius = Vector3.Distance(transform.position, planet.position);
        surfaceRadius += surfaceOffset;
        // 第一幀先校正位置
        transform.position = planet.position + surfaceNormal * surfaceRadius;

        // rotationAxis 是這條球面路徑所繞的軸
        // surfaceNormal = 當前星球法線
        // moveDirection = 當前切線移動方向
        rotationAxis = Vector3.Cross(surfaceNormal, moveDirection).normalized;

        if(rotationAxis.sqrMagnitude < 0.0001f)
        {
            poolable.Release();
            return;
        }

        // 線速度 v = 角速度 ω × 半徑 r
        // Mathf.Rad2Deg 將弧度轉為角度 因為之後 Quaternion.AngleAxis() 要用的是角度
        angularSpeed = moveSpeed / surfaceRadius * Mathf.Rad2Deg;

        isActive = true;
    }

    void Update()
    {
        if (!isActive || planet == null) return;

        MoveAlongPlanet();

        remainingLifetime -= Time.deltaTime;

        if(remainingLifetime <= 0f)
        {
            poolable.Release();
        }
    }

    private void MoveAlongPlanet()
    {
        // 取得當前法線
        Vector3 currentNormal = (transform.position - planet.position).normalized;

        // 計算這一幀應旋轉的角度
        float angle = angularSpeed * Time.deltaTime;

        // 將法線繞旋轉軸旋轉
        Vector3 nextNormal = Quaternion.AngleAxis(angle, rotationAxis) * currentNormal;

        // 重新計算目前的移動方向
        Vector3 moveDirection = Vector3.Cross(rotationAxis, nextNormal).normalized;

        // 根據新法線設定位置
        transform.position = planet.position + nextNormal * surfaceRadius;

        // 根據切線方向和表面法線設定朝向
        transform.rotation = Quaternion.LookRotation(moveDirection, nextNormal);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;
        if(other.TryGetComponent<PlayerHealth>(out var player))
        {
            player.TakeDamage(damage);
            poolable.Release();
        }
    }

    public void OnSpawnFromPool()
    {
        isActive = false;
    }

    public void OnReturnToPool()
    {
        isActive = false;
        planet = null;
        remainingLifetime = 0;
        damage = 0f;
        rotationAxis = Vector3.zero;
        surfaceRadius = 0f;
        angularSpeed = 0f;
    }
}
