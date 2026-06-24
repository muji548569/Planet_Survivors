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
            GameFlowManager.Instance.StartGame();
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
}
