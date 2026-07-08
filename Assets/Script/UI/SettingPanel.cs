using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : BasePanel
{
    [SerializeField] private Button btnSure;
    [SerializeField] private Toggle togBGMOn;
    [SerializeField] private Toggle togSFXOn;
    [SerializeField] private Slider sliderBGM;
    [SerializeField] private Slider sliderSFX;

    private void Start()
    {
        btnSure.onClick.AddListener(() => 
        {
            UIManager.Instance.ClosePopup(PanelType);
        });

        if (AudioManager.Instance == null)
        {
            Debug.LogError("SettingPanel 找不到 AudioManager.Instance");
            return;
        }

        togBGMOn.onValueChanged.AddListener(AudioManager.Instance.SetBGMOn);
        togSFXOn.onValueChanged.AddListener(AudioManager.Instance.SetSFXOn);
        sliderBGM.onValueChanged.AddListener(AudioManager.Instance.SetBGMVolume);
        sliderSFX.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);
    }
}
