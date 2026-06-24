using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    private void Start()
    {
        UIManager.Instance.Init();
        UIManager.Instance.SwitchScreen(E_PanelType.Start);

        // TODO: 播放音樂
    }
}
