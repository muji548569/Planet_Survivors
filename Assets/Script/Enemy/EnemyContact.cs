using UnityEngine;

public class EnemyContact : MonoBehaviour
{
    [SerializeField] private EnemyData data;
    private float timer;

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (timer > 0) return;
        // 玩家扣血功能
        timer = data.attackCooldown;
    }
}
