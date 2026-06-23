using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartPanel : BasePanel
{
    [SerializeField] private Button btnStart;
    [SerializeField] private Button btnSetting;
    [SerializeField] private Button btnExit;
    private void Start()
    {
        btnStart.onClick.AddListener(() =>
        {
            StartCoroutine(StartGame());
        });

        btnSetting.onClick.AddListener(() => 
        {
            print("呼出設定面板");
            UIManager.Instance.OpenPopup(E_PanelType.Setting);
        });

        btnExit.onClick.AddListener(() => 
        {
            Application.Quit();
        });
    }

    // 開一個協程 等場景加載完再切換UI
    private IEnumerator StartGame()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("GameScene");
        yield return operation;
        PlayerDataManager.Instance.Init();
        UIManager.Instance.SwitchScreen(E_PanelType.Game);
    }
}
