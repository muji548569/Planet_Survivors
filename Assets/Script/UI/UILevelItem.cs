using UnityEngine;
using UnityEngine.UI;

public class UILevelItem : MonoBehaviour
{
    [SerializeField] private Image imgIcon;
    [SerializeField] private Text textLevel;

    public void UpdateInfo(Sprite icon, int level)
    {
        imgIcon.sprite = icon;
        textLevel.text = "Lv." + level;
    }
}
