using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

public class WeaponDataManager : MonoBehaviour
{
    public static WeaponDataManager Instance { get; private set; }
    private Dictionary<E_WeaponType, Dictionary<int, WeaponLevelData>> levelDataDic = new();
    [SerializeField] private List<WeaponData> weaponSODataList = new();
    private Dictionary<E_WeaponType, WeaponData> weaponSODataDic = new(); 

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this; 

        foreach(WeaponData soData in weaponSODataList)
        {
            weaponSODataDic[soData.weaponType] = soData;
        }

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
        levelDataDic = new Dictionary<E_WeaponType, Dictionary<int, WeaponLevelData>>();
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
            levelDataDic[weaponTable.weaponType] = levelDict;
        }
    }

    public WeaponLevelData GetLevelData(E_WeaponType weaponType, int level)
    {
        // 先確定有沒有該武器 如果有會傳出該武器的等級資料字典
        if (!levelDataDic.TryGetValue(weaponType, out var levelDict))
        {
            Debug.LogError($"沒有該武器資料: {weaponType}");
            return null;
        }
        // 再確定有沒有該武器 有沒有 該等級資料 
        // 如果有會傳出 該等級資料
        if (!levelDict.TryGetValue(level, out var levelData))
        {
            Debug.LogError($"找不到武器等級資料 weaponId: {weaponType}, level: {level}");
            return null;
        }
        // 返回傳出的等級資料
        return levelData;
    }

    public WeaponData GetWeaponData(E_WeaponType weaponType)
    {
        if (!weaponSODataDic.TryGetValue(weaponType, out WeaponData data))
        {
            Debug.LogError($"找不到 WeaponData: {weaponType}");
            return null;
        }

        return data;
    }

    public int GetWeaponMaxLevel(E_WeaponType weaponType)
    {
        return levelDataDic[weaponType].Values.Count;
    }

    public Sprite GetWeaponIcon(E_WeaponType weaponType)
    {
        return weaponSODataDic[weaponType].icon;
    }

    public string GetWeaponName(E_WeaponType weaponType)
    {
        return weaponSODataDic[weaponType].weaponName;
    }

    public string GetWeaponDescription(E_WeaponType weaponType, int level)
    {
        if(level == 1)
            return weaponSODataDic[weaponType].description;

        WeaponLevelData previousData = levelDataDic[weaponType][level-1];
        WeaponLevelData nextData = levelDataDic[weaponType][level];

        return GetLevelDifferenceText(previousData, nextData);
    }

    /// <summary>
    /// 找出前後升級前後數值的變化
    /// </summary>
    /// <param name="previousData"></param>
    /// <param name="nextData"></param>
    /// <returns></returns>
    public string GetLevelDifferenceText(WeaponLevelData previousData, WeaponLevelData nextData)
    {
        StringBuilder sb = new StringBuilder();

        FieldInfo[] fields = typeof(WeaponLevelData).GetFields(BindingFlags.Instance | BindingFlags.Public);

        foreach (FieldInfo field in fields)
        {
            if (field.Name == "level")
                continue;

            object previousValue = field.GetValue(previousData);
            object nextValue = field.GetValue(nextData);

            if (Equals(previousValue, nextValue))
                continue;

            sb.AppendLine(GetFieldDifferenceText(field.Name, previousValue, nextValue));
        }

        return sb.ToString();
    }

    /// <summary>
    /// 將有差異的數值變為字串
    /// </summary>
    /// <returns></returns>
    public string GetFieldDifferenceText(string fieldName, object previousValue, object nextValue)
    {
        if(previousValue is float previousFloat && nextValue is float nextFloat)
        {
            float diff = nextFloat - previousFloat;
            if (diff >= 0)
                return $"{GetFieldDisplayName(fieldName)} +{diff}";
            else 
                return $"{GetFieldDisplayName(fieldName)} -{Mathf.Abs(diff)}";
        }

        if(previousValue is int previousInt && nextValue is int nextInt)
        {
            int diff = nextInt - previousInt;
            if (diff >= 0)
                return $"{GetFieldDisplayName(fieldName)} +{diff}";
            else
                return $"{GetFieldDisplayName(fieldName)} -{Mathf.Abs(diff)}";
        }

        if(previousValue is bool previousBool && nextValue is bool nextBool)
        {
            return nextBool 
                ? $"{GetFieldDisplayName(fieldName)} : 啟用" 
                : $"{GetFieldDisplayName(fieldName)} : 停用";
        }

        return $"{GetFieldDisplayName(fieldName)}: {previousValue} → {nextValue} ";
    }

    /// <summary>
    /// 將欄位名稱轉換成顯示在UI上的文字
    /// </summary>
    /// <returns></returns>
    public string GetFieldDisplayName(string fieldName)
    {
        switch (fieldName)
        {
            default:
                return fieldName;
            case "damage":
                return "攻擊力";
            case "cooldown":
                return "攻擊間隔";
            case "range":
                return "攻擊範圍";
            case "duration":
                return "持續時間";
            case "speed":
                return "彈速";
            case "searchRadius":
                return "鎖敵半徑";
            case "projectileCount":
                return "子彈數量";
            case "pierce":
                return "穿透";
        }
    }
}
