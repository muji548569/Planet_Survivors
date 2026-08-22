using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public static class StreamingAssetLoader
{
    /// <summary>
    /// 讀取StreamingAssets文件
    /// </summary>
    /// <param name="relativePath">StreamingAssets裡的位置</param>
    /// <param name="onSuccess">成功執行的委派</param>
    /// <param name="onError">失敗執行的委派(可空)</param>
    /// <returns></returns>
    public static IEnumerator LoadText(
        string relativePath, 
        Action<string> onSuccess, 
        Action<string> onError = null) 
    {
        string path = Path.Combine(Application.streamingAssetsPath, relativePath);

#if UNITY_WEBGL && !UNITY_EDITOR
        // 這裡的using不是引用命名空間的那個using
        // 而是C#的 using declaration
        // request 會在離開目前 scope 時自動 Dispose
        //
        // UnityWebRequest.Get() 建立一個針對該位置發送 GET 請求的 UnityWebRequest
        using UnityWebRequest request = UnityWebRequest.Get(path);

        // 正式送出 Request 並暫停目前協程coroutine 直到Request結束
        // 不會因此阻塞整個 Unity 主執行緒
        yield return request.SendWebRequest();

        // Request 完成後檢查結果
        if(request.result != UnityWebRequest.Result.Success)
        {
            string error = 
            $"[StreamingAssetLoader] 載入失敗\n" + 
            $"Path: {path}\n" + 
            $"Error: {request.error}";
            
            Debug.LogError(error);

            // 如果外部有傳入 onError 委派，就呼叫它
            onError?.Invoke(error);

            // 結束目前 Coroutine
            // 不繼續執行後面的 onSuccess
            yield break;
        }

        string text = request.downloadHandler.text.TrimStart('\uFEFF', ' ', '\r', '\n', '\t');

        // downloadHandler 保存下載回來的資料
        // .text 將下載內容以 string 形式取得
        // 如果讀取的是 JSON，這時取得的仍然只是 JSON 格式的字串
        // 還沒有轉換成實際的 C# 資料物件
        onSuccess?.Invoke(text);

#elif UNITY_ANDROID && !UNITY_EDITOR
        using UnityWebRequest request = UnityWebRequest.Get(path);

        yield return request.SendWebRequest();

        if(request.result != UnityWebRequest.Result.Success)
        {
            string error = 
            $"[StreamingAssetLoader] 載入失敗\n" + 
            $"Path: {path}\n" + 
            $"Error: {request.error}";
            
            Debug.LogError(error);
            onError?.Invoke(error);
            yield break;
        }

        string text = request.downloadHandler.text.TrimStart('\uFEFF', ' ', '\r', '\n', '\t');

        onSuccess?.Invoke(text);
#else
        if (!File.Exists(path))
        {
            string error =
                $"[StreamingAssetLoader] 找不到檔案: {path}";

            Debug.LogError(error);
            onError?.Invoke(error);
            yield break;
        }

        string text = File.ReadAllText(path);
        onSuccess?.Invoke(text);
#endif
    }
}
