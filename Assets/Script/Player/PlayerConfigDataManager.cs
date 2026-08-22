using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerConfigDataManager : MonoBehaviour
{
    public E_LoadState LoadState { get; private set; }
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
        StartCoroutine(LoadPlayerConfigData());
    }

    /// <summary>
    /// 加載角色升級數據
    /// </summary>
    private IEnumerator LoadPlayerConfigData()
    {
        LoadState = E_LoadState.Loading;

        string path = "Json/PlayerUpgrade.json";
        string jsonStr = null;
        bool requestFailed = false;

        // 非同步獲取角色數據
        yield return StreamingAssetLoader.LoadText(
            path,
            (text) => { jsonStr = text; },
            (text) => { requestFailed = true; });

        if (requestFailed || string.IsNullOrEmpty(jsonStr))
        {
            LoadState = E_LoadState.Failed;

            Debug.LogError($"[PlayerConfigDataManager] PlayerUpgrade.json 請求失敗: {path}");
            yield break;
        }

        // 反序列化
        try
        {
            PlayerConfigDataRoot root = JsonUtility.FromJson<PlayerConfigDataRoot>(jsonStr);
            if (root == null || root.stats == null)
            {
                LoadState = E_LoadState.Failed;

                Debug.LogError("PlayerUpgrade.json 解析失敗或 stats 為空");
                yield break;
            }

            playerUpgradeDataDic = new Dictionary<E_PlayerStat, PlayerConfigData>();
            foreach (PlayerConfigData playerConfigData in root.stats)
            {
                if (playerUpgradeDataDic.ContainsKey(playerConfigData.stat))
                {
                    Debug.LogWarning($"重複的角色升級資料: {playerConfigData.stat}，後者會覆蓋前者");
                }

                playerUpgradeDataDic[playerConfigData.stat] = playerConfigData;
            }

            LoadState = E_LoadState.Success;
        }
        catch(Exception e)
        {
            LoadState = E_LoadState.Failed;

            Debug.LogError(
                $"[PlayerConfigDataManager] PlayerUpgrade.json 解析失敗\n" +
                $"Path: {path}\n" +
                $"Length: {jsonStr?.Length ?? 0}\n" +
                $"Exception: {e}"
            );

            yield break;
        }
        
    }

    public float GetValue(E_PlayerStat stat, int level)
    {
        if (LoadState != E_LoadState.Success || playerUpgradeDataDic == null)
        {
            Debug.LogError($"[PlayerConfigDataManager] 資料不可用，目前狀態: {LoadState}");
            return 0f;
        }

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
        if (LoadState != E_LoadState.Success || playerUpgradeDataDic == null)
        {
            Debug.LogError($"[PlayerConfigDataManager] 資料不可用，目前狀態: {LoadState}");
            return 0;
        }

        if (!playerUpgradeDataDic.TryGetValue(statType, out PlayerConfigData data))
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
        if (LoadState != E_LoadState.Success || playerUpgradeDataDic == null)
        {
            Debug.LogError($"[PlayerConfigDataManager] 資料不可用，目前狀態: {LoadState}");
            return "";
        }
        if (!playerUpgradeDataDic.TryGetValue(statType, out PlayerConfigData data))
        {
            Debug.LogError($"[PlayerConfigDataManager] 找不到角色屬性資料: {statType}");
            return "";
        }
        if (level <= 0 || level > data.values.Count)
        {
            Debug.LogError($"[PlayerConfigDataManager] {statType} 等級超出範圍: {level}");
            return "";
        }

        string diff = GetUpgradeDifference(data, level);
        return data.description + diff;
    }

    public string GetStatName(E_PlayerStat statType)
    {
        if (LoadState != E_LoadState.Success || playerUpgradeDataDic == null)
        {
            Debug.LogError($"[PlayerConfigDataManager] 資料不可用，目前狀態: {LoadState}");
            return "";
        }

        if(!playerUpgradeDataDic.TryGetValue(statType, out PlayerConfigData data))
        {
            Debug.LogError(
            $"[PlayerConfigDataManager] 找不到角色屬性資料: {statType}");
            return "";
        }

        return data.description;
    }

    private string GetUpgradeDifference(PlayerConfigData data, int level)
    {
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
