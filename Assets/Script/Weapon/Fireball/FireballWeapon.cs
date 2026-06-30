using UnityEngine;

public class FireballWeapon : WeaponBase
{
    private WeaponLevelData currentData => WeaponDataManager.Instance.GetLevelData(weaponData.weaponId, level);
    private Transform planet;
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
        GameObject hitbox = Object.Instantiate(weaponData.projectilePrefab, owner.position, owner.rotation);
        Fireball fireball = hitbox.GetComponent<Fireball>();
        fireball.Init(currentData.damage, 
                      owner, 
                      currentData.duration, 
                      currentData.speed, 
                      planet, 
                      currentData.searchRadius);
    }
}
