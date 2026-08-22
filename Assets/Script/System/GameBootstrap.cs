using System.Collections;
using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    private IEnumerator Start()
    {
        // 需加載配置文件的Manager存不存在
        if (PlayerConfigDataManager.Instance == null)
        {
            Debug.LogError("[GameBootstrap] PlayerConfigDataManager 不存在");
            yield break;
        }
        if (WeaponDataManager.Instance == null)
        {
            Debug.LogError("[GameBootstrap] WeaponDataManager 不存在");
            yield break;
        }
        // 加載配置文件
        yield return new WaitUntil(() =>
            PlayerConfigDataManager.Instance.LoadState != E_LoadState.Loading &&
            WeaponDataManager.Instance.LoadState != E_LoadState.Loading
        );
        // 判斷是否加載成功
        if (PlayerConfigDataManager.Instance.LoadState == E_LoadState.Failed)
        {
            Debug.LogError("[GameBootstrap] PlayerConfigData 載入失敗");
            yield break;
        }
        if (WeaponDataManager.Instance.LoadState == E_LoadState.Failed)
        {
            Debug.LogError("[GameBootstrap] WeaponData 載入失敗");
            yield break;
        }

        if (SaveManager.Instance == null)
        {
            Debug.LogError("[GameBootstrap] SaveManager 不存在，無法讀取存檔數據。");
            yield break;
        }

        if (AudioManager.Instance == null)
        {
            Debug.LogError("[GameBootstrap] AudioManager 不存在，無法播放音樂。");
            yield break;
        }
        AudioManager.Instance.Init();
        AudioManager.Instance.PlayBGM(E_BGM.Menu);

        if (UIManager.Instance == null)
        {
            Debug.LogError("[GameBootstrap] UIManager 不存在，無法切換 Game UI。");
            yield break;
        }
        UIManager.Instance.Init();
        UIManager.Instance.SwitchScreen(E_PanelType.Start);

        if(GamePauseManager.Instance == null)
        {
            Debug.LogError("[GameBootstrap] GamePauseManager 不存在，暫停功能無法使用。");
            yield break;
        }
        GamePauseManager.Instance.gameObject.SetActive(false);

    }
}

public enum E_LoadState
{
    None,
    Loading,
    Success,
    Failed
}
