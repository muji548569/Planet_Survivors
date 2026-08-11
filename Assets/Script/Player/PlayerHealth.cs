using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public bool IsDead { get; private set; }

    public void TakeDamage(float damage)
    {
        if(IsDead) return;
        float newhp = PlayerDataManager.Instance.Data.Stat.currentHp - damage;
        PlayerDataManager.Instance.SetHealth(newhp);
        AudioManager.Instance?.PlaySFX(E_SFX.PlayerHurt);
        if (newhp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (IsDead) return;

        IsDead = true;

        GameSessionManager.Instance.LoseGame();

        gameObject.SetActive(false);
    }
}
