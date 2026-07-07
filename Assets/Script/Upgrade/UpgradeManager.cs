using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ApplyUpgrade(UpgradeOption option)
    {
        switch (option.upgradeType)
        {
            case E_UpgradeType.Player:
                ApplyPlayerStatUpgrade(option);
                break;
            case E_UpgradeType.Weapon:
                ApplyWeaponUpgrade(option);
                break;
        }
    }

    public void ApplyPlayerStatUpgrade(UpgradeOption option)
    {
        PlayerDataManager.Instance.ApplyStatUpgrade(option.playerStat);
    }

    public void ApplyWeaponUpgrade(UpgradeOption option)
    {
        WeaponController.Instance.UpgradeWeapon(option.weaponType);
    }
}