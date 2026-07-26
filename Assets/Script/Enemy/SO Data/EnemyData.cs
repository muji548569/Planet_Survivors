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
    public float knockbackResistance;
    [Header("Drop")]
    public DropEntry[] drops;
    public float dropSpreadRadius;
}