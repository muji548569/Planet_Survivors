using UnityEngine;

public static class DamageCalculator
{
    public static DamageResult CalculatePlayerAttackDamage(float weaponDamage)
    {
        // 獲得玩家數據
        PlayerStat stat = PlayerDataManager.Instance.Data.Stat;
        // 計算傷害
        float damage = weaponDamage;
        damage *= stat.atkMultiplier;
        // 是否爆擊
        bool isCritical = Random.value < stat.critiRate;
        if (isCritical)
        {
            // 乘算爆擊傷害
            damage *= stat.critiDamageMultiplier;
        }

        return new DamageResult(damage, isCritical, false);
    }

    public static DamageResult CalculatePlayerReceiveDamage(float baseDamage)
    {
        // 獲得玩家數據
        PlayerStat stat = PlayerDataManager.Instance.Data.Stat;

        // 判斷是否閃避
        bool isDodged = Random.value < stat.dodgeRate;
        if (isDodged)
        {
            // 如果閃避就直接返回
            return new DamageResult(0, false, true);
        }

        // 計算受擊傷害
        float damage = baseDamage;
        damage *= 1f - stat.defRate;
        damage -= stat.armor;
        // 受擊傷害數值 最少為1
        damage = Mathf.Max(1f, damage);
        
        return new DamageResult(damage, false, false);
    }
}

public struct DamageResult
{
    public float finalDamage;
    public bool isCritical;
    public bool isDodged;
    
    public DamageResult(float finalDamage, bool isCritical, bool isDodged)
    {
        this.finalDamage = finalDamage;
        this.isCritical = isCritical;
        this.isDodged = isDodged;
    }
}