using UnityEngine;

public class FireballWeapon : WeaponBase
{
    private WeaponLevelData currentData => WeaponDataManager.Instance.GetLevelData(weaponData.weaponType, level);
    private Transform planet;
    private float spreadAngle = 15f;
    public FireballWeapon(WeaponData data, Transform owner, Transform planet) : base(data, owner) 
    {
        this.planet = planet;
    }

    public override float GetCooldown()
    {
        return currentData.cooldown;
    }

    public override void Attack()
    {
        int count = Mathf.Max(1, currentData.projectileCount);
        Vector3 baseDir = FindTargetDirection(currentData.searchRadius);
        Vector3 surfaceNormal = (owner.position - planet.position).normalized;
        
        for (int i = 0; i < count; i++)
        {
            float offset = spreadAngle * (i - (count - 1) / 2f);

            Vector3 fireDir = Quaternion.AngleAxis(offset, surfaceNormal) * baseDir;

            PoolableObject poolable = PoolManager.Instance.Get(weaponData.projectilePrefab, owner.position, Quaternion.LookRotation(fireDir, surfaceNormal)); ;
            Fireball fireball = poolable.GetComponent<Fireball>();
            fireball.Init(currentData.damage, owner, currentData.duration, currentData.speed, planet, fireDir, currentData.pierce);
        }
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
            float distance = Vector3.Distance(owner.position, hit.transform.position);
            // 更新最近單位
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = hit.transform;
            }
        }

        // 得到最近敵人的方向 
        Vector3 rawDir = nearestEnemy == null ? owner.forward : nearestEnemy.position - owner.position;
        // 計算星球表面法向
        Vector3 surfaceNormal = (owner.position - planet.position).normalized;
        // 把子彈移動方向投影到星球切平面，避免子彈往星球內部或外部飛
        Vector3 tangentDir = Vector3.ProjectOnPlane(rawDir, surfaceNormal).normalized;

        return tangentDir;
    }

}
