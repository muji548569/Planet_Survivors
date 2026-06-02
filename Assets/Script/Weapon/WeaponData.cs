using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapon/WeaponData")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public float baseDamage;
    public float attackInterval;
    public float range;
    public GameObject projectilePrefab;
}
