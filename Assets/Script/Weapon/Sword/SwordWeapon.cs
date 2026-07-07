using UnityEngine;

public class SwordWeapon : WeaponBase
{
    private WeaponLevelData currentData => WeaponDataManager.Instance.GetLevelData(weaponData.weaponType, level);
    public SwordWeapon(WeaponData data, Transform owner) : base(data, owner) { }

    public override float GetCooldown()
    {
        return currentData.cooldown;
    }

    public override void Attack()
    {
        // 生成攻擊預設體
        GameObject hitbox = Object.Instantiate(weaponData.projectilePrefab, owner.position, owner.rotation);
        // 調整生成大小
        hitbox.transform.localScale = Vector3.one * currentData.range;
        // 初始化劍氣
        SwordSlash slash = hitbox.GetComponent<SwordSlash>();
        slash.Init(currentData.damage, owner, currentData.duration);
    }
}
