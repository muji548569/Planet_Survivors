using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanel : BasePanel
{
    [SerializeField] private List<Button> buttons = new List<Button>();
    [SerializeField] private UpgradeSelector upgradeSelector;
    [SerializeField] private int optionCount = 3;
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
        RenewOptions(upgradeSelector.CreateOptions(optionCount));
    }

    public void RenewOptions(List<UpgradeOption> options)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            UpgradeOptionItem item = buttons[i].GetComponent<UpgradeOptionItem>();

            if(i<optionCount)
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
