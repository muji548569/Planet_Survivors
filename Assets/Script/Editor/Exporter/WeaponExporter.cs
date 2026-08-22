using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ExcelDataReader;

public static class WeaponExporter
{
    [MenuItem("Tools/匯出 Weapon 設定檔")]
    public static void ExportExcelToJson()
    {
        string excelPath = Path.Combine(Application.dataPath, "DataSource", "Excel", "Weapon.xlsx");
        string jsonOutputPath = Path.Combine(Application.streamingAssetsPath, "Json", "Weapon.json");

        if (!File.Exists(excelPath))
        {
            Debug.LogError($"找不到 Excel 檔案: {excelPath}");
            return;
        }

        WeaponLevelDataTable root = new WeaponLevelDataTable
        {
            weapons = new List<WeaponLevelTable>()
        };

        using (var stream = File.Open(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        using (var reader = ExcelReaderFactory.CreateReader(stream))
        {
            do
            {
                string weaponTypeStr = reader.Name;

                if (!Enum.TryParse(weaponTypeStr, true, out E_WeaponType weaponType))
                {
                    Debug.LogError($"Sheet 名稱 '{weaponTypeStr}' 無法轉換為 E_WeaponType");
                    continue;
                }

                Dictionary<string, int> columnDic = new Dictionary<string, int>();

                WeaponLevelTable weaponTable = new WeaponLevelTable
                {
                    weaponType = weaponType,
                    levels = new List<WeaponLevelData>()
                };

                bool isHeader = true;

                while (reader.Read())
                {
                    if (isHeader)
                    {
                        for (int col = 0; col < reader.FieldCount; col++)
                        {
                            string header = reader.GetValue(col)?.ToString();

                            if (!string.IsNullOrWhiteSpace(header))
                                columnDic[header] = col;
                        }

                        isHeader = false;
                        continue;
                    }

                    if (!TryGetInt(reader, columnDic, "level", out int level))
                        continue;

                    WeaponLevelData item = new WeaponLevelData
                    {
                        level = level
                    };

                    SetFieldsByReflection(item, reader, columnDic);

                    weaponTable.levels.Add(item);
                }

                root.weapons.Add(weaponTable);

            } while (reader.NextResult());
        }

        string jsonStr = JsonUtility.ToJson(root, true);

        string directory = Path.GetDirectoryName(jsonOutputPath);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllText(jsonOutputPath, jsonStr, new UTF8Encoding(false));
        AssetDatabase.Refresh();

        Debug.Log($"Weapon JSON 匯出成功！路徑: {jsonOutputPath}");
    }

    private static void SetFieldsByReflection(
        WeaponLevelData item,
        IExcelDataReader reader,
        Dictionary<string, int> columnDic)
    {
        FieldInfo[] fields = typeof(WeaponLevelData).GetFields();

        foreach (FieldInfo field in fields)
        {
            if (field.Name == "level")
                continue;

            if (!columnDic.TryGetValue(field.Name, out int col))
                continue;

            object value = reader.GetValue(col);

            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                continue;

            try
            {
                if (field.FieldType == typeof(float))
                {
                    field.SetValue(item, Convert.ToSingle(value));
                }
                else if (field.FieldType == typeof(int))
                {
                    field.SetValue(item, Convert.ToInt32(value));
                }
                else if (field.FieldType == typeof(bool))
                {
                    field.SetValue(item, Convert.ToBoolean(value));
                }
                else if (field.FieldType.IsEnum)
                {
                    object enumValue = Enum.Parse(field.FieldType, value.ToString(), true);
                    field.SetValue(item, enumValue);
                }
                else if (field.FieldType == typeof(string))
                {
                    field.SetValue(item, value.ToString());
                }
                else
                {
                    Debug.LogWarning($"欄位 '{field.Name}' 的型別 '{field.FieldType}' 尚未支援。");
                }
            }
            catch
            {
                Debug.LogError($"欄位 '{field.Name}' 轉換失敗，Excel 值為: {value}");
            }
        }
    }

    private static bool TryGetInt(
        IExcelDataReader reader,
        Dictionary<string, int> columnDic,
        string columnName,
        out int result)
    {
        result = 0;

        if (!columnDic.TryGetValue(columnName, out int col))
            return false;

        object value = reader.GetValue(col);

        return value != null && int.TryParse(value.ToString(), out result);
    }
}