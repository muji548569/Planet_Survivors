using System;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; }
    public PlayerData Data { get; private set; }

    public Action<float, float> OnHealthChanged;
    public Action<int, int> OnExpChanged;
    public Action<int> OnCoinChanged;
    public Action<int> OnLevelChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Data = new PlayerData();
        Data.currentHp = Data.maxHp;
    }

    public void NotifyAll()
    {
        OnHealthChanged?.Invoke(Data.currentHp, Data.maxHp);
        OnExpChanged?.Invoke(Data.currentExp, GetExpToNextLevel());
        OnCoinChanged?.Invoke(Data.currentCoin);
        OnLevelChanged?.Invoke(Data.level);
    }

    public void SetHealth(float currentHealth)
    {
        Data.currentHp = Mathf.Clamp(currentHealth, 0, Data.maxHp);
        OnHealthChanged?.Invoke(Data.currentHp, Data.maxHp);
    }

    public void AddExp(int amount)
    {
        Data.currentExp += amount;
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

        // TODO: 觸發升級UI
        // TODO: 暫停遊戲
        // TODO: 顯示三選一強化
    }
}
