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

    private void Start()
    {
        if (GameSessionManager.Instance == null) return;

        GameSessionManager.Instance.OnGameWin += HandleGameWin;
        GameSessionManager.Instance.OnGameLose += HandleGameLose;
    }

    private void OnDestroy()
    {
        if (GameSessionManager.Instance == null) return;

        GameSessionManager.Instance.OnGameWin -= HandleGameWin;
        GameSessionManager.Instance.OnGameLose -= HandleGameLose;
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
        // 1.停止會生成或更新遊戲物件的系統
        GameSessionManager.Instance.EndSession();
        GamePauseManager.Instance.DisablePause();
        // 2.避免暫停狀態影響場景載入
        Time.timeScale = 1f;
        // 3.回收 active objects，再銷毀池內物件
        PoolManager.Instance.ClearAll();
        // 4.切換場景
        AsyncOperation operation = SceneManager.LoadSceneAsync("MainScene");
        yield return operation;
        UIManager.Instance.SwitchScreen(E_PanelType.Start);
        AudioManager.Instance.PlayBGM(E_BGM.Menu);
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

        PoolManager.Instance?.ClearAll();

        GamePoolInstaller installer = GetComponent<GamePoolInstaller>();
        installer?.Install();

        AudioManager.Instance?.PlayBGM(E_BGM.Game);
        AudioManager.Instance?.PlaySFX(E_SFX.LevelStart);
    }

    private void HandleGameWin()
    {
        GamePauseManager.Instance.PauseGame();
        UIManager.Instance.OpenPopup(E_PanelType.Victory);
        AudioManager.Instance.PlayBGM(E_BGM.Victory);
    }

    private void HandleGameLose()
    {
        GamePauseManager.Instance.PauseGame();
        UIManager.Instance.OpenPopup(E_PanelType.GameOver);
        AudioManager.Instance.PlaySFX(E_SFX.PlayerDie);
        AudioManager.Instance.PlayBGM(E_BGM.Lose);
    }
}
