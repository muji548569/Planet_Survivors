using UnityEngine;

public class PlayerProgression : MonoBehaviour
{
    [SerializeField] private int level = 1;
    [SerializeField] private int currentExp;
    [SerializeField] private int currentCoin;
    [SerializeField] private int baseExpToNextLevel = 10;
    [SerializeField] private float expGrowthRate = 1.25f;

    public void AddExp(int amount)
    {
        currentExp += amount;
        // 使用while避免一次獲得大量經驗值只升一等
        while (currentExp >= GetExpToNextLevel())
        {
            currentExp -= GetExpToNextLevel();
            LevelUp();
        }

        // TODO: 更新經驗值 UI
    }

    public void AddCoin(int amount) 
    {
        currentCoin += amount;
        // TODO: 更新金幣 UI
    }

    /// <summary>
    /// 計算下一個等級所需經驗量
    /// </summary>
    /// <returns></returns>
    private int GetExpToNextLevel()
    {
        return Mathf.RoundToInt(baseExpToNextLevel * Mathf.Pow(expGrowthRate, level - 1));
    }

    private void LevelUp()
    {
        level++;
        print($"升級，現在{level}等");
        // TODO: 觸發升級UI
        // TODO: 暫停遊戲
        // TODO: 顯示三選一強化
    }
}
