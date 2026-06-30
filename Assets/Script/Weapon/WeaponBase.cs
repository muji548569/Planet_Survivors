using UnityEngine;

public abstract class WeaponBase
{
    public WeaponData weaponData;
    public int level = 1;
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
        if (timer > GetCooldown())
        {
            Attack();
            timer = 0;
        }
    }

    public abstract void Attack();
    public abstract float GetCooldown();
}
