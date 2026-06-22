using UnityEngine;
using UnityEngine.UI;

public class ExpBarUI : MonoBehaviour
{
    [SerializeField] private Image imgFill;

    public void SetValue(int currentExp, int requiredExp)
    {
        if (requiredExp <= 0)
        {
            imgFill.fillAmount = 0;
            return;
        }
        imgFill.fillAmount = Mathf.Clamp01((float)currentExp / requiredExp);
    }
}
