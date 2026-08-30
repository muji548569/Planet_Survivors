using System.Collections.Generic;
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
    public Dictionary<E_PlayerStat, int> statLevels = new();
}

[System.Serializable]
public class PlayerStat
{
    public float maxHp;
    public float currentHp;
    public float atkMultiplier;        
    public float defRate;
    public int armor;
    public float moveSpeed;
    public float dodgeRate;
    public float attackSpeed;
    public float pickupRange;
    public float critiRate;
    public float critiDamageMultiplier  ;
    public int maxJumpTimes;
    public float jumpStrength;
    public float expRate;
}
