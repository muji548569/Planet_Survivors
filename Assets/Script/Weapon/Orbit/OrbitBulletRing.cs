using UnityEngine;

public class OrbitBulletRing : MonoBehaviour
{
    private Transform owner;
    private GameObject bulletPrefab;
    private int bulletCount;
    private float damage;
    private float radius;
    private float rotateSpeed;
    private float duration;
    private float currentAngle;     // 累積角度
    public void Init(Transform owner, GameObject bulletPrefab, int bulletCount, float damage, float radius, float rotateSpeed, float duration)
    {
        this.owner = owner;
        this.bulletPrefab = bulletPrefab;
        this.bulletCount = bulletCount;
        this.damage = damage;
        this.radius = radius;
        this.rotateSpeed = rotateSpeed;
        this.duration = duration;

        currentAngle = 0;

        // 生成時先和角色位置、方向一致
        transform.SetPositionAndRotation(owner.position, owner.rotation);

        SpawnBullet();

        Destroy(gameObject, this.duration);
    }

    // 使用LateUpdate可以在角色移動、轉向處理完後再跟隨
    void LateUpdate()
    {
        if(owner == null)
        {
            Debug.LogError("沒有owner");
            return;
        }

        // 計算累積角度
        currentAngle += rotateSpeed * Time.deltaTime;
        
        // 應用位置與旋轉
        transform.position = owner.position;
        transform.rotation = Quaternion.AngleAxis(currentAngle, Vector3.up); ;
    }

    public void SpawnBullet()
    {
        for(int i = 0; i < bulletCount; i++)
        {
            // 根據子彈數量得到每個子彈各自的角度
            float angle = 360f / bulletCount * i;
            // 計算出弧度
            float rad = angle * Mathf.Deg2Rad;

            // 根據高中三角函數
            // x = cosθ
            // y = sinθ
            // 得到單位向量後再乘上圓半徑(radius)，就可以得到子彈再圓周上的位置(x, y)
            Vector3 localPos = new Vector3(Mathf.Cos(rad) * radius, 0, Mathf.Sin(rad) * radius);

            GameObject bullet = Instantiate(bulletPrefab, transform);
            bullet.transform.localPosition = localPos;
            OrbitBullet orbitBullet = bullet.GetComponent<OrbitBullet>();
            orbitBullet.Init(damage, owner);
        }
    }
}
