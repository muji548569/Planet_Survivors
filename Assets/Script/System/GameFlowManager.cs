using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void QuitToMainScene()
    {
        StartCoroutine(QuitToMainSceneRoutine());
    }

    public void StartGame()
    {
        StartCoroutine(StartGameRoutine());
    }

    private IEnumerator QuitToMainSceneRoutine()
    {
        GamePauseManager.Instance.DisablePause();

        Time.timeScale = 1f;
        AsyncOperation operation = SceneManager.LoadSceneAsync("MainScene");
        yield return operation;
        UIManager.Instance.SwitchScreen(E_PanelType.Start);
    }

    private IEnumerator StartGameRoutine()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("GameScene");
        yield return operation;

        if (PlayerDataManager.Instance == null)
        {
            Debug.LogError("[GameFlowManager] PlayerDataManager 不存在，無法初始化玩家資料。");
            yield break;
        }
        PlayerDataManager.Instance.Init();

        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("[GameFlowManager] GameScene 找不到 PlayerController。");
            yield break;
        }
        player.Init(PlayerDataManager.Instance.Data);

        if (GameSessionManager.Instance == null)
        {
            Debug.LogError("[GameFlowManager] GameSessionManager 不存在，無法開始遊戲流程。");
            yield break;
        }
        GameSessionManager.Instance.StartSession();

        if (UIManager.Instance == null)
        {
            Debug.LogError("[GameFlowManager] UIManager 不存在，無法切換 Game UI。");
            yield break;
        }
        UIManager.Instance.SwitchScreen(E_PanelType.Game);

        if(GamePauseManager.Instance == null)
        {
            Debug.LogError("[GameFlowManager] GamePauseManager 不存在，暫停功能無法使用。");
            yield break;
        }
        GamePauseManager.Instance.EnablePause();
    }
}
