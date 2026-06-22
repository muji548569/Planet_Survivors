using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image imgFill;

    public void SetValue(float currentHealth, float maxHealth)
    {
        if(maxHealth <= 0)
        {
            imgFill.fillAmount = 0;
            return;
        }
        imgFill.fillAmount = currentHealth / maxHealth;
    }
}
