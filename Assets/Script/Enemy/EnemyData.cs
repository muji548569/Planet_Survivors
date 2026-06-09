using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/EnemyData")]
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
    public GameObject dropPrefab;
}
