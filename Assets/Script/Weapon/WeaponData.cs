using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapon/WeaponData")]
public class WeaponData : ScriptableObject
{
    public E_WeaponType weaponType;         // 武器類型
    public string weaponName;               // 武器名
    public float baseDamage;                // 基礎攻擊力
    public float attackInterval;            // 攻擊間隔
    public float attackRange;               // 攻擊範圍
    public float duration;                  // 持續時間
    public float projectileSpeed;           // 彈速
    public float searchRadius;              // 鎖敵半徑
    public int bulletCount;                 // 子彈數量
    public GameObject weaponPrefab;         // 武器預制體
    public GameObject projectilePrefab;     // 子彈預制體
}

public enum E_WeaponType
{
    Sword,
    Fireball,
    Orbit
}