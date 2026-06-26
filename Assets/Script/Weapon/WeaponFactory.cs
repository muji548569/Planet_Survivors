using UnityEngine;

public static class WeaponFactory
{
    public static WeaponBase Create(WeaponData data, Transform owner, Transform planet)
    {
        switch(data.weaponType)
        {
            case E_WeaponType.Sword:
                return new SwordWeapon(data, owner);
            case E_WeaponType.Fireball:
                return new FireballWeapon(data, owner, planet);
            case E_WeaponType.Orbit:
                return new OrbitWeapon(data, owner);
            default:
                Debug.LogError($"不支援的武器類型: {data.weaponType}");
                return null;
        }
    }
}
