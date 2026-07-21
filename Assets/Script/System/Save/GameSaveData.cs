using System;

/// <summary>
/// 所有存檔的根物件 用來序列化到Json中
/// </summary>
[Serializable]
public class GameSaveData
{
    public int saveVersion = 1; 
    // 音效設定資料
    public AudioSaveData audio = new AudioSaveData();

    // 未來可以直接擴充
    // 局外成長資料 MetaProgressionSaveData 
    // 解鎖內容資料 UnlockSaveData 
}
