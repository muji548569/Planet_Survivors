using System;
using System.IO;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; }
    public PlayerData Data { get; private set; }
    
    public Action<float, float> OnHealthChanged;
    public Action<int, int> OnExpChanged;
    public Action<int> OnCoinChanged;
    public Action<int> OnLevelChanged;

    public bool IsInitialized { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Init()
    {
        IsInitialized = true;
        ResetData();
    }

    public void ResetData()
    {
        Data = new PlayerData();
        Data.Stat.currentHp = Data.Stat.MaxHp;
        foreach(E_PlayerStat stat in Enum.GetValues(typeof(E_PlayerStat)))
        {
            Data.statLevels[stat] = 0;
        }
        NotifyAll();
    }

    public void NotifyAll()
    {
        if(!IsInitialized) return;
        OnHealthChanged?.Invoke(Data.Stat.currentHp, Data.Stat.MaxHp);
        OnExpChanged?.Invoke(Data.currentExp, GetExpToNextLevel());
        OnCoinChanged?.Invoke(Data.currentCoin);
        OnLevelChanged?.Invoke(Data.level);
    }

    public void SetHealth(float currentHealth)
    {
        Data.Stat.currentHp = Mathf.Clamp(currentHealth, 0, Data.Stat.MaxHp);
        OnHealthChanged?.Invoke(Data.Stat.currentHp, Data.Stat.MaxHp);
    }

    public void AddExp(int amount)
    {
        Data.currentExp += Mathf.RoundToInt(amount * Data.Stat.expRate);
        // 使用while避免一次獲得大量經驗值只升一等
        while (Data.currentExp >= GetExpToNextLevel())
        {
            Data.currentExp -= GetExpToNextLevel();
            LevelUp();
        }

        OnExpChanged?.Invoke(Data.currentExp, GetExpToNextLevel());
    }

    public void AddCoin(int amount)
    {
        Data.currentCoin += amount;
        OnCoinChanged?.Invoke(Data.currentCoin);
    }

    /// <summary>
    /// 計算下一個等級所需經驗量
    /// </summary>
    /// <returns></returns>
    private int GetExpToNextLevel()
    {
        return Mathf.RoundToInt(Data.baseExpToNextLevel * Mathf.Pow(Data.expGrowthRate, Data.level - 1));
    }

    private void LevelUp()
    {
        Data.level++;
    
        OnLevelChanged?.Invoke(Data.level);

        // 暫停遊戲
        GamePauseManager.Instance.PauseGame();
        // 觸發升級UI
        UIManager.Instance.OpenPopup(E_PanelType.Upgrade);
    }

    public void ApplyStatUpgrade(E_PlayerStat statType)
    {
        // 得到該屬性的下一個等級
        int nextLevel = GetNextStatLevel(statType);
        // 紀錄升級到哪個等級
        Data.statLevels[statType] = nextLevel;
        // 得到該屬性該等級的數值
        float value = PlayerConfigDataManager.Instance.GetValue(statType, nextLevel);
        switch (statType)
        {
            case E_PlayerStat.MaxHpFlat:
                Data.Stat.maxHpFlat += value;
                Data.Stat.currentHp += value;
                break;
            case E_PlayerStat.AtkMultiplier:
                Data.Stat.atkMultiplier = value;
                break;
            case E_PlayerStat.DefRate:
                Data.Stat.defRate = value;
                break;
            case E_PlayerStat.Armor:
                Data.Stat.armor = Mathf.RoundToInt(value);
                break;
            case E_PlayerStat.MoveSpeed:
                Data.Stat.moveSpeed = value;
                break;
            case E_PlayerStat.DodgeRate:
                Data.Stat.dodgeRate = value;
                break;
            case E_PlayerStat.AttackSpeed:
                Data.Stat.attackSpeed = value;
                break;
            case E_PlayerStat.PickupRange:
                Data.Stat.pickupRange = value;
                break;
            case E_PlayerStat.CritiRate:
                Data.Stat.critiRate = value;
                break;
            case E_PlayerStat.CritiDamageMultiplier:
                Data.Stat.critiDamageMultiplier = value;
                break;
            case E_PlayerStat.MaxJumpTimes:
                Data.Stat.maxJumpTimes = Mathf.RoundToInt(value);   
                break;
            case E_PlayerStat.JumpStrength:
                Data.Stat.jumpStrength = value;
                break;
            case E_PlayerStat.ExpRate:
                Data.Stat.expRate = value;
                break;
        }

        PlayerDataManager.Instance.NotifyAll();
    }

    public int GetNextStatLevel(E_PlayerStat statType)
    {
        return Data.statLevels[statType] + 1;
    }
}
