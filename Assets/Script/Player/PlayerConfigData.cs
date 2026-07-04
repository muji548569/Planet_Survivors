using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerConfigData
{
    public E_PlayerStat stat;
    public List<float> values;
    public E_StatModifier type;
    public string description;

    public float GetValueFloat(int level)
    {
        return values[level - 1];
    }

    public int GetValueInt(int level)
    {
        return (int)values[level - 1];
    }
}

[System.Serializable]
public class PlayerConfigDataRoot
{
    public List<PlayerConfigData> stats;
}

public enum E_StatModifier
{
    Add,
    Set,
}

public enum E_PlayerStat
{
    MaxHpFlat,
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
