using System;
using System.IO;
using System.Text;
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
            Debug.LogError($"找不到 Excel 檔案，請確認路徑是否正確: {excelPath}");
            return;
        }

        PlayerConfigDataRoot root = new PlayerConfigDataRoot
        {
            stats = new List<PlayerConfigData>()
        };

        using (var stream = File.Open(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        using (var reader = ExcelReaderFactory.CreateReader(stream))
        {
            bool isHeader = true;

            while (reader.Read())
            {
                if (isHeader)
                {
                    isHeader = false;
                    continue;
                }

                string statStr = reader.GetValue(0)?.ToString();

                if (string.IsNullOrWhiteSpace(statStr))
                    continue;

                PlayerConfigData item = new PlayerConfigData();

                if (!Enum.TryParse(statStr, true, out E_PlayerStat stat))
                {
                    Debug.LogError($"無法將表格中的 stat '{statStr}' 轉換為 E_PlayerStat，請檢查 Excel 第 1 欄。");
                    continue;
                }

                item.stat = stat;
                item.values = new List<float>();

                for (int col = 1; col <= 5; col++)
                {
                    var valObj = reader.GetValue(col);

                    if (valObj != null && float.TryParse(valObj.ToString(), out float val))
                    {
                        item.values.Add(val);
                    }
                    else
                    {
                        Debug.LogWarning($"stat '{statStr}' 的 Level {col} 數值無效或為空，已略過。");
                    }
                }

                string modifierStr = reader.GetValue(6)?.ToString();

                if (!Enum.TryParse(modifierStr, true, out E_StatModifier modifier))
                {
                    Debug.LogError($"無法將表格中的 modifier '{modifierStr}' 轉換為 E_StatModifier，請檢查 Excel 第 7 欄。");
                    continue;
                }

                item.type = modifier;
                item.description = reader.GetValue(7)?.ToString() ?? "";

                root.stats.Add(item);
            }
        }

        string jsonStr = JsonUtility.ToJson(root, true);

        string directory = Path.GetDirectoryName(jsonOutputPath);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllText(jsonOutputPath, jsonStr, Encoding.UTF8);
        AssetDatabase.Refresh();

        Debug.Log($"JSON 匯出成功！路徑: {jsonOutputPath}");
    }
}