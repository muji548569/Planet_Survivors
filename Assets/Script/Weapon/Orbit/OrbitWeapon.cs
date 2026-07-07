using UnityEngine;

public class OrbitWeapon : WeaponBase
{
    private WeaponLevelData currentData => WeaponDataManager.Instance.GetLevelData(weaponData.weaponType, level);
    public OrbitWeapon(WeaponData data, Transform owner) : base(data, owner) { }

    public override float GetCooldown()
    {
        return currentData.cooldown;
    }

    public override void Attack()
    {
        GameObject ring = Object.Instantiate(weaponData.weaponPrefab, owner.position, owner.rotation);
        OrbitBulletRing orbitRing = ring.GetComponent<OrbitBulletRing>();
        orbitRing.Init(owner, 
                       weaponData.projectilePrefab, 
                       currentData.projectileCount, 
                       currentData.damage,
                       currentData.range, 
                       currentData.speed,
                       currentData.duration);
    }
}
