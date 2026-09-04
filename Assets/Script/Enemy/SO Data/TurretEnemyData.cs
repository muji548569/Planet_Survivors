using UnityEngine;

[CreateAssetMenu(fileName = "TurretEnemyData", menuName = "Enemy/Turret Enemy Data")]
public class TurretEnemyData : EnemyData
{
    [Header("Turret")]
    public float remainLifetime;
    public GameObject projectilePrefab;
    public float projectileSpeed;
    public float projectileDamage;
    public float projectileLifetime;
}