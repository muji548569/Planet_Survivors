using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ExcelDataReader;

public class PlayerUpgradeExporter : EditorWindow
{
    [MenuItem("Tools/匯出 PlayerUpgrade 設定檔")]
    public static void ExportExcelToJson()
    {
        string excelPath = Path.Combine(Application.dataPath, "DataSource", "Excel", "PlayerUpgrade.xlsx");
        string jsonOutputPath = Path.Combine(Application.streamingAssetsPath, "Json", "PlayerUpgrade.json");

        if (!File.Exists(excelPath))
        {
            Debug.LogError($"找不到 Excel 檔案: {excelPath}");
            return;
        }

        PlayerConfigDataRoot root = new PlayerConfigDataRoot
        {
            stats = new List<PlayerConfigData>()
        };

        using (var stream = File.Open(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        using (var reader = ExcelReaderFactory.CreateReader(stream))
        {
            Dictionary<string, int> headerMap = ReadHeader(reader);

            while (reader.Read())
            {
                string statStr = GetCellString(reader, headerMap, "stat");

                if (string.IsNullOrWhiteSpace(statStr))
                    continue;

                PlayerConfigData item = new PlayerConfigData
                {
                    values = new List<float>()
                };

                FillFieldsByReflection(reader, headerMap, item);
                FillLevelValues(reader, headerMap, item);

                root.stats.Add(item);
            }
        }

        string jsonStr = JsonUtility.ToJson(root, true);

        string directory = Path.GetDirectoryName(jsonOutputPath);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllText(jsonOutputPath, jsonStr, Encoding.UTF8);
        AssetDatabase.Refresh();

        Debug.Log($"PlayerUpgrade.json 匯出成功: {jsonOutputPath}");
    }

    private static Dictionary<string, int> ReadHeader(IExcelDataReader reader)
    {
        Dictionary<string, int> headerMap = new Dictionary<string, int>();

        if (!reader.Read())
        {
            Debug.LogError("Excel 沒有標題列");
            return headerMap;
        }

        for (int i = 0; i < reader.FieldCount; i++)
        {
            string header = reader.GetValue(i)?.ToString();

            if (string.IsNullOrWhiteSpace(header))
                continue;

            headerMap[header.Trim()] = i;
        }

        return headerMap;
    }

    private static void FillFieldsByReflection(
        IExcelDataReader reader,
        Dictionary<string, int> headerMap,
        PlayerConfigData item)
    {
        FieldInfo[] fields = typeof(PlayerConfigData).GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (FieldInfo field in fields)
        {
            if (field.Name == "values")
                continue;

            if (!headerMap.TryGetValue(field.Name, out int col))
                continue;

            object cellValue = reader.GetValue(col);

            if (cellValue == null)
                continue;

            object convertedValue = ConvertValue(cellValue.ToString(), field.FieldType);

            if (convertedValue != null)
                field.SetValue(item, convertedValue);
        }
    }

    private static void FillLevelValues(
        IExcelDataReader reader,
        Dictionary<string, int> headerMap,
        PlayerConfigData item)
    {
        item.values.Clear();

        int level = 1;

        while (true)
        {
            string header = $"level{level}";

            if (!headerMap.TryGetValue(header, out int col))
                break;

            object cellValue = reader.GetValue(col);

            if (cellValue != null && float.TryParse(cellValue.ToString(), out float value))
            {
                item.values.Add(value);
            }
            else
            {
                Debug.LogWarning($"{item.stat} 的 {header} 數值無效，已略過");
            }

            level++;
        }
    }

    private static object ConvertValue(string value, Type targetType)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        try
        {
            if (targetType == typeof(string))
                return value;

            if (targetType == typeof(int))
                return int.Parse(value);

            if (targetType == typeof(float))
                return float.Parse(value);

            if (targetType == typeof(bool))
                return bool.Parse(value);

            if (targetType.IsEnum)
                return Enum.Parse(targetType, value, true);

            Debug.LogWarning($"尚未支援的欄位型別: {targetType}");
            return null;
        }
        catch
        {
            Debug.LogError($"轉換失敗: '{value}' -> {targetType}");
            return null;
        }
    }

    private static string GetCellString(
        IExcelDataReader reader,
        Dictionary<string, int> headerMap,
        string header)
    {
        if (!headerMap.TryGetValue(header, out int col))
            return null;

        return reader.GetValue(col)?.ToString();
    }
}