using UnityEngine;

public class FireballWeapon : WeaponBase
{
    private Transform planet;
    public FireballWeapon(WeaponData data, Transform owner, Transform planet) : base(data, owner) 
    {
        this.planet = planet;
    }

    public override void Attack()
    {
        GameObject hitbox = Object.Instantiate(weaponData.projectilePrefab, owner.position, owner.rotation);
        Fireball fireball = hitbox.GetComponent<Fireball>();
        fireball.Init(weaponData.baseDamage, 
                      owner, 
                      weaponData.lifetime, 
                      weaponData.projectileSpeed, 
                      planet, 
                      weaponData.searchRadius);
    }
}
