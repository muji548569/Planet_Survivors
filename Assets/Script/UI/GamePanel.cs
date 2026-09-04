using UnityEngine;
using UnityEngine.UI;

public class GamePanel : BasePanel
{
    [SerializeField] private HealthBarUI healthBar;
    [SerializeField] private ExpBarUI expBar;
    [SerializeField] private Text textCoin;
    [SerializeField] private Text textTimer;
    [SerializeField] private Text textHealth;
    [SerializeField] private Text textLevel;

    private void OnEnable()
    {
        if (PlayerDataManager.Instance == null) return;
        if (!PlayerDataManager.Instance.IsInitialized) return;

        PlayerDataManager.Instance.OnHealthChanged += SetHealth;
        PlayerDataManager.Instance.OnExpChanged += SetExp;
        PlayerDataManager.Instance.OnCoinChanged += SetCoin;
        PlayerDataManager.Instance.OnLevelChanged += SetLevel;
        PlayerDataManager.Instance.NotifyAll();

        GameSessionManager.Instance.OnTimeChanged += SetTime;
    }
    private void OnDisable()
    {
        if (PlayerDataManager.Instance == null) return;
        PlayerDataManager.Instance.OnHealthChanged -= SetHealth;
        PlayerDataManager.Instance.OnExpChanged -= SetExp;
        PlayerDataManager.Instance.OnCoinChanged -= SetCoin;
        PlayerDataManager.Instance.OnLevelChanged -= SetLevel;

        GameSessionManager.Instance.OnTimeChanged -= SetTime;
    }


    public void SetHealth(float currentHealth, float maxHealth)
    {
        healthBar.SetValue(currentHealth, maxHealth);

        int displayHealth = Mathf.CeilToInt(Mathf.Max(0, currentHealth));
        int displayMaxHealth = Mathf.CeilToInt(maxHealth);

        textHealth.text = displayHealth + "/" + displayMaxHealth;
    }

    public void SetExp(int currentExp, int requiredExp)
    {
        expBar.SetValue(currentExp, requiredExp);
    }

    public void SetCoin(int amount)
    {
        textCoin.text = amount.ToString();
    }

    public void SetLevel(int level)
    {
        textLevel.text = $"LEVEL {level}";
    }

    public void SetTime(float time)
    {
        textTimer.text = FormatTime(time);
    }

    private string FormatTime(float time)
    {
        int minute = Mathf.FloorToInt(time / 60f);
        int second = Mathf.FloorToInt(time % 60f);
        return $"{minute:00}:{second:00}";
    }
}
