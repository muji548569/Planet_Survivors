using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    private void Start()
    {
        if (UIManager.Instance == null)
        {
            Debug.LogError("[GameBootstrap] UIManager 不存在，無法切換 Game UI。");
        }
        UIManager.Instance.Init();
        UIManager.Instance.SwitchScreen(E_PanelType.Start);

        if(GamePauseManager.Instance == null)
        {
            Debug.LogError("[GameBootstrap] GamePauseManager 不存在，暫停功能無法使用。");
        }
        GamePauseManager.Instance.gameObject.SetActive(false);

        // TODO: 播放音樂
        if(AudioManager.Instance == null)
        {
            Debug.LogError("[GameBootstrap] AudioManager 不存在，無法播放音樂。");
        }

    }
}
