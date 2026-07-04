using UnityEngine;

public static class DamageCalculator
{
    public static DamageResult CalculatePlayerDamage(float weaponBaseDamage)
    {
        // 獲得玩家數據
        PlayerStat stat = PlayerDataManager.Instance.Data.Stat;
        // 計算傷害
        float damage = weaponBaseDamage;
        damage *= stat.atkMultiplier;
        // 是否爆擊
        bool isCritical = Random.value < stat.critiRate;
        if (isCritical)
        {
            // 乘算爆擊傷害
            damage *= stat.critiDamageMultiplier;
        }

        return new DamageResult(isCritical, damage);
    }
}

public struct DamageResult
{
    public bool isCritical;
    public float finalDamage;
    public DamageResult(bool isCritical, float finalDamage)
    {
        this.isCritical = isCritical;
        this.finalDamage = finalDamage;
    }
}