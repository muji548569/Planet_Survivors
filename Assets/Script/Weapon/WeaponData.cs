using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapon/WeaponData")]
public class WeaponData : ScriptableObject
{
    public E_WeaponType weaponType;         // 武器類型
    public string weaponId;                 // 武器ID
    public string weaponName;               // 武器名
    public GameObject weaponPrefab;         // 武器預制體
    public GameObject projectilePrefab;     // 子彈預制體
    public Sprite icon;                     // 圖片
    public string description;              // 介紹文字
}

[Serializable]
public class WeaponLevelData
{
    public int level;
    public float damage;                    // 基礎攻擊力
    public float cooldown;                  // 攻擊間隔
    public float range;                     // 攻擊範圍
    public float duration;                  // 持續時間
    public float speed;                     // 彈速
    public float searchRadius;              // 鎖敵半徑
    public int projectileCount;             // 子彈數量
    public bool pierce;                     // 是否穿透
}

[Serializable]
public class WeaponLevelTable
{
    public string weaponId;
    public List<WeaponLevelData> levels;

}

[Serializable]
public class WeaponLevelDataTable
{
    public List<WeaponLevelTable> weapons;
}

public enum E_WeaponType
{
    Sword,
    Fireball,
    Orbit
}