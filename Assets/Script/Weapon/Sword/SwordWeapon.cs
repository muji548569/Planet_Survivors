using UnityEngine;

public class SwordWeapon : WeaponBase
{
    public SwordWeapon(WeaponData data, Transform owner) : base(data, owner) { }
    public override void Attack()
    {
        // 生成攻擊預設體
        GameObject hitbox = Object.Instantiate(weaponData.projectilePrefab, owner.position, owner.rotation);
        // 調整生成大小
        hitbox.transform.localScale = Vector3.one * weaponData.attackRange;
        // 初始化劍氣
        SwordSlash slash = hitbox.GetComponent<SwordSlash>();
        slash.Init(weaponData.baseDamage, owner, weaponData.lifetime);
    }
}
