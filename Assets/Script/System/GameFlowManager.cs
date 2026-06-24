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
        Time.timeScale = 1f;
        AsyncOperation operation = SceneManager.LoadSceneAsync("MainScene");
        yield return operation;
        UIManager.Instance.SwitchScreen(E_PanelType.Start);
    }

    private IEnumerator StartGameRoutine()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("GameScene");
        yield return operation;
        PlayerDataManager.Instance.Init();
        UIManager.Instance.SwitchScreen(E_PanelType.Game);
    }
}
