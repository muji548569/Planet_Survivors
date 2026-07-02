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
}
