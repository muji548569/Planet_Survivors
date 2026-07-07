using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerConfigDataManager : MonoBehaviour
{
    public static PlayerConfigDataManager Instance { get; private set; }
    private Dictionary<E_PlayerStat, PlayerConfigData> playerUpgradeDataDic;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        LoadPlayerConfigData();
    }

    /// <summary>
    /// 加載角色升級數據
    /// </summary>
    private void LoadPlayerConfigData()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Json", "PlayerUpgrade.json");
        if (!File.Exists(path))
        {
            Debug.LogError($"找不到 PlayerUpgrade.json: {path}");
            return;
        }

        string jsonStr = File.ReadAllText(path);
        PlayerConfigDataRoot root = JsonUtility.FromJson<PlayerConfigDataRoot>(jsonStr);
        if (root == null || root.stats == null)
        {
            Debug.LogError("PlayerUpgrade.json 解析失敗或 stats 為空");
            return;
        }

        playerUpgradeDataDic = new Dictionary<E_PlayerStat, PlayerConfigData>();
        foreach(PlayerConfigData playerConfigData in root.stats)
        {
            if (playerUpgradeDataDic.ContainsKey(playerConfigData.stat))
            {
                Debug.LogWarning($"重複的角色升級資料: {playerConfigData.stat}，後者會覆蓋前者");
            }

            playerUpgradeDataDic[playerConfigData.stat] = playerConfigData;
        }
    }

    public float GetValue(E_PlayerStat stat, int level)
    {
        if (!playerUpgradeDataDic.TryGetValue(stat, out PlayerConfigData playerConfigData))
        {
            Debug.LogError($"找不到角色升級資料: {stat}");
            return 0f;
        }

        if (level <= 0 || level > playerConfigData.values.Count)
        {
            Debug.LogError($"{stat} 等級超出範圍: {level}");
            return 0f;
        }

        return playerConfigData.GetValueFloat(level);
    }

    public int GetStatMaxLevel(E_PlayerStat statType)
    {
        if(!playerUpgradeDataDic.TryGetValue(statType, out PlayerConfigData data))
        {
            Debug.LogError($"沒有對應屬性: {statType}");
            return 0;
        }

        return data.values.Count;
    }

    #region 升級系統UI
    public Sprite GetStatIcon(E_PlayerStat statType)
    {
        switch (statType)
        {
            default:
                Debug.LogError($"找不到對應角色數值: {statType}");
                return null;
            case E_PlayerStat.MaxHpFlat:
                return Resources.Load<Sprite>(Path.Combine("Icon", "PlayerStat", "MaxHpFlat"));
            case E_PlayerStat.AtkMultiplier:
                return Resources.Load<Sprite>(Path.Combine("Icon", "PlayerStat", "AtkMultiplier"));
            case E_PlayerStat.DefRate:
                return Resources.Load<Sprite>(Path.Combine("Icon", "PlayerStat", "DefRate"));
            case E_PlayerStat.Armor:
                return Resources.Load<Sprite>(Path.Combine("Icon", "PlayerStat", "Armor"));
            case E_PlayerStat.MoveSpeed:
                return Resources.Load<Sprite>(Path.Combine("Icon", "PlayerStat", "MoveSpeed"));
            case E_PlayerStat.DodgeRate:
                return Resources.Load<Sprite>(Path.Combine("Icon", "PlayerStat", "DodgeRate"));
            case E_PlayerStat.AttackSpeed:
                return Resources.Load<Sprite>(Path.Combine("Icon", "PlayerStat", "AttackSpeed"));
            case E_PlayerStat.PickupRange:
                return Resources.Load<Sprite>(Path.Combine("Icon", "PlayerStat", "PickupRange"));
            case E_PlayerStat.CritiRate:
                return Resources.Load<Sprite>(Path.Combine("Icon", "PlayerStat", "CritiRate"));
            case E_PlayerStat.CritiDamageMultiplier:
                return Resources.Load<Sprite>(Path.Combine("Icon", "PlayerStat", "CritiDamageMultiplier"));
            case E_PlayerStat.MaxJumpTimes:
                return Resources.Load<Sprite>(Path.Combine("Icon", "PlayerStat", "MaxJumpTimes"));
            case E_PlayerStat.JumpStrength:
                return Resources.Load<Sprite>(Path.Combine("Icon", "PlayerStat", "JumpStrength"));
            case E_PlayerStat.ExpRate:
                return Resources.Load<Sprite>(Path.Combine("Icon", "PlayerStat", "ExpRate"));
        }
    }
    public string GetStatDescription(E_PlayerStat statType, int level)
    {
        string diff = GetUpgradeDifference(statType, level);
        return playerUpgradeDataDic[statType].description + diff;
    }

    public string GetStatName(E_PlayerStat statType)
    {
        return playerUpgradeDataDic[statType].description;
    }

    private string GetUpgradeDifference(E_PlayerStat statType, int level)
    {
        PlayerConfigData data = playerUpgradeDataDic[statType];

        float previousValue = level == 1 ? data.baseValue : data.GetValueFloat(level - 1);
        float nextValue = data.GetValueFloat(level);
        float diff = nextValue - previousValue;
        
        string sign = diff >= 0 ? "+" : "-";
        float absDiff = Mathf.Abs(diff);

        switch (data.displayType)
        {
            default:
                return $"{sign} {absDiff}";
            case E_StatValueDisplayType.Number:
                return $"{sign} {absDiff}";
            case E_StatValueDisplayType.Percent:
                return $"{sign} {absDiff * 100}%";
            
        }
    }
    #endregion
}
