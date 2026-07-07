using UnityEngine;
using UnityEngine.UI;

public class UpgradeOptionItem : MonoBehaviour
{
    [SerializeField] private Image imgIcon;
    [SerializeField] private Text textOptionName;
    [SerializeField] private Text textLevel;
    [SerializeField] private Text textDescription;
    public UpgradeOption Option { get; private set; }
    public void UpdateInfo(UpgradeOption option)
    {
        Option = option;
        imgIcon.sprite = option.GetIcon();
        textLevel.text = "Lv." + option.level.ToString();
        textOptionName.text = option.GetOptionName();
        textDescription.text = option.GetDescription();
    }
}
