using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public bool isDead;
    public event Action<bool> OnPlayerDie;
    public void TakeDamage(float damage)
    {
        float newhp = PlayerDataManager.Instance.Data.currentHp - damage;
        PlayerDataManager.Instance.SetHealth(newhp);
        if (newhp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        OnPlayerDie?.Invoke(isDead);
        
        GamePauseManager.Instance.PauseGame();
        UIManager.Instance.OpenPopup(E_PanelType.GameOver);
        GameSessionManager.Instance.EndSession();

        gameObject.SetActive(false);
    }
}
