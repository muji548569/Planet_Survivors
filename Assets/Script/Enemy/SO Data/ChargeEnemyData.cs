using UnityEngine;

[CreateAssetMenu(fileName = "ChargeEnemyData", menuName = "Enemy/Charge Enemy Data")]
public class ChargeEnemyData : EnemyData
{
    [Header("Charge")]
    public float remainLifetime;
}
