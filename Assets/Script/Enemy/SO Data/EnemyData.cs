using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string EnemyName;
    [Header("Stats")]
    public float maxHealth;
    public float moveSpeed;
    public float contactDamage;
    [Header("Combat")]
    public float attackCooldown;
    [Range(0f, 1f)] public float knockbackResistance = 0.1f;
    public float knockbackSpeed = 8f;
    public float knockbackDeceleration = 30f;
    [Header("Drop")]
    public DropEntry[] drops;
    public float dropSpreadRadius;
}