using System;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    public GameSaveData Data { get; private set; }

    private string saveFilePath;
    private const string SaveFileName = "save.json";

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        saveFilePath = Path.Combine(Application.persistentDataPath, SaveFileName);

        Load();
    }

    public void Save()
    {
        try
        {
            EnsureDataIsValid();
            string json = JsonUtility.ToJson(Data, true);
            File.WriteAllText(saveFilePath, json);
#if UNITY_EDITOR
            Debug.Log($"存檔完成：{saveFilePath}");
#endif
        }
        catch(Exception exception)
        {
                Debug.LogError($"儲存存檔失敗：{exception}");
        }
    }

    public void Load()
    {
        if (!File.Exists(saveFilePath))
        {
            CreateNewSave();
            return;
        }

        try
        {
            string json = File.ReadAllText(saveFilePath);
            Data = JsonUtility.FromJson<GameSaveData>(json);
            Data.audio.Validate();

            EnsureDataIsValid();
            UpgradeSaveDataIfNeeded();
        }
        catch (Exception exception)
        {
            Debug.LogError($"讀取存檔失敗：{exception}");

            BackupCorruptedSave();
            CreateNewSave();
        }
    }

    public void DeleteSave()
    {
        try
        {
            if (File.Exists(saveFilePath))
            {
                File.Delete(saveFilePath);
            }

            CreateNewSave();
        }
        catch (Exception exception)
        {
            Debug.LogError($"刪除存檔失敗：{exception}");
        }
    }

    private void CreateNewSave()
    {
        Data = new GameSaveData();
        Save();
    }

    private void EnsureDataIsValid()
    {
        if(Data == null)
        {
            Data = new GameSaveData();
        }

        if(Data.audio == null)
        {
            Data.audio = new AudioSaveData();
        }

    }

    /// <summary>
    /// 之後存檔版本更新 可以直接在這裡做資料遷移 不會直接刪除舊版資料
    /// </summary>
    private void UpgradeSaveDataIfNeeded()
    {
        if(Data.saveVersion < 1)
        {
            Data.saveVersion = 1;
        }
    }

    /// <summary>
    /// 當存檔被判定為損壞時，保留一份損壞檔案的備份
    /// </summary>
    private void BackupCorruptedSave()
    {
        // 確認檔案是否存在
        if (!File.Exists(saveFilePath))
        {
            return;
        }

        // 嘗試複製損毀檔案
        try
        {
            string backupPath = saveFilePath + ".broken";

            // 從 saveFilePath 複製到 backupPath
            // 第三個參數 true 表示 如果 .broken存檔已存在 直接覆蓋
            File.Copy(saveFilePath, backupPath, true);
        }
        // 複製失敗則輸出錯誤到Console
        catch (Exception exception)
        {
            Debug.LogError($"備份損壞存檔失敗：{exception}");
        }
    }
}
