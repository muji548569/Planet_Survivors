using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerConfigData
{
    public E_PlayerStat stat;
    public float baseValue;
    public List<float> values;
    public E_StatValueDisplayType displayType;
    public string description;

    public float GetValueFloat(int level)
    {
        return values[level - 1];
    }
}

[System.Serializable]
public class PlayerConfigDataRoot
{
    public List<PlayerConfigData> stats;
}

public enum E_StatValueDisplayType
{
    Number,
    Percent,
}

public enum E_PlayerStat
{
    MaxHp,
    AtkMultiplier,
    DefRate,
    Armor,
    MoveSpeed,
    DodgeRate,
    AttackSpeed,
    PickupRange,
    CritiRate,
    CritiDamageMultiplier,
    MaxJumpTimes,
    JumpStrength,
    ExpRate,
}
