using UnityEngine;

public abstract class WeaponBase
{
    public WeaponData weaponData;
    public Transform owner;
    public float timer;

    public WeaponBase(WeaponData weaponData, Transform owner)
    {
        this.weaponData = weaponData;
        this.owner = owner;
    }

    public void Tick(float deltaTime)
    {
        timer += deltaTime;
        if (timer > weaponData.attackInterval)
        {
            Attack();
            timer = 0;
        }
    }

    public abstract void Attack();
}
