using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanel : BasePanel
{
    [SerializeField] private List<Button> buttons = new List<Button>();
    [SerializeField] private UpgradeSelector upgradeSelector;
    private void Start()
    {
        foreach (var button in buttons)
        {
            button.onClick.AddListener(() =>
            {
                UpgradeOptionItem optionItem = button.GetComponent<UpgradeOptionItem>();
                UpgradeManager.Instance.ApplyUpgrade(optionItem.Option);
                GamePauseManager.Instance.ResumeGame();
                UIManager.Instance.ClosePopup(PanelType);
            });
        }

    }

    public override void ShowPanel()
    {
        base.ShowPanel();
        RenewOptions(upgradeSelector.CreateOptions(buttons.Count));
    }

    public void RenewOptions(List<UpgradeOption> options)
    {
        if (options.Count == 0 || options == null)
        {
            PlayerDataManager.Instance.AddCoin(100);
            GamePauseManager.Instance.ResumeGame();
            UIManager.Instance.ClosePopup(PanelType);
            return;
        }

        int showCount = Mathf.Min(buttons.Count, options.Count);

        for (int i = 0; i < buttons.Count; i++)
        {
            UpgradeOptionItem item = buttons[i].GetComponent<UpgradeOptionItem>();

            if(i < showCount)
            {
                buttons[i].gameObject.SetActive(true); 
                item.UpdateInfo(options[i]);
            }
            else
            {
                buttons[i].gameObject.SetActive(false);
            }
        }
    }
}
