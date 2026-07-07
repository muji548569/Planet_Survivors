using UnityEngine;

public enum E_UpgradeType
{
    Player,
    Weapon,
}

// 升級選項卡
public class UpgradeOption
{
    // 升級類型
    public E_UpgradeType upgradeType;
    public E_WeaponType weaponType;
    public E_PlayerStat playerStat;
    public int level;

    public Sprite GetIcon()
    {
        switch (upgradeType)
        {
            default:
                Debug.LogError($"找不到對應升級類型: {upgradeType}");
                return null;
            case E_UpgradeType.Player:
                return PlayerConfigDataManager.Instance.GetStatIcon(playerStat);
            case E_UpgradeType.Weapon:
                return WeaponDataManager.Instance.GetWeaponIcon(weaponType);
        }
    }

    public string GetOptionName()
    {
        switch (upgradeType)
        {
            default:
                Debug.LogError($"找不到對應升級類型: {upgradeType}");
                return null;
            case E_UpgradeType.Player:
                return PlayerConfigDataManager.Instance.GetStatName(playerStat);
            case E_UpgradeType.Weapon:
                return WeaponDataManager.Instance.GetWeaponName(weaponType);
        }
    }

    public string GetDescription()
    {
        switch (upgradeType)
        {
            default:
                Debug.LogError($"找不到對應升級類型: {upgradeType}");
                return null;
            case E_UpgradeType.Player:
                return PlayerConfigDataManager.Instance.GetStatDescription(playerStat, level);
            case E_UpgradeType.Weapon:
                return WeaponDataManager.Instance.GetWeaponDescription(weaponType, level);
        }
    }
}

