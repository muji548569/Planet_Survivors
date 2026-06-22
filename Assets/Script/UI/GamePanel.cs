using UnityEngine;
using UnityEngine.UI;

public class GamePanel : BasePanel
{
    [SerializeField] private HealthBarUI healthBar;
    [SerializeField] private ExpBarUI expBar;
    [SerializeField] private Text textCoin;
    [SerializeField] private Text textTimer;
    [SerializeField] private Text textHealth;

    private void Start()
    {
        PlayerDataManager.Instance.OnHealthChanged += SetHealth;
        PlayerDataManager.Instance.OnExpChanged += SetExp;
        PlayerDataManager.Instance.OnCoinChanged += SetCoin;
        PlayerDataManager.Instance.NotifyAll();
    }
    private void OnDestroy()
    {
        if (PlayerDataManager.Instance == null) return;
        PlayerDataManager.Instance.OnHealthChanged -= SetHealth;
        PlayerDataManager.Instance.OnExpChanged -= SetExp;
        PlayerDataManager.Instance.OnCoinChanged -= SetCoin;
    }


    public void SetHealth(float currentHealth, float maxHealth)
    {
        healthBar.SetValue(currentHealth, maxHealth);
        textHealth.text = currentHealth + "/" + maxHealth;
    }

    public void SetExp(int currentExp, int requiredExp)
    {
        expBar.SetValue(currentExp, requiredExp);
    }

    public void SetCoin(int amount)
    {
        textCoin.text = amount.ToString();
    }

    public void SetTime(int time)
    {
        int min = time / 60;
        int sec = time % 60;
        textTimer.text = min + ":" + sec;
    }
}
