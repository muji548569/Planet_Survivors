using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PausePanel : BasePanel
{
    [SerializeField] private Button btnResume;
    [SerializeField] private Button btnSetting;
    [SerializeField] private Button btnQuit;
    void Start()
    {
        btnResume.onClick.AddListener(() =>
        {
            GamePauseManager.Instance.ResumeGame();
        });

        btnSetting.onClick.AddListener(() =>
        {
            UIManager.Instance.OpenPopup(E_PanelType.Setting);
        });

        btnQuit.onClick.AddListener(() =>
        {
            GameFlowManager.Instance.QuitToMainScene();
        });
    }
}
