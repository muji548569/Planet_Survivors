using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapon/WeaponData")]
public class WeaponData : ScriptableObject
{
    public string weaponName;               // 武器名
    public float baseDamage;                // 基礎攻擊力
    public float attackInterval;            // 攻擊間隔
    public float attackRange;               // 攻擊範圍
    public float lifetime;                  // 持續時間
    public float projectileSpeed;           // 彈速
    public float searchRadius;              // 鎖敵半徑
    public GameObject projectilePrefab;     // 子彈預制體
}
