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

        SpawnBullet();

        Destroy(gameObject, this.duration);
    }

    void Update()
    {
        if(owner == null)
        {
            Debug.LogError("沒有owner");
            return;
        } 
        transform.position = owner.position;

        // 計算累積角度
        currentAngle += rotateSpeed * Time.deltaTime;
        // 負責讓環跟著角色方向對齊
        Quaternion baseRotation = Quaternion.LookRotation(owner.forward, owner.up);
        // 負責讓環持續累積旋轉
        Quaternion orbitRotation = Quaternion.AngleAxis(currentAngle, owner.up);

        transform.rotation = baseRotation * orbitRotation;
        
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
