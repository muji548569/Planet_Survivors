using Unity.Mathematics;
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

    private Vector3 lastNormal;     // 紀錄上一幀的 surface normal

    public void Init(Transform owner, GameObject bulletPrefab, int bulletCount, float damage, float radius, float rotateSpeed, float duration)
    {
        this.owner = owner;
        this.bulletPrefab = bulletPrefab;
        this.bulletCount = bulletCount;
        this.damage = damage;
        this.radius = radius;
        this.rotateSpeed = rotateSpeed;
        this.duration = duration;

        // 生成時先和角色位置、方向一致
        transform.SetPositionAndRotation(owner.position, owner.rotation);

        // 記錄目前球面法線
        lastNormal = owner.up;

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

        // 目前角色所在的的球面法向
        Vector3 currentNormal = owner.up;

        // 旋轉只跟隨玩家在球面法向變化
        // 算出上一幀法線轉到現在法線需要旋轉多少
        Quaternion surfaceDelta = Quaternion.FromToRotation(lastNormal, currentNormal);
        // 應用法向變化 (記得Quaternion相乘時 是以後乘的為基準旋轉)
        transform.rotation = surfaceDelta * transform.rotation;

        // 跟隨玩家位置
        transform.position = owner.position;

        // orbit 自己旋轉
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.Self);

        // 紀錄這一幀的法線
        lastNormal = currentNormal;
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
