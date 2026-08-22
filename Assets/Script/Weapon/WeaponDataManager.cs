using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

public class WeaponDataManager : MonoBehaviour
{
    public E_LoadState LoadState { get; private set; }
    public static WeaponDataManager Instance { get; private set; }
    private Dictionary<E_WeaponType, Dictionary<int, WeaponLevelData>> levelDataDic = new();
    [SerializeField] private List<WeaponData> weaponSODataList = new();
    private Dictionary<E_WeaponType, WeaponData> weaponSODataDic = new();
    private Dictionary<string, string> fieldDisplayNames = new()
    {
        { "damage", "攻擊力" },
        { "cooldown", "攻擊間隔" },
        { "range", "攻擊範圍" },
        { "duration", "持續時間" },
        { "speed", "彈速" },
        { "searchRadius", "鎖敵半徑"},
        { "projectileCount", "子彈數量" },
        { "pierce", "穿透" }
    };

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

        StartCoroutine(LoadLevelData());
    }

    private IEnumerator LoadLevelData()
    {
        LoadState = E_LoadState.Loading;
        // 1.獲取json文件
        string jsonStr = null;
        bool loadFailed = false;
        string path = "Json/Weapon.json";
        // 非同步請求文件
        yield return StreamingAssetLoader.LoadText(
            path,
            (text) => { jsonStr = text; },
            (text) => { loadFailed = true; }
            );

        if(loadFailed || string.IsNullOrEmpty(jsonStr))
        {
            LoadState = E_LoadState.Failed;
            Debug.LogError($"[WeaponDataManager] Weapon.json 請求失敗: {path}");
            yield break;
        }
        // 反序列化
        try
        {
            WeaponLevelDataTable table = JsonUtility.FromJson<WeaponLevelDataTable>(jsonStr);
            if (table == null)
            {
                LoadState = E_LoadState.Failed;
                Debug.LogError($"[WeaponDataManager] Weapon.json 解析失敗: {path}");
                yield break;
            }
            if (table.weapons == null || table.weapons.Count == 0)
            {
                LoadState = E_LoadState.Failed;
                Debug.LogError("[WeaponDataManager] Weapon.json 中 weapons 為空或不存在");
                yield break;
            }

            // 2.將武器等級資料保存到字典中
            levelDataDic = new Dictionary<E_WeaponType, Dictionary<int, WeaponLevelData>>();
            // 先遍歷所有武器
            foreach (WeaponLevelTable weaponTable in table.weapons)
            {
                if (weaponTable == null)
                {
                    Debug.LogError("[WeaponDataManager] Weapon.json 內有空的 weaponTable");
                    continue;
                }
                if (weaponTable.levels == null || weaponTable.levels.Count == 0)
                {
                    Debug.LogError($"[WeaponDataManager] 武器 {weaponTable.weaponType} 沒有 levels 資料");
                    continue;
                }

                Dictionary<int, WeaponLevelData> levelDict = new Dictionary<int, WeaponLevelData>();
                // 再遍歷該武器所有等級
                foreach (WeaponLevelData levelData in weaponTable.levels)
                {
                    if (levelData == null)
                    {
                        Debug.LogError($"[WeaponDataManager] 武器 {weaponTable.weaponType} 有空的等級資料");
                        continue;
                    }
                    if (levelDict.ContainsKey(levelData.level))
                    {
                        Debug.LogError($"[WeaponDataManager] 武器 {weaponTable.weaponType} 等級重複: {levelData.level}");
                        continue;
                    }
                    if (!IsValidLevelData(weaponTable.weaponType, levelData))
                        continue;

                    // 把等級資料抓出來
                    levelDict[levelData.level] = levelData;
                }

                if (levelDict.Count == 0)
                {
                    LoadState = E_LoadState.Failed;
                    Debug.LogError($"[WeaponDataManager] 武器 {weaponTable.weaponType} 沒有任何有效等級資料");
                    yield break;
                }

                // 把武器資料抓出來
                levelDataDic[weaponTable.weaponType] = levelDict;
            }

            if (levelDataDic.Count == 0)
            {
                LoadState = E_LoadState.Failed;
                Debug.LogError("[WeaponDataManager] 沒有任何武器資料成功載入");
                yield break;
            }

            LoadState = E_LoadState.Success;
        }
        catch(Exception e)
        {
            LoadState = E_LoadState.Failed;

            Debug.LogError(
                $"[WeaponDataManager] Weapon.json 解析失敗\n" +
                $"Path: {path}\n" +
                $"Length: {jsonStr?.Length ?? 0}\n" +
                $"Exception: {e}"
            );

            yield break;
        }
        
    }

    /// <summary>
    /// 驗證數據欄位是否合法
    /// </summary>
    /// <param name="weaponType"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    private bool IsValidLevelData(E_WeaponType weaponType, WeaponLevelData data)
    {
        bool isValid = true;
        if (data.level <= 0)
        {
            Debug.LogError($"[WeaponDataManager] {weaponType} level 無效: {data.level}");
            isValid = false;
        }

        if (data.damage < 0)
        {
            Debug.LogError($"[WeaponDataManager] {weaponType} Lv.{data.level} damage 不可小於 0");
            isValid = false;
        }

        if (data.cooldown <= 0)
        {
            Debug.LogError($"[WeaponDataManager] {weaponType} Lv.{data.level} cooldown 必須大於 0");
            isValid = false;
        }

        if (data.duration < 0)
        {
            Debug.LogError($"[WeaponDataManager] {weaponType} Lv.{data.level} duration 不可小於 0");
            isValid = false;
        }

        switch (weaponType)
        {
            case E_WeaponType.Sword:
                if (data.range < 0)
                {
                    Debug.LogError($"[WeaponDataManager] {weaponType} Lv.{data.level} range 不可小於 0");
                    isValid = false;
                }
                break;
            case E_WeaponType.Fireball:
                if (data.speed < 0)
                {
                    Debug.LogError($"[WeaponDataManager] {weaponType} Lv.{data.level} speed 不可小於 0");
                    isValid = false;
                }
                if (data.projectileCount <= 0)
                {
                    Debug.LogError($"[WeaponDataManager] {weaponType} Lv.{data.level} projectileCount 必須大於 0");
                    isValid = false;
                }
                if(data.searchRadius <= 0)
                {
                    Debug.LogError($"[WeaponDataManager] {weaponType} Lv.{data.level} searchRadius 必須大於 0");
                    isValid = false;
                }
                break;
                
            case E_WeaponType.Orbit:
                if (data.range < 0)
                {
                    Debug.LogError($"[WeaponDataManager] {weaponType} Lv.{data.level} range 不可小於 0");
                    isValid = false;
                }
                if (data.speed < 0)
                {
                    Debug.LogError($"[WeaponDataManager] {weaponType} Lv.{data.level} speed 不可小於 0");
                    isValid = false;
                }
                if (data.projectileCount <= 0)
                {
                    Debug.LogError($"[WeaponDataManager] {weaponType} Lv.{data.level} projectileCount 必須大於 0");
                    isValid = false;
                }
                break;
        }

        return isValid;
    }

    public WeaponLevelData GetLevelData(E_WeaponType weaponType, int level)
    {
        if (LoadState != E_LoadState.Success || levelDataDic == null)
        {
            Debug.LogError($"[WeaponDataManager] 資料不可用，目前狀態: {LoadState}");
            return null;
        }

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
        if(weaponSODataDic == null)
        {
            Debug.LogError($"[WeaponDataManager] weaponSODataDic 未被初始化");
            return null;
        }

        if (!weaponSODataDic.TryGetValue(weaponType, out WeaponData data))
        {
            Debug.LogError($"[WeaponDataManager] 找不到 WeaponData: {weaponType}");
            return null;
        }

        return data;
    }

    public int GetWeaponMaxLevel(E_WeaponType weaponType)
    {
        if (LoadState != E_LoadState.Success || levelDataDic == null)
        {
            Debug.LogError($"[WeaponDataManager] 資料不可用，目前狀態: {LoadState}");
            return 0;
        }

        if (!levelDataDic.TryGetValue(weaponType, out var levelDict))
        {
            Debug.LogError($"[WeaponDataManager] 找不到武器等級資料: {weaponType}");
            return 0;
        }

        return levelDict.Keys.Max();
    }

    public Sprite GetWeaponIcon(E_WeaponType weaponType)
    {
        WeaponData data = GetWeaponData(weaponType);
        return data != null ? data.icon : null;
    }

    public string GetWeaponName(E_WeaponType weaponType)
    {
        WeaponData data = GetWeaponData(weaponType);
        return data != null ? data.weaponName : weaponType.ToString();
    }

    public string GetWeaponDescription(E_WeaponType weaponType, int level)
    {
        WeaponData weaponData = GetWeaponData(weaponType);

        if(weaponData == null)
            return "";
        if (level == 1)
            return weaponData.description;

        WeaponLevelData previousData = GetLevelData(weaponType, level - 1);
        WeaponLevelData nextData = GetLevelData(weaponType, level);

        if(previousData == null || nextData == null)
            return weaponData.description;

        return GetLevelDifferenceText(previousData, nextData);
    }

    /// <summary>
    /// 找出前後升級前後數值的變化
    /// </summary>
    /// <param name="previousData"></param>
    /// <param name="nextData"></param>
    /// <returns></returns>
    private string GetLevelDifferenceText(WeaponLevelData previousData, WeaponLevelData nextData)
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
    private string GetFieldDifferenceText(string fieldName, object previousValue, object nextValue)
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
    private string GetFieldDisplayName(string fieldName)
    {
        return fieldDisplayNames.TryGetValue(fieldName, out var name) ? name : fieldName;
    }
}
