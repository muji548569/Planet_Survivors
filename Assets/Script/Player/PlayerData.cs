using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int level = 1;
    public int currentExp;
    public int currentCoin;

    public int baseExpToNextLevel = 10;
    public float expGrowthRate = 1.25f;

    public PlayerStat Stat { get; private set; } = new PlayerStat();
}

[System.Serializable]
public class PlayerStat
{
    private float baseMaxHp = 20;
    public float maxHpFlat = 0;
    public float MaxHp => baseMaxHp + maxHpFlat;
    public float currentHp;
    public float atkMultiplier = 1f;        
    public float defRate = 0f;
    public int armor = 0;
    public float moveSpeed = 4f;
    public float dodgeRate = 0f;
    public float attackSpeed = 1f;
    public float pickupRange = 1f;
    public float critiRate = 0f;
    public float critiDamageRate = 2f;
    public int maxJumpTimes = 1;
    public float jumpStrength = 6f;
    public float expRate = 1f;
}
