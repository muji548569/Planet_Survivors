using UnityEngine;

public class EnemyAnimationEventReceiver : MonoBehaviour
{
    private EnemyHealth enemyHealth;
    private void Awake()
    {
        enemyHealth = GetComponentInParent<EnemyHealth>();
    }

    public void ReleaseAfterDeathAnimation()
    {
        enemyHealth.ReleaseAfterDeathAnimation();
    }
}
