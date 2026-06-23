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
    private float Timer;

    private void OnEnable()
    {
        if (PlayerDataManager.Instance == null) return;
        if (!PlayerDataManager.Instance.IsInitialized) return;

        PlayerDataManager.Instance.OnHealthChanged += SetHealth;
        PlayerDataManager.Instance.OnExpChanged += SetExp;
        PlayerDataManager.Instance.OnCoinChanged += SetCoin;
        PlayerDataManager.Instance.OnLevelChanged += SetLevel;
        PlayerDataManager.Instance.NotifyAll();
    }
    private void OnDisable()
    {
        if (PlayerDataManager.Instance == null) return;
        PlayerDataManager.Instance.OnHealthChanged -= SetHealth;
        PlayerDataManager.Instance.OnExpChanged -= SetExp;
        PlayerDataManager.Instance.OnCoinChanged -= SetCoin;
        PlayerDataManager.Instance.OnLevelChanged -= SetLevel;
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

    public void SetTime(float time)
    {
        int runtime = Mathf.FloorToInt(time);

        int hour = runtime / 3600;
        int minute = (runtime % 3600) / 60;
        int second = runtime % 60;

        if (hour > 0)
            textTimer.text = $"{hour:D2}:{minute:D2}:{second:D2}";
        else
            textTimer.text = $"{minute:D2}:{second:D2}";
    }

    public void SetLevel(int level)
    {
        textLevel.text = $"LEVEL {level}";
    }

    private void Update()
    {
        Timer += Time.deltaTime;
        SetTime(Timer);
    }
}
