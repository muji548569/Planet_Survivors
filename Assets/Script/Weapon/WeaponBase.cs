using UnityEngine;

public abstract class WeaponBase
{
    public WeaponData weaponData;
    public Transform ownerPos;
    public float timer;

    public WeaponBase(WeaponData weaponData, Transform ownerPos)
    {
        this.weaponData = weaponData;
        this.ownerPos = ownerPos;
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
