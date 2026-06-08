using UnityEngine;

public class OrbitWeapon : WeaponBase
{
    public OrbitWeapon(WeaponData data, Transform owner) : base(data, owner) { }

    public override void Attack()
    {
        GameObject ring = Object.Instantiate(weaponData.weaponPrefab, owner.position, owner.rotation);
        OrbitBulletRing orbitRing = ring.GetComponent<OrbitBulletRing>();
        orbitRing.Init(owner, weaponData.projectilePrefab, weaponData.bulletCount, weaponData.baseDamage, weaponData.attackRange, weaponData.projectileSpeed, weaponData.duration);
    }
}
