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
            SceneManager.LoadScene("GameScene");
            // UIManager.Instance.SwitchScreen(E_PanelType.Game);
        });

        btnSetting.onClick.AddListener(() => 
        {
            print("呼出設定面板");
            // UIManager.Instance.OpenPopup(E_PanelType.Setting);
        });

        btnExit.onClick.AddListener(() => 
        {
            Application.Quit();
        });
    }
}
