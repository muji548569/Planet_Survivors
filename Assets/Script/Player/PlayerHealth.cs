using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public bool isDead;

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
        Destroy(gameObject);
        // TODO: 死亡UI
    }
}
