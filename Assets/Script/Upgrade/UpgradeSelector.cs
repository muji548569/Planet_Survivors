using System.Collections.Generic;
using UnityEngine;

public class UpgradeSelector : MonoBehaviour
{
    public List<UpgradeOption> CreateOptions(int count)
    {
        List<UpgradeOption> candidates = new List<UpgradeOption>();

        AddPlayerStatOptions(candidates);
        AddWeaponOptions(candidates);

        return PickRandomOption(candidates, count);
    }

    /// <summary>
    /// 生成角色數值升級選項
    /// </summary>
    /// <param name="candidates"></param>
    public void AddPlayerStatOptions(List<UpgradeOption> candidates)
    {
        foreach(E_PlayerStat stat in System.Enum.GetValues(typeof(E_PlayerStat)))
        {
            int nextLevel = PlayerDataManager.Instance.GetNextStatLevel(stat);
            int maxLevel = PlayerConfigDataManager.Instance.GetStatMaxLevel(stat);
            if (nextLevel > maxLevel) continue;
            candidates.Add(new UpgradeOption
            {
                upgradeType = E_UpgradeType.Player,
                playerStat = stat,
                level = nextLevel,
            });
        }
    }
    /// <summary>
    /// 生成武器升級選項
    /// </summary>
    /// <param name="candidates"></param>
    public void AddWeaponOptions(List<UpgradeOption> candidates)
    {
        foreach (E_WeaponType weapon in System.Enum.GetValues(typeof(E_WeaponType)))
        {
            int nextLevel = WeaponController.Instance.GetNextWeaponLevel(weapon);
            int maxLevel = WeaponDataManager.Instance.GetWeaponMaxLevel(weapon);
            if (nextLevel > maxLevel) continue;
            candidates.Add(new UpgradeOption
            {
                upgradeType = E_UpgradeType.Weapon,
                weaponType = weapon,
                level = nextLevel,
            });
        }
    }

    public List<UpgradeOption> PickRandomOption(List<UpgradeOption> candidates, int count)
    {
        List<UpgradeOption> picked = new List<UpgradeOption>();
        // 避免可抽取選項比需求的選項數少
        int pickCount = Mathf.Min(count, candidates.Count);
        // 隨機抽需求數量的選項
        for (int i = 0; i < pickCount; i++)
        {
            int index = Random.Range(0, candidates.Count);
            picked.Add(candidates[index]);
            candidates.RemoveAt(index);
        }
        return picked;
    }
}
