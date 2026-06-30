using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WeaponDataManager : MonoBehaviour
{
    public static WeaponDataManager Instance { get; private set; }
    private Dictionary<string, Dictionary<int, WeaponLevelData>> levelDataDic;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        LoadLevelData();
    }

    private void LoadLevelData()
    {

        // 1.獲取json文件
        string path = Path.Combine(Application.streamingAssetsPath, "Json", "Weapon.json");
        if (!File.Exists(path))
        {
            Debug.LogError($"找不到武器資料檔案: {path}");
            return;
        }
        string jsonStr = File.ReadAllText(path);
        WeaponLevelDataTable table = JsonUtility.FromJson<WeaponLevelDataTable>(jsonStr);

        // 2.將武器等級資料保存到字典中
        levelDataDic = new Dictionary<string, Dictionary<int, WeaponLevelData>>();
        // 先遍歷所有武器
        foreach(WeaponLevelTable weaponTable in table.weapons)
        {
            Dictionary<int, WeaponLevelData> levelDict = new Dictionary<int, WeaponLevelData>();
            // 再遍歷該武器所有等級
            foreach(WeaponLevelData levelData in weaponTable.levels)
            {
                // 把等級資料抓出來
                levelDict[levelData.level] = levelData;
            }
            // 把武器資料抓出來
            levelDataDic[weaponTable.weaponId] = levelDict;
        }
    }

    public WeaponLevelData GetLevelData(string weaponId, int level)
    {
        // 先確定有沒有該武器 如果有會傳出該武器的等級資料字典
        if (!levelDataDic.TryGetValue(weaponId, out var levelDict))
        {
            Debug.LogError($"沒有該武器資料: {weaponId}");
            return null;
        }
        // 再確定有沒有該武器 有沒有 該等級資料 
        // 如果有會傳出 該等級資料
        if (!levelDict.TryGetValue(level, out var levelData))
        {
            Debug.LogError($"找不到武器等級資料 weaponId: {weaponId}, level: {level}");
            return null;
        }
        // 返回傳出的等級資料
        return levelData;
    }
}
